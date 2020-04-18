using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
                Console.WriteLine("Reading the doc, wait...");

                string[] text = File.ReadAllLines(@"C:\Users\nathan.barsoti\Desktop\application\console-application\access (1).log");
                string formatPattern = (@"(?<date>\d{10}.\d+)(\s{5}|\s{4})\d+(\s(?<ip>\d{3}\.\d{3}\.\d{2}\.(\d{3}|\d{2}))(?:[^h]*)(?<domain>[^\s]+))");

                Regex rgx = new Regex(formatPattern);

                for (int i = 0; i < text.Length; i++)
                {
                    if (rgx.IsMatch(text[i]))
                    {
                        var url = rgx.Match(text[i]).Groups[6].Value;
                        var ip = rgx.Match(text[i]).Groups[5].Value;
                        var date = rgx.Match(text[i]).Groups[4].Value;

                        double parseDouble = Double.Parse(date);
                        DateTime r_date = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(parseDouble);

                        DataToSend send = new DataToSend();
                        send.Date = r_date;
                        send.Ip = ip;
                        send.Url = url;

                        var bytes = GetBytes(send);

                        stream = client.GetStream();
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }

                stream.Close();
                client.Close();
                Console.ReadKey();
            }

            catch (WebException exc)
            {Console.WriteLine("Failed when trying to send", exc);}
        }

        public static byte[] GetBytes(DataToSend send)
        {
            int size = Marshal.SizeOf(send);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(send, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public struct DataToSend
        {
            public DateTime Date { get; set; }
            public string Ip { get; set; }
            public string Url { get; set; }
        }

    }
}
