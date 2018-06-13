using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
        int sessionCounter = 0;
        private Options Options = new Options();

        private string AppDataFolder => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ToolSL\\";
        private string OptionsFilePath => $"{AppDataFolder}options.xml";
        private string HashFilePath => $"{AppDataFolder}filehashes.xml";

        private FileSystemWatcher TxtFileWatcher = new FileSystemWatcher();
        private FileSystemWatcher XmlFileWatcher = new FileSystemWatcher();

        private void LoadOptions()
        {
            if (Alphaleonis.Win32.Filesystem.File.Exists(OptionsFilePath))
                Options = Options.Load(OptionsFilePath);
        }

        public MainForm()
        {
            InitializeComponent();
            LoadOptions();
            Init();
        }

        private void NoConnectionClose()
        {
            Logger.Info("Connection problem");
            MessageBox.Show("Can't connect to import server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            allowedClosing = true;
            Close();
        }

        public void RegisterNewToken(bool closeProgram = false)
        {
            var answer = "Not allowed";
            var counter = 0;
            string login = string.Empty;
            string password = string.Empty;
            while (answer == "Not allowed")
            {
                counter++;
                if (counter == 4)
                {
                    if (closeProgram)
                    {
                        allowedClosing = true;
                        Close();
                    }
                    return;
                }
                var passform = new LoginForm();
                passform.SetData(login, password);
                var res = passform.ShowDialog(this);
                login = passform.Login;
                password = passform.Password;
                if (res == DialogResult.OK)
                {
                    try
                    {
                        answer = Remote.Service.RegisterToken(login, password, Utils.MachineToken, "USER");
                    }
                    catch (Exception ex)
                    {
                        if (ex is System.ServiceModel.EndpointNotFoundException)
                        {
                            MessageBox.Show("Server is not available", "Login problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            NoConnectionClose();
                        }
                        else
                        {
                            throw ex;
                        }
                    }
                }
                if (res == DialogResult.Cancel)
                {
                    if (closeProgram)
                    {
                        allowedClosing = true;
                        Close();
                    }
                    return;
                }
                if (answer == "Not allowed")
                {
                    MessageBox.Show("Wrong login or password, please try again. If nothing helps ask SpinLegends Team.", "Wrong password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
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
                RegisterNewToken(closeProgram:true);
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
                if (MessageBox.Show("A new version is available. Download new version now?", "New Version", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;

                    try
                    {
                        var name = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        var filename = string.Format($"{name}\\ToolSL_{maxVersion}_setup.msi");
                        var res = Remote.Service.GetUpdate(Utils.MachineToken);
                        Alphaleonis.Win32.Filesystem.File.WriteAllBytes(filename, res.Data);
                        if (MessageBox.Show($"ToolSL_{maxVersion}_setup.msi saved to desktop.{Environment.NewLine}Start installation now?", "New Version", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

            historyFolderTextBox.Text = Options.HistoryFolder;

            var mversion_message = string.Empty;
            if (productVersion.CompareTo(maxVersion) < 0)
            {
                mversion_message = $". Latest version is {version.MaxVersion}";
            }

            statusLabel.Text = $"Version: {Application.ProductVersion}{mversion_message}";
            autoStartCheckBox.Checked = Options.AutostartImport;

            if (Alphaleonis.Win32.Filesystem.File.Exists(HashFilePath))
                hashes = Alphaleonis.Win32.Filesystem.File.ReadAllLines(HashFilePath).ToList();

            string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\ToolSL.lnk";

            startupWindows.CheckedChanged -= startupWindows_CheckedChanged;
            startupWindows.Checked = Alphaleonis.Win32.Filesystem.File.Exists(shortcutAddress);
            startupWindows.CheckedChanged += startupWindows_CheckedChanged;

            if (Options.AutostartImport && Alphaleonis.Win32.Filesystem.Directory.Exists(Options.HistoryFolder))
                StartImport();

            isWorking = false;

            if (!string.IsNullOrEmpty(historyFolderTextBox.Text) && Options.AutostartImport)
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void PT_AccessWaring(string path)
        {
            MessageBox.Show($"Hand sender tool doesn't have access to '{path}'{Environment.NewLine}You need to start ToolSL with administator rights or select folder manualy", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void autoFindButton_Click(object sender, EventArgs e)
        {
            var path = $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\PokerTracker 4\\Config\\PokerTracker.cfg";
            try
            {
                var dir = FileSystem.AutofindFolder(path);
                if (dir != null)
                {
                    historyFolderTextBox.Text = dir;
                    Options.HistoryFolder = dir;
                    Options.Save(OptionsFilePath);
                }
                else
                {
                    PT_AccessWaring(path);
                }
            }
            catch (Exception ex)
            {
                PT_AccessWaring(path);
            }

        }

        private void selectButton_Click(object sender, EventArgs e)
        {           
            using (var ofd = new FolderBrowserDialog())
            {
                if (!string.IsNullOrEmpty(historyFolderTextBox.Text))
                    ofd.SelectedPath = historyFolderTextBox.Text;

                ofd.Description = "Select folder with histories from tracker or holdem manager";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        historyFolderTextBox.Text = ofd.SelectedPath;
                        Options.HistoryFolder = ofd.SelectedPath;
                        Options.Save(OptionsFilePath);
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
            Options.AutostartImport = autoStartCheckBox.Checked;
            Options.Save(OptionsFilePath);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            StartImport();
        }

        private async void StartImport()
        {
            stopButton.Enabled = true;
            startButton.Enabled = false;
            autoFindButton.Enabled = false;
            selectButton.Enabled = false;
            startContextmenuItem.Visible = false;
            stopcontextMenuItem.Visible = true;
            workAllowed = true;

            await Task.Factory.StartNew(() =>
            {
                AnalyseFileList(Alphaleonis.Win32.Filesystem.Directory.EnumerateFiles(Options.HistoryFolder, "*.*",
                                Alphaleonis.Win32.Filesystem.DirectoryEnumerationOptions.Recursive).ToList());

                TxtFileWatcher.Path = Options.HistoryFolder;
                TxtFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.Size;
                TxtFileWatcher.Filter = "*.txt";
                TxtFileWatcher.Created += FileWatcher_Changed;
                TxtFileWatcher.IncludeSubdirectories = true;

                XmlFileWatcher.Path = Options.HistoryFolder;
                XmlFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.Size;
                XmlFileWatcher.Filter = "*.xml";
                XmlFileWatcher.Created += FileWatcher_Changed;
                XmlFileWatcher.IncludeSubdirectories = true;

                TxtFileWatcher.EnableRaisingEvents = true;
                XmlFileWatcher.EnableRaisingEvents = true;
            });
        }

        private async void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var file = e.FullPath;

            await Task.Factory.StartNew(() =>
            {
                while (isWorking)
                    Thread.Sleep(500);

                var tempList = new List<string>();
                tempList.Add(file);
                AnalyseFileList(tempList);
            });
        }

        private void StopImport()
        {
            stopButton.Enabled = false;
            startButton.Enabled = true;
            autoFindButton.Enabled = true;
            selectButton.Enabled = true;
            startContextmenuItem.Visible = true;
            stopcontextMenuItem.Visible = false;
            workAllowed = false;

            TxtFileWatcher.Created -= FileWatcher_Changed;
            XmlFileWatcher.Created -= FileWatcher_Changed;
            TxtFileWatcher.EnableRaisingEvents = false;
            XmlFileWatcher.EnableRaisingEvents = false;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
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

        private void AnalyseFileList(List<string> list)
        {
            isWorking = true;

            var importList = new Dictionary<string,string>();
            var existedFileHashes = new List<string>();
        
            foreach (var filename in list)
            {
                if (!filename.ToLower().EndsWith(".txt") && !filename.ToLower().EndsWith(".xml"))
                    continue;

                var hash = GetHashCode(filename.Replace(Options.HistoryFolder.Trim(new char[] { '\\' }),string.Empty));
                existedFileHashes.Add(hash);
                if (!hashes.Contains(hash))
                    importList.Add(hash, filename);
            }

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

                var bytes = ReadFile(item.Value);
                if (bytes == null)
                    continue;
                var hash = GetDataHash(bytes);

                SendFile(hash, item.Key, item.Value, bytes);

                if (!Alphaleonis.Win32.Filesystem.File.Exists(HashFilePath))
                {
                    Alphaleonis.Win32.Filesystem.File.WriteAllText(HashFilePath, string.Empty);
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

            CreateTextIcon(0);
        }

        private byte[] ReadFile(string filename, int attemps = 10)
        {
            if (attemps == 0)
                return null;
            try
            {
                return Alphaleonis.Win32.Filesystem.File.ReadAllBytes(filename);
            }
            catch (Exception ex)
            {
                Thread.Sleep(500);
                return ReadFile(filename, attemps - 1);
            }
        }

        private void SendFile(string hash, string key, string value, byte[] bytes)
        {
            try
            {
                var result = Remote.Service.CheckFileHash(Utils.MachineToken, hash);
                if (result == "OK")
                {
                    if (Remote.Service.SendFile(Utils.MachineToken, hash,
                                                value.Replace(Options.HistoryFolder, string.Empty).Trim(new char[] { '\\' }),
                                                Utils.Compress(bytes)) == "OK")
                    {
                        Alphaleonis.Win32.Filesystem.File.AppendAllText(HashFilePath, key + Environment.NewLine);
                        hashes.Add(key);

                        sessionCounter++;
                        if (sessionCounter == 8)
                            sessionCounter = 0;

                        CreateTextIcon(sessionCounter);

                        notifyIcon.Text = $"ToolSL is working ({sessionCounter} files updloaded during this session)";
                    }
                }
                if (result == "Has file")
                {
                    Alphaleonis.Win32.Filesystem.File.AppendAllText(HashFilePath, key + Environment.NewLine);
                    hashes.Add(key);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Sending problem {ex.Message} : {ex.StackTrace}");
                Thread.Sleep(10000);
                SendFile(hash, key, value, bytes);
            }
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
                WindowState = FormWindowState.Minimized;
            }
        }

        private void changeAuthorizationButton_Click(object sender, EventArgs e)
        {            
            RegisterNewToken(closeProgram:false);
        }

        public static Icon GetIcon(int counter)
        {
            //Create bitmap, kind of canvas
            Bitmap bitmap = new Bitmap(32, 32);

            Icon icon = Properties.Resources.white_icon;

            SolidBrush drawBrush = new SolidBrush(Color.Red);

            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            graphics.DrawIcon(icon, 0, 0);

            //graphics.FillEllipse(new SolidBrush(Color.White), 0, 0, 32, 32);

            if (counter != 0)
            {
                graphics.DrawArc(new Pen(new SolidBrush(Color.LimeGreen),6), new Rectangle(2, 2, 27, 27), 45 * counter, 45 * (counter + 1));
            }

            Icon createdIcon = Icon.FromHandle(bitmap.GetHicon());

            drawBrush.Dispose();
            graphics.Dispose();
            bitmap.Dispose();

            return createdIcon;
        }

        public void CreateTextIcon(int counter)
        {
            try
            {
                if (notifyIcon != null)
                    notifyIcon.Icon = GetIcon(counter);
            }
            catch
            {

            }

        }
       
    }
}
