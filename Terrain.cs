using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IllegalOctopusFishing
{
    class Terrain : GameObject
    {
        private float worldSize;

        public Terrain(float worldSize)
        {
            this.worldSize = worldSize;
        }
    }
}
