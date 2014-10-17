using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    public class Camera
    {
        internal Vector3 pos;
        internal Matrix view;
        internal Matrix projection;
        private float maxDistanceFromPlayer;
        private float minDistanceFromPlayer;
        private float speedDistanceAdjustment;
        private float targetDistanceForwardOfPlayer;

        public Camera(ExtremeSailingGame game, Vector3 playerPos, Vector3 playerDir, Vector3 playerVel)
        {
            this.projection = Matrix.PerspectiveFovRH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 10000.0f);
            this.maxDistanceFromPlayer = 50f;
            this.minDistanceFromPlayer = 30f;
            this.speedDistanceAdjustment = 0.1f;
            this.targetDistanceForwardOfPlayer = 40f;

            Update(playerPos, playerDir, playerVel);
        }

        internal void Update(Vector3 playerPos, Vector3 playerDir, Vector3 playerVel)
        {
            float playerSpeed = playerVel.Length();
            float metresBackFromPlayer = Math.Min(speedDistanceAdjustment * playerSpeed + minDistanceFromPlayer, maxDistanceFromPlayer);
            float metresAbovePlayer = 0.4f * metresBackFromPlayer;
            this.pos = playerPos - (metresBackFromPlayer * playerDir) + (metresAbovePlayer * Vector3.UnitY);
            Vector3 target = playerPos + targetDistanceForwardOfPlayer * playerDir;
            
            this.view = Matrix.LookAtRH(pos, target, Vector3.UnitY);
        }
    }
}
