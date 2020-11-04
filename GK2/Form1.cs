using GK2.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GK2
{
    public partial class Form1 : Form
    {
        DirectBitmap drawArea;
        DirectBitmap texture;
        DirectBitmap beforeMove;
        TriangleGrid grid;
        Triangle[] trianglesToUpdate;
        Vertex movingV;
        LambertColor lambert;
        Color objectColor;
        Vector3 lightColor;
        Vector3 L;
        bool dragV;
        bool objectsHaveColor;
        public Form1()
        {
            dragV = false;
            
            InitializeComponent();
            Bitmap bm = new Bitmap(Resources.Funny_Cat, pictureBox1.Width, pictureBox1.Height);
            texture = ConvertBitmap(bm);
            grid = new TriangleGrid(5, 5);
            L = new Vector3(0, 0, 1);
            bm = new Bitmap(Resources.normal_map, 250, 250);
            lambert = new LambertColor(0.5, 0.5, 0, ConvertBitmap(bm), new Vector3(0, 0, 1));
            lightColor = new Vector3(1, 1, 1);
            objectColor = Color.White;
            objectsHaveColor = false;
            grid.CreateGrid(pictureBox1.Width, pictureBox1.Height, objectColor, objectsHaveColor);
            drawArea = new DirectBitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = drawArea.Bitmap;
            UpdateArea();
        }

        DirectBitmap ConvertBitmap(Bitmap b)
        {
            DirectBitmap dBm = new DirectBitmap(b.Width, b.Height);

            using (Graphics g = Graphics.FromImage(dBm.Bitmap))
            {
                g.DrawImage(b, 0, 0);
            }
            return dBm;
        }

        DirectBitmap GetBitmapWithout(Triangle[] triangles)
        {
            DirectBitmap newArea = texture.Copy();
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
            //DirectBitmap newArea = new DirectBitmap(pictureBox1.Width, pictureBox1.Height);

            using(Graphics g = Graphics.FromImage(drawArea.Bitmap))
            {
                g.DrawImage(texture.Bitmap, 0, 0);
            }
            Parallel.ForEach(grid.Triangles, (tri) =>
             {
                 tri.Fill(drawArea, lambert, lightColor, L);
             });
            grid.Draw(drawArea);
            pictureBox1.Image = drawArea.Bitmap;
            pictureBox1.Invalidate();
        }

        void UpdateTriangles(Triangle[] tri)
        {
            DirectBitmap newArea = beforeMove.Copy();
            foreach (Triangle triangle in tri)
            {
                triangle.Fill(newArea, lambert, lightColor, L);
            }
            grid.Draw(newArea);
            pictureBox1.Image = newArea.Bitmap;
            pictureBox1.Refresh();
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

        private bool PickColor(out Color color)
        {
            ColorDialog myDialog = new ColorDialog();
            color = new Color();
            myDialog.AllowFullOpen = true;

            if (myDialog.ShowDialog() == DialogResult.OK)
            {
                color = myDialog.Color;
                return true;
            }
            return false;
        }
        private void ColorLabel_Click(object sender, EventArgs e)
        {
            Color newColor;
            if(PickColor(out newColor))
            {
                ColorLabel.BackColor = newColor;
                objectColor = newColor;
            }          
            grid.ChangeColor(objectColor);
            radioButtonConstTexture.Checked = true;
        }

        bool LoadImage(out DirectBitmap result, int width, int height)
        {
            result = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    Bitmap bm = new Bitmap(filePath);
                    bm = new Bitmap(bm, width, height);
                    result = ConvertBitmap(bm);
                    return true;
                }
            }
            return false;
        }

        private void TextureLoad_Click(object sender, EventArgs e)
        {
            DirectBitmap newTexture;
            if(LoadImage(out newTexture, pictureBox1.Width, pictureBox1.Height))
            {
                texture = newTexture;
                if(!radioButtonTexture.Checked)
                {
                    radioButtonTexture.Checked = true;
                }
                else
                {
                    UpdateArea();
                }
                
            }
        }

        private void normalMapLoad_Click(object sender, EventArgs e)
        {
            DirectBitmap newNormalMap;
            if(LoadImage(out newNormalMap, 300, 300))
            {
                lambert.ChangeNormalMap(newNormalMap);
                if (!radioButtonNormalMap.Checked)
                {
                    radioButtonNormalMap.Checked = true;
                }
                else
                {
                    UpdateArea();
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            TrackBar bar = sender as TrackBar;
            double value = bar.Value / 100f;
            lambert.ChangeKD(value);
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateArea();
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            TrackBar bar = sender as TrackBar;
            double value = bar.Value / 100f;
            lambert.ChangeKS(value);
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            TrackBar bar = sender as TrackBar;
            lambert.ChangeM(bar.Value);
        }

        private void LightColorLabel_Click(object sender, EventArgs e)
        {
            Color newColor;
            if(PickColor(out newColor))
            {
                LightColorLabel.BackColor = newColor;
                lightColor.X = newColor.R / 255f;
                lightColor.Y = newColor.G / 255f;
                lightColor.Z = newColor.B / 255f;
            }
            UpdateArea();
        }

        private void radioButtonNormalMap_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                lambert.ChangeConstantN(false);
                UpdateArea();
            }
        }

        private void radioButtonTexture_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if(button.Checked)
            {
                objectsHaveColor = false;
                grid.SwitchColor(objectsHaveColor);
                UpdateArea();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                lambert.ChangeConstantN(true);
                UpdateArea();
            }
        }

        private void radioButtonConstTexture_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (button.Checked)
            {
                objectsHaveColor = true;
                grid.SwitchColor(objectsHaveColor);
                UpdateArea();
            }
        }

        double SpriralX(double fi, double a)
        {
            double radians = fi * Math.PI / 180f;
            return a * fi * Math.Sin(radians) + pictureBox1.Width / 2f;
        }

        double SpiralY(double fi, double a)
        {
            double radians = fi * Math.PI / 180f;
            return a * fi * Math.Cos(radians) + pictureBox1.Height / 2f;
        }

        private void animateButton_Click(object sender, EventArgs e)
        {
            lambert.startAnimation();
            if (backgroundWorker1.IsBusy != true)
                backgroundWorker1.RunWorkerAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.WorkerSupportsCancellation == true)
                backgroundWorker1.CancelAsync();
            lambert.endAnimation();
            UpdateArea();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int delay = 100;
            double fi = 1090;
            double dfi = -3;
            double h = 60;
            double dh = -1;
            while (!worker.CancellationPending)
            {
                fi += dfi;
                h += dh;
                double x = SpriralX(fi, 0.3);
                double y = SpiralY(fi, 0.2);

                lambert.ChangeLightPoint(new Vector3((int)x, (int)y,Math.Abs((int)h)));
                UpdateArea();
                if (h< 25)
                {
                    dh = -dh;
                }
                if (h>100)
                {
                    dh = -dh;
                }
                if (fi < -1090)
                {
                    dfi = -dfi;
                }
                if(fi > 1090)
                {
                    dfi = -dfi;
                }

                Thread.Sleep(delay);
            }
            e.Cancel = true;
        }
    }
}
