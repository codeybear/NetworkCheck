using System;
using System.IO;

namespace NetworkCheck
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            try {
                string addressFile = args.Length > 0 ? args[0] : Path.Combine(Environment.CurrentDirectory, "Addresses.txt");
                string recipientFile = args.Length > 1 ? args[1] : Path.Combine(Environment.CurrentDirectory, "Recipients.txt");

                NetworkCheck networkCheck = new NetworkCheck(addressFile, recipientFile);
                networkCheck.Check();
            }
            catch (Exception ex) {
                Utils.WriteException(ex);
            }
        }
    }
}
