using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Schema;

namespace GK2
{
    class Triangle
    {
        const int MAX = 1000;
        List<Edge> edges;
        Vertex vA, vB, vC;
        public bool OwnColor { get; set; }
        public Color Color { get; set; }
        public Triangle(Vertex A, Vertex B, Vertex C, Color c, bool ownColor)
        {
            Edge e1 = new Edge(A, B);
            Edge e2 = new Edge(B, C);
            Edge e3 = new Edge(C, A);
            edges = new List<Edge>();
            edges.Add(e1);
            edges.Add(e2);
            edges.Add(e3);
            Color = c;
            this.OwnColor = ownColor;
            this.vA = A;
            this.vB = B;
            this.vC = C;
        }

        public bool Contain(Vertex v)
        {
            foreach(Edge e in edges)
            {
                if (e.A == v || e.B == v) return true;
            }
            return false;
        }

        public (List<NodeAET>[],int) createET()
        {
            int edgeCounter = 0;
            List<NodeAET>[] ET = new List<NodeAET>[MAX];
            foreach(Edge e in edges)
            {
                int ind;
                double m;
                int Ymax;
                int Xmin;
                NodeAET node;

                if (e.A.Y == e.B.Y) continue;

                edgeCounter++;
                if (e.A.Y < e.B.Y)
                {
                    m = (double)(e.B.X - e.A.X) / (double)(e.B.Y - e.A.Y);
                    Ymax = e.B.Y;
                    Xmin = e.A.X;
                    ind = e.A.Y;
                }
                else
                {
                    m = (double)(e.A.X - e.B.X) / (double)(e.A.Y - e.B.Y);
                    Ymax = e.A.Y;
                    Xmin = e.B.X;
                    ind = e.B.Y;
                }

                node = new NodeAET(Ymax, Xmin, m, null);
                if (ET[ind] != null)
                    ET[ind].Add(node);
                else
                    ET[ind] = new List<NodeAET> { node };
            }
            return (ET, edgeCounter);
        }

        public void Draw(DirectBitmap b)
        {
            foreach (Edge e in edges)
                e.Draw(b);
        }

        private double CalculateArea()
        {
            return Math.Abs((vB.X - vA.X) * (vC.Y - vA.Y) - (vB.Y - vA.Y) * (vC.X - vA.X)) / 2f;
        }

        public void FillInterpolation(DirectBitmap b, LambertColor lambert, Vector3 lightColor)
        {
            List<NodeAET>[] ET;
            int edgeCounter;
            (ET, edgeCounter) = createET();

            (Vertex, Color)[] data = new (Vertex, Color)[3];
            data[0].Item1 = vA;
            data[1].Item1 = vB;
            data[2].Item1 = vC;
            double area = CalculateArea();

            for (int i = 0; i < 3; i++)
            {
                data[i].Item2 = lambert.MakeColor(b.GetPixel(data[i].Item1.X, data[i].Item1.Y), lightColor, data[i].Item1.X, data[i].Item1.Y);
            }

            int y = 0;
            while (ET[y] == null)
            {
                y++;
                if (y == MAX) return;
            }

            List<NodeAET> AET = new List<NodeAET>();
            while (AET.Any() || edgeCounter != 0)
            {
                if (ET[y] != null)
                {
                    AET.AddRange(ET[y]);
                    edgeCounter -= ET[y].Count;
                }

                AET.Sort((NodeAET q, NodeAET p) =>
                {
                    if (q.X == p.X) return 0;
                    else if (q.X < p.X) return -1;
                    else return 1;
                });

                for (int i = 0; i < AET.Count; i += 2)
                {
                    int xMin = (int)AET[i].X;
                    int xMax = (int)AET[i + 1].X;

                    for (int x = xMin; x <= xMax; x++)
                    {
                        Color newColor = lambert.MakeInterpolatedColor(data, area, lightColor, x, y);
                        b.SetPixel(x, y, newColor);
                    }
                }

                y++;
                AET.RemoveAll(node => node.Ymax == y);

                foreach (NodeAET node in AET)
                {
                    node.X += node.d;
                }
            }
        }

        public void Fill(DirectBitmap b, LambertColor lambert, Vector3 lightColor)
        {
            List<NodeAET>[] ET;
            int edgeCounter;
            (ET, edgeCounter) = createET();

            int y = 0;
            while(ET[y] == null)
            {
                y++;
                if (y == MAX) return;
            }

            List<NodeAET> AET = new List<NodeAET>();
            while (AET.Any() || edgeCounter!=0) 
            {
                if(ET[y] != null)
                {
                    AET.AddRange(ET[y]);
                    edgeCounter -= ET[y].Count;
                }

                AET.Sort((NodeAET q, NodeAET p) =>
                {
                    if (q.X == p.X) return 0;
                    else if (q.X < p.X) return -1;
                    else return 1;
                });

                for(int i=0;i<AET.Count;i+=2)
                {
                    int xMin = (int)AET[i].X;
                    int xMax = (int)AET[i + 1].X;

                    for(int x=xMin;x<=xMax;x++)
                    {
                        Color color;
                        if (!OwnColor)
                        {
                            color = b.GetPixel(x, y);
                        }
                        else
                        {
                            color = Color;
                        }

                        Color newColor = lambert.MakeColor(color, lightColor, x, y);
                        b.SetPixel(x, y, newColor);
                    }
                }

                y++;
                AET.RemoveAll(node => node.Ymax == y);

                foreach(NodeAET node in AET)
                {
                    node.X += node.d;
                }
            }
        }
    }

}
