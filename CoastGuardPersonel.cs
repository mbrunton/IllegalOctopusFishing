using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    class CoastGuardPersonel : ModelGameObject
    {
        private Random random;
        internal float attackPlayerCooloff;
        internal float maxAttackPlayerCooloff;
        internal float fireCooloff;
        internal float maxFireCooloff;
        internal float spotPlayerRange;
        internal float harpoonSpeed;

        public CoastGuardPersonel(ExtremeSailingGame game, Vector3 startingPos, Model model, int difficulty)
            : base(game, startingPos, model)
        {
            acc = 0.00001f;
            maxVel = 0.01f;

            // difficulty [0-5] dictates how long coastguard chases player
            attackPlayerCooloff = 0f;
            fireCooloff = 0f;
            float attackSeconds;
            float fireInterval;
            switch (difficulty)
            {
                case 1:
                    attackSeconds = 3;
                    fireInterval = 3;
                    break;
                case 2:
                    attackSeconds = 5;
                    fireInterval = 2;
                    break;
                case 3:
                    attackSeconds = 10;
                    fireInterval = 1;
                    break;
                case 4:
                    attackSeconds = 20;
                    fireInterval = .5f;
                    break;
                case 5:
                    attackSeconds = 60;
                    fireInterval = 0.2f;
                    break;
                default:
                    throw new Exception("difficulty should be an integer between 1 and 5 inclusive");
            }

            maxAttackPlayerCooloff = attackSeconds * 1000f;
            maxFireCooloff = fireInterval * 1000f;
            spotPlayerRange = 500f;
            harpoonSpeed = 0.5f;


            // TEMPORARY - UNTIL I FIX MODELPHONG.FX
            effect.Parameters["color"].SetValue(Color.BurlyWood.ToVector3());

            this.random = new Random(Guid.NewGuid().GetHashCode());
        }

        internal void Update(World world, GameTime gameTime, float terrainHeight, float oceanHeight, Vector3 playerPos, Vector3 playerDir)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // reduce time till next firing
            if (fireCooloff > 0)
            {
                fireCooloff -= delta;
                if (fireCooloff < 0)
                {
                    fireCooloff = 0;
                }
            }

            // if we're attacking, attack (if allowed) and reduce attack cooloff
            bool isAttacking = attackPlayerCooloff > 0;
            if (isAttacking)
            {
                if (fireCooloff == 0)
                {
                    AttackPlayer(world, playerPos, playerDir);
                    fireCooloff = maxFireCooloff;
                }

                attackPlayerCooloff -= delta;
                if (attackPlayerCooloff < 0)
                {
                    attackPlayerCooloff = 0;
                }
            }

            // check if we should start attacking
            if (!isAttacking)
            {
                float playerDist = (pos - playerPos).Length();
                if (playerDist <= spotPlayerRange)
                {
                    attackPlayerCooloff = maxAttackPlayerCooloff;
                }
            }

            // movement

            /*
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
            */

            this.World = getWorld();
        }

        internal void AttackPlayer(World world, Vector3 playerPos, Vector3 playerVel)
        {
            // TODO: make this better
            Vector3 harpoonVel = (playerPos - pos);
            harpoonVel.Normalize();
            harpoonVel = harpoonSpeed * harpoonVel;
            world.AddHarpoon(pos, playerPos - pos, harpoonVel);
        }
    }
}
