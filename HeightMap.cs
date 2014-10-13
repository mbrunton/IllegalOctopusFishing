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
        private Random random;

        public List<List<Vector3>> grid;
        private int numSideVertices;
        public float minX, maxX, minZ, maxZ;
        public float minPossibleY, maxPossibleY;
        public float stepSize;

        public HeightMap(float worldSize, float verticesPerLength, float cornerHeight, float middleHeight)
        {
            this.worldSize = worldSize;
            this.verticesPerLength = verticesPerLength;
            this.cornerHeight = cornerHeight;
            this.middleHeight = middleHeight;
            this.random = new Random();

            minX = -1 * worldSize / 2;
            maxX = worldSize / 2;
            minZ = minX;
            maxZ = maxX;

            minPossibleY = 1.5f * middleHeight;
            maxPossibleY = -1f * minPossibleY;

            // how many vertices down one side of map
            float floatSideVertices = worldSize * verticesPerLength + 1;
            numSideVertices = (int)Math.Pow(Math.Ceiling(Math.Log(floatSideVertices, 2)), 2); // next power of 2

            stepSize = worldSize / (numSideVertices - 1);

            fillGrid();
        }

        public class Index
        {
            public int i, j;
            public Index(int i, int j)
            {
                this.i = i; this.j = j;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }
                Index that = obj as Index;
                return this.i == that.i && this.j == that.j;
            }
        }

        private void fillGrid()
        {
            // create heights first
            List<List<float>> heights = new List<List<float>>();
            Dictionary<Index, bool> preSelected = new Dictionary<Index, bool>(); // marks which heights we're choosing ourselves
            for (int i = 0; i < numSideVertices; i++)
            {
                // add z-row
                List<float> zRow = new List<float>();
                for (int j = 0; j < numSideVertices; j++)
                {
                    zRow.Add(0f);
                    preSelected.Add(new Index(i, j), false);
                }
                heights.Add(zRow);
            }

            // fill in corners and middle of grid
            heights[0][0] = cornerHeight;
            heights[0][numSideVertices - 1] = cornerHeight;
            heights[numSideVertices - 1][numSideVertices - 1] = cornerHeight;
            heights[numSideVertices - 1][0] = cornerHeight;
            heights[numSideVertices / 2][numSideVertices / 2] = middleHeight;

            preSelected.Add(new Index(0, 0), true);
            preSelected.Add(new Index(0, numSideVertices - 1), true);
            preSelected.Add(new Index(numSideVertices - 1, numSideVertices - 1), true);
            preSelected.Add(new Index(numSideVertices - 1, 0), true);
            preSelected.Add(new Index(numSideVertices / 2, numSideVertices / 2), true);

            // diamond square
            float minRandY = minPossibleY;
            float maxRandY = maxPossibleY;
            int sideLength = numSideVertices - 1;
            while (sideLength > 1)
            {
                // square step
                for (int i = 0; i < numSideVertices - sideLength; i += sideLength)
                {
                    for (int j = 0; j < numSideVertices - sideLength; j += sideLength)
                    {
                        int mi = i / 2;
                        int mj = j / 2;
                        bool pre;
                        preSelected.TryGetValue(new Index(mi, mj), out pre);
                        if (pre)
                        {
                            continue;
                        }
                        float average = getAverageOfSquareCorners(i, j, sideLength, heights);
                        float adjustment = getRandInRange(minRandY, maxRandY);
                        heights[mi][mj] = average + adjustment;
                    }
                }
                // diamond step
                for (int i = sideLength / 2; i < numSideVertices - sideLength/2; i += sideLength)
                {
                    for (int j = 0; j < numSideVertices; j += sideLength)
                    {
                        bool pre;
                        preSelected.TryGetValue(new Index(i, j), out pre);
                        if (pre)
                        {
                            continue;
                        }
                        float average = getAverageOfDiamondNeighbours(i, j, sideLength, heights);
                        float adjustment = getRandInRange(minRandY, maxRandY);
                        heights[i][j] = average + adjustment;
                    }
                }
                for (int i = 0; i < numSideVertices; i += sideLength)
                {
                    for (int j = sideLength / 2; j < numSideVertices - sideLength / 2; j += sideLength)
                    {
                        bool pre;
                        preSelected.TryGetValue(new Index(i, j), out pre);
                        if (pre)
                        {
                            continue;
                        }
                        float average = getAverageOfDiamondNeighbours(i, j, sideLength, heights);
                        float adjustment = getRandInRange(minRandY, maxRandY);
                        heights[i][j] = average + adjustment;
                    }
                }

                minRandY = minRandY / 2f;
                maxRandY = maxRandY / 2f;
                sideLength = sideLength / 2;
            }

            // fill grid from heights
            grid = new List<List<Vector3>>();
            for (int i = 0; i < numSideVertices; i++)
            {
                List<Vector3> zRow = new List<Vector3>();
                for (int j = 0; j < numSideVertices; j++)
                {
                    float x = minX + i * stepSize;
                    float z = minZ + i * stepSize;
                    float y = heights[i][j];
                    zRow.Add(new Vector3(x, y, z));
                }
                grid.Add(zRow);
            }
        }

        private float getAverageOfSquareCorners(int i, int j, int side, List<List<float>> heights)
        {
            // square has corners (i,j), (i+side,j), (i,j+side), (i+side,j+side)
            float topLeft = heights[i][j];
            float topRight = heights[i][j + side];
            float bottomLeft = heights[i + side][j];
            float bottomRight = heights[i + side][j + side];
            float average = (topLeft + topRight + bottomLeft + bottomRight) / 4.0f;
            return average;
        }

        private float getAverageOfDiamondNeighbours(int i, int j, int side, List<List<float>> heights)
        {
            // diamond centre is at (i,j)
            List<float> neighbours = new List<float>();
            if (i - side / 2 >= 0)
            {
                neighbours.Add(heights[i - side / 2][j]);
            }
            if (i + side / 2 < this.numSideVertices)
            {
                neighbours.Add(heights[i + side / 2][j]);
            }
            if (j - side / 2 >= 0)
            {
                neighbours.Add(heights[i][j - side / 2]);
            }
            if (j + side / 2 < this.numSideVertices)
            {
                neighbours.Add(heights[i][j + side / 2]);
            }
            float average = neighbours.Sum() / neighbours.Count();
            return average;
        }

        private float getRandInRange(float a, float b)
        {
            float fl = (float)random.NextDouble();
            fl = fl * (b - a);
            fl = fl + a;
            return fl;
        }
    }
}
