using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK2
{
    class Edge
    {
        public Vertex A, B;
        public Edge(Vertex a, Vertex b)
        {
            this.A = a;
            this.B = b;
        }

        public void Draw(DirectBitmap b)
        {
            using (Graphics g = Graphics.FromImage(b.Bitmap))
            {
                Pen pen = new Pen(Color.Black, 1);
                g.DrawLine(pen, A, B);
                pen.Dispose();
            }
        }
    }
}
