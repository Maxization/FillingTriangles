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
        Vector3 N, V;

        public LambertColor(double kd, double ks, int m, Vector3 N, Vector3 V)
        {
            this.kd = kd;
            this.ks = ks;
            this.m = m;
            this.N = N;
            this.V = V;
        }

        public int MakeColor(int ObjColor, Vector3 L, double Il)
        {
            Vector3 R = 2 * Vector3.Dot(N, L) * N - L;
            double Io = ObjColor / 255f;
            double I = kd * Il * Io * Vector3.Dot(N, L) + ks * Il * Io * Math.Pow(Vector3.Dot(V, R), m);

            int result = (int)(I * 255f / 2f) ; // <0,255>
            return result;
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
