using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;

namespace IllegalOctopusFishing
{
    class TwoDTree
    {
        private HorizontalNode root;

        public TwoDTree(List<List<Vector3>> grid, int numSideVertices)
        {
            if (!IsPowerOfTwo(numSideVertices - 1))
            {
                throw new ArgumentException("numSideVertices should be a power of two, plus 1, for TwoDTree");
            }
            if (numSideVertices > 1 && grid[0][0].X > grid[1][0].X)
            {
                throw new ArgumentException("outer list should be increasing X axis");
            }
            if (numSideVertices > 1 && grid[0][0].Z > grid[0][1].Z)
            {
                throw new ArgumentException("inner list should be increasing Z axis");
            }

            int horizLow = 0;
            int horizHigh = numSideVertices - 1;
            int vertLow = horizLow;
            int vertHigh = horizHigh;
            root = BuildTreeHoriz(grid, horizLow, horizHigh, vertLow, vertHigh);
        }

        class HorizontalNode
        {
            public Vector3 key;
            public VerticalNode leftChild;
            public VerticalNode rightChild;

            public HorizontalNode(Vector3 key)
            {
                this.key = key;
            }
        }

        class VerticalNode
        {
            public Vector3 key;
            public HorizontalNode topChild;
            public HorizontalNode bottomChild;
        
            public VerticalNode(Vector3 key)
            {
                this.key = key;
            }
        }

        private HorizontalNode BuildTreeHoriz(List<List<Vector3>> grid, int horizLow, int horizHigh, int vertLow, int vertHigh)
        {
            if (horizHigh < horizLow || vertHigh < vertLow)
            {
                return null;
            }
            int i = horizLow + (horizHigh - horizLow) / 2;
            int j = vertLow + (vertHigh - vertLow) / 2;
            Vector3 key = grid[i][j];
            HorizontalNode horizNode = new HorizontalNode(key);
            horizNode.leftChild = BuildTreeVert(grid, horizLow, i - 1, vertLow, vertHigh);
            horizNode.leftChild = BuildTreeVert(grid, i + 1, horizHigh, vertLow, vertHigh);

            return horizNode;
        }

        private VerticalNode BuildTreeVert(List<List<Vector3>> grid, int horizLow, int horizHigh, int vertLow, int vertHigh)
        {
            if (horizHigh < horizLow || vertHigh < vertLow)
            {
                return null;
            }
            int i = horizLow + (horizHigh - horizLow) / 2;
            int j = vertLow + (vertHigh - vertLow) / 2;
            Vector3 key = grid[i][j];
            VerticalNode vertNode = new VerticalNode(key);
            vertNode.bottomChild = BuildTreeHoriz(grid, horizLow, horizHigh, vertLow, j - 1);
            vertNode.topChild = BuildTreeHoriz(grid, horizLow, horizHigh, j + 1, vertHigh);

            return vertNode;
        }

        private bool IsPowerOfTwo(int x)
        {
            if (x <= 0)
            {
                return false;
            }
            uint y = (uint)x;
            return ((y & (y - 1)) == 0);
        }
    }
}
