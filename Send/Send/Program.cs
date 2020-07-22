using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
                Int32 port = 11000;
                Console.WriteLine("Reading the doc, wait...");
                IPEndPoint groupEp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);

                TcpClient client = new TcpClient("127.0.0.1", port);
                var sendList = new List<Data>();

                using (StreamReader text = new StreamReader(@"C:\Users\nathan.barsoti\Desktop\application\console-application\access (1).log"))
                {
                    string formatPattern = (@"(?<date>\d{10}.\d+)(\s{5}|\s{4})\d+(\s(?<ip>\d{3}\.\d{3}\.\d{2}\.(\d{3}|\d{2}))(?:[^h]*)(?<domain>[^\s]+))");

                    Regex rgx = new Regex(formatPattern);

                    string x;
                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    while ((x = text.ReadLine()) != null)
                    {
                        if (rgx.IsMatch(x))
                        {
                            var url = rgx.Match(x).Groups[6].Value;
                            var ip = rgx.Match(x).Groups[5].Value;
                            var date = rgx.Match(x).Groups[4].Value;

                            double parseDouble = Double.Parse(date);
                            DateTime r_date = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(parseDouble);

                            Data send = new Data();
                            send.Date = r_date;
                            send.Ip = ip;
                            send.Url = url;

                            sendList.Add(send);
                        }
                    }

                    watch.Stop();
                    Console.WriteLine("Time (seconds) to read and parse: {0}", watch.ElapsedMilliseconds / 1000f);

                    var listConverted = ConvertToByte(sendList);

                    stream = client.GetStream();
                    stream.Write(listConverted, 0, listConverted.Length);

                    Console.WriteLine("Read all doc and transfered");

                    stream.Close();
                    client.Close();

                    Console.WriteLine("Connection is closed");
                }
            }

            catch (WebException exc)
            {
                Console.WriteLine("Failed when trying to send", exc);
            }
        }

        public static byte[] ConvertToByte(List<Data> list)
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            MemoryStream mStream = new MemoryStream();
            binFormat.Serialize(mStream, list);

            byte[] bytes = mStream.ToArray();
            mStream.Flush();

            return bytes;
        }
    }
}
