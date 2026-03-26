using System;
using System.Drawing;
using System.Windows.Forms;
using System.Numerics;

namespace FractalGraphLab3
{
    public partial class Form1 : Form
    {
        private int width = 800;
        private int height = 600;
        private int maxIterations = 25;
        private double tolerance = 1e-6;

        private Complex[] roots;

        public Form1()
        {
            InitializeComponent();
            this.Size = new Size(width, height);
            this.Text = "Бассейн Ньютона";

            // Для z^3 - 1 = 0 корни: 1, e^(2pii/3), e^(4pii/3)
            roots = new Complex[]
            {
                new Complex(1, 0),
                new Complex(-0.5, Math.Sqrt(3)/2),
                new Complex(-0.5, -Math.Sqrt(3)/2)
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawNewtonBasin(e.Graphics);
        }

        private void DrawNewtonBasin(Graphics g)
        {
            Bitmap bitmap = new Bitmap(width, height);

            double scaleX = 3.0 / width;  
            double scaleY = 3.0 / height;  

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double re = (x - width / 2.0) * scaleX;
                    double im = (y - height / 2.0) * scaleY;
                    Complex z = new Complex(re, im);

                    int rootIndex = FindRoot(z, out int iterations);

                    Color color = GetColor(rootIndex, iterations);
                    bitmap.SetPixel(x, y, color);
                }
            }

            g.DrawImage(bitmap, 0, 0);
        }

        private int FindRoot(Complex z0, out int iterations)
        {
            Complex z = z0;
            iterations = 0;

            for (int i = 0; i < maxIterations; i++)
            {
                iterations = i;

       
                Complex f = Complex.Pow(z, 3) - 1;
                Complex fPrime = 3 * Complex.Pow(z, 2);

                if (Complex.Abs(fPrime) < tolerance)
                    break;

                Complex zNew = z - f / fPrime;

                if (Complex.Abs(zNew - z) < tolerance)
                {
                    z = zNew;
                    break;
                }

                z = zNew;
            }

            int rootIndex = -1;
            double minDistance = double.MaxValue;

            for (int i = 0; i < roots.Length; i++)
            {
                double distance = Complex.Abs(z - roots[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    rootIndex = i;
                }
            }

            return rootIndex;
        }

        private Color GetColor(int rootIndex, int iterations)
        {
            if (rootIndex == -1)
                return Color.Black;

            Color[] baseColors = new Color[]
            {
                Color.Red,
                Color.Green,
                Color.Blue,
                Color.Yellow,
                Color.Magenta,
                Color.Cyan
            };

            Color baseColor = baseColors[rootIndex % baseColors.Length];

            float factor = 1.0f - (float)iterations / maxIterations;
            return Color.FromArgb(
                (int)(baseColor.R * factor),
                (int)(baseColor.G * factor),
                (int)(baseColor.B * factor)
            );
        }
    }
}