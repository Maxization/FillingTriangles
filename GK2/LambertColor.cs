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
        Vector3 V, constN, constL;
        DirectBitmap normalMap;
        bool constantN;
        bool animated;
        Vector3 lightPoint;

        public LambertColor(double kd, double ks, int m, DirectBitmap normalMap, Vector3 V)
        {
            this.kd = kd;
            this.ks = ks;
            this.m = m;
            this.normalMap = normalMap;
            this.V = V;
            constN = new Vector3(0, 0, 1);
            constL = new Vector3(0, 0, 1);
            lightPoint = new Vector3(0, 0, 0);
            constantN = false;
            animated = false;
        }
        public void ChangeConstantN(bool constant)
        {
            constantN = constant;
        }

        public void ChangeLightPoint(Vector3 v)
        {
            lightPoint = v;
        }

        Vector3 CreateL(int x, int y)
        {
            Vector3 l = new Vector3(lightPoint.X - x, lightPoint.Y - y, lightPoint.Z);
            l.Normalize();
            return l;
        }

        public int MakeColor(int ObjColor, double Il,int x,int y)
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
            Vector3 L;
            if(animated)
            {
                L = CreateL(x, y);
            }
            else
            {
                L = constL;
            }

            Vector3 R = 2 * Vector3.Dot(N, L) * N - L;

            double Io = ObjColor / 255f;
            double I = kd * Il * Io * Vector3.Dot(N, L) + ks * Il * Io * Math.Pow(Vector3.Dot(V, R), m);
            if (I < 0)
                I = 0;
            if (I > 1)
                I = 1;
            int result = (int)(I * 255f) ; // <0,255>
            return result;
        }

        int nfmod(double a, double b)
        {
            return (int)(a - b * Math.Floor(a /b));
        }
        public Vector3 CreateN(int x, int y)
        {
            Color color = normalMap.GetPixel(x % normalMap.Width, nfmod((normalMap.Height - y), normalMap.Height));
            Vector3 result = new Vector3(0, 0, 0);
            result.X = 2f * color.R / 255f - 1;
            result.Y = 2f * color.G / 255f - 1;
            result.Z = color.B / 255f;
            result.Normalize();
            return result;
        }

        public void ChangeNormalMap(DirectBitmap newNormalMap)
        {

            normalMap = newNormalMap;
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
