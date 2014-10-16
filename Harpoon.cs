using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class Harpoon : ModelGameObject
    {
        internal float cooloff;
        internal float attackRange;
        internal float damage;

        public Harpoon(IllegalOctopusFishingGame game, Vector3 startPos, Vector3 initialDir, String modelName) : base(game, startPos, initialDir, modelName)
        {
            this.cooloff = 10f;
            this.attackRange = 2f;
            this.damage = 50f;
        }

        internal void Update(GameTime gameTime)
        {
            if (cooloff > 0f)
            {
                float delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                cooloff -= delta;
                if (cooloff < 0)
                {
                    cooloff = 0f;
                }
            }

        }
    }
}
