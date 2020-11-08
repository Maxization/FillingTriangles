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
    class EdgeTable
    {
        const int MAX = 1000;
        NodeAET[] ET;

        public int Count { get; private set; }

        public NodeAET this[int index]
        {
            get => ET[index];
            set => Add(index, value); 
        }

        private void Add(int index, NodeAET node)
        {
            Count++;
            if (ET[index] != null)
            {
                NodeAET it = ET[index];
                while (it.next != null)
                {
                    it = it.next;
                }
                it.next = node;
            }
            else
            {
                ET[index] = node;
            }
        }

        public void Delete(int index)
        {
            ET[index] = null;
            Count = Math.Max(0, Count - 1);
        }
        public EdgeTable()
        {
            ET = new NodeAET[MAX];
        }

        public void AddItem(int ind, NodeAET node)
        {
            ET[ind] = node;
        }
    }

    class NodeAET
    {
        public int Ymax;
        public double d,X;
        public NodeAET next;

        public NodeAET(int Ymax, double X, double d, NodeAET next)
        {
            this.Ymax = Ymax;
            this.X = X;
            this.d = d;
            this.next = next;
        }

        public void Add(NodeAET node)
        {
            NodeAET it = this;
            while (it.next != null)
            {
                it = it.next;
            }
            it.next = node;
        }

        public NodeAET Sort()
        {
            NodeAET head = this;
            NodeAET prev = this;
            NodeAET it = next;
            
            while(it != null)
            {
                if(it.X < prev.X)
                {
                    NodeAET itnext = it.next;
                    NodeAET itprev = prev.next;
                    if (it.X<head.X)
                    {
                        prev.next = it.next;
                        it.next = head;
                        head = it;
                    }
                    else
                    {
                        NodeAET mit = head.next;
                        NodeAET mprev = head;
                        while(mit!=null)
                        {
                            if(mit.X > it.X)
                            {
                                prev.next = it.next;
                                mprev.next = it;
                                it.next = mit;
                                break;
                            }
                            mit = mit.next;
                            mprev = mprev.next;
                        }
                    }
                    it = itnext;
                    prev = itprev;
                    continue;
                }
                it = it.next;
                prev = prev.next;
            }

            return head;
        }

        public NodeAET Delete(int y)
        {
            NodeAET head = this;
            while(head != null && head.Ymax == y)
            {
                head = head.next;
            }
            if (head == null) return head;

            NodeAET it = head.next;
            NodeAET prev = head;
            while (it != null)
            {
                if (it.Ymax == y)
                {
                    prev.next = it.next;

                }
                it = it.next;
                prev = prev.next;
            }

            return head;
        }

        public void UpdateX()
        {
            NodeAET it = this;
            while(it != null)
            {
                it.X = (it.X + it.d);
                it = it.next;
            }
        }
    }

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

        public EdgeTable createET()
        {
            EdgeTable ET = new EdgeTable();
            foreach(Edge e in edges)
            {
                int ind;
                double m;
                int Ymax;
                int Xmin;
                NodeAET node;
                if (e.A.Y == e.B.Y) continue;

                if (e.A.Y < e.B.Y)
                {
                    m = (double)(e.B.Y - e.A.Y) / (double)(e.B.X - e.A.X);
                    
                    Ymax = e.B.Y;
                    Xmin = e.A.X;
                    ind = e.A.Y;
                }
                else
                {
                    m = (double)(e.A.Y - e.B.Y) / (double)(e.A.X - e.B.X);
                    Ymax = e.A.Y;
                    Xmin = e.B.X;
                    ind = e.B.Y;
                }

                if (e.A.X == e.B.X)
                    m = 0;
                else
                    m = 1.0 / m;

                node = new NodeAET(Ymax, Xmin, m, null);
                if(ET[ind] != null)
                {
                    ET[ind].Add(node);
                }
                else
                {
                    ET[ind] = node;
                }
            }
            return ET;
        }

        public void Draw(DirectBitmap b)
        {
            foreach (Edge e in edges)
                e.Draw(b);
        }

        private double CalculateArea(Point A, Point B, Point C)
        {
            return Math.Abs((B.X - A.X) * (C.Y - A.Y) - (B.Y - A.Y) * (C.X - A.X)) / 2f;
        }

        public void FillInterpolation(DirectBitmap b, LambertColor lambert, Vector3 lightColor)
        {
            if(OwnColor)
            {
                Fill(b, lambert, lightColor);
                return;
            }

            EdgeTable ET = createET();
            double area = CalculateArea(vA, vB, vC);
            Color[] color = new Color[3];
            Vertex[] vertices = new Vertex[] { vA, vB, vC };
            for(int i=0;i<3;i++)
            {
                Color c = b.GetPixel(vertices[i].X, vertices[i].Y);
                int R = lambert.MakeColor(c.R, lightColor.X, vertices[i].X, vertices[i].Y);
                int G = lambert.MakeColor(c.G, lightColor.Y, vertices[i].X, vertices[i].Y);
                int B = lambert.MakeColor(c.B, lightColor.Z, vertices[i].X, vertices[i].Y);
                color[i] = Color.FromArgb(R, G, B);
            }
            
            int y = 0;
            for (int i = 0; i < MAX; i++)
            {
                if (ET[i] != null)
                {
                    y = i;
                    break;
                }
            }
            NodeAET AET = null;
            while (AET != null || ET.Count != 0)
            {
                if (ET[y] != null)
                {
                    if (AET == null)
                    {
                        AET = ET[y];
                    }
                    else
                    {
                        AET.Add(ET[y]);
                    }
                    ET.Delete(y);
                }

                AET = AET.Sort();

                NodeAET node1 = AET;
                NodeAET node2 = AET.next;
                while (node1 != null && node2 != null)
                {
                    int xMin = (int)node1.X;
                    int xMax = (int)node2.X;
                    for (int i = xMin; i <= xMax; i++)
                    {
                        double p1 = CalculateArea(new Point(i, y), vertices[0], vertices[1]);
                        double p2 = CalculateArea(new Point(i, y), vertices[2], vertices[1]);
                        double p3 = CalculateArea(new Point(i, y), vertices[2], vertices[0]);

                        p1 /= area;
                        p2 /= area;
                        p3 /= area;

                        int R = (int)(p1 * color[2].R + p2 * color[0].R + p3 * color[1].R);
                        int G = (int)(p1 * color[2].G + p2 * color[0].G + p3 * color[1].G);
                        int B = (int)(p1 * color[2].B + p2 * color[0].B + p3 * color[1].B);

                        if (R > 255) R = 255;
                        if (G > 255) G = 255;
                        if (B > 255) B = 255;

                        Color newColor = Color.FromArgb(R, G, B);
                        b.SetPixel(i, y, newColor);
                    }
                    node1 = node2.next;
                    if (node1 != null)
                        node2 = node1.next;
                }

                y++;
                AET = AET.Delete(y);

                if (AET != null)
                {
                    AET.UpdateX();
                }
            }
        }

        public void Fill(DirectBitmap b, LambertColor lambert, Vector3 lightColor)
        {
            EdgeTable ET = createET();

            int y=0;
            while(ET[y]!= null)
            {
                y++;
            }
            for(int i=0;i<MAX;i++)
            {
                if(ET[i] != null)
                {
                    y = i;
                    break;
                }
            }
            NodeAET AET = null;
            while (AET != null || ET.Count!=0) 
            {
                if(ET[y] != null)
                {
                    if(AET == null)
                    {
                        AET = ET[y];
                    }
                    else
                    {
                        AET.Add(ET[y]);
                    }
                    ET.Delete(y);
                }  

                AET = AET.Sort();

                NodeAET node1 = AET;
                NodeAET node2 = AET.next;
                while (node1 != null && node2 != null)
                {
                    int xMin = (int)node1.X;
                    int xMax = (int)node2.X;
                    for (int i = xMin; i <= xMax; i++)
                    {
                        Color color;
                        if (!OwnColor)
                        {
                            color = b.GetPixel(i, y);
                        }
                        else
                        {
                            color = Color;
                        }
                        int R = lambert.MakeColor(color.R, lightColor.X, i, y);
                        int G = lambert.MakeColor(color.G, lightColor.Y, i, y);
                        int B = lambert.MakeColor(color.B, lightColor.Z, i, y);

                        Color newColor = Color.FromArgb(R, G, B);
                        b.SetPixel(i, y, newColor);
                    }
                    node1 = node2.next;
                    if (node1 != null)
                        node2 = node1.next;
                }

                y++;
                AET = AET.Delete(y);

                if (AET != null)
                {
                    AET.UpdateX();
                }  
            }
        }
    }

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

    class TriangleGrid
    {
        int N, M;
        public List<Triangle> Triangles { get; set; }
        Vertex[,] vertices;

        public TriangleGrid(int N, int M)
        {
            this.N = N;
            this.M = M;
            Triangles = new List<Triangle>();
            vertices = new Vertex[N + 1, M + 1];
        }

        public void SwitchColor(bool b)
        {
            foreach (Triangle tri in Triangles)
            {
                tri.OwnColor = b;
            }
        }
        public void ChangeColor(Color color)
        {
            foreach(Triangle tri in Triangles)
            {
                tri.Color = color;
            }
        }

        private void Clear()
        {
            vertices = new Vertex[N + 1, M + 1];
            Triangles.Clear();
        }
        private double Distance(Point a, Point b)
        {
            return Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }
        public Vertex GetVertex(Point p)
        {
            for(int i=0;i<N+1;i++)
            {
                for(int j=0;j<M+1;j++)
                {
                    if(Distance(vertices[i,j],p) < 10)
                    {
                        return vertices[i, j];
                    }
                }
            }
            return null;
        }

        public Triangle[] GetTriangles(Vertex v)
        {
            List<Triangle> tri = new List<Triangle>();
            foreach(Triangle triangle in Triangles)
            {
                if (triangle.Contain(v))
                    tri.Add(triangle);
            }
            return tri.ToArray();
        }

        public void Draw(DirectBitmap b)
        {
            foreach(Triangle tri in Triangles)
            {
                tri.Draw(b);
            }
        }

        public void CreateGrid(int width, int height, Color color, bool ownColor)
        {
            Clear();
            width -= 40;
            height -= 20;

            int w = width / M;
            int h = height / N;

            for(int i=0;i<=M;i++)
            {
                vertices[0, i] = new Vertex(i * w + 20, 15);
            }

            for(int i=0;i<N;i++)
            {
                for(int j=0;j<M;j++)
                {
                    vertices[i + 1, j] = new Vertex(j * w +20, (i + 1) * h + 15);

                    Triangle tri = new Triangle(vertices[i, j], vertices[i, j + 1], vertices[i + 1, j], color, ownColor);
                    Triangles.Add(tri);
                    if(j!=0)
                    {
                        tri = new Triangle(vertices[i, j], vertices[i + 1, j], vertices[i + 1, j - 1], color, ownColor);
                        Triangles.Add(tri);
                        if (j == M - 1)
                        {
                            vertices[i + 1, j + 1] = new Vertex((j + 1) * w + 20, (i + 1) * h + 15);
                            tri = new Triangle(vertices[i + 1, j], vertices[i + 1, j + 1], vertices[i, j + 1], color, ownColor);
                            Triangles.Add(tri);
                        }
                    }
                }
            }
        }
    }
}
