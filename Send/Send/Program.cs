using System;
using System.IO;
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


            Int32 port = 110000;
            TcpClient client = new TcpClient("127.0.0.1", port);


            //string[] text = File.ReadAllLines(@"C:\Users\nathan.barsoti\Desktop\application\console-application\access (1).log");
            
            //string formatPattern = (@"(?<date>\d{10}.\d+)(\s{5}|\s{4})\d+(\s(?<ip>\d{3}\.\d{3}\.\d{2}\.(\d{3}|\d{2}))(?:[^h]*)(?<domain>[^\s]+))");

            //Regex rgx = new Regex(formatPattern);

            //for (int i = 0; i < text.Length; i++)
            //{
            //    if (rgx.IsMatch(text[i]))
            //    {
            //        var domain = rgx.Match(text[i]).Groups[6].Value;
            //        var ip = rgx.Match(text[i]).Groups[5].Value;
            //        var date = rgx.Match(text[i]).Groups[4].Value;

            //        double parseDouble = Double.Parse(date);
            //        DateTime r_date = new DateTime().AddSeconds(parseDouble);
            //    }
            //}

            string text2 = File.ReadAllText(@"C:\Users\nathan.barsoti\Desktop\application\console-application\access (1).log");

            Byte[] data = Encoding.ASCII.GetBytes(text2);
            stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent: {0}", data);
            Console.WriteLine("Size bye {0} in data send", data.Length);

            data = new Byte[data.Length];
            string responseData = String.Empty;
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);

            stream.Close();
            client.Close();
            Console.ReadKey();
        }

        //public struct DataToSend
        //{
        //    public DateTime Date { get; set; }
        //    public string Ip { get; set; }
        //    public string Domain { get; set; }
        //}
    }
}
