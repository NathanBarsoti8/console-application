using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Receive
{
    class MainClass
    {
        static void Main()
        {
            MainClass main = new MainClass();
            TcpListener server = null;
            TcpClient client = null;
            NetworkStream stream = null;

            Byte[] bytes = new Byte[701264];
            string data = null;

            try
            {
                Int32 port = 5000;
                IPAddress local = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(local, port);
                server.Start();

                while (true)
                {
                    Console.WriteLine("Trying to connect...");
                    client = server.AcceptTcpClient();
                    Console.WriteLine("Connected");
                    data = null;
                    stream = client.GetStream();
                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        //Translate data bytes to a ASCII string
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        data = data.ToUpper();

                    }

                    client.Close();
                }
            }
            catch (WebException exc)
            {
                Console.WriteLine("Connect failed", exc);
            }
            finally
            {
                server.Stop();
            }

        }
    }
}
