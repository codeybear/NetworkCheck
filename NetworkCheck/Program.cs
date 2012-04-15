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
                string exePath = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                string addressFile = args.Length > 0 ? args[0] : Path.Combine(exePath, "Addresses.txt");
                string recipientFile = args.Length > 1 ? args[1] : Path.Combine(exePath, "Recipients.txt");

                NetworkCheck networkCheck = new NetworkCheck(addressFile, recipientFile);
                networkCheck.Check();
            }
            catch (Exception ex) {
                Utils.WriteException(ex);
            }
        }
    }
}
