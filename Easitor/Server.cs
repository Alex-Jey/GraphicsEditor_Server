using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.Net.Sockets;
using System.Threading;

namespace Easitor
{
    class Server
    {
        TimeredActions Model;

        int Port;
        IPAddress IP;
        Socket socket;

        Socket handler;

        string Data;

        public Server(TimeredActions Model, int Port = 10001, string ip = "0.0.0.0")
        {
            this.Model = Model;
            this.Port = Port;
            this.IP = IPAddress.Parse(ip);
            Data = null;
        }

        public void ChangeSize(double Size)
        {
            Model.MoveSlider(Size, GlobalConstants.ToolType.Radius);
        }

        public void ChangeHardness(double Size)
        {
            Model.MoveSlider(Size, GlobalConstants.ToolType.Hardness);
        }

        public void ChangeTransparency(double Size)
        {
            Model.MoveSlider(Size, GlobalConstants.ToolType.Transparency);
        }

        public void ChangeColor(string Color)
        {
            Model.sliderColorView = Color;
            // Model.SelectedColor = Color;
            // Model.sliderColor = Color;
           
        }

        public void Start()
        {
            Thread server = new Thread(new ThreadStart (ServerThread));

            server.Start();

           // MessageBox.Show("Server is start");
        }

        void ParseCommand(String command)
        {
            var args = command.Split(' ');
            string prefix = args[0];
            string value = args[1];

            switch (prefix)
            {
                case "Color":
                    {
                        ChangeColor(value);
                        break;
                    }
            }
        }

        public void SendCommand()
        {
            try
            {                
                Random random = new Random();               
                handler.Send(Encoding.UTF8.GetBytes("Otvet "+random.Next(20).ToString()));
                handler.Shutdown(SocketShutdown.Both);
            }

            catch
            {
            }
            
        }
        
        public void ServerThread()
        {
      
            IPEndPoint ipEndPoint = new IPEndPoint(IP, Port);

            socket = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try
            {
                socket.Bind(ipEndPoint);
                socket.Listen(11);

                // Начинаем слушать соединения
                while (true)
                {
                    MessageBox.Show("Ожидаем соединение через порт {0}", ipEndPoint.ToString());

                    handler = socket.Accept();

                    Data = null;
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    Data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    MessageBox.Show("Получено: " + Data + "\n\n");
                    ParseCommand(Data);


                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                
            }
        }

    }


}
