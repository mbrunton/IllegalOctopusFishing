using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class Gravity
    {
        private Vector3 dir;
        private float g; // ms^-2

        public Gravity(Vector3 dir)
        {
            this.dir = dir;
            this.g = 0.01f;
        }

        public Vector3 getDir()
        {
            return this.dir;
        }

        public float getG()
        {
            return this.g;
        }
    }
}
