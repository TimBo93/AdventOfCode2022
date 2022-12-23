using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace Day_18
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            var cubes = lines.Select(x => x.Split(",")).Select(x => new Cube(int.Parse(x[0]), int.Parse(x[1]), int.Parse(x[2]))).ToList();

            

            var maxX = cubes.Max(x => x.X);
            var maxY = cubes.Max(x => x.Y);
            var maxZ = cubes.Max(x => x.Z);

            var map = new bool[maxX+2, maxY+2, maxZ+2];
            foreach (var cube in cubes)
            {
                map[cube.X, cube.Y, cube.Z] = true;
            }

            Console.WriteLine($"Solution Part 1 {GetSurface(cubes, map)}");

            var reachableMap = new bool[maxX + 2, maxY + 2, maxZ + 2];
            MarkAllReachableCubes(map, reachableMap);

            var xDimension = reachableMap.GetLength(0);
            var yDimension = reachableMap.GetLength(1);
            var zDimension = reachableMap.GetLength(2);

            List<Cube> cubesAndLockedCubes = new();
            for (int x = 0; x < xDimension; x++)
            {
                for (int y = 0; y < yDimension; y++)
                {
                    for (int z = 0; z < zDimension; z++)
                    {
                        if (!reachableMap[x, y, z] || map[x, y, z])
                        {
                            cubesAndLockedCubes.Add(new Cube(x, y, z));
                        }
                    }
                }
            }

            var lockedMap = new bool[maxX + 2, maxY + 2, maxZ + 2];
            foreach (var lockedCube in cubesAndLockedCubes)
            {
                lockedMap[lockedCube.X, lockedCube.Y, lockedCube.Z] = true;
            }

            Console.WriteLine($"Solution Part 2 {GetSurface(cubesAndLockedCubes, lockedMap)}");
        }

        private static void MarkAllReachableCubes(bool[,,] map, bool[,,] reachableMap)
        {
            Queue<(int x, int y, int z)> expandQueue = new();

            var xDimension = reachableMap.GetLength(0);
            var yDimension = reachableMap.GetLength(1);
            var zDimension = reachableMap.GetLength(2);

            // Top / Bottom
            for (int x = 0; x < xDimension; x++)
            {
                for (int y = 0; y < yDimension; y++)
                {
                    expandQueue.Enqueue((x, y, 0));
                    expandQueue.Enqueue((x, y, zDimension - 1));
                }
            }

            // Left / Right
            for (int y = 0; y < yDimension; y++)
            {
                for (int z = 0; z < zDimension; z++)
                {
                    expandQueue.Enqueue((0, y, z));
                    expandQueue.Enqueue((xDimension - 1, y, z));
                }
            }

            // Front Back
            for (int x = 0; x < xDimension; x++)
            {
                for (int z = 0; z < zDimension; z++)
                {
                    expandQueue.Enqueue((x, 0, z));
                    expandQueue.Enqueue((x, yDimension - 1, z));
                }
            }

            while (expandQueue.TryDequeue(out var result))
            {
                reachableMap[result.x, result.y, result.z] = true;

                if (map[result.x, result.y, result.z])
                {
                    // blocked
                    continue;
                }
                
                // top
                if (result.z + 1 < zDimension && !map[result.x, result.y, result.z + 1] && !reachableMap[result.x, result.y, result.z + 1])
                {
                    expandQueue.Enqueue((result.x, result.y, result.z + 1));
                }
                // bottom
                if (result.z > 0 && !map[result.x, result.y, result.z - 1] && !reachableMap[result.x, result.y, result.z - 1])
                {
                    expandQueue.Enqueue((result.x, result.y, result.z -  1));
                }

                // left
                if (result.x + 1 < xDimension && !map[result.x + 1, result.y, result.z] && !reachableMap[result.x + 1, result.y, result.z])
                {
                    expandQueue.Enqueue((result.x + 1, result.y, result.z));
                }
                // right
                if (result.x - 1 > 0 && !map[result.x - 1, result.y, result.z] && !reachableMap[result.x - 1, result.y, result.z])
                {
                    expandQueue.Enqueue((result.x - 1, result.y, result.z));
                }

                // front
                if (result.y + 1 < yDimension && !map[result.x, result.y + 1, result.z] && !reachableMap[result.x, result.y + 1, result.z])
                {
                    expandQueue.Enqueue((result.x, result.y + 1, result.z));
                }
                // back
                if (result.y > 0 && !map[result.x, result.y - 1, result.z] && !reachableMap[result.x, result.y - 1, result.z])
                {
                    expandQueue.Enqueue((result.x, result.y - 1, result.z));
                }

            }
        }


        private static int GetSurface(List<Cube> cubes, bool[,,] map)
        {
            var sumSurface = 0;
            foreach (var cube in cubes)
            {
                // top
                sumSurface += map[cube.X, cube.Y, cube.Z + 1] ? 0 : 1;
                // bottom
                sumSurface += map[cube.X, cube.Y, cube.Z - 1] ? 0 : 1;

                // left
                sumSurface += map[cube.X - 1, cube.Y, cube.Z] ? 0 : 1;
                // right
                sumSurface += map[cube.X + 1, cube.Y, cube.Z] ? 0 : 1;

                // front
                sumSurface += map[cube.X, cube.Y + 1, cube.Z] ? 0 : 1;
                // back
                sumSurface += map[cube.X, cube.Y - 1, cube.Z] ? 0 : 1;
            }
            return sumSurface;
        }
    }

    internal class Cube
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public Cube(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}