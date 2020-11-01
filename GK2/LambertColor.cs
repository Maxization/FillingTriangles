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
        double kd, ks; // <0;1>
        int m;
        Vector3 V, constN;
        Bitmap normalMap;
        bool constantN;

        public LambertColor(double kd, double ks, int m, Bitmap normalMap, Vector3 V)
        {
            this.kd = kd;
            this.ks = ks;
            this.m = m;
            this.normalMap = new Bitmap(normalMap, 250, 250);
            this.V = V;
            constN = new Vector3(0, 0, 1);
            constantN = false;
        }
        public void ChangeConstantN(bool constant)
        {
            constantN = constant;
        }

        public int MakeColor(int ObjColor, Vector3 L, double Il,int x,int y)
        {
            Vector3 N;
            if (constantN)
            {
                N = constN;
            }
            else
            {
                N = CreateN(x, y);
            }
             
            Vector3 R = 2 * Vector3.Dot(N, L) * N - L;
            double Io = ObjColor / 255f;
            double I = kd * Il * Io * Vector3.Dot(N, L) + ks * Il * Io * Math.Pow(Vector3.Dot(V, R), m);
            if (I < 0)
                I = -I;
            int result = (int)(I * 255f / 2f) ; // <0,255>
            return result;
        }

        public Vector3 CreateN(int x, int y)
        {
            Color color = normalMap.GetPixel(x  % normalMap.Width, y % normalMap.Height);
            Vector3 result = new Vector3(0, 0, 0);
            result.X = 2f * color.R / 255f - 1;
            result.Y = 2f * color.G / 255f - 1;
            double z = 2f * color.B / 255f - 1;
            if (z < 0) z = 0;
            result.Z = z;
            return result;
        }

        public void ChangeNormalMap(Bitmap newNormalMap)
        {
            normalMap = new Bitmap(newNormalMap, 250, 250);
        }
        public void ChangeKD(double newkd)
        {
            kd = newkd;
        }

        public void ChangeKS(double newks)
        {
            ks = newks;
        }

        public void ChangeM(int newm)
        {
            m = newm;
        }
    }
}
