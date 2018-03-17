using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToolSL
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Logger.Info("Program start");
                String thisprocessname = Process.GetCurrentProcess().ProcessName;

                if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1)
                    return;

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                Logger.Error($"Error: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
        }
    }
}
