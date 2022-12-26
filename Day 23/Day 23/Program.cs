using System.Collections.ObjectModel;

namespace Day_23;

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var simulator = new Simulator(lines);

        for (int i = 0; i < 10; i++)
        {
            simulator.SimulateRound();
        }
        Console.WriteLine($"Solution Part 1: {simulator.GetFreeSpace()}");


        simulator = new Simulator(lines);
        var round = 1;
        while (simulator.SimulateRound())
        {
            round++;
        }
        Console.WriteLine($"Solution Part 2: {round}");
    }
}

internal enum ProbeDirection
{
    North,
    South,
    East,
    West
}

internal class Simulator
{
    private readonly IReadOnlyList<Elf> _elves;

    public LinkedList<ProbeDirection> DirectionPriority = new(new[]
        { ProbeDirection.North, ProbeDirection.South, ProbeDirection.West, ProbeDirection.East });

    public Simulator(string[] map)
    {
        _elves = Parse(map).ToList();
    }


    public bool SimulateRound()
    {
        var map = CreateMap().AsReadOnly();
        //Render(map);
        IReadOnlyList<ProbeDirection> directions = new List<ProbeDirection>(DirectionPriority);
        UpdateTargetPositions(map, directions);

        if (!_elves.Any(x => x.CanMove))
        {
            return false;
        }

        var proposedPositions = CreateProposedPositionMap();
        MoveElves(proposedPositions);
        ShuffleDirection();

        return true;
    }

    private void MoveElves(IReadOnlyDictionary<(int x, int y), List<Elf>> proposedPositions)
    {
        foreach (var elf in _elves)
        {
            elf.Move(proposedPositions);
        }
    }

    private IReadOnlyDictionary<(int x, int y), List<Elf>> CreateProposedPositionMap()
    {
        var proposedPositionMap = new Dictionary<(int x, int y), List<Elf>>();

        foreach (var elf in _elves.Where(x => x.CanMove))
        {
            if (proposedPositionMap.TryGetValue((elf.ProposedPosition.Value.x, elf.ProposedPosition.Value.y), out var list))
            {
                list.Add(elf);
                continue;
            }
            proposedPositionMap[(elf.ProposedPosition.Value.x, elf.ProposedPosition.Value.y)] = new List<Elf> { elf };
        }
        return proposedPositionMap;
    }

    private void Render(ReadOnlyDictionary<(int x, int y), Elf> map)
    {
        var minX = _elves.Min(x => x.X);
        var minY = _elves.Min(x => x.Y);

        var maxX = _elves.Max(x => x.X);
        var maxY = _elves.Max(x => x.Y);

        for (int y = minY; y <= maxY; y++)
        {
            for(int x = minX; x <= maxX; x++)
            {
                Console.Write(map.ContainsKey((x, y)) ? "#" : ".");
            }
            Console.WriteLine();
        }
    }

    private void UpdateTargetPositions(ReadOnlyDictionary<(int x, int y), Elf> map,
        IReadOnlyList<ProbeDirection> directions)
    {
        foreach (var elf in _elves) elf.UpdateTargetPosition(map, directions);
    }

    private void ShuffleDirection()
    {
        DirectionPriority.AddLast(DirectionPriority.First.Value);
        DirectionPriority.RemoveFirst();
    }

    private Dictionary<(int x, int y), Elf> CreateMap()
    {
        var map = new Dictionary<(int x, int y), Elf>();
        foreach (var elf in _elves) map.Add((elf.X, elf.Y), elf);
        return map;
    }

    private IEnumerable<Elf> Parse(string[] map)
    {
        for (var y = 0; y < map.Length; y++)
        {
            var line = map[y];
            for (var x = 0; x < line.Length; x++)
                if (line[x] == '#')
                    yield return new Elf(x, y);
        }
    }

    public int GetFreeSpace()
    {
        var minX = _elves.Min(x => x.X);
        var minY = _elves.Min(x => x.Y);

        var maxX = _elves.Max(x => x.X);
        var maxY = _elves.Max(x => x.Y);

        return (maxX - minX + 1) * (maxY - minY + 1) - _elves.Count;
    }
}

internal class Elf
{
    public Elf(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; private set; }
    public int Y { get; private set; }

    public bool CanMove { get; private set; } = false;
    public (int x, int y)? ProposedPosition { get; private set; } = null;

    public void UpdateTargetPosition(ReadOnlyDictionary<(int x, int y), Elf> map,
        IReadOnlyList<ProbeDirection> directionPriorities)
    {
        var nw = map.TryGetValue((X - 1, Y - 1), out _);
        var n = map.TryGetValue((X, Y - 1), out _);
        var ne = map.TryGetValue((X + 1, Y - 1), out _);
        var e = map.TryGetValue((X + 1, Y), out _);
        var se = map.TryGetValue((X + 1, Y + 1), out _);
        var s = map.TryGetValue((X, Y + 1), out _);
        var sw = map.TryGetValue((X - 1, Y + 1), out _);
        var w = map.TryGetValue((X - 1, Y), out _);

        if (!(nw || n || ne || e || se || s || sw || w))
        {
            CanMove = false;
            ProposedPosition = null;
            return;
        }

        foreach (var directionPriority in directionPriorities)
        {
            switch (directionPriority)
            {
                case ProbeDirection.North:
                    if (!(nw || n || ne))
                    {
                        ProposedPosition = (X, Y - 1);
                        CanMove = true;
                        return;
                    }
                    break;
                case ProbeDirection.South:
                    if (!(sw || s || se))
                    {
                        ProposedPosition = (X, Y + 1);
                        CanMove = true;
                        return;
                    }
                    break;
                case ProbeDirection.East:
                    if (!(ne || e || se))
                    {
                        ProposedPosition = (X + 1, Y);
                        CanMove = true;
                        return;
                    }
                    break;
                case ProbeDirection.West:
                    if (!(nw || w || sw))
                    {
                        ProposedPosition = (X - 1, Y);
                        CanMove = true;
                        return;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        CanMove = false;
        ProposedPosition = null;
    }

    public void Move(IReadOnlyDictionary<(int x, int y), List<Elf>> proposedPositions)
    {
        try
        {
            if (!CanMove)
            {
                return;
            }

            if (proposedPositions[(ProposedPosition.Value.x, ProposedPosition.Value.y)].Count == 1)
            {
                X = this.ProposedPosition.Value.x;
                Y = this.ProposedPosition.Value.y;
            }
        }
        finally
        {
            this.ProposedPosition = null;
            this.CanMove = false;
        }
    }
}