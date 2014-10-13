using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class Terrain : GameObject
    {
        private float worldSize;
        private float verticesPerLength; // how many terrain vertices per unit length
        private HeightMap heightMap;
        private DiamondSquare diamondSquare;

        public Terrain(IllegalOctopusFishingGame game, float worldSize) : base(game)
        {
            this.worldSize = worldSize;
            this.verticesPerLength = 4;

            this.heightMap = new HeightMap(worldSize, verticesPerLength);

            float cornerHeight = 0f;
            float middleHeight = -50f;
            this.diamondSquare = new DiamondSquare(heightMap.numSideVertices, cornerHeight, middleHeight);

            heightMap.fillGridFromDiamondSquare(diamondSquare.heights);
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
            throw new NotImplementedException();
        }

        internal float getTerrainHeightAtPosition(float x, float z)
        {
            throw new NotImplementedException();
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
