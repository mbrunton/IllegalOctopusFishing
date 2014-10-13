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

        public Terrain(IllegalOctopusFishingGame game, float worldSize) : base(game)
        {
            this.worldSize = worldSize;
        }

        internal Vector3 getUnderWaterLocation()
        {
            throw new NotImplementedException();
        }

        internal Vector3 getOnWaterLocation()
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
