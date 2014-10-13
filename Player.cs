using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class Player : MovingGameObject
    {
        private float length, width;

        public Player(IllegalOctopusFishingGame game, Vector3 startingPos) : base(game, startingPos)
        {
        }

        public enum HullPositions {BACK_LEFT, BACK_RIGHT, FRONT_LEFT, FRONT_RIGHT};

        internal Dictionary<HullPositions, Vector3> getBottomPositions()
        {
            throw new NotImplementedException();
        }

        internal void Update(GameTime gameTime, Dictionary<HullPositions, float> playerHeightMap, float seaLevel, Wind wind, Gravity gravity)
        {
            throw new NotImplementedException();
        }
    }
}
