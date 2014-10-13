using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class HeightMap
    {
        private float worldSize;
        private float verticesPerLength;
        private float cornerHeight;
        private float middleHeight;

        public List<List<Vector3>> grid;
        public int numSideVertices;
        public float minX, maxX, minZ, maxZ;
        public float stepSize;

        public HeightMap(float worldSize, float verticesPerLength) : this(worldSize, verticesPerLength, false)
        {
            
        }

        public HeightMap(float worldSize, float verticesPerLength, bool fillWithZero)
        {
            this.worldSize = worldSize;
            this.verticesPerLength = verticesPerLength;

            minX = -1 * worldSize / 2;
            maxX = worldSize / 2;
            minZ = minX;
            maxZ = maxX;

            // how many vertices down one side of map
            float floatSideVertices = worldSize * verticesPerLength + 1;
            numSideVertices = (int)Math.Pow(Math.Ceiling(Math.Log(floatSideVertices, 2)), 2); // next power of 2
            stepSize = worldSize / (numSideVertices - 1);

            if (fillWithZero)
            {
                fillGridWithZero();
            }
        }

        public void fillGridWithZero()
        {
            grid = new List<List<Vector3>>();
            for (int i = 0; i < numSideVertices; i++)
            {
                List<Vector3> zRow = new List<Vector3>();
                for (int j = 0; j < numSideVertices; j++)
                {
                    float x = minX + i * stepSize;
                    float z = minZ + i * stepSize;
                    float y = 0f;
                    zRow.Add(new Vector3(x, y, z));
                }
                grid.Add(zRow);
            }
        }

        internal void fillGridFromDiamondSquare(List<List<float>> diamondSquareHeights)
        {
            grid = new List<List<Vector3>>();
            for (int i = 0; i < numSideVertices; i++)
            {
                List<Vector3> zRow = new List<Vector3>();
                for (int j = 0; j < numSideVertices; j++)
                {
                    float x = minX + i * stepSize;
                    float z = minZ + i * stepSize;
                    float y = diamondSquareHeights[i][j];
                    zRow.Add(new Vector3(x, y, z));
                }
                grid.Add(zRow);
            }
        }
    }
}
