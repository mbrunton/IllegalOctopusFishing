using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class Camera
    {
        private Matrix view;
        private Matrix projection;

        public Camera(IllegalOctopusFishingGame game, Vector3 playerPos, Vector3 playerDir, Vector3 playerVel)
        {
            Update(playerPos, playerDir, playerVel);
            this.projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 10000.0f);
        }

        internal void Update(Vector3 playerPos, Vector3 playerDir, Vector3 playerVel)
        {
            float speedSquared = playerVel.LengthSquared();
            float metresBackFromPlayer = 10 + speedSquared;
            float metresAbovePlayer = 0.3f * metresBackFromPlayer;
            Vector3 eye = playerPos - (metresBackFromPlayer * playerDir) + (metresAbovePlayer * Vector3.UnitY);

            this.view = Matrix.LookAtLH(eye, playerPos, Vector3.UnitY);
        }

        public Matrix getView()
        {
            return this.view;
        }

        public Matrix getProjection()
        {
            return this.projection;
        }
    }
}
