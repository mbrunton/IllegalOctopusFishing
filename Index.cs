using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IllegalOctopusFishing
{
    public class Index
    {
        public int i, j;
        public Index(int i, int j)
        {
            this.i = i; this.j = j;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Index that = obj as Index;
            return this.i == that.i && this.j == that.j;
        }

        public override int GetHashCode()
        {
            int hash = 31;
            hash = (hash * 17) + i.GetHashCode();
            hash = (hash * 17) + j.GetHashCode();
            return hash;
        }
    }
}
