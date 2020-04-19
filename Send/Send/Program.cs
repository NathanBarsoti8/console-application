using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Int32 port = 11000;
                UdpClient client = new UdpClient("127.0.0.1", port);
                Console.WriteLine("Reading the doc, wait...");

                using (StreamReader text = new StreamReader(@"C:\Users\nathan.barsoti\Desktop\application\console-application\access (1).log"))
                {
                    string formatPattern = (@"(?<date>\d{10}.\d+)(\s{5}|\s{4})\d+(\s(?<ip>\d{3}\.\d{3}\.\d{2}\.(\d{3}|\d{2}))(?:[^h]*)(?<domain>[^\s]+))");

                    Regex rgx = new Regex(formatPattern);

                    string x;
                    while ((x = text.ReadLine()) != null)
                    {
                        if (rgx.IsMatch(x))
                        {
                            var url = rgx.Match(x).Groups[6].Value;
                            var ip = rgx.Match(x).Groups[5].Value;
                            var date = rgx.Match(x).Groups[4].Value;

                            double parseDouble = Double.Parse(date);
                            DateTime r_date = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(parseDouble);

                            DataToSend send = new DataToSend();
                            send.Date = r_date;
                            send.Ip = ip;
                            send.Url = url;

                            var bytes = GetBytes(send);
                            client.Send(bytes, bytes.Length);
                        }
                    }

                    Console.WriteLine("Read all doc and transfered");

                    client.Close();
                    Console.ReadKey();
                }
            }

            catch (WebException exc)
            {
                Console.WriteLine("Failed when trying to send", exc);
            }
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
            public DateTime Date;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 300)]
            public string Ip;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 300)]
            public string Url;
        }

    }
}
