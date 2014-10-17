using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    class Ocean : BasicGameObject
    {
        private float worldSize;
        private float seaLevel;
        private float verticesPerLength;
        private HeightMap heightMap;
        private SearchTree searchTree;

        public Ocean(ExtremeSailingGame game, float worldSize, float seaLevel) : base(game)
        {
            this.worldSize = worldSize;
            this.seaLevel = seaLevel;

            this.verticesPerLength = 0.001f;

            this.heightMap = new HeightMap(worldSize, verticesPerLength, true);
            
            bool isRound = false;
            heightMap.fillNormalGrid(isRound);

            this.searchTree = new SearchTree(heightMap);

            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                heightMap.getVertexPositionNormalColorList(getColorAtHeight).ToArray());
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

        internal Color getColorAtHeight(float y)
        {
            float alpha = 0.4f + 0.2f*(y - seaLevel)/20f;
            Color b = Color.DarkBlue;
            return new Color(b.ToVector3(), alpha);
        }

        internal void Update(GameTime gameTime)
        {
            //float total = gameTime.TotalGameTime.TotalMilliseconds;

        }

        internal float getOceanHeightAtPosition(float x, float z)
        {
            Index index = searchTree.getIndexAtPosition(x, z);
            float height = heightMap.getHeightAtPosition(index, x, z);
            return height;
        }

        internal Dictionary<Player.HullPositions, float> getOceanHeightsForPlayerHull(Dictionary<Player.HullPositions, Vector3> hullPositions)
        {
            Dictionary<Player.HullPositions, float> hullHeights = new Dictionary<Player.HullPositions, float>();
            foreach (KeyValuePair<Player.HullPositions, Vector3> keyVal in hullPositions)
            {
                Player.HullPositions hullKey = keyVal.Key;
                Vector3 hullPosition = keyVal.Value;
                hullHeights.Add(hullKey, getOceanHeightAtPosition(hullPosition.X, hullPosition.Z));
            }
            return hullHeights;
        }

        public override void Draw(Camera camera, Color ambientColor, HeavenlyBody sun, HeavenlyBody moon)
        {
            game.GraphicsDevice.SetBlendState(game.GraphicsDevice.BlendStates.AlphaBlend);
            base.Draw(camera, ambientColor, sun, moon);
        }
    }
}
