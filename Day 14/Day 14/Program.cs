using System.Runtime.CompilerServices;

namespace Day_14
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var map = ReadMap();
            var minHeight = map.Select(x => x.y).Max();
            var sandAdded = 0;
            while (true)
            {
                var result = findTarget(map, minHeight);
                if (result == null)
                {
                    Console.WriteLine($"Part 1: Amount Sand Added {sandAdded}");
                    break;
                }
                map.Add(result.Value);
                sandAdded++;
            }

            map = ReadMap();
            sandAdded = 0;
            var bottomLineHeight = map.Select(x => x.y).Max() + 2;
            while (true)
            {
                var result = findTargetPart2(map, bottomLineHeight);
                if (result == (500, 0))
                {
                    Console.WriteLine($"Part 2: Amount Sand Added {sandAdded + 1}");
                    return;
                }
                map.Add(result.Value);
                sandAdded++;
            }
        }

        private static HashSet<(int x, int y)> ReadMap()
        {
            HashSet<(int x, int y)> map = new();
            var lines = File.ReadAllLines("input.txt");
            foreach (var line in lines)
            {
                var coordinates = line.Replace(" ", "").Split("->").Select(s =>
                {
                    var coord = s.Split(",");
                    return (x: int.Parse(coord[0]), y: int.Parse(coord[1]));
                }).ToList();

                for (var i = 0; i < coordinates.Count() - 1; i++)
                {
                    InsertLines(coordinates[i], coordinates[i + 1], map);
                }
            }

            return map;
        }

        private static (int x, int y)? findTarget(HashSet<(int x, int y)> map, int minHeight)
        {
            (int x, int y) position = (500, 0);
            while (true)
            {
                if (position.y > minHeight)
                {
                    return null;
                }

                var below = (position.x, position.y + 1);
                if (!map.Contains(below))
                {
                    position = below;
                    continue;
                }

                var leftBelow = (position.x - 1, position.y + 1);
                if (!map.Contains(leftBelow))
                {
                    position = leftBelow;
                    continue;
                }

                var rightBelow = (position.x + 1, position.y + 1);
                if (!map.Contains(rightBelow))
                {
                    position = rightBelow;
                    continue;
                }

                return position;
            }
        }

        private static (int x, int y)? findTargetPart2(HashSet<(int x, int y)> map, int bottomLineHeight)
        {
            (int x, int y) position = (500, 0);
            while (true)
            {
                if (position.y == bottomLineHeight - 1)
                {
                    return position;
                }

                var below = (position.x, position.y + 1);
                if (!map.Contains(below))
                {
                    position = below;
                    continue;
                }

                var leftBelow = (position.x - 1, position.y + 1);
                if (!map.Contains(leftBelow))
                {
                    position = leftBelow;
                    continue;
                }

                var rightBelow = (position.x + 1, position.y + 1);
                if (!map.Contains(rightBelow))
                {
                    position = rightBelow;
                    continue;
                }

                return position;
            }
        }

        private static void InsertLines((int x, int y) c1, (int x, int y) c2,
            HashSet<(int x, int y)> map)
        {
            if (c1.x == c2.x)
            {
                for (var i = Math.Min(c1.y, c2.y); i <= Math.Max(c1.y, c2.y); i++)
                {
                    map.Add((c1.x, i));
                }
                return;
            }

            for (var i = Math.Min(c1.x, c2.x); i <= Math.Max(c1.x, c2.x); i++)
            {
                map.Add((i, c1.y));
            }
        }
    }
}