using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace HorrorTrojan
{
    public partial class BloodEffect : Form
    {
        public static int howmuch = 0;
        public static double pictrans = 0.99;

        private System.Windows.Forms.Timer effectTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer topMostTimer = new System.Windows.Forms.Timer();
        private Random rnd = new Random();

        private int maxx;
        private int maxy;
        private int dropdownxpos = 0;
        private int finishdrop = 0;
        private int kalinlik = 0;

        public BloodEffect()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "";
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.BackColor = Color.Maroon;
            this.TransparencyKey = Color.Maroon;
            this.ShowInTaskbar = false;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.WindowState = FormWindowState.Maximized;

            effectTimer.Interval = 30;
            effectTimer.Tick += EffectTimer_Tick;

            topMostTimer.Interval = 100;
            topMostTimer.Tick += (s, e) => { this.TopMost = true; };
            topMostTimer.Start();

            this.Load += BloodEffect_Load;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        private void BloodEffect_Load(object sender, EventArgs e)
        {
            maxx = this.Size.Width;
            maxy = this.Size.Height + 20;

            if (howmuch == 3)
            {
                howmuch = 4;
                pictrans = 0.99;
            }
            else if (howmuch == 2)
            {
                howmuch = 3;
                pictrans = 0.85;
                BloodEffect mf = new BloodEffect();
                mf.Show();
            }
            else if (howmuch == 1)
            {
                howmuch = 2;
                pictrans = 0.75;
                BloodEffect mf = new BloodEffect();
                mf.Show();
            }
            else if (howmuch == 0)
            {
                howmuch = 1;
                pictrans = 0.60;
                BloodEffect mf = new BloodEffect();
                mf.Show();
            }

            this.Opacity = pictrans;
            effectTimer.Start();
        }

        private void EffectTimer_Tick(object sender, EventArgs e)
        {
            int k = rnd.Next(0, 10);

            if (k < 3)
                sagaCiz();
            else if (k > 2 && k < 5)
                solaCiz();
            else if (k == 5 || k == 8)
                usteCiz();
            else if (k == 6 || k == 7)
                altaCiz();
            else if (k == 9)
                verticalDrop();
        }

        private void sagaCiz()
        {
            using (Graphics g = this.CreateGraphics())
            using (SolidBrush b = new SolidBrush(Color.FromArgb(rnd.Next(100, 255), Color.Red)))
            {
                int size = rnd.Next(3, 15);
                int pos = 0;
                int posType = rnd.Next(0, 11);

                if (posType < 5)
                    pos = rnd.Next(-5, 65);
                else if (posType > 4 && posType < 8)
                    pos = rnd.Next(65, 120);
                else if (posType == 8 || posType == 9)
                    pos = rnd.Next(120, 250);
                else if (posType == 10)
                    pos = rnd.Next(250, 500);

                Rectangle rect = new Rectangle(maxx - pos, rnd.Next(0, maxy - 50), size, rnd.Next(3, 15));

                if (rnd.Next(0, 2) == 1)
                    g.FillEllipse(b, rect);
                else
                    g.FillRectangle(b, rect);
            }
        }

        private void solaCiz()
        {
            using (Graphics g = this.CreateGraphics())
            using (SolidBrush b = new SolidBrush(Color.FromArgb(rnd.Next(100, 255), Color.Red)))
            {
                int size = rnd.Next(3, 15);
                int pos = 0;
                int posType = rnd.Next(0, 11);

                if (posType < 5)
                    pos = rnd.Next(-5, 65);
                else if (posType > 4 && posType < 8)
                    pos = rnd.Next(65, 120);
                else if (posType == 8 || posType == 9)
                    pos = rnd.Next(120, 250);
                else if (posType == 10)
                    pos = rnd.Next(250, 500);

                Rectangle rect = new Rectangle(pos, rnd.Next(0, maxy - 50), size, rnd.Next(3, 15));

                if (rnd.Next(0, 2) == 1)
                    g.FillEllipse(b, rect);
                else
                    g.FillRectangle(b, rect);
            }
        }

        private void usteCiz()
        {
            using (Graphics g = this.CreateGraphics())
            using (SolidBrush b = new SolidBrush(Color.FromArgb(rnd.Next(100, 255), Color.Red)))
            {
                int size = rnd.Next(3, 15);
                int pos = 0;
                int posType = rnd.Next(0, 11);

                if (posType < 5)
                    pos = rnd.Next(-5, 20);
                else if (posType > 4 && posType < 8)
                    pos = rnd.Next(20, 40);
                else if (posType == 8 || posType == 9)
                    pos = rnd.Next(40, 60);
                else if (posType == 10)
                    pos = rnd.Next(60, 100);

                Rectangle rect = new Rectangle(rnd.Next(20, maxx - 20), pos, size, rnd.Next(3, 15));

                if (rnd.Next(0, 2) == 1)
                    g.FillEllipse(b, rect);
                else
                    g.FillRectangle(b, rect);
            }
        }

        private void altaCiz()
        {
            using (Graphics g = this.CreateGraphics())
            using (SolidBrush b = new SolidBrush(Color.FromArgb(rnd.Next(100, 255), Color.Red)))
            {
                int size = rnd.Next(3, 15);
                int pos = 0;
                int posType = rnd.Next(0, 11);

                if (posType < 5)
                    pos = rnd.Next(-5, 20);
                else if (posType > 4 && posType < 8)
                    pos = rnd.Next(20, 40);
                else if (posType == 8 || posType == 9)
                    pos = rnd.Next(40, 60);
                else if (posType == 10)
                    pos = rnd.Next(60, 100);

                Rectangle rect = new Rectangle(rnd.Next(20, maxx - 20), maxy - pos, size, rnd.Next(3, 15));

                if (rnd.Next(0, 2) == 1)
                    g.FillEllipse(b, rect);
                else
                    g.FillRectangle(b, rect);
            }
        }

        private void verticalDrop()
        {
            dropdownxpos = rnd.Next(1, maxx - 5);
            finishdrop = rnd.Next(5, maxy - 20);
            kalinlik = rnd.Next(5, 25);

            ThreadPool.QueueUserWorkItem(dropit);
        }

        private void dropit(object state)
        {
            int startpos = dropdownxpos;
            int bitir = finishdrop;
            int kalinlikx = kalinlik;

            for (int a = 0; a < bitir; a++)
            {
                try
                {
                    using (Graphics g = this.CreateGraphics())
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(255, Color.Red)))
                    {
                        g.FillEllipse(b, new Rectangle(startpos, -20, kalinlikx, a));
                    }
                    Thread.Sleep(2);
                }
                catch { }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                effectTimer?.Dispose();
                topMostTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
