using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkStream stream = null;

            try
            {
                Int32 port = 5000;
                TcpClient client = new TcpClient("127.0.0.1", port);

                Console.WriteLine("Cliente ------> Trying to connect, wait...");
                Console.WriteLine("Reading the doc, wait...");

                string text2 = File.ReadAllText(@"C:\Users\nathan.barsoti\Desktop\application\console-application\access (1).log");

                Byte[] data = Encoding.ASCII.GetBytes(text2);
                stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", data);
                Console.WriteLine("Size byte {0} in data send", data.Length);

                stream.Close();
                client.Close();
                Console.ReadKey();
            }

            catch (WebException exc)
            {Console.WriteLine("Failed ---> SEND", exc);}
        }

    }
}
