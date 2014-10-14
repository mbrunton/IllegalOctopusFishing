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
            mass = 10f;
            acc = 0.01f;
        }

        internal void Update(GameTime gameTime, float fishTerrainHeight, float seaLevel, Gravity gravity)
        {
            //throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }
    }
}
