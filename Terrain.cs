using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class Terrain : GameObject
    {
        private float worldSize;

        public Terrain(float worldSize)
        {
            this.worldSize = worldSize;
        }

        internal Vector3 getUnderWaterLocation()
        {
            throw new NotImplementedException();
        }
    }
}
