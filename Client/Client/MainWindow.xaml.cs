using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace Client
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
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

        BackgroundWorker work = new BackgroundWorker();
        TcpClient client;
        NetworkStream Ag;
        StreamWriter Yazici;
        StreamReader Okuyucu;

        int durum;
        string data = null;

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            work.DoWork += Work_DoWork;
            work.RunWorkerCompleted += Work_RunWorkerCompleted;
            work.RunWorkerAsync();
        }

        private void Work_DoWork(object sender, EventArgs e)
        {
            try
            {
                data = Okuyucu.ReadLine();
            }
            catch (Exception)
            {

            }
        }

        private void Work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    switch (data.Substring(0, 1))
                    {
                        case "L":
                            {
                                durum = Convert.ToInt32(data.Substring(1));
                                break;
                            }
                        case "T":
                            {
                                textBtn.Content = data.Substring(1);
                                break;
                            }
                    }
                    if (durum == 1)
                    {
                        lampBtn.Content = "AÇIK";
                        lampBtn.Background = Brushes.Lime;
                    }
                    else
                    {
                        lampBtn.Content = "KAPALI";
                        lampBtn.Background = Brushes.DarkRed;
                    }
                }
            }
            catch (Exception)
            {
            }

            data = null;
            if (client != null)
            {
                if (!client.Connected)
                    Conn();
                else
                    work.RunWorkerAsync();
            }
            else
                Conn();
        }


        public async void Conn()
        {
            try
            {
                client = new TcpClient("localhost", 123);
                Ag = client.GetStream();
                Yazici = new StreamWriter(Ag);
                Okuyucu = new StreamReader(Ag);
                Yazici.WriteLine("D");
                Yazici.Flush();
                work.RunWorkerAsync();
            }
            catch (Exception)
            {
                var settings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Evet",
                    NegativeButtonText = "Kapat",
                };
                var dialog = await this.ShowMessageAsync("HATA", "BAĞLANTI HATASI \nTEKRAR DENEMEK İSTER MİSİNİZ?", MessageDialogStyle.AffirmativeAndNegative, settings);
                if (dialog == MessageDialogResult.Affirmative)
                {
                    Conn();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }


        private void TextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (client.Connected)
            {
                Yazici.WriteLine("T");
                Yazici.Flush();
            }
        }

        private void LampBtn_Click(object sender, RoutedEventArgs e)
        {
            if (client.Connected)
            {
                Yazici.WriteLine("L" + durum + "");
                Yazici.Flush();
            }
        }
    }
}
