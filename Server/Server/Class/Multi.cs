using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server;

namespace Server.Class
{
    class Multi
    {
        TcpClient socket;
        NetworkStream Ag;
        StreamReader Okuyucu;
        public static List<TcpClient> clientList = new List<TcpClient>();

        public void Conn(TcpClient clientsocket)
        {
            socket = clientsocket;
            clientList.Add(socket);
            Ag = socket.GetStream();
            Okuyucu = new StreamReader(Ag);
            Thread reader = new Thread(Reader);
            reader.SetApartmentState(ApartmentState.STA);
            reader.Start();
        }

        private void Reader()
        {
            while (true)
            {
                try
                {
                    string data = Okuyucu.ReadLine();
                    if (data != null)
                    {
                        MainWindow.main.Dispatcher.Invoke(() =>
                        {
                            MainWindow.main.Read(data);
                        });
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        public static void Write(string sendData)
        {
            foreach (TcpClient clients in clientList)
            {
                if (clients.Connected)
                {
                    StreamWriter Yazici = new StreamWriter(clients.GetStream());
                    Yazici.WriteLine(sendData);
                    Yazici.Flush();
                }
            }
        }
    }
}
