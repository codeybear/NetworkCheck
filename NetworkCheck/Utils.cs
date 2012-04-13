using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Windows.Forms;

namespace NetworkCheck
{
    public class Utils
    {
        /// <summary>
        /// Get the output from a command line program
        /// </summary>
        public static string GetCommandLineOutput(string command, string arguments)
        {
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = arguments;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
        }

        /// <summary>
        /// Write a message to the event log
        /// </summary>
        public static void WriteToEventApplicationLog(string source, string message)
        {
            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, "Application");

            EventLog.WriteEntry(source, message);
            
        }

        /// <summary>
        /// Attempt to write a message to the event log, on fail write to a local file
        /// </summary>
        public static void WriteException(Exception ex)
        {
            string message = ex.Message + Environment.NewLine +
                             "Callstack:" + Environment.NewLine +
                             ex.StackTrace;

            try
            {
                // Write to system event log
                WriteToEventApplicationLog(Application.ProductName, message);                
            }
            catch
            {
                // Unable to write to event log, write to a text file
                System.IO.File.AppendAllText("ErrorLog.txt", 
                                             Environment.NewLine +
                                             DateTime.Now + 
                                             Environment.NewLine + 
                                             message);
            }
        }

        /// <summary>
        /// Send failure message to a list of recipients
        /// </summary>
        public static void SendMail(string[] recipients, string serverName, string output)
        {
            try
            {
                SmtpClient mail = new SmtpClient(Properties.Settings.Default.Host, Properties.Settings.Default.Port);

                using (MailMessage msg = new MailMessage())
                {
                    foreach (string recipient in recipients)
                        msg.To.Add(recipient);

                    msg.From = new MailAddress("do-not-reply@english-heritage.org.uk");
                    msg.Subject = "Website Notification.";
                    msg.Body = "The network monitoring tool cannot connect to the following server - " +
                               serverName + Environment.NewLine + Environment.NewLine +
                               "Ping output:" + Environment.NewLine +
                               output;

                    mail.Send(msg);
                }
            }
            catch
            {
                Console.WriteLine("SendMail failed");
                throw;
            }
        }
    }
}
