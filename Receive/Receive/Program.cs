using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Receive
{
    class MainClass
    {
        static void Main()
        {
            MainClass main = new MainClass();
            Int32 port = 11000;

            TcpListener server = null;
            TcpClient client = null;
            NetworkStream stream = null;
            byte[] bytes = new Byte[16000000];
            List<Data> data = new List<Data>();

            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                server.Start();
                Console.WriteLine("Waiting for a connection...");

                client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                stream = client.GetStream();
                stream.Read(bytes, 0, bytes.Length);

                var memory = new MemoryStream();
                var binForm = new BinaryFormatter();
                memory.Write(bytes, 0, bytes.Length);
                memory.Position = 0;
                data = binForm.Deserialize(memory) as List<Data>;

                main.InsertInDb(data);
            }
            catch (WebException exc)
            {
                Console.WriteLine("Connect failed", exc);
            }
            finally
            {
                //TODO: método para acessar a url;
                stream.Close();
                client.Close();
            }

        }

        public void InsertInDb(List<Data> list)
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

                    foreach(Data data in list)
                    {
                        DataRow row = table.NewRow();

                        row["Id"] = Guid.NewGuid();
                        row["Ip"] = data.Ip;
                        row["Date"] = data.Date;
                        row["Url"] = data.Url;

                        table.Rows.Add(row);
                    }
                    
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

        //public static List<Data> ToClass(byte[] bytes)
        //{
        //    MemoryStream memStream = new MemoryStream();
        //    var binForm = new BinaryFormatter();
        //    memStream.Write(bytes, 0, bytes.Length);
        //    memStream.Seek(0, SeekOrigin.Begin);
        //    List<Data> obj = binForm.Deserialize(memStream) as List<Data>;
        //    return obj;
        //}


    }
}
