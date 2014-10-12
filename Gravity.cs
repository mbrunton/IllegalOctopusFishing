﻿using System;
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

        public Gravity(Vector3 dir, float g)
        {
            this.dir = dir;
            this.g = g;
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