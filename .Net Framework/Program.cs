using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.IO;
using System.Data.SqlClient;
using OSIsoft.AF;
using OSIsoft.AF.PI;
using OSIsoft.AF.Time;
using OSIsoft.AF.Asset;
using System.Globalization;
using System.Net;
using System.Collections.Generic;
using System;
namespace SMS
{
    class Program
    {
        public string pipointlist = null;
        static void Main(string[] args)
        {
            if (args[0] == "ReadSQLTable")
            {
                ReadSQLTable();
                return;
            }
            if (args[0] == "CheckAndSendMessage")
            {
                CheckAndSendMessage();
                return;
            }
            if (args[0] == "Help")
            {
                Console.WriteLine("Two possible arguments | SMS.exe CheckAndSendMessage | SMS.exe ReadSQLTable");
                return;
            }
            if (args[0] == "")
            {
                return;
            }
        }
        static void ReadSQLTable()
        {
            string connectionString = null;
            SqlConnection connection;
            SqlCommand command;
            SqlDataReader sReader;
            string sql = null;

            connectionString = "Data Source=<YourSqlServerInstance>;Initial Catalog=master;User ID=sa;Password=<Your_saPassword>";
            sql = "USE [master] EXEC('[dbo].[ReadSMSTable]')";
            connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                sReader = command.ExecuteReader();
                //string[] mytags = null;
                var i = 0;
                List<string> list = new List<string>();
                while (sReader.Read())
                {
                    list.Add(String.Format("{0}", sReader[0]));
                    i = i + 1;
                }
                String[] str = list.ToArray();
                Readpipoint(str);
                command.Dispose();
                connection.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Data);
            }
        }
        static void Readpipoint(string[] tags)
        {
            try
            {
                // piservs is a variable that we use to store all piservers collectives
                PIServers piservs = new PIServers();
                // piserv is a particular server that is pulled among the piservs collectives and which is its default server.
                PIServer piserv = piservs.DefaultPIServer;
                // Now we are going to initiate our first connection to this server through the method Connect.
                piserv.Connect();
                string stream = null;
                string streamend = null;
                string fullstream = null;
                var i = 1;
                foreach (var tag in tags)
                {
                    var mytag = PIPoint.FindPIPoint(piserv, tag);
                    var mytagtmp = mytag.CurrentValue().Timestamp.LocalTime.ToString();
                    var mytagval = mytag.CurrentValue().ToString();
                    stream = "(''" + mytag.ToString() + "'',''" + mytagtmp.ToString() + "'',''" + mytagval.ToString() + "''),";
                    streamend = "(''" + mytag.ToString() + "'',''" + mytagtmp.ToString() + "'',''" + mytagval.ToString() + "'')";
                    if (i < tags.Length)
                    {
                        fullstream = fullstream + stream;
                    }
                    else
                    {
                        fullstream = fullstream + streamend;
                    }
                    i = i + 1;
                }
                MergeSQLTable(fullstream);
                piserv.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        static void MergeSQLTable(string str)
        {
            string connectionString = null;
            SqlConnection connection;
            SqlCommand command;
            string sql = null;
            connectionString = "Data Source=<YourSqlServerInstance>;Initial Catalog=master;User ID=sa;Password=<Your_saPassword>";
            sql = "EXEC [dbo].[MergeTable] '" + str + "'";
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
                GenerateSMSQueueList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Data);
            }
        }
        static void GenerateSMSQueueList()
        {
            string connectionString = null;
            SqlConnection connection;
            SqlCommand command;

            string sql = null;
            connectionString = "Data Source=<YourSqlServerInstance>;Initial Catalog=master;User ID=sa;Password=<Your_saPassword>";
            sql = "EXEC [dbo].[GenerateSMSQueueList]";
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Data);
            }
        }
        static void CheckAndSendMessage()
        {
            string connectionString = null;
            SqlConnection connection;
            SqlCommand command;
            SqlDataReader sReader;
            string sql = null;
            //Here we use SQL authentication method using sa account, feel free to change to windows if required. 
            connectionString = "Data Source=<YourSqlServerInstance>;Initial Catalog=master;User ID=sa;Password=<Your_saPassword>";
            //The PISMSQueueList covers all 
            sql = "SELECT * FROM [SMS].[dbo].[PISmsQueueList]";
            connection = new SqlConnection(connectionString);
            string package = null;
            //we connect to the SQL Server Database Instance, and prepare the list of sms to be sent depending on sql table values property. 
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                sReader = command.ExecuteReader();
                while (sReader.Read())
                {
                    //check PI Bad values
                    if (sReader[2].ToString().ToLower().Contains("configure") == false || sReader[2].ToString().ToLower().Contains("error") == false || sReader[2].ToString().ToLower().Contains("bad") == false || sReader[2].ToString().ToLower().Contains("connect") == false || sReader[2].ToString().ToLower().Contains("scan") == false)
                    {
                        //more than
                        if (sReader[3].ToString() == ">" && sReader[7].ToString() == "ready")
                        {
                            if (float.Parse(sReader[2].ToString()) > float.Parse(sReader[4].ToString()))
                            {
                                package = package + sReader[6].ToString() + "|" + "Hello " + sReader[5].ToString() + ", " + sReader[0].ToString() + " " + sReader[3].ToString() + " " + sReader[4].ToString() + ", current value at " + sReader[1].ToString() + " is: " + sReader[2].ToString() + ". Thank you. PIHelp@ambatovy.mg" + "|" + "" + sReader[0].ToString() + "|" + sReader[3].ToString() + "|" + sReader[4].ToString() + "|" + sReader[5].ToString() + "$";
                            }
                        }
                        if (sReader[3].ToString() == ">" && sReader[7].ToString() == "sent")
                        {
                            if (float.Parse(sReader[2].ToString()) < float.Parse(sReader[4].ToString()))
                            {
                                UpdatePISmsQueueList("READY", sReader[0].ToString(), sReader[3].ToString(), sReader[4].ToString(), sReader[5].ToString());
                            }
                        }
                        //less than
                        if (sReader[3].ToString() == "<" && sReader[7].ToString() == "ready")
                        {
                            if (float.Parse(sReader[2].ToString()) < float.Parse(sReader[4].ToString()))
                            {
                                package = package + sReader[6].ToString() + "|" + "Hello " + sReader[5].ToString() + ", " + sReader[0].ToString() + " " + sReader[3].ToString() + " " + sReader[4].ToString() + ", current value at " + sReader[1].ToString() + " is: " + sReader[2].ToString() + ". Thank you. PIHelp@ambatovy.mg" + "|" + "" + sReader[0].ToString() + "|" + sReader[3].ToString() + "|" + sReader[4].ToString() + "|" + sReader[5].ToString() + "$";
                            }
                        }
                        if (sReader[3].ToString() == "<" && sReader[7].ToString() == "sent")
                        {
                            if (float.Parse(sReader[2].ToString()) > float.Parse(sReader[4].ToString()))
                            {
                                UpdatePISmsQueueList("READY", sReader[0].ToString(), sReader[3].ToString(), sReader[4].ToString(), sReader[5].ToString());
                            }
                        }
                        //inside
                        if (sReader[3].ToString() == "inside" && sReader[7].ToString() == "ready")
                        {
                            string[] vartrigger = null;
                            vartrigger = sReader[4].ToString().Split(',');

                            if ((float.Parse(sReader[2].ToString()) > float.Parse(vartrigger[0].ToString())) && (float.Parse(sReader[2].ToString()) < float.Parse(vartrigger[1].ToString())))
                            {
                                package = package + sReader[6].ToString() + "|" + "Hello " + sReader[5].ToString() + ", " + sReader[0].ToString() + " " + sReader[3].ToString() + " " + sReader[4].ToString() + ", current value at " + sReader[1].ToString() + " is: " + sReader[2].ToString() + ". Thank you. PIHelp@ambatovy.mg" + "|" + "" + sReader[0].ToString() + "|" + sReader[3].ToString() + "|" + sReader[4].ToString() + "|" + sReader[5].ToString() + "$";
                            }
                        }
                        if (sReader[3].ToString() == "inside" && sReader[7].ToString() == "sent")
                        {
                            string[] vartrigger = null;
                            vartrigger = sReader[4].ToString().Split(',');

                            if ((float.Parse(sReader[2].ToString()) < float.Parse(vartrigger[0].ToString())) || (float.Parse(sReader[2].ToString()) > float.Parse(vartrigger[1].ToString())))
                            {
                                UpdatePISmsQueueList("READY", sReader[0].ToString(), sReader[3].ToString(), sReader[4].ToString(), sReader[5].ToString());
                            }
                        }
                        //outside
                        if (sReader[3].ToString() == "outside" && sReader[7].ToString() == "ready")
                        {
                            string[] vartrigger = null;
                            vartrigger = sReader[4].ToString().Split(',');

                            if ((float.Parse(sReader[2].ToString()) < float.Parse(vartrigger[0].ToString())) || (float.Parse(sReader[2].ToString()) > float.Parse(vartrigger[1].ToString())))
                            {
                                package = package + sReader[6].ToString() + "|" + "Hello " + sReader[5].ToString() + ", " + sReader[0].ToString() + " " + sReader[3].ToString() + " " + sReader[4].ToString() + ", current value at " + sReader[1].ToString() + " is: " + sReader[2].ToString() + ". Thank you. PIHelp@ambatovy.mg" + "|" + "" + sReader[0].ToString() + "|" + sReader[3].ToString() + "|" + sReader[4].ToString() + "|" + sReader[5].ToString() + "$";
                            }
                        }
                        if (sReader[3].ToString() == "outside" && sReader[7].ToString() == "sent")
                        {
                            string[] vartrigger = null;
                            vartrigger = sReader[4].ToString().Split(',');

                            if ((float.Parse(sReader[2].ToString()) > float.Parse(vartrigger[0].ToString())) && (float.Parse(sReader[2].ToString()) < float.Parse(vartrigger[1].ToString())))
                            {
                                UpdatePISmsQueueList("READY", sReader[0].ToString(), sReader[3].ToString(), sReader[4].ToString(), sReader[5].ToString());
                            }
                        }
                        //equal to
                        if (sReader[3].ToString() == "=" && sReader[7].ToString() == "ready")
                        {
                            if (sReader[2].ToString() == sReader[4].ToString())
                            {
                                package = package + sReader[6].ToString() + "|" + "Hello " + sReader[5].ToString() + ", " + sReader[0].ToString() + " " + sReader[3].ToString() + " " + sReader[4].ToString() + ", current value at " + sReader[1].ToString() + " is: " + sReader[2].ToString() + ". Thank you. PIHelp@ambatovy.mg" + "|" + "" + sReader[0].ToString() + "|" + sReader[3].ToString() + "|" + sReader[4].ToString() + "|" + sReader[5].ToString() + "$";
                            }
                        }
                        if (sReader[3].ToString() == "=" && sReader[7].ToString() == "sent")
                        {
                            if (sReader[2].ToString() != sReader[4].ToString())
                            {
                                UpdatePISmsQueueList("READY", sReader[0].ToString(), sReader[3].ToString(), sReader[4].ToString(), sReader[5].ToString());
                            }
                        }
                    }
                }
                SendSMS(package.ToString());
                command.Dispose();
                connection.Close();
                return;
            }
            catch (Exception)
            {
                return;
            }
        }
        static void UpdatePISmsQueueList(string Status, string TagName, string Operator, string TriggerCondition, string UserName)
        {
            string connectionString = null;
            SqlConnection connection;
            SqlCommand command;
            string sql = null;
            connectionString = "Data Source=<YourSqlServerInstance>;Initial Catalog=master;User ID=sa;Password=<Your_saPassword>";
            if (Status == "OK")
            {
                sql = "UPDATE [SMS].[dbo].[PISmsQueueList] SET SMS_Status = 'sent' WHERE PIPoint = '" + TagName + "' AND Operator = '" + Operator + "' AND TriggerCondition = '" + TriggerCondition + "' AND UserName = '" + UserName + "'";
            }
            if (Status == "READY")
            {
                sql = "UPDATE [SMS].[dbo].[PISmsQueueList] SET SMS_Status = 'ready' WHERE PIPoint = '" + TagName + "' AND Operator = '" + Operator + "' AND TriggerCondition = '" + TriggerCondition + "' AND UserName = '" + UserName + "'";
            }
            connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
                command.Dispose();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update Issue " + ex.Data);
            }
        }
        static void SendSMS(string package)
        {
            string pack = null;
            pack = package.ToString().Remove(package.Length - 1);
            string[] messages = null;
            string[] submessages = null;
            messages = pack.Split('$');
            foreach (var message in messages)
            {
                try
                {
                    submessages = message.ToString().Split('|');
                    goto Reload;
                Reload:
                    var SMS_Port = new SerialPort();
	     //my portName is COM3, please feel free to change to fit yours.
                    SMS_Port.PortName = "COM3";
                    SMS_Port.BaudRate = 115200;
                    SMS_Port.Parity = Parity.None;
                    SMS_Port.DataBits = 8;
                    SMS_Port.StopBits = StopBits.One;
                    SMS_Port.Handshake = Handshake.None;
                    SMS_Port.DtrEnable = true;
                    SMS_Port.RtsEnable = true;

                    SMS_Port.Open();
                    SMS_Port.WriteLine("AT" + "\r\n");
                    System.Threading.Thread.Sleep(200);
                    SMS_Port.WriteLine("AT+CMGS=\"" + submessages[0].ToString() + "\"\r\n");
                    System.Threading.Thread.Sleep(200);
                    SMS_Port.WriteLine(submessages[1].ToString() + "\x1A");
                    System.Threading.Thread.Sleep(3000);
                    var responsestr = "";
                    responsestr = SMS_Port.ReadExisting().ToString();
                    SMS_Port.Close();

                    if (responsestr.Contains("ERROR"))
                    {
                        goto Reload;
                    }
                    else
                    {
                        UpdatePISmsQueueList("OK", submessages[2].ToString(), submessages[3].ToString(), submessages[4].ToString(), submessages[5].ToString());
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }
    }
}