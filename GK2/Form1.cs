using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GK2
{
    public partial class Form1 : Form
    {
        Bitmap drawArea;
        TriangleGrid grid;
        Vertex movingV;
        LambertColor lambert;
        Vector3 lightColor;
        Vector3 L;
        bool dragV;
        public Form1()
        {
            dragV = false;
            
            InitializeComponent();
            drawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            grid = new TriangleGrid(5, 5);
            lambert = new LambertColor(0.5, 0.5, new Vector3(0, 0, 1), new Vector3(0, 0, 1));
            lightColor = new Vector3(1, 0, 0);
            L = new Vector3(0, 0, 1);
            grid.CreateGrid(pictureBox1.Width, pictureBox1.Height, 50);
            UpdateArea();
        }

        void UpdateArea()
        {
            Bitmap newArea = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            
            foreach (Triangle tri in grid.Triangles)
            {
                tri.Fill(newArea, lambert, lightColor, L);
            }
            grid.Draw(newArea);
            pictureBox1.Image = newArea;
            pictureBox1.Refresh();
            drawArea = newArea;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                movingV = grid.GetVertex(e.Location);
                if(movingV != null)
                {
                    dragV = true;
                }               
            }
        }

        private int ConvertIToC(double i)
        {
            return (int)(((i + 1f) * 255f) / 2f);
        }

        private double ConvertCToI(double c)
        {
            return (2f * c / 255f - 1f);
        }

        private double AngleBetween(Vector3 A, Vector3 B)
        {
            return Vector3.Dot(A, B) / (A.Length() * B.Length());
        }

        private float CalculateI(int color)
        {
            double kd = 1;
            double ks = 1;
            int Il = 1;
            double Io = ConvertCToI(color);
            Vector3 L = new Vector3(0, 0, 1);
            Vector3 N = new Vector3(0, 0, 1);
            Vector3 V = new Vector3(0, 0, 1);
            double k = 2 * Vector3.Dot(N, L);
            Vector3 R = k * N * N - L;
            int m = 50;

            return (float)(kd * Il * Io * Vector3.Dot(N, L) + ks * Il * Io * Math.Pow(AngleBetween(V, R), m));
        }
        private Color MakeColor(Color color)
        {
            float R = CalculateI(color.R);
            float G = CalculateI(color.G);
            float B = CalculateI(color.B);

            Vector3 v = new Vector3(R, G, B);

            v.Normalize();

            return Color.FromArgb(ConvertIToC(v.X), ConvertIToC(v.Y), ConvertIToC(v.Z));
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            dragV = false;
        }
        private Point MapPointOnPictureBox(Point w)
        {
            Point p = w;
            if (p.X > pictureBox1.Width)
                p.X = pictureBox1.Width - 1;
            if (p.X < 0)
                p.X = 0;
            if (p.Y < 0)
                p.Y = 0;
            if (p.Y > pictureBox1.Height)
                p.Y = pictureBox1.Height - 1;
            return p;
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if(dragV)
            {
                Point p = MapPointOnPictureBox(e.Location);
                movingV.MoveOn(p);
                UpdateArea();
            }        
        }
    }
}
