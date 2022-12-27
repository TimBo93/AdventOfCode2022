namespace Day_24;

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var map = new Map(lines);
        map.GetBliizardMapForMinute(0, 0);
        var simulator = new Simulator(map);

        var startPosition = (0, -1);
        var targetPosition = (map.Width - 1, map.Height - 1);
        var solutionPart1 = simulator.Simulate(startPosition, targetPosition, 0);
        Console.WriteLine($"Solution Part 1: {solutionPart1}");

        var startPositionBack = (map.Width - 1, map.Height);
        var targetPositionBack = (0, 0);

        var stepBack = simulator.Simulate(startPositionBack, targetPositionBack, solutionPart1);
        Console.WriteLine($"Back {stepBack}");
        var stepForth= simulator.Simulate(startPosition, targetPosition, solutionPart1 + stepBack);
        Console.WriteLine($"And back again: {stepForth}");
        Console.WriteLine($"Solution Part 2: {solutionPart1 + stepBack + stepForth}");
    }
}

internal class Simulator
{
    private readonly Map _map;
    private readonly HashSet<(int x, int y, int minute)> _alreadyEnqueued = new();
    private readonly PriorityQueue<((int x, int y) position, int minute), int> _queue = new();

    public Simulator(Map map)
    {
        _map = map;
    }

    public int Simulate((int x, int y) initPosition, (int x, int y) targetPosition, int minuteOffset)
    {
        _map.Reset();
        _queue.Clear();
        _alreadyEnqueued.Clear();

        _queue.Enqueue((initPosition, 0), CalculatePriority(initPosition, targetPosition, 0));

        while (_queue.TryDequeue(out var state, out _))
        {
            var position = state.position;
            var minute = state.minute;

            if (position == targetPosition)
            {
                // trivial -> move to exit
                return minute + 1;
            }
                

            var nextMinute = minute + 1;
            var nextBlizzardMap = _map.GetBliizardMapForMinute(nextMinute, minuteOffset);

            if (position == initPosition)
            {
                if (initPosition.y < 0)
                {
                    if (nextBlizzardMap[0, 0] == false)
                        // move down
                        Queue((initPosition.x, initPosition.y + 1), targetPosition, nextMinute);

                    // stay here
                    Queue(initPosition, targetPosition, nextMinute);
                }
                else
                {
                    if (nextBlizzardMap[initPosition.x, initPosition.y - 1] == false)
                        // move up
                        Queue((initPosition.x, initPosition.y - 1), targetPosition, nextMinute);

                    // stay here
                    Queue(initPosition, targetPosition, nextMinute);
                }
                


                continue;
            }

            // Up
            if (position.y > 0)
            {
                var nextPos = (position.x, y: position.y - 1);
                if (!nextBlizzardMap[nextPos.x, nextPos.y])
                    Queue(nextPos, targetPosition, nextMinute);
            }

            // Down
            if (position.y < _map.Height - 1)
            {
                var nextPos = (position.x, y: position.y + 1);
                if (!nextBlizzardMap[nextPos.x, nextPos.y])
                    Queue(nextPos, targetPosition, nextMinute);
            }

            // Left 
            if (position.x > 0)
            {
                var nextPos = (x: position.x - 1, position.y);
                if (!nextBlizzardMap[nextPos.x, nextPos.y])
                    Queue(nextPos, targetPosition, nextMinute);
            }

            // Right
            if (position.x < _map.Width - 1)
            {
                var nextPos = (x: position.x + 1, position.y);
                if (!nextBlizzardMap[nextPos.x, nextPos.y])
                    Queue(nextPos, targetPosition, nextMinute);
            }

            // Wait
            if (!nextBlizzardMap[position.x, position.y])
                Queue(position, targetPosition, nextMinute);
        }

        throw new Exception("No Solution found");
    }

    private void Queue((int x, int y) position, (int x, int y) targetPosition, int minute)
    {
        var queueItem = (position.x, position.y, minute);
        if (_alreadyEnqueued.Contains(queueItem))
        {
            return;
        }

        _alreadyEnqueued.Add(queueItem);
        _queue.Enqueue((position, minute), CalculatePriority(position, targetPosition, minute));
    }

    private int CalculatePriority((int x, int y) position, (int x, int y) targetPosition, int minute)
    {
        return minute + Math.Abs(targetPosition.x - position.x) + Math.Abs(targetPosition.y - position.y);
    }
}

internal class Map
{
    private readonly List<bool[,]> _blizzardMap = new();
    private readonly IReadOnlyList<Blizzard> _blizzards;

    public Map(string[] lines)
    {
        Width = lines[0].Length - 2;
        Height = lines.Length - 2;

        var blizzards = new List<Blizzard>();
        for (var y = 0; y < Height; y++)
        {
            var line = lines[y + 1];
            for (var x = 0; x < Width; x++)
            {
                if (line[x + 1] == '.') continue;

                blizzards.Add(new Blizzard(x, y, line[x + 1] switch
                {
                    '^' => Direction.Up,
                    'v' => Direction.Down,
                    '<' => Direction.Left,
                    '>' => Direction.Right,
                    _ => throw new Exception("Invalid input")
                }));
            }
        }

        _blizzards = blizzards;
    }

    public int Width { get; }
    public int Height { get; }

    public bool[,] GetBliizardMapForMinute(int minute, int minuteOffset)
    {
        var minuteToFetch = minute + minuteOffset;

        if (_blizzardMap.Count >= minuteToFetch + 1) return _blizzardMap[minuteToFetch];

        _blizzardMap.Add(CalculateBlizzardMap(minuteToFetch));
        return _blizzardMap.Last();
    }

    private bool[,] CalculateBlizzardMap(int minute)
    {
        var result = new bool[Width, Height];
        foreach (var blizzard in _blizzards)
        {
            var pos = blizzard.GetPositionForMinute(minute, Width, Height);
            result[pos.x, pos.y] = true;
        }

        return result;
    }

    public void Reset()
    {
        _blizzardMap.Clear();
    }
}

internal enum Direction
{
    Up,
    Down,
    Left,
    Right
}

internal class Blizzard
{
    public Blizzard(int x, int y, Direction direction)
    {
        X = x;
        Y = y;
        Direction = direction;
    }

    public int X { get; }
    public int Y { get; }
    public Direction Direction { get; }

    public (int x, int y) GetPositionForMinute(int minute, int mapWidth, int mapHeight)
    {
        return ((X + Direction switch
            {
                Direction.Up => 0,
                Direction.Down => 0,
                Direction.Left => mapWidth - 1,
                Direction.Right => 1,
                _ => throw new Exception("Invalid direction")
            } * minute) % mapWidth,
            (Y + Direction switch
            {
                Direction.Up => mapHeight - 1,
                Direction.Down => 1,
                Direction.Left => 0,
                Direction.Right => 0,
                _ => throw new Exception("Invalid direction")
            } * minute) % mapHeight);
    }
}