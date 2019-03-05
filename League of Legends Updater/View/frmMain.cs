#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace League_of_Legends_Updater.View
{
    public partial class frmMain : Form
    {
        private readonly BackgroundWorker bw = new BackgroundWorker();

        private readonly Process myProcess = new Process();
        private bool _isUpdate;

        public frmMain()
        {
            //this._isUpdate = isUpdate;
            InitializeComponent();
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString,
            int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString,
            int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private static WindowInfo[] GetAllDesktopWindows()
        {
            var wndList = new List<WindowInfo>();
            EnumWindows(delegate(IntPtr hWnd, int lParam)
            {
                var wnd = new WindowInfo();
                var sb = new StringBuilder(256);
                //get hwnd
                wnd.hWnd = hWnd;
                //get window name
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();
                //get window class
                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();
                Console.WriteLine("Window handle=" + wnd.hWnd.ToString().PadRight(20) + " szClassName=" +
                                  wnd.szClassName.PadRight(20) + " szWindowName=" + wnd.szWindowName);
                //add it into list
                wndList.Add(wnd);
                return true;
            }, 0);
            return wndList.ToArray();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker worker = sender as BackgroundWorker;

            KillAll();
            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = txtLocation.Text;
                //myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();
                // This code assumes the process you are starting will terminate itself. 
                // Given that is is started without a window so you cannot terminate it 
                // on the desktop, it must terminate itself or you can do it programmatically
                // from this application using the Kill method. 
                var lolWindow = new IntPtr(0);

                //在这里等待LOL 完全打开
                Thread.Sleep(15000);
                var allWindows = GetAllDesktopWindows();

                foreach (var info in allWindows)
                {
                    RECT srcRect;
                    if (GetWindowRect(info.hWnd, out srcRect))
                    {
                        var width = srcRect.Right - srcRect.Left;
                        if (width == 1024)
                            lolWindow = info.hWnd;
                    }
                }

                //IntPtr ParenthWnd = new IntPtr(0);
                //IntPtr hWnd = new IntPtr(0);

                //lolWindow = FindWindow(null, "LoL Patcher");

                if (lolWindow != IntPtr.Zero)
                {
                    WriteLog("Handler IntPtr: " +lolWindow.ToString());
                    RECT srcRect;
                    if (GetWindowRect(lolWindow, out srcRect))
                    {
                        var width = srcRect.Right - srcRect.Left;
                        var height = srcRect.Bottom - srcRect.Top;
                        WriteLog("width: " + width);
                        WriteLog("height: " + height);
                        var bmp = new Bitmap(width, height);
                        var screenG = Graphics.FromImage(bmp);

                        try
                        {
                            screenG.CopyFromScreen(srcRect.Left, srcRect.Top,
                                0, 0, new Size(width, height),
                                CopyPixelOperation.SourceCopy);
                            //这里可以保存
                            bmp.Save(Application.StartupPath + "\\lol.bmp", ImageFormat.Bmp);
                            Thread.Sleep(5000);

                            //直接处理内存BMP
                            var pixelColor = bmp.GetPixel(42, 42);
                            WriteLog("pixelColor.R: " + pixelColor.R);
                            WriteLog("pixelColor.G: " + pixelColor.G);
                            WriteLog("pixelColor.B: " + pixelColor.B);
                            //255 227 181 138
                            //255 224 181 137
                            //255 234 186 138
                            //WriteLog("COLOR: " + pixelColor.A + " " + pixelColor.R + " " + pixelColor.G + " " + pixelColor.B);
                            if (
                                pixelColor.R >= 147 && pixelColor.R <= 167 &&
                                pixelColor.G >= 106 && pixelColor.G <= 126 &&
                                pixelColor.B >= 44 && pixelColor.B <= 64)
                            {
                                //表示有LAUNCH 按钮 程序是需要更新的
                                WriteLog("Found Update Button");
                                _isUpdate = true;

                                //进入更新流程
                                while (!(
                                    pixelColor.R >= 147 && pixelColor.R <= 167 &&
                                    pixelColor.G >= 106 && pixelColor.G <= 126 &&
                                    pixelColor.B >= 44 && pixelColor.B <= 64))
                                {
                                    screenG.CopyFromScreen(srcRect.Left, srcRect.Top,
                                        0, 0, new Size(width, height),
                                        CopyPixelOperation.SourceCopy);
                                    pixelColor = bmp.GetPixel(42, 42);

                                    if (
                                        Process.GetProcessesByName(Path.GetFileNameWithoutExtension("LeagueClient"))
                                            .Length ==
                                        0 ||
                                        Process.GetProcessesByName(Path.GetFileNameWithoutExtension("BsSndRpt"))
                                            .Length >
                                        0)
                                    {
                                        //说明程序崩溃了
                                        _isUpdate = false;
                                        WriteLog("Application Crashed");
                                        foreach (
                                            var process in
                                            Process.GetProcessesByName(Path.GetFileNameWithoutExtension("BsSndRpt"))
                                        )
                                            try
                                            {
                                                process.Kill();
                                            }
                                            catch (Exception)
                                            {
                                                break;
                                            }
                                    }
                                }
                            }
                            else
                            {
                                WriteLog("Cannot Found Update");
                                _isUpdate = false;


                                
                            }


                            //MessageBox.Show("Color: " + pixelColor.A.ToString() + pixelColor.R + pixelColor.G + pixelColor.B);
                            //WriteLog("Color: "+ pixelColor.A.ToString() + pixelColor.R + pixelColor.G + pixelColor.B);
                        }
                        catch (Exception ex)
                        {
                            _isUpdate = false;
                            WriteLog("Application Crashed ERROR: " + ex.Message);
                            foreach (
                                var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("BsSndRpt")))
                                try
                                {
                                    process.Kill();
                                }
                                catch (Exception)
                                {
                                    break;
                                }
                        }
                        finally
                        {
                            screenG.Dispose();
                            bmp.Dispose();
                        }
                    }
                }
                else
                {
                    WriteLog("Cannot found window");
                    foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("BsSndRpt")))
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    KillAll();
                }


                //处理BMP
            }
            catch (Exception ex)
            {
                _isUpdate = false;
                foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("BsSndRpt")))
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception)
                    {
                        break;
                    }
                WriteLog("Application Crashed ERROR: " + ex.Message);
            }
        }

        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter">
        ///     A delegate that returns true for windows
        ///     that should be returned and false for windows that should
        ///     not be returned
        /// </param>
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //WriteLog("Launcher Check Completed");
            if (e.Error != null)
            {
                WriteLog("Error: " + e.Error.Message);
            }

            else
            {
                KillAll();
                if (_isUpdate)
                {
                    WriteLog("Update is processing");
                    //call update
                    //var processInfo = new ProcessStartInfo("cmd.exe", "/c" + Application.StartupPath + "\\LOLupdate.bat");
                    Process.Start(Application.StartupPath + "\\LOLupdate.bat");
                    //processInfo.CreateNoWindow = true;

                    //processInfo.UseShellExecute = true;

                    //processInfo.RedirectStandardError = true;
                    //processInfo.RedirectStandardOutput = true;

                    //var process = new Process();//.Start(processInfo);

                    //process.Start(processInfo);

                    //process.WaitForExit();

                    //string output = process.StandardOutput.ReadToEnd();
                    //string error = process.StandardError.ReadToEnd();
                    //var processInfo2 = new ProcessStartInfo("cmd.exe",
                    //    "/c" + Application.StartupPath + "\\LOLupdate2.bat");
                    Process.Start(Application.StartupPath + "\\LOLupdate2.bat");
                    //processInfo.CreateNoWindow = true;

                    //processInfo2.UseShellExecute = true;

                    //processInfo.RedirectStandardError = true;
                    //processInfo.RedirectStandardOutput = true;

                    //var process2 = new Process(); //Process.Start(processInfo2);

                    //process2.Start();

                    //process2.WaitForExit();
                    //WriteLog("Update Completed");
                    _isUpdate = false;


                    //string output = process.StandardOutput.ReadToEnd();
                    //string error = process.StandardError.ReadToEnd();
                }

                //清理临时文件
                /*
                foreach (va r filePath in Directory.GetFiles(@"C:\Users\K9998\AppData\Local\Temp\", "*.*",
                    SearchOption.AllDirectories))
                    try
                    {
                        var currentFile = new FileInfo(filePath);
                        currentFile.Delete();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error on file: {0}\r\n   {1}", filePath, ex.Message);
                    }
                    */
            }
        }


        private void cmdStart_Click(object sender, EventArgs e)
        {
            cmdStart.Enabled = false;
            if (!File.Exists(txtLocation.Text))
            {
                WriteLog("The League of Legends execution file is not correct");
                cmdStart.Enabled = true;
                return;
            }
            WriteLog("The task has been started");
            tmStart.Enabled = true;
        }

        private void cmdAbout_Click(object sender, EventArgs e)
        {
            var iAbout = new frmAbout();
            iAbout.ShowDialog();
        }

        private void cmdPath_Click(object sender, EventArgs e)
        {
            openFile.FileName = "";
            openFile.Filter = "League of Legends execution file|LeagueClient.exe";
            if (openFile.ShowDialog() == DialogResult.OK)
                txtLocation.Text = openFile.FileName;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted +=
                bw_RunWorkerCompleted;
            WriteLog("System Initialised");
        }

        private void WriteLog(string log)
        {
            lstLog.Items.Add(DateTime.Now + "  " + log);
            lstLog.SelectedIndex++;
        }

        private void tmStart_Tick(object sender, EventArgs e)
        {
            if (_isUpdate)
                return;
            if (bw.IsBusy != true)
                bw.RunWorkerAsync();
        }

        private void KillAll()
        {
            /*
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("LoLPatcherUx")))
            {
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    break;
                }
            }
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("LoLPatcher")))
            {
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    break;
                }
            }
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("LoLLauncher")))
            {
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    break;
                }
            }
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("rads_user_kernel")))
            {
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    break;
                }
            }
            */
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("LeagueClient")))
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    break;
                }
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("LeagueClientUx")))
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    break;
                }
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("LeagueClientUxRender"))
            )
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    break;
                }
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("LeagueClient.exe")))
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    break;
                }
            foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("LeagueClientUx.exe")))
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    break;
                }
            foreach (var process in Process.GetProcessesByName(
                Path.GetFileNameWithoutExtension("LeagueClientUxRender.exe")))
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    break;
                }
        }

        private void cmdTest_Click(object sender, EventArgs e)
        {
            try
            {
                //myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                ///myProcess.StartInfo.FileName = txtLocation.Text;
                //myProcess.StartInfo.CreateNoWindow = true;
                //myProcess.Start();
                // This code assumes the process you are starting will terminate itself. 
                // Given that is is started without a window so you cannot terminate it 
                // on the desktop, it must terminate itself or you can do it programmatically
                // from this application using the Kill method. 
                IntPtr lolWindow;

                //在这里等待LOL 完全打开
                Thread.Sleep(10000);
                //lolWindow = FindWindowByCaption(IntPtr.Zero, "LoL Patcher");
                lolWindow = FindWindowByCaption(IntPtr.Zero, "LoL Patcher");
                if (lolWindow != IntPtr.Zero)
                {
                    WriteLog(lolWindow.ToString());
                    RECT srcRect;
                    if (GetWindowRect(lolWindow, out srcRect))
                    {
                        var width = srcRect.Right - srcRect.Left;
                        var height = srcRect.Bottom - srcRect.Top;
                        WriteLog(width.ToString());
                        WriteLog(height.ToString());
                        var bmp = new Bitmap(width, height);
                        var screenG = Graphics.FromImage(bmp);

                        try
                        {
                            screenG.CopyFromScreen(srcRect.Left, srcRect.Top,
                                0, 0, new Size(width, height),
                                CopyPixelOperation.SourceCopy);
                            //这里可以保存
                            bmp.Save(Application.StartupPath + "\\lol.bmp", ImageFormat.Bmp);
                            Thread.Sleep(2000);

                            //直接处理内存BMP
                            var pixelColor = bmp.GetPixel(42, 42);
                            WriteLog(pixelColor.R.ToString());
                            WriteLog(pixelColor.G.ToString());
                            WriteLog(pixelColor.B.ToString());
                            //255 227 181 138
                            //255 224 181 137
                            //255 234 186 138
                            //WriteLog("COLOR: " + pixelColor.A + " " + pixelColor.R + " " + pixelColor.G + " " + pixelColor.B);
                            if (
                                pixelColor.R >= 147 && pixelColor.R <= 167 &&
                                pixelColor.G >= 106 && pixelColor.G <= 126 &&
                                pixelColor.B >= 44 && pixelColor.B <= 64)
                            {
                                //表示有LAUNCH 按钮 程序是不需要更新的
                                WriteLog("Found Launch Button");
                                _isUpdate = false;
                            }
                            else
                            {
                                WriteLog("Cannot Found Launch");
                                _isUpdate = true;


                                //进入更新流程
                                while (!(
                                    pixelColor.R >= 147 && pixelColor.R <= 167 &&
                                    pixelColor.G >= 106 && pixelColor.G <= 126 &&
                                    pixelColor.B >= 44 && pixelColor.B <= 64))
                                {
                                    screenG.CopyFromScreen(srcRect.Left, srcRect.Top,
                                        0, 0, new Size(width, height),
                                        CopyPixelOperation.SourceCopy);
                                    pixelColor = bmp.GetPixel(42, 42);

                                    if (
                                        Process.GetProcessesByName(Path.GetFileNameWithoutExtension("LeagueClient"))
                                            .Length ==
                                        0 ||
                                        Process.GetProcessesByName(Path.GetFileNameWithoutExtension("BsSndRpt"))
                                            .Length >
                                        0)
                                    {
                                        //说明程序崩溃了
                                        _isUpdate = false;
                                        WriteLog("Application Crashed");
                                        foreach (
                                            var process in
                                            Process.GetProcessesByName(Path.GetFileNameWithoutExtension("BsSndRpt"))
                                        )
                                            try
                                            {
                                                process.Kill();
                                            }
                                            catch (Exception)
                                            {
                                                break;
                                            }
                                    }
                                }
                            }


                            //MessageBox.Show("Color: " + pixelColor.A.ToString() + pixelColor.R + pixelColor.G + pixelColor.B);
                            //WriteLog("Color: "+ pixelColor.A.ToString() + pixelColor.R + pixelColor.G + pixelColor.B);
                        }
                        catch (Exception ex)
                        {
                            _isUpdate = false;
                            WriteLog("Application Crashed ERROR: " + ex.Message);
                            foreach (
                                var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("BsSndRpt")))
                                try
                                {
                                    process.Kill();
                                }
                                catch (Exception)
                                {
                                    break;
                                }
                        }
                        finally
                        {
                            screenG.Dispose();
                            bmp.Dispose();
                        }
                    }
                }
                else
                {
                    foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("BsSndRpt")))
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    KillAll();
                }


                //处理BMP
            }
            catch (Exception ex)
            {
                _isUpdate = false;
                foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("BsSndRpt")))
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception)
                    {
                        break;
                    }
                WriteLog("Application Crashed ERROR: " + ex.Message);
            }
        }

        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);

        public struct WindowInfo
        {
            public IntPtr hWnd;
            public string szWindowName;
            public string szClassName;
        }

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}