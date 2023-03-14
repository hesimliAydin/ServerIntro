using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            var ipAddress = IPAddress.Parse("192.168.100.155");
            var port = 27001;

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp))
            {
                var endPoint = new IPEndPoint(ipAddress, port);
                socket.Bind(endPoint);

                socket.Listen(10);

                Console.WriteLine($"Listen over {socket.LocalEndPoint}");

                while (true)
                {
                    var client = socket.Accept();
                    Task.Run(() =>
                    {
                        Console.WriteLine($"{client.RemoteEndPoint} connected successfully");

                        var length = 0;
                        var bytes = new byte[1024];

                        do
                        {
                            length = client.Receive(bytes);
                            var msg = Encoding.UTF8.GetString(bytes, 0, length);
                            Console.WriteLine($"CLIENT : {client.RemoteEndPoint} : {msg}");
                            if (msg == "exit")
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"CLIENT : {client.RemoteEndPoint} : disconnected");
                                Console.ResetColor();
                                client.Shutdown(SocketShutdown.Both);
                                client.Dispose();
                                break;
                            }
                            else if (msg == "time")
                            {
                                var result = DateTime.Now.ToLongDateString();
                                var bytesResult = Encoding.UTF8.GetBytes(result);
                                client.Send(bytesResult);
                            }

                        } while (true);
                    });
                }
            }
        }
    }


}
