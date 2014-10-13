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
        private float waterDensity;

        public Ocean(IllegalOctopusFishingGame game, float worldSize, float seaLevel) : base(game)
        {
            this.worldSize = worldSize;
            this.seaLevel = seaLevel;
            this.waterDensity = 1000f; // kgm^-3
        }
    }
}
