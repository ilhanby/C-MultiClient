using MahApps.Metro;
using MahApps.Metro.Controls;
using Server.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Server
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow main;

        public MainWindow()
        {
            InitializeComponent();
            main = this;
        }
        //Tema değişim butonu
        private void toogle_Checked(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, ThemeManager.GetAppTheme("Base" + "Dark"));
        }
        //Tema değişim butonu
        private void toogle_Unchecked(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, ThemeManager.GetAppTheme("Base" + "Light"));
        }

        TcpListener TcpDinle;
        TcpClient client;
        BackgroundWorker work = new BackgroundWorker();
        static int durum;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LampControl();
            TcpDinle = new TcpListener(123);
            TcpDinle.Start();

            work.DoWork += Work_DoWork;
            work.RunWorkerCompleted += Work_RunWorkerCompleted;
            work.WorkerSupportsCancellation = true;
            work.RunWorkerAsync();
        }

        private void Work_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                client = TcpDinle.AcceptTcpClient();
                Multi multi = new Multi();
                multi.Conn(client);
            }
            catch (Exception)
            {
            }
        }

        public void Read(string ReceiveData)
        {
            switch (ReceiveData.Substring(0, 1))
            {
                case "L":
                    {

                        durum = Convert.ToInt32(ReceiveData.Substring(1));
                        LampBtn_Click(null, null);
                        break;
                    }
                case "T":
                    {
                        TextBtn_Click(null, null);
                        break;
                    }
                case "D":
                    {
                        Multi.Write("L" + durum + "");
                        TextTxt_SelectionChanged(null, null);
                        break;
                    }
            }
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            work.RunWorkerAsync();
        }

        private void TextBtn_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int sayi = random.Next();
            textTxt.Text = sayi.ToString();
        }

        private void TextTxt_SelectionChanged(object sender, RoutedEventArgs e)
        {
            Multi.Write("T" + textTxt.Text + "");
        }

        private void LampBtn_Click(object sender, RoutedEventArgs e)
        {
            LampControl();
            Multi.Write("L" + durum + "");
        }

        public void LampControl()
        {
            if (durum == 0)
            {
                durum = 1;
                lampBtn.Content = "AÇIK";
                lamp.Fill = Brushes.Lime;
            }
            else
            {
                durum = 0;
                lampBtn.Content = "KAPALI";
                lamp.Fill = Brushes.DarkRed;
            }
        }
    }
}
