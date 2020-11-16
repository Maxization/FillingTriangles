using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK2
{
    class TriangleNet
    {
        int N, M;
        public List<Triangle> Triangles { get; set; }
        Vertex[,] vertices;

        public TriangleNet(int N, int M)
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
            foreach (Triangle tri in Triangles)
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
            for (int i = 0; i < N + 1; i++)
            {
                for (int j = 0; j < M + 1; j++)
                {
                    if (Distance(vertices[i, j], p) < 10)
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
            foreach (Triangle triangle in Triangles)
            {
                if (triangle.Contain(v))
                    tri.Add(triangle);
            }
            return tri.ToArray();
        }

        public void Draw(DirectBitmap b)
        {
            foreach (Triangle tri in Triangles)
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

            for (int i = 0; i <= M; i++)
            {
                vertices[0, i] = new Vertex(i * w + 20, 15);
            }

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    vertices[i + 1, j] = new Vertex(j * w + 20, (i + 1) * h + 15);

                    Triangle tri = new Triangle(vertices[i, j], vertices[i, j + 1], vertices[i + 1, j], color, ownColor);
                    Triangles.Add(tri);
                    if (j != 0)
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
