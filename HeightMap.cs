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
    class HeightMap
    {
        private float worldSize;
        private float verticesPerLength;

        private List<List<Vector3>> grid;
        private List<List<Vector3>> normalGrid;
        private bool isGridFilled = false;
        private bool isNormalGridFilled = false;

        public int numSideVertices;
        public float minX, maxX, minZ, maxZ;
        public float stepSize;

        public HeightMap(float worldSize, float verticesPerLength) : this(worldSize, verticesPerLength, false) {}

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
            numSideVertices = (int)Math.Pow(2, Math.Ceiling(Math.Log(floatSideVertices, 2))) + 1; // next power of 2, plus 1
            stepSize = worldSize / (numSideVertices - 1);

            if (fillWithZero)
            {
                fillGridWithZero();
            }
        }

        internal float getMinY()
        {
            if (!isGridFilled)
            {
                throw new Exception("cannot yet min y before grid has been filled");
            }

            float minY = grid[0][0].Y;
            for (int i = 0; i < numSideVertices; i++)
            {
                for (int j = 0; j < numSideVertices; j++)
                {
                    if (grid[i][j].Y < minY)
                    {
                        minY = grid[i][j].Y;
                    }
                }
            }
            return minY;
        }

        internal float getMaxY()
        {
            if (!isGridFilled)
            {
                throw new Exception("cannot yet max y before grid has been filled");
            }
            float maxY = grid[0][0].Y;
            for (int i = 0; i < numSideVertices; i++)
            {
                for (int j = 0; j < numSideVertices; j++)
                {
                    if (grid[i][j].Y > maxY)
                    {
                        maxY = grid[i][j].Y;
                    }
                }
            }
            return maxY;
        }

        // (i,j) is indices of corner square, which contains position (x,z)
        internal float getHeightAtPosition(Index index, float x, float z)
        {
            int i = index.i;
            int j = index.j;
            if (i == numSideVertices - 1 || j == numSideVertices - 1 || i == 0 || j == 0)
            {
                // we don't have a bounding square.. just return height at vertex
                return grid[i][j].Y;
            }

            float leftX = grid[i][j].X;
            float rightX = grid[i + 1][j].X;
            float bottomZ = grid[i][j].Z;
            float topZ = grid[i][j + 1].Z;

            if (!(leftX <= x && x <= rightX)) { throw new Exception("x coord not within bounding square"); }
            if (!(bottomZ <= z && z <= topZ)) { throw new Exception("z coord not within bounding square"); }

            float bottomLeftY = grid[i][j].Y;
            float topLeftY = grid[i][j+1].Y;
            float bottomRightY = grid[i+1][j].Y;

            float xProp = (x - leftX) / (rightX - leftX);
            float zProp = (z - bottomZ) / (topZ - bottomZ);

            float height = bottomLeftY + xProp * (bottomRightY - bottomLeftY) + zProp * (topLeftY - bottomLeftY);
            return height;
            /*
             * max height
            float maxHeight = grid[i][j].Y;
            for (int m = i; m < i + 1; m++)
            {
                for (int n = j; n < j + 1; n++)
                {
                    if (m < numSideVertices && n < numSideVertices)
                    {
                        if (grid[m][n].Y > maxHeight)
                        {
                            maxHeight = grid[m][n].Y;
                        }
                    }
                }
            }
            return maxHeight;
             */
        }

        public List<List<Vector3>> getGrid()
        {
            if (!isGridFilled)
            {
                throw new ArgumentException("Grid hasn't been filled");
            }
            return this.grid;
        }

        public List<List<Vector3>> getNormalGrid()
        {
            if (!isNormalGridFilled)
            {
                throw new ArgumentException("Normal grid hasn't been filled");
            }
            return this.normalGrid;
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
                    float z = minZ + j * stepSize;
                    float y = 0f;
                    zRow.Add(new Vector3(x, y, z));
                }
                grid.Add(zRow);
            }

            isGridFilled = true;
        }

        internal void fillGridFromHeights(List<List<float>> diamondSquareHeights)
        {
            grid = new List<List<Vector3>>();
            for (int i = 0; i < numSideVertices; i++)
            {
                List<Vector3> zRow = new List<Vector3>();
                for (int j = 0; j < numSideVertices; j++)
                {
                    float x = minX + i * stepSize;
                    float z = minZ + j * stepSize;
                    float y = diamondSquareHeights[i][j];
                    zRow.Add(new Vector3(x, y, z));
                }
                grid.Add(zRow);
            }

            isGridFilled = true;
        }

        internal void fillNormalGrid(bool isRound)
        {
            if (isRound)
            {
                fillRoundNormalGrid();
            }
            else
            {
                fillFlatNormalGrid();
            }

            isNormalGridFilled = true;
        }

        private void fillRoundNormalGrid()
        {
            normalGrid = new List<List<Vector3>>();
            for (int i = 0; i < numSideVertices; i++)
            {
                List<Vector3> row = new List<Vector3>();
                for (int j = 0; j < numSideVertices; j++)
                {
                    Vector3 normal = Vector3.Zero;
                    Vector3 u, v;
                    // normal of upper left
                    if (i > 0 && j > 0)
                    {
                        u = grid[i][j] - grid[i - 1][j];
                        v = grid[i][j - 1] - grid[i][j];
                        normal = normal + Vector3.Cross(u, v);
                    }
                    // normal of upper right
                    if (i > 0 && j < grid[0].Count - 1)
                    {
                        u = grid[i][j] - grid[i][j + 1];
                        v = grid[i - 1][j] - grid[i][j];
                        normal = normal + Vector3.Cross(u, v);
                    }
                    // normal of lower right
                    if (i < numSideVertices - 1 && j < numSideVertices - 1)
                    {
                        u = grid[i][j] - grid[i + 1][j];
                        v = grid[i][j + 1] - grid[i][j];
                        normal = normal + Vector3.Cross(u, v);
                    }
                    // normal of lower left
                    if (i < numSideVertices - 1 && j > 0)
                    {
                        u = grid[i][j] - grid[i][j - 1];
                        v = grid[i + 1][j] - grid[i][j];
                        normal = normal + Vector3.Cross(u, v);
                    }
                    normal.Normalize();
                    row.Add(normal);
                }
                normalGrid.Add(row);
            }
        }

        private void fillFlatNormalGrid()
        {
            normalGrid = new List<List<Vector3>>();
            for (int i = 0; i < numSideVertices; i++)
            {
                List<Vector3> row = new List<Vector3>();
                for (int j = 0; j < numSideVertices; j++)
                {
                    Vector3 u, v;
                    if (i < numSideVertices - 1 && j < numSideVertices - 1)
                    {
                        u = grid[i][j] - grid[i + 1][j];
                        v = grid[i][j + 1] - grid[i][j];
                    }
                    else if (i < numSideVertices - 1 && j > 0)
                    {
                        u = grid[i][j] - grid[i][j - 1];
                        v = grid[i + 1][j] - grid[i][j];
                    }
                    else if (i > 0 && j < numSideVertices - 1)
                    {
                        u = grid[i][j] - grid[i][j + 1];
                        v = grid[i - 1][j] - grid[i][j];
                    }
                    else if (i > 0 && j > 0)
                    {
                        u = grid[i][j] - grid[i - 1][j];
                        v = grid[i][j - 1] - grid[i][j];
                    }
                    else
                    {
                        throw new ArgumentException("HeightMap.numSideVertices must be >= 2 to calculate normals");
                    }
                    Vector3 normal = Vector3.Cross(u, v);
                    normal.Normalize();
                    row.Add(normal);
                }
                normalGrid.Add(row);
            }
        }

        internal List<VertexPositionNormalColor> getVertexPositionNormalColorList(Func<float, Color> getColorAtHeight)
        {
            if (!isGridFilled || !isNormalGridFilled)
            {
                throw new ArgumentException("Cannot get VPNC list until grid and normalGrid both filled");
            }
            
            List<VertexPositionNormalColor> VPNClist = new List<VertexPositionNormalColor>();
            for (int i = 0; i < numSideVertices - 1; i++)
            {
                for (int j = 0; j < numSideVertices - 1; j++)
                {
                    VertexPositionNormalColor topleft = new VertexPositionNormalColor(grid[i][j], normalGrid[i][j], getColorAtHeight(grid[i][j].Y));
                    VertexPositionNormalColor topright = new VertexPositionNormalColor(grid[i][j + 1], normalGrid[i][j + 1], getColorAtHeight(grid[i][j + 1].Y));
                    VertexPositionNormalColor bottomright = new VertexPositionNormalColor(grid[i + 1][j + 1], normalGrid[i + 1][j + 1], getColorAtHeight(grid[i + 1][j + 1].Y));
                    VertexPositionNormalColor bottomleft = new VertexPositionNormalColor(grid[i + 1][j], normalGrid[i + 1][j], getColorAtHeight(grid[i + 1][j].Y));

                    VPNClist.Add(topleft);
                    VPNClist.Add(bottomright);
                    VPNClist.Add(topright);
                    
                    VPNClist.Add(topleft);
                    VPNClist.Add(bottomleft);
                    VPNClist.Add(bottomright);
                }
            }
            
            return VPNClist;
        }
    }
}
