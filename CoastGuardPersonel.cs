using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class CoastGuardPersonel : ModelGameObject
    {
        public CoastGuardPersonel(IllegalOctopusFishingGame game, Vector3 startingPos, String modelName)
            : base(game, startingPos, modelName)
        {
            mass = 1000f;
            acc = 0.01f;
            maxVel = 0.01f;
        }

        internal void Update(GameTime gameTime, float coastGuardTerrainHeight, float seaLevel, Gravity gravity)
        {
            //throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }
    }
}
