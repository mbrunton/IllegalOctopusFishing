using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using System.Diagnostics;

namespace IllegalOctopusFishing
{
    class Harpoon : ModelGameObject
    {
        internal float cooloff;
        internal float attackRange;
        internal float damage;

        private float omega;

        public Harpoon(ExtremeSailingGame game, Vector3 startPos, Vector3 modelDir, Vector3 initialDir, Vector3 initialVel, String modelName) : base(game, startPos, modelName)
        {
            this.modelDir = modelDir;
            this.dir = new Vector3(initialDir.X, 0, initialDir.Z);
            Vector3 left = Vector3.Cross(initialDir, up);
            Matrix dirRotation = Matrix.RotationAxis(left, (float)Math.PI / 8);
            this.dir = Vector3.TransformCoordinate(initialDir, dirRotation);
            this.up = Vector3.TransformCoordinate(up, dirRotation);
            dir.Normalize();
            up.Normalize();
            
            this.maxVel = initialVel.Length() + 0.2f;
            this.omega = 0.001f;

            // unused
            this.acc = 0f;
            this.vel = Vector3.Zero;

            this.cooloff = 1000f;
            this.attackRange = 2f;
            this.damage = 50f;
        }

        internal void Update(GameTime gameTime, Gravity gravity)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //if (!MathUtil.IsOne(dir.Length())) { throw new Exception("harpoon dir vector doesn't have length 1"); }
            //if (!MathUtil.IsOne(up.Length())) { throw new Exception("harpoon up vector doesn't have length 1"); }

            if (cooloff > 0f)
            {
                cooloff -= delta;
                if (cooloff < 0)
                {
                    cooloff = 0f;
                }
            }

            /*
            Vector3 deltaVel = delta * acc * dir;
            //deltaVel += gravity.getG() * gravity.getDir();
            vel += deltaVel;
            */
 
            float dirGravityAngle = (float)Math.Acos(Vector3.Dot(dir, gravity.getDir()));
            
            // stop rotating when we're nearly pointing straight down
            if (dirGravityAngle > 0.01f) {
                Vector3 right = Vector3.Cross(up, dir);
                float deltaDirTheta = omega * delta;
                Matrix dirRotation = Matrix.RotationAxis(right, deltaDirTheta);
                dir = Vector3.TransformCoordinate(dir, dirRotation);
                up = Vector3.TransformCoordinate(up, dirRotation);

                dir.Normalize();
                vel.Normalize();
            }

            /*
            if (vel.Length() > maxVel)
            {
                vel.Normalize();
                vel = maxVel * vel;
            }
            */

            pos += delta * maxVel * dir;

            Matrix rotation = getRotationMatrix();
            Matrix translation = getTranslationMatrix();

            this.World = rotation * translation;
        }
    }
}
