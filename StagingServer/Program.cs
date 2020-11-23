using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading;

namespace StagingServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005);

            List<ClientHandler> clients = new List<ClientHandler>();

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();

                    for (int i = 0; i < clients.Count; ++i)
                    {
                        TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks - clients[i].lastUsedTime);

                        if (!clients[i].finished && timeSpan.TotalSeconds > 30)
                        {
                            clients[i].Stop();
                            clients.Remove(clients[i]);
                            --i;
                        }
                        else if(clients[i].finished)
                        {
                            clients[i].Stop();
                            clients.Remove(clients[i]);
                            --i;
                        }
                    }

                    clients.Add(new ClientHandler(handler));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }




        
    }
}
