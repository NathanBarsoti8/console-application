using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Receive
{
    class MainClass
    {
        static void Main()
        {
            MainClass main = new MainClass();
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


                    int i = 1;
                    Console.WriteLine("Finished opp");
                    Console.WriteLine("count");
                    Console.WriteLine(i++);
                    //client.Close();
                    main.InsertInDb(data);
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

        public void InsertInDb(DataToReceive data)
        {
            string connectionString = @"Data Source = N092\SQLEXPRESS;" +
                                      "Integrated Security=true;" +
                                      "Initial Catalog = APPLICATION;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    DataTable table = new DataTable();
                    table.Columns.Add("Id", typeof(Guid));
                    table.Columns.Add("Ip", typeof(string));
                    table.Columns.Add("Date", typeof(DateTime));
                    table.Columns.Add("Url", typeof(string));

                    DataRow row = table.NewRow();

                    row["Id"] = Guid.NewGuid();
                    row["Ip"] = data.Ip;
                    row["Date"] = data.Date;
                    row["Url"] = data.Url;

                    table.Rows.Add(row);
                    
                    SqlBulkCopy bulkCopy = new SqlBulkCopy(connection);
                    bulkCopy.DestinationTableName = "Info";
                    bulkCopy.ColumnMappings.Add("Ip", "Ip");
                    bulkCopy.ColumnMappings.Add("Date", "Date");
                    bulkCopy.ColumnMappings.Add("Url", "Url");

                    connection.Open();
                    bulkCopy.WriteToServer(table);
                    connection.Close();
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Error => {0}", exc);
                }

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
