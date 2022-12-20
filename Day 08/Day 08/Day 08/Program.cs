namespace Day_08
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            var numTilesX = lines[0].Length + 2;
            var numTilesY = lines.Length + 2;

            var tiles = new TreePosition[numTilesX, numTilesY];

            for (var y = 0; y < numTilesY; y++)
            {
                for (var x = 0; x < numTilesX; x++)
                {
                    // add border of 0-height trees
                    if (x == 0 || y == 0 || x == numTilesX - 1 || y == numTilesY - 1)
                    {
                        tiles[x, y] = new TreePosition() { TreeHeight = -1 };
                        continue;
                    }

                    tiles[x, y] = new TreePosition
                    {
                        TreeHeight = int.Parse(lines[y - 1].Substring(x - 1, 1))
                    };
                }
            }

            for (var y = 0; y < numTilesY; y++)
            {
                int maxHeightLeft = -1;
                for (var x = 0; x < numTilesX; x++)
                {
                    maxHeightLeft = Math.Max(maxHeightLeft, tiles[x, y].TreeHeight);
                    tiles[x, y].MaxHeightLeft = maxHeightLeft;
                }

                int maxHeightRight = -1;
                for (var x = numTilesX - 1; x >= 0; x--)
                {
                    maxHeightRight = Math.Max(maxHeightRight, tiles[x, y].TreeHeight);
                    tiles[x, y].MaxHeightRight = maxHeightRight;
                }
            }

            for (var x = 0; x < numTilesX; x++)
            {
                int maxHeightBottom = -1;
                for (var y = numTilesY - 1; y >= 0; y--)
                {
                    maxHeightBottom = Math.Max(maxHeightBottom, tiles[x, y].TreeHeight);
                    tiles[x, y].MaxHeightBottom = maxHeightBottom;
                }

                int maxHeightTop = -1;
                for (var y = 0; y < numTilesY; y++)
                {
                    maxHeightTop = Math.Max(maxHeightTop, tiles[x, y].TreeHeight);
                    tiles[x, y].MaxHeightTop = maxHeightTop;
                }
            }

            var numVisible = 0;
            for (var y = 1; y < numTilesY-1; y++)
            {
                for (var x = 1; x < numTilesX-1; x++)
                {
                    var tileHeight = tiles[x, y].TreeHeight;
                   
                    if (tileHeight > tiles[x - 1, y].MaxHeightLeft ||
                        tileHeight > tiles[x + 1, y].MaxHeightRight ||
                        tileHeight > tiles[x, y - 1].MaxHeightTop ||
                        tileHeight > tiles[x, y + 1].MaxHeightBottom)
                    {
                        numVisible++;
                    }
                }
            }
            Console.WriteLine(numVisible);

            int maxView = 0;
            for (var y = 1; y < numTilesY - 1; y++)
            {
                for (var x = 1; x < numTilesX - 1; x++)
                {
                    var tileHeight = tiles[x, y].TreeHeight;
                    var numVisibleTop = CountTrees(tiles, x, y, 0, -1, tileHeight);
                    var numVisibleBottom = CountTrees(tiles, x, y, 0, 1, tileHeight);
                    
                    var numVisibleLeft = CountTrees(tiles, x, y, -1, 0, tileHeight);
                    var numVisibleRight = CountTrees(tiles, x, y, 1, 0, tileHeight);
                    
                    maxView = Math.Max(maxView, numVisibleTop * numVisibleLeft * numVisibleRight * numVisibleBottom);
                }
            }
            Console.WriteLine(maxView);
        }

        internal static int CountTrees(TreePosition[,] tiles,int posX, int posY, int directionX, int directionY, int height)
        {
            int sum = 0;
            while(true)
            {
                posX += directionX;
                posY += directionY;

                var tileHeight = tiles[posX, posY].TreeHeight;
                if (tileHeight == -1)
                {
                    return sum;
                }

                sum++;

                if (tileHeight >= height)
                {
                    return sum;
                }

            }

            return sum;
        }
    }

    internal class TreePosition
    {
        public int MaxHeightTop { get; set; }
        public int MaxHeightLeft { get; set; }
        public int MaxHeightRight { get; set; }
        public int MaxHeightBottom { get; set; }

        public int TreeHeight { get; set; }
    }

}