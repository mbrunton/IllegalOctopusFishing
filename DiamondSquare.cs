using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class DiamondSquare
    {
        private int numSideVertices;
        private float cornerHeight;
        private float middleHeight;
        private float minPossibleY, maxPossibleY;
        private Random random;
        
        public List<List<float>> heights;
        private Dictionary<Index, bool> preSelected;

        public DiamondSquare(int numSideVertices, float cornerHeight, float middleHeight)
        {
            this.numSideVertices = numSideVertices;
            this.cornerHeight = cornerHeight;
            this.middleHeight = middleHeight;
            this.minPossibleY = 1.5f * middleHeight;
            this.maxPossibleY = -1f * minPossibleY;

            this.random = new Random();

            this.heights = new List<List<float>>();
            this.preSelected = new Dictionary<Index, bool>(); // marks which heights we're choosing ourselves

            fillHeights();
        }

        private void fillHeights()
        {
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
                for (int i = sideLength / 2; i < numSideVertices - sideLength / 2; i += sideLength)
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

        public float getRandInRange(float a, float b)
        {
            float fl = (float)random.NextDouble();
            fl = fl * (b - a);
            fl = fl + a;
            return fl;
        }
    }
}
