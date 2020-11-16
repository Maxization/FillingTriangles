using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GK2
{
    class NodeAET
    {
        public int Ymax;
        public double d, X;
        public NodeAET next;

        public NodeAET(int Ymax, double X, double d, NodeAET next)
        {
            this.Ymax = Ymax;
            this.X = X;
            this.d = d;
            this.next = next;
        }
    }
}
