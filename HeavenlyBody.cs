using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    public class  HeavenlyBody
    {
        internal Vector3 pos;
        internal float distFromOrigin;
        internal Vector3 revolutionNormal;
        internal float omega;
        internal Color specularColor;

        public HeavenlyBody(Vector3 initialPos, float distFromOrigin, Vector3 revolutionNormal, float secsPerGameDay, Color specularColor)
        {
            this.pos = initialPos;
            this.distFromOrigin = distFromOrigin;
            this.revolutionNormal = revolutionNormal;
            // want to revolve by 2pi radians every "secsPerGameDay" seconds
            this.omega = 2 * (float)Math.PI / secsPerGameDay;
            this.specularColor = specularColor;
        }

        public void Update(GameTime gameTime) {
            float deltaSecs = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            float deltaRadians = omega * deltaSecs;
            Matrix rotation = Matrix.RotationAxis(revolutionNormal, deltaRadians);
            this.pos = Vector3.TransformCoordinate(pos, rotation);
        }
    }
}
