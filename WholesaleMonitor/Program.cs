using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WholesaleMonitor.View;

namespace WholesaleMonitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GetSingleThread();

            MenuView menuView = new MenuView();

            Application.Run();

            //Application.Run(new Form1());
        }

        private static void GetSingleThread()
        {
            string name = Process.GetCurrentProcess().ProcessName;
            int id = Process.GetCurrentProcess().Id;
            Process[] prc = Process.GetProcesses();
            foreach (Process pr in prc)
            {
                if ((name == pr.ProcessName) && (pr.Id != id))
                {
                    MessageBox.Show(Application.ProductName + " is running.", Application.CompanyName + " " + Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    System.Environment.Exit(0);
                }
            }
        }
    }
}
