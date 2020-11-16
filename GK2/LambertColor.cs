using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK2
{
    class LambertColor
    {
        public double Kd { get; set; }
        public double Ks { get; set; }
        public int M { get; set; }
        Vector3 V, constN, constL;
        public DirectBitmap NormalMap { get; set; }
        public bool ConstantN { get; set; }
        bool animated;
        public Vector3 LightPoint { get; set; }
        public LambertColor(double kd, double ks, int m, DirectBitmap normalMap, Vector3 V)
        {
            this.Kd = kd;
            this.Ks = ks;
            this.M = m;
            this.NormalMap = normalMap;
            this.V = V;
            constN = new Vector3(0, 0, 1);
            constL = new Vector3(0, 0, 1);
            LightPoint = new Vector3(0, 0, 0);
            ConstantN = false;
            animated = false;
        }
        Vector3 CreateL(int x, int y)
        {
            Vector3 l = new Vector3(LightPoint.X - x, LightPoint.Y - y, LightPoint.Z);
            l.Normalize();
            return l;
        }
       
        public Color GetValidColor(int R, int G, int B)
        {
            if (R < 0) R = 0;
            if (R > 255) R = 255;

            if (G < 0) G = 0;
            if (G > 255) G = 255;

            if (B < 0) B = 0;
            if (B > 255) B = 255;

            return Color.FromArgb(R, G, B);
        }
        public Color MakeColor(Color ObjColor, Vector3 Il,int x,int y)
        {
            Vector3 N = ConstantN ? constN : CreateN(x, y);
            Vector3 L = animated ? CreateL(x, y) : constL;
            Vector3 R = 2 * Vector3.Dot(N, L) * N - L;

            double Ior = ObjColor.R / 255f;
            double Iog = ObjColor.G / 255f;
            double Iob = ObjColor.B / 255f;
            double IR = Kd * Il.X * Ior * Vector3.Dot(N, L) + Ks * Il.X * Ior * Math.Pow(Vector3.Dot(V, R), M);
            double IG = Kd * Il.Y * Iog * Vector3.Dot(N, L) + Ks * Il.Y * Iog * Math.Pow(Vector3.Dot(V, R), M);
            double IB = Kd * Il.Z * Iob * Vector3.Dot(N, L) + Ks * Il.Z * Iob * Math.Pow(Vector3.Dot(V, R), M);

            if (IR < 0) IR = 0;
            if (IR > 1) IR = 1;

            if (IG < 0) IG = 0;
            if (IG > 1) IG = 1;

            if (IB < 0) IB = 0;
            if (IB > 1) IB = 1;

            return GetValidColor((int)(IR * 255f), (int)(IG * 255f), (int)(IB * 255f));
        }

        public Color MakeInterpolatedColor((Vertex, Color)[] k, double area, Vector3 Il, int x, int y)
        {

            double p1 = CalculateTriangleArea(new Point(x, y), k[0].Item1, k[1].Item1);
            double p2 = CalculateTriangleArea(new Point(x, y), k[2].Item1, k[1].Item1);
            double p3 = CalculateTriangleArea(new Point(x, y), k[2].Item1, k[0].Item1);

            double alfa = p1 / area;
            double beta = p2 / area;
            double gamma = p3 / area;

            int R = (int)(alfa * k[2].Item2.R + beta * k[0].Item2.R + gamma * k[1].Item2.R);
            int G = (int)(alfa * k[2].Item2.G + beta * k[0].Item2.G + gamma * k[1].Item2.G);
            int B = (int)(alfa * k[2].Item2.B + beta * k[0].Item2.B + gamma * k[1].Item2.B);

            return GetValidColor(R, G, B);
        }

        private double CalculateTriangleArea(Point A, Point B, Point C)
        {
            return Math.Abs((B.X - A.X) * (C.Y - A.Y) - (B.Y - A.Y) * (C.X - A.X)) / 2f;
        }
        public Vector3 CreateN(int x, int y)
        {
            Color color = NormalMap.GetPixel(x % NormalMap.Width, y % NormalMap.Height);
            Vector3 result = new Vector3(0, 0, 0);
            result.X = 2f * color.R / 255f - 1;
            result.Y = 2f * color.G / 255f - 1;
            result.Y = -result.Y;
            result.Z = color.B / 255f;
            result.Normalize();
            return result;
        }
        public void startAnimation()
        {
            animated = true;
        }
        public void endAnimation()
        {
            animated = false;
        }
    }
}
