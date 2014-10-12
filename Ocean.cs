using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IllegalOctopusFishing
{
    class Ocean : GameObject
    {
        private float worldSize;
        private float seaLevel;

        public Ocean(float worldSize, float seaLevel)
        {
            this.worldSize = worldSize;
            this.seaLevel = seaLevel;
        }
    }
}
