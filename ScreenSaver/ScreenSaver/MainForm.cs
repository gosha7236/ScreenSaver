using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScreenSaver.Properties;

namespace ScreenSaver
{
    /// <summary>
    /// основная форма
    /// </summary>
    public partial class MainForm : Form
    {
        Random rand = new Random();
        int snowCount = 40;
        Image snowflakeOriginal;
        List<Snowflake> snowflakes;
        List<Image> cachedSnowflakes;

        int width;
        int height;
        /// <summary>
        /// конструктор по умолчанию
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            width = this.ClientSize.Width;
            height = this.ClientSize.Height;

            this.SizeChanged += (s, e) =>
            {
                width = this.ClientSize.Width;
                height = this.ClientSize.Height;
            };

            snowflakeOriginal = Resources.snowFlake;

            CacheSnowflakes();   // кэшируем изображения
            CreateSnowflakes();  // создаём снежинки

            timer1.Interval = 50;
            timer1.Tick += timer1_Tick;
            timer1.Start();

            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };
            this.MouseClick += (s, e) => this.Close();
        }


        // 📦 КЭШИРУЕМ готовые снежинки
        private void CacheSnowflakes()
        {
            cachedSnowflakes = new List<Image>();
            for (int size = 10; size <= 40; size += 3)
            {
                Bitmap bmp = new Bitmap(size, size);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(snowflakeOriginal, 0, 0, size, size);
                }
                cachedSnowflakes.Add(bmp);
            }
        }

        private Image GetCachedSnowflake(int size)
        {
            return cachedSnowflakes[Math.Max(0, Math.Min(cachedSnowflakes.Count - 1, (size - 10) / 3))];
        }

        private void CreateSnowflakes()
        {
            snowflakes = new List<Snowflake>();

            for (int i = 0; i < snowCount; i++)
            {
                snowflakes.Add(new Snowflake
                {
                    Position = new PointF(rand.Next(0, width), rand.Next(0, height)),
                    Speed = (float)(rand.Next(15, 25) / 10.0),
                    Size = rand.Next(10, 40)
                });
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (var snowflake in snowflakes)
            {
                snowflake.Position = new PointF(
                    snowflake.Position.X + (float)Math.Sin(snowflake.Position.Y / 30) * 1,
                    snowflake.Position.Y + snowflake.Speed
                );

                if (snowflake.Position.Y > height)
                {
                    snowflake.Position = new PointF(rand.Next(0, width), -snowflake.Size);
                    snowflake.Speed = (float)(rand.Next(15, 25) / 10.0);
                    snowflake.Size = rand.Next(10, 40);
                }
            }

            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (var snowflake in snowflakes)
            {
                Image img = GetCachedSnowflake((int)snowflake.Size);
                e.Graphics.DrawImage(img, snowflake.Position.X, snowflake.Position.Y);
            }
        }
    }
}