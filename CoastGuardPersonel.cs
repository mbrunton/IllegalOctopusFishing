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
        private Random random;

        public CoastGuardPersonel(IllegalOctopusFishingGame game, Vector3 startingPos, String modelName)
            : base(game, startingPos, modelName)
        {
            acc = 0.00001f;
            maxVel = 0.01f;

            this.random = new Random(Guid.NewGuid().GetHashCode());
        }

        internal void Update(World world, GameTime gameTime, float terrainHeight, float oceanHeight)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // TESTING: random movement
            if (random.NextFloat(0, 1) < 0.01) {
                this.dir = Vector3.TransformCoordinate(dir, Matrix.RotationY(random.NextFloat(0, 2*(float)Math.PI)));
            }

            if (random.NextFloat(0, 1) < 0.01)
            {
                world.AddHarpoon(pos, dir, vel);
            }

            this.vel += delta * acc * dir;
            if (terrainHeight > pos.Y)
            {
                vel = Vector3.Zero;
            }
            this.pos += delta * vel;
            this.World = getWorld();
        }
    }
}
