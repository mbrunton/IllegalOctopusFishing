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

        public Ocean(IllegalOctopusFishingGame game, float worldSize, float seaLevel) : base(game)
        {
            this.worldSize = worldSize;
            this.seaLevel = seaLevel;

            this.verticesPerLength = 0.01f;

            this.diffuseColor = Color.DarkBlue;

            this.heightMap = new HeightMap(worldSize, verticesPerLength, true);
            
            bool isRound = false;
            heightMap.fillNormalGrid(isRound);

            this.searchTree = new SearchTree(heightMap);

            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                heightMap.getVertexPositionNormalColorList(height => diffuseColor).ToArray());
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

        internal void Update(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }

        internal float getOceanHeightAtPosition(float x, float z)
        {
            Index index = searchTree.getIndexAtPosition(x, z);
            float height = heightMap.getHeightAtIndex(index);
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
    }
}
