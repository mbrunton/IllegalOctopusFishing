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
        private float maxDistanceFromPlayer;
        private float minDistanceFromPlayer;

        public Camera(IllegalOctopusFishingGame game, Vector3 playerPos, Vector3 playerDir, Vector3 playerVel)
        {
            this.projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 10000.0f);
            this.maxDistanceFromPlayer = 20f;
            this.minDistanceFromPlayer = 15f;

            Update(playerPos, playerDir, playerVel);
        }

        internal void Update(Vector3 playerPos, Vector3 playerDir, Vector3 playerVel)
        {
            float playerSpeed = playerVel.Length();
            float metresBackFromPlayer = Math.Min(0.1f * playerSpeed + minDistanceFromPlayer, maxDistanceFromPlayer);
            float metresAbovePlayer = 0.3f * metresBackFromPlayer;
            Vector3 eye = playerPos - (metresBackFromPlayer * playerDir) + (metresAbovePlayer * Vector3.UnitY);

            // DEBUGGING - hold bird's eye view
            //this.view = Matrix.LookAtLH(100 * Vector3.UnitY, Vector3.Zero, Vector3.UnitX);
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
