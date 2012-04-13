using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace NetworkCheck
{
    public class NetworkCheck
    {
        List<string> _addresses = new List<string>();
        List<string> _recipients = new List<string>();

        public NetworkCheck(string addressFile, string recipientFile)
        {
            _addresses.AddRange(GetConfigFileToList(addressFile));
            _recipients.AddRange(GetConfigFileToList(recipientFile));
        }

        private IEnumerable<string> GetConfigFileToList(string addressFile)
        {
            string[] addresses = File.ReadAllLines(addressFile);
            foreach (string address in addresses)
            {
                if(address.Trim().Substring(0,2) != "//")
                    yield return address;
            }
        }

        public void Check()
        {
            foreach (string address in _addresses)
            {
                string output = Utils.GetCommandLineOutput(@"ping.exe", address);
                //string output = Utils.GetCommandLineOutput(Path.Combine(dir, @"ping.exe"), address);
                int lost = ParseOutputForLostCount(output);

                if (lost > 0)
                {
                    Console.WriteLine("Not found " + address);
                    SendMail(address, output);
                }
                else
                    Console.WriteLine("Found " + address);
            }
        }

        /// <summary>
        /// Parse the output of http-ping.exe and get hold of the count of lost packets
        /// </summary>
        private static int ParseOutputForLostCount(string output)
        {
            string[] lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            // Get the results line i.e. "Packets: Sent = 4, Received = 0, Lost = 4 (100% loss)"
            string resultLine = lines[8];
            // Parse out the lost count
            string lost = resultLine.Split(new string[] {", "}, StringSplitOptions.None)[2];
            lost = lost.Substring(lost.IndexOf("= ") + 2);
            lost = lost.Substring(0, lost.IndexOf(" "));
            return int.Parse(lost);
        }

        private void SendMail(string serverName, string output)
        {
            try
            {
                SmtpClient mail = new SmtpClient(Properties.Settings.Default.Host, Properties.Settings.Default.Port);

                using (MailMessage msg = new MailMessage())
                {
                    foreach (string recipient in _recipients)
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
