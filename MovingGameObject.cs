using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    abstract class MovingGameObject : GameObject
    {
        internal Model model;
        internal bool isModelSet = false;
        internal Vector3 pos, dir, vel, up;
        internal float mass;
        internal float acc;
        internal float maxVel;

        public MovingGameObject(IllegalOctopusFishingGame game, Vector3 startPos) : base(game)
        {
            this.pos = startPos;
            this.up = Vector3.UnitY;
            this.dir = Vector3.UnitX;
        }
    }
}
