using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bankrablas
{
    abstract class VarosElem
    {
        public bool latott;
        public bool banditaMellet = false;
        public int elemX;
        public int elemY;
        public VarosElem()
        {
        }
        public override string ToString() {
            return ".";
        }
    }
}
