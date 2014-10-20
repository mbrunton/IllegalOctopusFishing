using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class Wind
    {
        internal Vector3 dir; // in x-z plane
        internal float speed; // [0-1]
        
        private Random random;
        private float secondsTillSwitch;
        private float secondsSinceSwitch;
        private float probabilityOfSwitch;

        public Wind()
        {
            //dir = Vector3.UnitZ - Vector3.UnitX;
            dir = Vector3.UnitX;
            dir.Normalize();
            speed = 1f;

            random = new Random();
            secondsTillSwitch = 20f;
            secondsSinceSwitch = 0f;
            probabilityOfSwitch = 1.0f;
        }

        internal void Update(GameTime gameTime)
        {
            float delta = gameTime.ElapsedGameTime.Milliseconds;
            secondsSinceSwitch += delta / 1000f;
            if (secondsSinceSwitch >= secondsTillSwitch)
            {
                secondsSinceSwitch = 0f;
                if (random.NextFloat(0.6f, 1f) < probabilityOfSwitch)
                {
                    windSwitch();
                }
            }
        }

        internal void windSwitch()
        {
            float theta = random.NextFloat(0f, 1000f) % (2 * (float)Math.PI);
            Matrix rotation = Matrix.RotationY(theta);
            dir = Vector3.TransformCoordinate(dir, rotation);

            speed = random.NextFloat(0, 1);
        }
    }
}
