using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            Int32 port = 11000;
            UdpClient server = new UdpClient(port);
            IPEndPoint groupEp = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast...");

                    byte[] bytes = server.Receive(ref groupEp);
                    var size = bytes.Length;

                    DataToReceive data = FromBytes(bytes);
                    Console.WriteLine(data.Ip);
                    Console.WriteLine(data.Date);
                    Console.WriteLine(data.Url);
                    
                    

                    Console.WriteLine("Finished opp");
                    //client.Close();
                }
            }
            catch (WebException exc)
            {
                Console.WriteLine("Connect failed", exc);
            }
            finally
            {
                server.Close();
            }

        }

        public static DataToReceive FromBytes(byte[] arr)
        {
            DataToReceive receive = new DataToReceive();

            int size = Marshal.SizeOf(receive);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(arr, 0, ptr, size);

            receive = (DataToReceive)Marshal.PtrToStructure(ptr, receive.GetType());
            Marshal.FreeHGlobal(ptr);

            return receive;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DataToReceive
        {
            public DateTime Date;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 300)]
            public string Ip;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 300)]
            public string Url;
        }
    }
}
