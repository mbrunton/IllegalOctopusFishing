using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    using System.Diagnostics;
    public class Player : ModelGameObject
    {
        public enum BoatSize { SMALL, LARGE };
        public enum HullPositions { BACK_LEFT, BACK_RIGHT, FRONT_LEFT, FRONT_RIGHT };

        private Model sailModel;
        private Matrix sailWorld;
        private float sailTheta, sailOmega, sailAlpha;

        private BoatSize boatSize;
        private float length, width;
        private float slack;
        private Dictionary<HullPositions, Vector3> hullPositions;

        public Player(IllegalOctopusFishingGame game, Vector3 startingPos, String boatModelName, String sailModelName, BoatSize boatSize) : base(game, startingPos, boatModelName)
        {
            hullPositions = new Dictionary<HullPositions, Vector3>();
            hullPositions.Add(HullPositions.BACK_LEFT, new Vector3());
            hullPositions.Add(HullPositions.BACK_RIGHT, new Vector3());
            hullPositions.Add(HullPositions.FRONT_LEFT, new Vector3());
            hullPositions.Add(HullPositions.FRONT_RIGHT, new Vector3());

            this.boatSize = boatSize;
            if (boatSize == BoatSize.SMALL)
            {
                length = 5f;
                width = 3f;
                mass = 400f;
                acc = 0.001f;
                maxVel = 0.08f;
            }
            else
            {
                length = 10f;
                width = 4f;
                mass = 800f;
                acc = 0.1f;
                maxVel = 0.15f;
            }

            // sail
            bool success = game.nameToModel.TryGetValue(sailModelName, out sailModel);
            if (!success)
            {
                throw new ArgumentException("failed to load sail model");
            }
            this.sailWorld = Matrix.Identity;
            this.sailTheta = (float)Math.PI;
            this.sailOmega = 0.001f;
            this.sailAlpha = 0.005f;
            this.slack = (float)Math.PI/4f;
        }

        internal Dictionary<HullPositions, Vector3> getHullPositions()
        {
            Vector3 left = Vector3.Cross(dir, up);
            Vector3 right = -1f * left;
            Vector3 back = -1f * dir;

            Vector3 backLeft = new Vector3(pos.X, pos.Y, pos.Z) + length*back + width*left;
            Vector3 backRight = new Vector3(pos.X, pos.Y, pos.Z) + length*back + width*right;
            Vector3 frontLeft = new Vector3(pos.X, pos.Y, pos.Z) + length*dir + width*left;
            Vector3 frontRight = new Vector3(pos.X, pos.Y, pos.Z) + length*dir + width*right;

            hullPositions[HullPositions.BACK_LEFT] = backLeft;
            hullPositions[HullPositions.BACK_RIGHT] = backRight;
            hullPositions[HullPositions.FRONT_LEFT] = frontLeft;
            hullPositions[HullPositions.FRONT_RIGHT] = frontRight;
            return hullPositions;
        }

        internal void Update(GameTime gameTime, Dictionary<HullPositions, float> hullTerrainHeights, Dictionary<HullPositions, float> hullOceanHeights, Wind wind, Gravity gravity)
        {
            float delta = gameTime.ElapsedGameTime.Milliseconds;
            
            // TESTING
            
            Matrix rotation = Matrix.RotationY(0.001f * delta);
            dir = Vector3.TransformCoordinate(dir, rotation);
            vel += delta * acc * dir;
            
            // actual code

            // wind
            Vector3 xzDir = new Vector3(dir.X, 0, dir.Z);
            xzDir.Normalize();

            Vector3 cross = Vector3.Cross(xzDir, wind.getDir());
            float windAngle = (float)Math.Acos(Vector3.Dot(xzDir, wind.getDir()));
            if (cross.Y < 0)
            {
                windAngle = 2 * (float)Math.PI - windAngle;
            }




            if (vel.Length() > maxVel)
            {
                vel.Normalize();
                vel = maxVel * vel;
            }
            pos += delta * vel;

            this.World = getWorld();
            this.sailWorld = World;
        }

        public override void Draw(GameTime gameTime)
        {
            sailModel.Draw(game.GraphicsDevice, sailWorld, View, Projection);
            base.Draw(gameTime);
        }
    }
}
