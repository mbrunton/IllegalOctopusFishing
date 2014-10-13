using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class CoastGuardPersonel : MovingGameObject
    {
        public CoastGuardPersonel(IllegalOctopusFishingGame game, Vector3 startingPos)
            : base(game, startingPos)
        {
            float mass = 1000f;
            float acc = 6f;
            base.SetMass(mass);
            base.SetAcc(acc);
        }

        internal void Update(GameTime gameTime, float coastGuardTerrainHeight, float seaLevel, Gravity gravity)
        {
            throw new NotImplementedException();
        }
    }
}
