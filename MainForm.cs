using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace ToolSL
{
    public partial class MainForm : Form
    {
        private bool isWorking = false;
        private bool workAllowed = false;
        private List<string> hashes = new List<string>();
        bool allowedClosing = false;

        private string HashFilePath
        {
            get
            {
                var folder = Alphaleonis.Win32.Filesystem.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                return $"{folder}\\filehashes.xml";
            }
        }

        public MainForm()
        {
            InitializeComponent();
            Init();
        }

        private void NoConnectionClose()
        {
            Logger.Info("Connection problem");
            MessageBox.Show("Can't connect to import server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            allowedClosing = true;
            Close();
        }

        public void Init()
        {
            isWorking = true;
            var allowedConnection = false;
            try
            {
                allowedConnection = Remote.Service.CheckConnection(Utils.MachineToken, "USER");
            }
            catch (Exception ex)
            {
                NoConnectionClose();
            }

            if (!allowedConnection)
            {
                var answer = "Not allowed";
                var counter = 0;
                while (answer == "Not allowed")
                {
                    counter++;
                    if (counter == 4)
                    {
                        allowedClosing = true;
                        Close();
                        return;
                    }
                    var passform = new LoginForm();
                    if (passform.ShowDialog(this) == DialogResult.OK)
                    {
                        var login = passform.Login;
                        var password = passform.Password;
                        try
                        {
                            answer = Remote.Service.RegisterToken(login, password, Utils.MachineToken, "USER");
                        }
                        catch (Exception ex)
                        {
                            if (ex is System.ServiceModel.EndpointNotFoundException)
                            {
                                NoConnectionClose();
                            }
                            else
                            {
                                throw ex;
                            }
                        }
                    }
                }
            }

            hhService.ClientVersion version = null;
            try
            {
                version = Remote.Service.GetVersion();
            }
            catch (Exception ex)
            {
                if (ex is System.ServiceModel.EndpointNotFoundException)
                {
                    NoConnectionClose();
                }
                else
                {
                    throw ex;
                }
            }

            var productVersion = new Version(Application.ProductVersion);
            var minVersion = new Version(version.MinVersion);
            var maxVersion = new Version(version.MaxVersion);
            if (productVersion.CompareTo(minVersion) < 0)
            {
                if (MessageBox.Show("You version of tool is not supported. Please update", "Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;

                    try
                    {
                        var res = Remote.Service.GetUpdate(Utils.MachineToken);
                        var filename = System.IO.Path.GetTempFileName().Replace(".tmp", ".msi");
                        Alphaleonis.Win32.Filesystem.File.WriteAllBytes(filename, res.Data);
                        Process.Start(filename);
                    }
                    catch (Exception ex)
                    {
                        if (ex is System.ServiceModel.EndpointNotFoundException)
                        {
                            NoConnectionClose();
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                    
                    Cursor = Cursors.Default;
                }
                    
                allowedClosing = true;
                Close();
            }

            historyFolderTextBox.Text = Properties.Settings.Default.HistoryFolder;

            var mversion_message = string.Empty;
            if (productVersion.CompareTo(maxVersion) < 0)
            {
                mversion_message = $". Latest version is {version.MaxVersion}";
            }

            statusLabel.Text = $"Version: {Application.ProductVersion}{mversion_message}";
            autoStartCheckBox.Checked = Properties.Settings.Default.autostartImport;

            if (Alphaleonis.Win32.Filesystem.File.Exists(HashFilePath))
                hashes = Alphaleonis.Win32.Filesystem.File.ReadAllLines(HashFilePath).ToList();

            string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\ToolSL.lnk";

            startupWindows.CheckedChanged -= startupWindows_CheckedChanged;
            startupWindows.Checked = Alphaleonis.Win32.Filesystem.File.Exists(shortcutAddress);
            startupWindows.CheckedChanged += startupWindows_CheckedChanged;

            if (Properties.Settings.Default.autostartImport && Alphaleonis.Win32.Filesystem.Directory.Exists(Properties.Settings.Default.HistoryFolder))
                StartImport();
            isWorking = false;

            //Logger.Info("program started");
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void autoFindButton_Click(object sender, EventArgs e)
        {
            var dir = FileSystem.AutofindFolder();
            if (dir != null)
            {
                historyFolderTextBox.Text = dir;
                Properties.Settings.Default.HistoryFolder = dir;
                Properties.Settings.Default.Save();
            }

        }

        private void selectButton_Click(object sender, EventArgs e)
        {           
            using (var ofd = new FolderBrowserDialog())
            {
                ofd.Description = "Select folder with histories from tracker or holdem manager";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        historyFolderTextBox.Text = ofd.SelectedPath;
                        Properties.Settings.Default.HistoryFolder = ofd.SelectedPath;
                        Properties.Settings.Default.Save();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void autoStartCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.autostartImport = autoStartCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (!timer.Enabled)
                StartImport();
        }

        private void StartImport()
        {
            if (timer.Enabled)
                return;

            timer.Start();
            stopButton.Enabled = true;
            startButton.Enabled = false;
            autoFindButton.Enabled = false;
            selectButton.Enabled = false;
            startContextmenuItem.Visible = false;
            stopcontextMenuItem.Visible = true;
            workAllowed = true;
        }

        private void StopImport()
        {
            if (!timer.Enabled)
                return;

            timer.Stop();
            stopButton.Enabled = false;
            startButton.Enabled = true;
            autoFindButton.Enabled = true;
            selectButton.Enabled = true;
            startContextmenuItem.Visible = true;
            stopcontextMenuItem.Visible = false;
            workAllowed = false;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (timer.Enabled)
                StopImport();
        }

        private static string GetHashCode(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var code = Convert.ToBase64String(hash);
            return code;
        }

        private static string GetDataHash(byte[] data)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(data);
            return Convert.ToBase64String(hash);
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            if (isWorking)
                return;

            if (!Alphaleonis.Win32.Filesystem.Directory.Exists(Properties.Settings.Default.HistoryFolder))
            {
                StopImport();
                return;
            }

            await Task.Factory.StartNew(() =>
            {
                isWorking = true;
                var importList = new Dictionary<string,string>();
                var existedFileHashes = new List<string>();
                foreach (var filename in Alphaleonis.Win32.Filesystem.Directory.EnumerateFiles(Properties.Settings.Default.HistoryFolder, "*.*", 
                                                                      Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.Recursive))
                {
                    var hash = GetHashCode(filename.Replace(Properties.Settings.Default.HistoryFolder.Trim(new char[] { '\\' }),string.Empty));
                    existedFileHashes.Add(hash);
                    if (!hashes.Contains(hash))
                        importList.Add(hash, filename);
                }

                var removingHashes = new List<string>();
                foreach (var h in hashes)
                {
                    if (!existedFileHashes.Contains(h))
                        removingHashes.Add(h);
                }
                if (removingHashes.Count != 0)
                {
                    hashes.RemoveAll(item => removingHashes.Contains(item));
                    Alphaleonis.Win32.Filesystem.File.WriteAllLines(HashFilePath, hashes);
                }

                foreach (var h in removingHashes)
                    hashes.Remove(h);

                progressBar.Invoke((MethodInvoker)delegate
                {
                    progressBar.Maximum = importList.Count();
                    progressBar.Value = 0;
                });

                var counter = 0;
                foreach (var item in importList)
                {
                    counter++;

                    processLabel.Invoke((MethodInvoker)delegate
                    {
                        processLabel.Text = $"File {counter} of {importList.Count}";
                    });
                    
                    if (!workAllowed)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            progressBar.Value = 0;
                        });
                        break;
                    }

                    if (!Alphaleonis.Win32.Filesystem.File.Exists(HashFilePath))
                    {
                        Alphaleonis.Win32.Filesystem.File.WriteAllText(HashFilePath, string.Empty);
                    }

                    var bytes = Alphaleonis.Win32.Filesystem.File.ReadAllBytes(item.Value);
                    var hash = GetDataHash(bytes);

                    try
                    {
                        var result = Remote.Service.CheckFileHash(Utils.MachineToken, hash);
                        if (result == "OK")
                        {
                            if (Remote.Service.SendFile(Utils.MachineToken, hash,
                                                        item.Value.Replace(Properties.Settings.Default.HistoryFolder, string.Empty).Trim(new char[] { '\\' }),
                                                        Utils.Compress(bytes)) == "OK")
                            {
                                Alphaleonis.Win32.Filesystem.File.AppendAllText(HashFilePath, item.Key + Environment.NewLine);
                                hashes.Add(item.Key);
                            }
                        }
                        if (result == "Has file")
                        {
                            Alphaleonis.Win32.Filesystem.File.AppendAllText(HashFilePath, item.Key + Environment.NewLine);
                            hashes.Add(item.Key);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is System.ServiceModel.EndpointNotFoundException)
                        {
                            NoConnectionClose();
                        }
                        else
                        {
                            throw ex;
                        }
                    }

                    progressBar.Invoke((MethodInvoker)delegate
                    {
                        progressBar.Value++;
                    });
                }
                Invoke((MethodInvoker)delegate
                {
                    processLabel.Text = string.Empty;
                });

                isWorking = false;
            });
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(500);
                Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
            }
        }

        private void activateMainWindow()
        {
            Show();
            Visible = true;
            WindowState = FormWindowState.Normal;
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            activateMainWindow();
        }

        private void openMainFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activateMainWindow();
        }

        private void startContextmenuItem_Click(object sender, EventArgs e)
        {
            StartImport();
        }

        private void stopcontextMenuItem_Click(object sender, EventArgs e)
        {
            StopImport();
        }
        
        private void closeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            allowedClosing = true;
            Close();
        }

        private void startupWindows_CheckedChanged(object sender, EventArgs e)
        {
            string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\ToolSL.lnk";
            if (Alphaleonis.Win32.Filesystem.File.Exists(shortcutAddress))
                Alphaleonis.Win32.Filesystem.File.Delete(shortcutAddress);
            else
            {
                object shDesktop = "ToolSL";
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
                shortcut.Description = "Shortcut for SpinLegends tool";
                shortcut.TargetPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                shortcut.Save();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowedClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}
