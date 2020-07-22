using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Send
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

                MemoryStream memory = new MemoryStream();
                var binForm = new BinaryFormatter();
                binForm.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                memory.Write(bytes, 0, bytes.Length);
                memory.Seek(0, SeekOrigin.Begin);
                List<Data> data = (List<Data>)binForm.Deserialize(memory);

                main.InsertInDb(data);
            }
            catch (WebException exc)
            {
                Console.WriteLine("Connect failed", exc);
            }
            finally
            {
                //TODO: enviar email;
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

                    foreach (Data data in list)
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

                    Stopwatch watch = new Stopwatch();
                    connection.Open();
                    watch.Start();
                    bulkCopy.WriteToServer(table);
                    connection.Close();
                    watch.Stop();
                    Console.WriteLine("Time (seconds) to save in db: {0}", watch.ElapsedMilliseconds / 1000f);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Error => {0}", exc);
                }

            }
        }

     
    }
}
