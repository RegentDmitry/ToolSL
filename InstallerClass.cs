using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.IO;

namespace ToolSL
{
    [RunInstaller(true)]
    public class InstallerClass : Installer
    {
        public InstallerClass() : base()
        {
            Committed += new InstallEventHandler(MyInstaller_Committed);
        }

        private void MyInstaller_Committed(object sender, InstallEventArgs e)
        {
            try
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\ToolSL.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Can't autostart ToolSL: {ex.Message}","error",MessageBoxButtons.OK);
            }
        }
    }
}
