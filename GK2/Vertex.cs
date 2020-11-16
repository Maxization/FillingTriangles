using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK2
{
    class Vertex
    {
        public int X, Y;

        public Vertex(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static implicit operator Point(Vertex v)
        {
            return new Point(v.X, v.Y);
        }

        public static implicit operator Vertex(Point p)
        {
            return new Vertex(p.X, p.Y);
        }

        public void MoveOn(Point p)
        {
            X = p.X;
            Y = p.Y;
        }
    }
}
