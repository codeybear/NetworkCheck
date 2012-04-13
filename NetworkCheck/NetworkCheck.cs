using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.Mail;

namespace NetworkCheck
{
    public class NetworkCheck
    {
        string[] _addresses;
        string[] _recipients;

        public NetworkCheck(string addressFile, string recipientFile) {
            _addresses = GetConfigFileToEnumerable(addressFile).ToArray();
            _recipients = GetConfigFileToEnumerable(recipientFile).ToArray();
        }

        /// <summary>
        /// Main methed to check the list of addresses
        /// </summary>
        public void Check() {
            foreach (string address in _addresses) {
                string output = Utils.GetCommandLineOutput(Properties.Settings.Default.Command, address);
                int lost = ParseOutputForLostCount(output);

                if (lost > 0) {
                    Console.WriteLine("Not found " + address);
                    Utils.SendMail(_recipients, address, output);
                } else
                    Console.WriteLine("Found " + address);
            }
        }

        private IEnumerable<string> GetConfigFileToEnumerable(string addressFile) {
            foreach (string address in File.ReadAllLines(addressFile)) {
                if (address.Trim().Substring(0, 2) != "//")
                    yield return address;
            }
        }

        /// <summary>
        /// Parse the output of http-ping.exe and get hold of the count of lost packets
        /// </summary>
        private static int ParseOutputForLostCount(string output) {
            string[] lines = output.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            // Get the results line i.e. "Packets: Sent = 4, Received = 0, Lost = 4 (100% loss)"
            string resultLine = lines.First(line => line.Trim() == "Packets:");
            // Parse out the lost count
            string lost = resultLine.Split(new string[] { ", " }, StringSplitOptions.None)[2];
            lost = lost.Substring(lost.IndexOf("= ") + 2);
            lost = lost.Substring(0, lost.IndexOf(" "));
            return int.Parse(lost);
        }
    }
}
