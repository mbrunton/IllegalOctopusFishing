using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    class Terrain : BasicGameObject
    {
        private float worldSize;
        private float minX, maxX;
        private float minZ, maxZ;
        private float seaLevel;
        private float verticesPerLength; // how many terrain vertices per unit length
        private HeightMap heightMap;
        private DiamondSquare diamondSquare;
        private SearchTree searchTree;

        public Terrain(ExtremeSailingGame game, float worldSize, float seaLevel) : base(game)
        {
            this.worldSize = worldSize;
            this.seaLevel = seaLevel;
            this.verticesPerLength = 0.01f;

            this.heightMap = new HeightMap(worldSize, verticesPerLength);
            this.minX = heightMap.minX;
            this.maxX = heightMap.maxX;
            this.minZ = heightMap.minZ;
            this.maxZ = heightMap.maxZ;

            float cornerHeight = 0f;
            float middleHeight = -1 * Math.Min(worldSize / 2f, 200f);
            float randomFactor = 5f;
            this.diamondSquare = new DiamondSquare(heightMap.numSideVertices, cornerHeight, middleHeight, randomFactor);

            heightMap.fillGridFromHeights(diamondSquare.heights);
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
            return Color.Green;
        }

        internal Vector3 getRandomUnderWaterLocation()
        {
            int maxAttempts = (int)(worldSize * worldSize);
            float buffer = 2f; // how shallow we'll allow water to be
            for (int i = 0; i < maxAttempts; i++)
            {
                float x = diamondSquare.getRandInRange(minX, maxX);
                float z = diamondSquare.getRandInRange(minZ, maxZ);
                float height = getTerrainHeightAtPosition(x, z);
                if (height + buffer <= seaLevel)
                {
                    float y = height + (seaLevel - height) / 2;
                    return new Vector3(x, y, z);
                }
            }

            throw new Exception("Could not find random location on water.. max attempts reached");
        }

        internal Vector3 getRandomOnWaterLocation()
        {
            int maxAttempts = (int)(worldSize * worldSize);
            float buffer = 4f; // how shallow we'll allow water to be
            for (int i = 0; i < maxAttempts; i++)
            {
                float x = diamondSquare.getRandInRange(minX, maxX);
                float z = diamondSquare.getRandInRange(minZ, maxZ);
                float height = getTerrainHeightAtPosition(x, z);
                if (height + buffer <= seaLevel)
                {
                    return new Vector3(x, seaLevel, z);
                }
            }

            throw new Exception("Could not find random location on water.. max attempts reached");    
        }

        internal Vector3 getPlayerStartPos()
        {
            return new Vector3(0, 0, 0);
            /*
            int maxAttempts = (int)(worldSize * worldSize);
            float buffer = 4f; // how shallow we'll allow water to be
            for (int i = 0; i < maxAttempts; i++)
            {
                float x = diamondSquare.getRandInRange(minX / 4f, maxX / 4f);
                float z = diamondSquare.getRandInRange(minZ / 4f, maxZ / 4f);
                float height = getTerrainHeightAtPosition(x, z);
                if (height + buffer <= seaLevel)
                {
                    return new Vector3(x, seaLevel, z);
                }
            }

            throw new Exception("Could not find suitable Player start pos");
            */
        }

        internal float getTerrainHeightAtPosition(float x, float z)
        {
            Index index = searchTree.getIndexAtPosition(x, z);
            float height = heightMap.getHeightAtPosition(index, x, z);
            return height;
        }

        internal Dictionary<Player.HullPositions, float> getTerrainHeightsForPlayerHull(Dictionary<Player.HullPositions, Vector3> hullPositions)
        {
            Dictionary<Player.HullPositions, float> hullHeights = new Dictionary<Player.HullPositions,float>();
            foreach (KeyValuePair<Player.HullPositions,Vector3> keyVal in hullPositions)
            {
                Player.HullPositions hullKey = keyVal.Key;
                Vector3 hullPosition = keyVal.Value;
                hullHeights.Add(hullKey, getTerrainHeightAtPosition(hullPosition.X, hullPosition.Z));               
            }
            return hullHeights;
        }
    }
}
