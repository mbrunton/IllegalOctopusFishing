using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class SearchTree
    {
        Dictionary<float, Dictionary<float, Index>> map;
        List<float> xVals;
        List<float> zVals;

        public SearchTree(HeightMap heightMap)
        {
            int numSideVertices = heightMap.numSideVertices;
            List<List<Vector3>> grid = heightMap.getGrid();
            if (numSideVertices > 1 && !MathUtil.IsZero(grid[0][0].X - grid[0][1].X))
            {
                throw new ArgumentException("SearchTree expects HeightMap.grid to have constant X values in inner list");
            }
            if (numSideVertices > 1 && !MathUtil.IsZero(grid[0][0].Z - grid[1][0].Z))
            {
                throw new ArgumentException("SearchTree expects HeightMap.grid to have constant Z values in outer list");
            }
            
            float minX = heightMap.minX;
            float maxX = heightMap.maxX;
            float minZ = heightMap.minZ;
            float maxZ = heightMap.maxZ;
            float stepSize = heightMap.stepSize;

            xVals = new List<float>();
            for (int i = 0; i < numSideVertices; i++)
            {
                xVals.Add(minX + i * stepSize);
            }

            zVals = new List<float>();
            for (int j = 0; j < numSideVertices; j++)
            {
                zVals.Add(minZ + j * stepSize);
            }

            map = new Dictionary<float, Dictionary<float, Index>>();
            for (int i = 0; i < numSideVertices; i++)
            {
                float x = xVals[i];
                Dictionary<float, Index> zToIndex = new Dictionary<float, Index>();
                for (int j = 0; j < numSideVertices; j++)
                {
                    float z = zVals[j];
                    zToIndex.Add(z, new Index(i, j));
                }
                map.Add(x, zToIndex);
            }
        }

        public Index getIndexAtPosition(float x, float z)
        {
            float xFloor = getFloorOfX(x);
            float zFloor = getFloorOfZ(z);

            Dictionary<float, Index> zToIndex;
            map.TryGetValue(xFloor, out zToIndex);
            Index index;
            zToIndex.TryGetValue(zFloor, out index);

            return index;
        }

        private float getFloorOfX(float x)
        {
            int lo = 0;
            int hi = xVals.Count - 1;
            if (x < xVals[lo]) {
                return xVals[lo];
            }
            if (x > xVals[hi]) {
                return xVals[hi];
            }

            while (hi >= lo)
            {
                int mid = lo + (hi - lo) / 2;
                if (x >= xVals[mid] && x < xVals[mid + 1])
                {
                    return xVals[mid];
                }
                else if (x < xVals[mid])
                {
                    hi = mid - 1;
                }
                else
                {
                    lo = mid + 1;
                }
            }

            throw new Exception("getFloorOfX failed with argument: " + x.ToString());
        }

        private float getFloorOfZ(float z)
        {
            int lo = 0;
            int hi = zVals.Count - 1;
            if (z < zVals[lo])
            {
                return zVals[lo];
            }
            if (z > zVals[hi])
            {
                return zVals[hi];
            }

            while (hi >= lo)
            {
                int mid = lo + (hi - lo) / 2;
                if (z >= zVals[mid] && z < zVals[mid + 1])
                {
                    return zVals[mid];
                }
                else if (z < zVals[mid])
                {
                    hi = mid - 1;
                }
                else
                {
                    lo = mid + 1;
                }
            }

            throw new Exception("getFloorOfZ failed with argument: " + z.ToString());
        }
    }
}
