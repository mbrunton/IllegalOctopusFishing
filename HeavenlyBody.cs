using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    public class HeavenlyBody
    {
        private Vector3 dir;
        private Vector3 revolutionNormal;
        private float omega;
        private Color specularColor;

        public HeavenlyBody(Vector3 initialDir, Vector3 revolutionNormal, float secsPerGameDay, Color specularColor)
        {
            this.dir = initialDir;
            this.revolutionNormal = revolutionNormal;
            // want to revolve by 2pi radians every "secsPerGameDay" seconds
            this.omega = 2 * (float)Math.PI / secsPerGameDay;
            this.specularColor = specularColor;
        }

        public void Update(GameTime gameTime) {
            float deltaSecs = gameTime.ElapsedGameTime.Milliseconds / 1000f;
            float deltaRadians = omega * deltaSecs;
            Matrix rotation = Matrix.RotationAxis(revolutionNormal, deltaRadians);
            this.dir = Vector3.TransformCoordinate(dir, rotation);
        }

        public Vector3 getDir()
        {
            return this.dir;
        }

        public Color getSpecularColor()
        {
            return this.specularColor;
        }
    }
}
