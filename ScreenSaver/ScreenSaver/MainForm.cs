using System;
using System.Drawing;
using System.Windows.Forms;
using ScreenSaver.Properties;

namespace ScreenSaver
{
    public partial class MainForm : Form
    {
        Random rand = new Random();
        int snowCount = 150;        // количество снежинок
        PointF[] snowBalls;         // координаты снежинок
        int[] speed;                // скорость падения
        int[] size;                 // размер снежинок
        float[] oscillation;        // фаза колебаний для каждой снежинки
        Image snowflakeImage;       // изображение снежинки

        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None; // без рамки
            this.WindowState = FormWindowState.Maximized; // на весь экран
            this.TopMost = true; // поверх всех окон
            this.BackColor = Color.Black; // черный фон

            // Загружаем изображение снежинки
            snowflakeImage = Resources.snowFlake; // исправлено имя ресурса

            // Инициализация массивов
            snowBalls = new PointF[snowCount];
            speed = new int[snowCount];
            size = new int[snowCount];
            oscillation = new float[snowCount];

            // Создание снежинок по всему экрану
            CreateSnowflakes();

            // Настройка таймера
            timer1.Interval = 30; // плавное движение
            timer1.Tick += timer1_Tick;
            timer1.Start();

            this.KeyDown += MainForm_KeyDown;
            this.MouseClick += (s, e) => this.Close(); // выход по клику мыши
        }

        private void CreateSnowflakes()
        {
            int width = Screen.PrimaryScreen.Bounds.Width;
            int height = Screen.PrimaryScreen.Bounds.Height;

            for (int i = 0; i < snowCount; i++)
            {
                // ❄️ размещаем снежинки сразу по всему экрану
                snowBalls[i] = new PointF(rand.Next(0, width), rand.Next(0, height));
                speed[i] = rand.Next(2, 8);   // скорость падения
                size[i] = rand.Next(10, 25);  // размер снежинок
                oscillation[i] = rand.Next(0, 360); // случайная фаза колебаний
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int height = Screen.PrimaryScreen.Bounds.Height;
            int width = Screen.PrimaryScreen.Bounds.Width;

            for (int i = 0; i < snowCount; i++)
            {
                // движение вниз
                snowBalls[i].Y += speed[i];

                // немного колебаний по горизонтали для реализма
                oscillation[i] += 0.05f;
                snowBalls[i].X += (float)Math.Sin(oscillation[i]) * 1.5f;

                // если снежинка упала или улетела за границы — переносим наверх
                if (snowBalls[i].Y > height || snowBalls[i].X < -size[i] || snowBalls[i].X > width + size[i])
                {
                snowBalls[i].Y = -size[i];
                snowBalls[i].X = rand.Next(0, width);
                speed[i] = rand.Next(2, 8);
                size[i] = rand.Next(10, 25);
            }
        }

        Invalidate(); // перерисовать форму
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Очищаем экран
            e.Graphics.Clear(Color.Black);

            // Рисуем снежинки
            for (int i = 0; i < snowCount; i++)
            {
                // Проверяем, что изображение загружено
                if (snowflakeImage != null)
                {
                    e.Graphics.DrawImage(snowflakeImage,
                        snowBalls[i].X, snowBalls[i].Y, size[i], size[i]);
                }
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            this.Close(); // выход по любой клавише
        }
    }
}