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

            //Debug.WriteLine(heightMap.getGrid().ToString());

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
            int maxAttempts = (int)(worldSize * worldSize);
            float buffer = 8f; // how shallow we'll allow water to be
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
            float x = minX + (maxX - minX) / 2;
            float z = minZ + (maxZ - minZ) / 2;
            float height = getTerrainHeightAtPosition(x, z);
            if (height > seaLevel)
            {
                throw new Exception("Player start pos is over land!");
            }

            return new Vector3(x, seaLevel, z);
        }

        internal float getTerrainHeightAtPosition(float x, float z)
        {
            Index index = searchTree.getIndexAtPosition(x, z);
            float height = heightMap.getHeightAtIndex(index);
            return height;
        }

        internal Dictionary<Player.HullPositions, float> getTerrainHeightsForPlayerHull(Dictionary<Player.HullPositions, Vector3> posMap)
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
