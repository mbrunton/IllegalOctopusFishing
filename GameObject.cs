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
    using SharpDX.Toolkit.Input;
    abstract public class GameObject
    {
        protected ExtremeSailingGame game;
        internal Effect effect;

        internal Matrix View;
        internal Matrix Projection;
        internal Matrix World;

        public GameObject(ExtremeSailingGame game)
        {
            this.game = game;
            this.View = Matrix.Identity;
            this.Projection = Matrix.Identity;
            this.World = Matrix.Identity;
        }

        abstract public void Draw(Camera camera, Color ambientColor, HeavenlyBody sun, HeavenlyBody moon);

        abstract internal void SetEffectValues(Camera camera, Color ambientColor, HeavenlyBody sun, HeavenlyBody moon);
    }
}
