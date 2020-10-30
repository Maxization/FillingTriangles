using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public void Draw(Bitmap b)
        {
            using (Graphics g = Graphics.FromImage(b))
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
        public Color Color { get; set; }
        List<Edge> edges;

        public Triangle(Vertex A, Vertex B, Vertex C, Color c)
        {
            Edge e1 = new Edge(A, B);
            Edge e2 = new Edge(B, C);
            Edge e3 = new Edge(C, A);
            edges = new List<Edge>();
            edges.Add(e1);
            edges.Add(e2);
            edges.Add(e3);
            Color = c;
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

                //Increment ind
                ind++;
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

        public void Draw(Bitmap b)
        {
            foreach (Edge e in edges)
                e.Draw(b);
        }

        public void Fill(Bitmap b, Color c)
        {
            EdgeTable ET = createET();
            int y=0;
            int start = 0;
            for(int i=0;i<MAX;i++)
            {
                if(ET[i] != null)
                {
                    start = i;
                    break;
                }
            }
            y = start;
            NodeAET AET = null;

            AET = ET[y];
            while (AET != null) 
            {
                if(ET[y] != null && start != y)
                {
                    AET.Add(ET[y]);
                }  

                AET = AET.Sort();

                using (Graphics g = Graphics.FromImage(b))
                {
                    Pen pen = new Pen(c, 1);

                    NodeAET node1 = AET;
                    NodeAET node2 = AET.next;
                    while (node1 != null && node2 != null)
                    {
                        g.DrawLine(pen, (int)node1.X, y, (int)node2.X, y);
                        node1 = node2.next;
                        if(node1 != null)
                            node2 = node1.next;
                    }

                    pen.Dispose();
                }

                AET = AET.Delete(y);
                y++;
                if(AET != null)
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
        Random rnd;

        public TriangleGrid(int N, int M)
        {
            this.N = N;
            this.M = M;
            Triangles = new List<Triangle>();
            vertices = new Vertex[N + 1, M + 1];
            rnd = new Random();
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

        public void Draw(Bitmap b)
        {
            foreach(Triangle tri in Triangles)
            {
                tri.Draw(b);
            }
        }

        public Color RandomColor()
        {
            return Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
        }
        public void Fill(Bitmap b, Color color)
        {
            foreach(Triangle tri in Triangles)
            {
                tri.Fill(b, color);
            }
        }

        public void CreateGrid(int width, int height)
        {
            width -= 40;
            height -= 20;
            //N - wysokosc
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

                    Triangle tri = new Triangle(vertices[i, j], vertices[i, j + 1], vertices[i + 1, j], RandomColor());
                    Triangles.Add(tri);
                    if(j!=0)
                    {
                        tri = new Triangle(vertices[i, j], vertices[i, j + 1], vertices[i + 1, j], RandomColor());
                        Triangles.Add(tri);
                        tri = new Triangle(vertices[i, j], vertices[i + 1, j], vertices[i + 1, j - 1], RandomColor());
                        Triangles.Add(tri);
                        if (j == M - 1)
                        {
                            vertices[i + 1, j + 1] = new Vertex((j + 1) * w + 20, (i + 1) * h + 15);
                            tri = new Triangle(vertices[i + 1, j], vertices[i + 1, j + 1], vertices[i, j + 1], RandomColor());
                            Triangles.Add(tri);
                        }
                    }
                }
               
            }
        }
    }
}
