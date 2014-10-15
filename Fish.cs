using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class Fish : ModelGameObject
    {
        public Fish(IllegalOctopusFishingGame game, Vector3 startingPos, String modelName)
            : base(game, startingPos, modelName)
        {
            mass = 10f;
            acc = 0.01f;
            maxVel = 0.4f;
        }

        internal void Update(GameTime gameTime, float terrainHeight, float oceanHeight, Gravity gravity)
        {
            //throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }
    }
}
