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

        private float length, width;
        private Model boatModel;
        private Model sailModel;
        private bool isModelsSet = false;

        public Player(IllegalOctopusFishingGame game, Vector3 startingPos) : base(game, startingPos)
        {
        }

        public enum HullPositions {BACK_LEFT, BACK_RIGHT, FRONT_LEFT, FRONT_RIGHT};

        internal Dictionary<HullPositions, Vector3> getBottomPositions()
        {
            throw new NotImplementedException();
        }

        internal void Update(GameTime gameTime, Dictionary<HullPositions, float> playerHeightMap, float seaLevel, Wind wind, Gravity gravity)
        {
            throw new NotImplementedException();
        }

        internal void SetSelectedBoat(BoatSize selectedBoat)
        {
            bool success = false;
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

            if (!success)
            {
                throw new ArgumentException("failed to load player blender model");
            }

            isModelsSet = true;
            // TODO: length, width from model
        }
    }
}
