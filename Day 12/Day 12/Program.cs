namespace Day_12;

internal class Program
{
    private static void Main(string[] args)
    {
        var mapLines = File.ReadAllLines("input.txt");
        var mapWidth = mapLines[0].Length;
        var mapHeight = mapLines.Length;

        var heightMap = new int[mapWidth, mapHeight];

        (int x, int y) startPosition = (0,0);
        (int x, int y) endPosition = (0, 0);
        for (var y = 0; y < mapHeight; y++)
        {
            var line = mapLines[y];
            for (var x = 0; x < mapWidth; x++)
            {
                if (line[x] == 'S')
                {
                    heightMap[x, y] = 0;
                    startPosition = (x, y);
                    continue;
                }

                if (line[x] == 'E')
                {
                    heightMap[x, y] = 'z' - 'a';
                    endPosition = (x, y);
                    continue;
                }

                var height = line[x] - 'a';
                heightMap[x, y] = height;
            }
        }
        var part1 = SolveFromStartingPoint(heightMap, startPosition, endPosition);
        Console.WriteLine($"found shortest path from start after {part1} steps.");

        // Part 2
        var minHeight = int.MaxValue;
        for (var x = 0; x < mapWidth; x++)
        {
            for (var y = 0; y < mapHeight; y++)
            {
                if (heightMap[x, y] == 0)
                {
                    minHeight = Math.Min(minHeight, SolveFromStartingPoint(heightMap, (x, y), endPosition));
                }
            }
        }
        Console.WriteLine($"found shortest from any position after {minHeight} steps.");
    }

    private static int SolveFromStartingPoint(int[,] heightMap, (int x, int y) startingPoint, (int x, int y) endPoint)
    {
        var mapWidth = heightMap.GetLength(0);
        var mapHeight = heightMap.GetLength(1);

        var alreadyVisited = new bool[mapWidth, mapHeight];

        var extendablePositions = new PriorityQueue<WeightedPosition, int>();
        extendablePositions.Enqueue(new WeightedPosition(){Position = startingPoint, Weight = 0}, 0);

        while (true)
        {
            if (extendablePositions.Count == 0)
            {
                return int.MaxValue;
            }

            var shortestPath = extendablePositions.Dequeue();

            if (alreadyVisited[shortestPath.Position.x, shortestPath.Position.y]) continue;

            alreadyVisited[shortestPath.Position.x, shortestPath.Position.y] = true;
            foreach (var neighbor in GetAllNeighbors(heightMap, alreadyVisited, shortestPath.Position))
            {
                if (neighbor == endPoint)
                {
                    return shortestPath.Weight + 1;
                }

                extendablePositions.Enqueue(
                    new WeightedPosition { Position = neighbor, Weight = shortestPath.Weight + 1 },
                    shortestPath.Weight + 1);
            }
        }
    }

    private static IEnumerable<(int x, int y)> GetAllNeighbors(int[,] heightMap, bool[,] alreadyVisited,
        (int x, int y) currentPosition)
    {
        var currentHeight = heightMap[currentPosition.x, currentPosition.y];

        if (currentPosition.x > 0 && heightMap[currentPosition.x - 1, currentPosition.y] <= currentHeight + 1)
            yield return (currentPosition.x - 1, currentPosition.y);

        if (currentPosition.y > 0 && heightMap[currentPosition.x, currentPosition.y - 1] <= currentHeight + 1)
            yield return (currentPosition.x, currentPosition.y - 1);

        var mapWidth = heightMap.GetLength(0);
        var mapHeight = heightMap.GetLength(1);
        if (currentPosition.x + 1 < mapWidth &&
            heightMap[currentPosition.x + 1, currentPosition.y] <= currentHeight + 1)
            yield return (currentPosition.x + 1, currentPosition.y);

        if (currentPosition.y + 1 < mapHeight &&
            heightMap[currentPosition.x, currentPosition.y + 1] <= currentHeight + 1)
            yield return (currentPosition.x, currentPosition.y + 1);
    }
}

internal class WeightedPosition
{
    public int Weight { get; init; }
    public (int x, int y) Position { get; init; }
}