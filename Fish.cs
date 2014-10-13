using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class Fish : MovingGameObject
    {
        public Fish(IllegalOctopusFishingGame game, Vector3 startingPos)
            : base(game, startingPos)
        {
            float fishMass = 10f;
            float fishAcc = 4f;
            base.SetMass(fishMass);
            base.SetAcc(fishAcc);
        }

        internal void Update(GameTime gameTime, float fishTerrainHeight, float seaLevel, Gravity gravity)
        {
            throw new NotImplementedException();
        }
    }
}
