using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK2
{
    class Vector3
    {
        public double X, Y, Z;
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        public void Normalize()
        {
            double len = Length();
            X /= len;
            Y /= len;
            Z /= len;
        }
        public static double Dot(Vector3 A, Vector3 B)
        {
            return A.X * B.X + A.Y * B.Y + A.Z * B.Z;
        }
        public static Vector3 operator+(Vector3 A, Vector3 B)
        {
            return new Vector3(A.X + B.X, A.Y + B.Y, A.Z + B.Z);
        }
        public static Vector3 operator-(Vector3 A, Vector3 B)
        {
            return new Vector3(A.X - B.X, A.Y - B.Y, A.Z - B.Z);
        }
        public static Vector3 operator*(Vector3 A, Vector3 B)
        {
            return new Vector3(A.X * B.X, A.Y * B.Y, A.Z * B.Z);
        }
        public static Vector3 operator*(double k, Vector3 A)
        {
            return new Vector3(k * A.X, k * A.Y, k * A.Z);
        }
    }
}
