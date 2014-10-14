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
        private Model boatModel;
        private Model sailModel;
        private bool isModelsSet = false;
        private Dictionary<HullPositions, Vector3> hullPositions;

        public Player(IllegalOctopusFishingGame game, Vector3 startingPos) : base(game, startingPos)
        {
            hullPositions = new Dictionary<HullPositions, Vector3>();
            diffuseColor = Color.Red;
        }

        internal Dictionary<HullPositions, Vector3> getHullPositions()
        {
            if (!isModelsSet)
            {
                throw new Exception("Player models not loaded, yet trying to get hull positions");
            }
            return hullPositions;
        }

        internal void Update(GameTime gameTime, Dictionary<HullPositions, float> hullHeights, float seaLevel, Wind wind, Gravity gravity)
        {
            //throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime)
        {
            boatModel.Draw(game.GraphicsDevice, basicEffect.World, basicEffect.View, basicEffect.Projection, effectOverride:basicEffect);
            sailModel.Draw(game.GraphicsDevice, basicEffect.World, basicEffect.View, basicEffect.Projection, effectOverride:new BasicEffect(game.GraphicsDevice));
        }

        internal void SetSelectedBoat(BoatSize selectedBoat)
        {
            bool success = false;
            success = game.nameToModel.TryGetValue("Car", out boatModel);
            success &= game.nameToModel.TryGetValue("Skull", out sailModel);
            length = 10;
            width = 5;
            
            /*
            switch (selectedBoat)
            {
                case BoatSize.SMALL:
                    success = game.nameToModel.TryGetValue("small_boat", out boatModel);
                    success &= game.nameToModel.TryGetValue("small_sail", out boatModel);
                    break;
                case BoatSize.LARGE:
                    success = game.nameToModel.TryGetValue("large_boat", out boatModel);
                    success &= game.nameToModel.TryGetValue("large_sail", out boatModel);
                    break;
            }
            */

            if (!success)
            {
                throw new ArgumentException("failed to load player blender model");
            }

            isModelsSet = true;
            // TODO: length, width from model
        }
    }
}
