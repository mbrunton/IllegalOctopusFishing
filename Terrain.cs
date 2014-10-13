using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    class Terrain : GameObject
    {
        private float worldSize;
        private float minX, maxX;
        private float minZ, maxZ;
        private float seaLevel;
        private float verticesPerLength; // how many terrain vertices per unit length
        private HeightMap heightMap;
        private DiamondSquare diamondSquare;
        private SearchTree searchTree;

        public Terrain(IllegalOctopusFishingGame game, float worldSize, float seaLevel) : base(game)
        {
            this.worldSize = worldSize;
            this.seaLevel = seaLevel;
            this.verticesPerLength = 4;

            this.heightMap = new HeightMap(worldSize, verticesPerLength);
            this.minX = heightMap.minX;
            this.maxX = heightMap.maxX;
            this.minZ = heightMap.minZ;
            this.maxZ = heightMap.maxZ;

            float cornerHeight = 0f;
            float middleHeight = -50f;
            this.diamondSquare = new DiamondSquare(heightMap.numSideVertices, cornerHeight, middleHeight);

            heightMap.fillGridFromDiamondSquare(diamondSquare.heights);
            bool isRound = true;
            heightMap.fillNormalGrid(isRound);

            searchTree = new SearchTree(heightMap);

            vertices = Buffer.Vertex.New(
                game.GraphicsDevice, 
                heightMap.getVertexPositionNormalColorList(getColorAtHeight).ToArray());
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

        private Color getColorAtHeight(float y)
        {
            return Color.Brown;
        }

        internal Vector3 getRandomUnderWaterLocation()
        {
            throw new NotImplementedException();
        }

        internal Vector3 getRandomOnWaterLocation()
        {
            throw new NotImplementedException();
        }

        internal Vector3 getPlayerStartPos()
        {
            int maxAttempts = 1000;
            for (int i = 0; i < maxAttempts; i++)
            {
            }
        }

        internal float getTerrainHeightAtPosition(float x, float z)
        {
            Index index = searchTree.getIndexAtPosition(x, z);
            float height = heightMap.getHeightAtIndex(index);
            return height;
        }

        internal Dictionary<Player.HullPositions, float> getTerrainHeightsAtPositions(Dictionary<Player.HullPositions, Vector3> posMap)
        {
            Dictionary<Player.HullPositions, float> heightMap = new Dictionary<Player.HullPositions,float>();
            foreach (KeyValuePair<Player.HullPositions,Vector3> keyVal in posMap)
            {
                Player.HullPositions hullPos = keyVal.Key;
                Vector3 pos = keyVal.Value;
                heightMap.Add(hullPos, getTerrainHeightAtPosition(pos.X, pos.Z));               
            }
            return heightMap;
        }
    }
}
