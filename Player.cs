using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    class Player : MovingGameObject
    {
        public enum BoatSize { SMALL, LARGE };
        public enum HullPositions { BACK_LEFT, BACK_RIGHT, FRONT_LEFT, FRONT_RIGHT };

        private float length, width;
        private Dictionary<HullPositions, Vector3> hullPositions;

        public Player(IllegalOctopusFishingGame game, Vector3 startingPos) : base(game, startingPos)
        {
            hullPositions = new Dictionary<HullPositions, Vector3>();
            hullPositions.Add(HullPositions.BACK_LEFT, new Vector3());
            hullPositions.Add(HullPositions.BACK_RIGHT, new Vector3());
            hullPositions.Add(HullPositions.FRONT_LEFT, new Vector3());
            hullPositions.Add(HullPositions.FRONT_RIGHT, new Vector3());
        }

        internal Dictionary<HullPositions, Vector3> getHullPositions()
        {
            if (!isModelSet)
            {
                throw new Exception("Player models not loaded, yet trying to get hull positions");
            }

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

        internal void Update(GameTime gameTime, Dictionary<HullPositions, float> hullHeights, float seaLevel, Wind wind, Gravity gravity)
        {
            if (!isModelSet)
            {
                throw new Exception("Player model not loaded, so don't have acceleration set (dependent on model)");
            }
            float delta = gameTime.ElapsedGameTime.Milliseconds;
            this.vel += dir * delta * this.acc;

            if (vel.Length() > maxVel)
            {

            }
        }

        public override void Draw(GameTime gameTime)
        {
            model.Draw(game.GraphicsDevice, basicEffect.World, basicEffect.View, basicEffect.Projection);
        }

        internal void setModel(BoatSize selectedBoat, String modelName)
        {
            bool success = game.nameToModel.TryGetValue(modelName, out model);
            switch (selectedBoat)
            {
                // TODO: large boat
                case BoatSize.LARGE:
                case BoatSize.SMALL:
                    length = 5f;
                    width = 3f;
                    acc = 0.05f;
                    maxVel = 0.1f;
                    break;
            }

            if (!success)
            {
                throw new ArgumentException("failed to load player blender model");
            }

            isModelSet = true;
        }

        internal void setModelLighting(float p, HeavenlyBody sun, HeavenlyBody moon)
        {
            //throw new NotImplementedException();
        }
    }
}
