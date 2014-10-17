using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    class Fish : ModelGameObject
    {
        public Fish(ExtremeSailingGame game, Vector3 startingPos, Model model)
            : base(game, startingPos, model)
        {
            acc = 0.01f;
            maxVel = 0.4f;
        }

        internal void Update(GameTime gameTime, float terrainHeight, float oceanHeight, Gravity gravity)
        {
            //throw new NotImplementedException();
        }
    }
}
