using GK2.Properties;
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
        Bitmap texture;
        Bitmap beforeMove;
        TriangleGrid grid;
        Triangle[] trianglesToUpdate;
        Vertex movingV;
        LambertColor lambert;
        Color objectColor;
        Vector3 lightColor;
        Vector3 L, N;
        bool dragV;
        bool objectsHaveColor;
        public Form1()
        {
            dragV = false;
            
            InitializeComponent();
            drawArea = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            texture = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            InitializeTexture(Resources.Funny_Cat);
            pictureBox1.Image = drawArea;
            grid = new TriangleGrid(5, 5);
            L = new Vector3(0, 0, 1);
            N = new Vector3(0, 0, 1);
            lambert = new LambertColor(1, 1, N, new Vector3(0, 0, 1));
            lightColor = new Vector3(1, 0, 0);
            objectColor = Color.White;
            objectsHaveColor = false;
            grid.CreateGrid(pictureBox1.Width, pictureBox1.Height, 1, objectColor, objectsHaveColor);
            UpdateArea();
        }

        Bitmap GetBitmapWithout(Triangle[] triangles)
        {
            Bitmap newArea = new Bitmap(texture);
            foreach(Triangle tri in grid.Triangles)
            {
                if(!triangles.Contains(tri))
                {
                    tri.Fill(newArea, lambert, lightColor, L);
                    tri.Draw(newArea);
                }
            }
            return newArea;
        }
        void UpdateArea()
        {
            Bitmap newArea = new Bitmap(texture);

            foreach (Triangle tri in grid.Triangles)
            {
                tri.Fill(newArea, lambert, lightColor, L);
            }
            grid.Draw(newArea);
            pictureBox1.Image = newArea;
            pictureBox1.Refresh();
            drawArea = newArea;
        }

        void UpdateTriangles(Triangle[] tri)
        {
            Bitmap newArea = new Bitmap(beforeMove);
            foreach (Triangle triangle in tri)
            {
                triangle.Fill(newArea, lambert, lightColor, L);
            }
            grid.Draw(newArea);
            pictureBox1.Image = newArea;
            pictureBox1.Refresh();
            drawArea = newArea;
        }

        void InitializeTexture(Bitmap b)
        {
            int width = b.Width;
            int height = b.Height;
            for(int i=0;i<drawArea.Height;i++)
            {
                for(int j=0;j<drawArea.Width;j++)
                {
                    Color color = b.GetPixel(j % width, i % height);
                    texture.SetPixel(j, i, color);
                }
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                movingV = grid.GetVertex(e.Location);
                if(movingV != null)
                {
                    dragV = true;
                    trianglesToUpdate = grid.GetTriangles(movingV);
                    beforeMove = GetBitmapWithout(trianglesToUpdate);
                }               
            }
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
                UpdateTriangles(trianglesToUpdate);
            }        
        }

        private void ColorLabel_Click(object sender, EventArgs e)
        {
            ColorDialog myDialog = new ColorDialog();

            myDialog.AllowFullOpen = true;

            if(myDialog.ShowDialog() == DialogResult.OK)
            {
                objectColor = myDialog.Color;
                ColorLabel.BackColor = objectColor;
            }

            grid.ChangeColor(objectColor);
            UpdateArea();
        }

        private void TextureLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    radioButtonTexture.Checked = true;
                    string filePath = openFileDialog.FileName;
                    Bitmap Texture = new Bitmap(filePath);
                    InitializeTexture(Texture);
                    UpdateArea();
                }
            }
        }

        private void radioButtonConstTexture_Click(object sender, EventArgs e)
        {
            objectsHaveColor = true;
            grid.SwitchColor(objectsHaveColor);
            UpdateArea();
        }

        private void radioButtonTexture_Click(object sender, EventArgs e)
        {
            objectsHaveColor = false;
            grid.SwitchColor(objectsHaveColor);
            UpdateArea();
        }
    }
}
