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
        Vector3 N, V;

        public LambertColor(double kd, double ks, Vector3 N, Vector3 V)
        {
            this.kd = kd;
            this.ks = ks;
            this.N = N;
            this.V = V;
        }

        public int MakeColor(int ObjColor, Vector3 L, double Il, double m)
        {
            Vector3 R = 2 * Vector3.Dot(N, L) * N - L;
            double Io = (2f * ObjColor) / 255f - 1f;
            double I = kd * Il * Io * Vector3.Dot(N, L) + ks * Il * Io * Math.Pow(Vector3.Dot(V, R), m);
            
            int result = (int)(((I + 2f) * 255f) / (4f)); // <0,255>
            return result;
        }

        

    }
}
