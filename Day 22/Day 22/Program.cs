using System.Diagnostics;
using static System.Char;

// 134050 too high 
// 105298 too high
// 11562 false

namespace Day_22;

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadLines("input.txt").ToArray();
        var map = new Map(lines);

        var instructionSet = new InstructionSet(lines.Last());

        Edge a = new(new Position(50, 0), new Position(99, 0), Direction.Up);
        Edge b = new(new Position(100, 0), new Position(149, 0), Direction.Up);
        Edge c = new(new Position(149, 0), new Position(149, 49), Direction.Right);
        Edge d = new(new Position(100, 49), new Position(149, 49), Direction.Down);
        Edge e = new(new Position(99, 50), new Position(99, 99), Direction.Right);
        Edge f = new(new Position(99, 100), new Position(99, 149), Direction.Right);
        Edge g = new(new Position(50, 149), new Position(99, 149), Direction.Down);
        Edge h = new(new Position(49, 150), new Position(49, 199), Direction.Right);
        Edge i = new(new Position(0, 199), new Position(49, 199), Direction.Down);
        Edge j = new(new Position(0, 150), new Position(0, 199), Direction.Left);
        Edge k = new(new Position(0, 100), new Position(0, 149), Direction.Left);
        Edge l = new(new Position(0, 100), new Position(49, 100), Direction.Up);
        Edge m = new(new Position(50, 50), new Position(50, 99), Direction.Left);
        Edge n = new(new Position(50, 0), new Position(50, 49), Direction.Left);

        var reflections = new List<Reflection>
        {
            new(a, j, true),
            new(b, i, true),
            new(c, f, false),
            new(d, e, true),
            new(g, h, true),
            new(k, n, false),
            new(l, m, true)
        };

        var simulator = new Simulator(map, instructionSet, new WrapStrategyPart2(map, reflections));
        var endPosition = simulator.Simulate();

        var row = endPosition.Position.Y + 1;
        var column = endPosition.Position.X + 1;

        Console.WriteLine($"Solution Part 1: {1000 * row + 4 * column + GetScoreForDirection(endPosition.Direction)}");
        var t = 0;
    }

    private static int GetScoreForDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Right => 0,
            Direction.Down => 1,
            Direction.Left => 2,
            Direction.Up => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}

internal interface IWrapStrategy
{
    (Position position, Direction direction) GetNextPosition(Position currentPosition, Direction direction);
}

internal class WrapStrategyPart1 : IWrapStrategy
{
    private readonly Map _map;

    public WrapStrategyPart1(Map map)
    {
        _map = map;
    }

    public (Position position, Direction direction) GetNextPosition(Position currentPosition, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return (GetWrapTop(currentPosition.X), direction);
            case Direction.Down:
                return (GetWrapDown(currentPosition.X), direction);
            case Direction.Left:
                return (GetWrapLeft(currentPosition.Y), direction);
            case Direction.Right:
                return (GetWrapRight(currentPosition.Y), direction);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public Position GetWrapTop(int x)
    {
        for (var y = _map.MapHeight - 1; y > 0; y--)
        {
            var probePosition = new Position(x, y);
            if (_map.GetTypeAt(probePosition) != TileType.OutsideMap) return probePosition;
        }

        throw new Exception("no Position found");
    }

    public Position GetWrapDown(int x)
    {
        for (var y = 0; y < _map.MapHeight; y++)
        {
            var probePosition = new Position(x, y);
            if (_map.GetTypeAt(probePosition) != TileType.OutsideMap) return probePosition;
        }

        throw new Exception("no Position found");
    }

    public Position GetWrapLeft(int y)
    {
        for (var x = _map.MapWidth - 1; x > 0; x--)
        {
            var probePosition = new Position(x, y);
            if (_map.GetTypeAt(probePosition) != TileType.OutsideMap) return probePosition;
        }

        throw new Exception("no Position found");
    }

    public Position GetWrapRight(int y)
    {
        for (var x = 0; x < _map.MapHeight; x++)
        {
            var probePosition = new Position(x, y);
            if (_map.GetTypeAt(probePosition) != TileType.OutsideMap) return probePosition;
        }

        throw new Exception("no Position found");
    }
}

internal class WrapStrategyPart2 : IWrapStrategy
{
    private readonly Map _map;
    private readonly List<Reflection> _reflections;

    public WrapStrategyPart2(Map map, List<Reflection> reflections)
    {
        _map = map;
        _reflections = reflections;
    }

    public (Position position, Direction direction) GetNextPosition(Position currentPosition, Direction direction)
    {
        return _reflections.Single(x => x.CanReflect(currentPosition, direction)).Reflect(currentPosition, direction);
    }
}

internal class Reflection
{
    public Reflection(Edge edgeA, Edge edgeB, bool isProportional)
    {
        EdgeA = edgeA;
        EdgeB = edgeB;
        IsProportional = isProportional;
    }

    public Edge EdgeA { get; }
    public Edge EdgeB { get; }
    public bool IsProportional { get; }

    public bool CanReflect(Position position, Direction direction)
    {
        return (EdgeA.ContainsPosition(position) && EdgeA.Direction == direction) ||
               (EdgeB.ContainsPosition(position) && EdgeB.Direction == direction);
    }

    public (Position position, Direction direction) Reflect(Position position, Direction direction)
    {
        if (EdgeA.ContainsPosition(position))
        {
            Debug.Assert(direction == EdgeA.Direction);
            var index = EdgeA.GetIndexOfPosition(position);
            return (EdgeB.GetPositionOfIndex(index, IsProportional), GetOppositeDirection(EdgeB.Direction));
        }

        if (EdgeB.ContainsPosition(position))
        {
            Debug.Assert(direction == EdgeB.Direction);
            var index = EdgeB.GetIndexOfPosition(position);
            return (EdgeA.GetPositionOfIndex(index, IsProportional), GetOppositeDirection(EdgeA.Direction));
        }

        throw new InvalidOperationException("non of the edges contains position");
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Right => Direction.Left,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            _ => throw new InvalidOperationException()
        };
    }
}

internal class Edge
{
    public Edge(Position from, Position to, Direction direction)
    {
        From = from;
        To = to;
        Direction = direction;
    }

    public Position From { get; }
    public Position To { get; }
    public Direction Direction { get; }

    public bool ContainsPosition(Position position)
    {
        if (From.Y == To.Y) return position.Y == From.Y && From.X <= position.X && position.X <= To.X;

        if (From.X == To.X) return position.X == From.X && From.Y <= position.Y && position.Y <= To.Y;

        throw new InvalidOperationException("edges must be horizontally or vertically");
    }

    public int GetIndexOfPosition(Position position)
    {
        if (From.Y == To.Y) return position.X - From.X;

        if (From.X == To.X) return position.Y - From.Y;

        throw new InvalidOperationException("edges must be horizontally or vertically");
    }

    public Position GetPositionOfIndex(int index, bool isProportional)
    {
        if (!isProportional)
        {
            var length = Math.Abs(From.X - To.X) + Math.Abs(From.Y - To.Y);
            index = length - index;
        }

        if (From.Y == To.Y) return new Position(From.X + index, From.Y);

        if (From.X == To.X) return new Position(From.X, From.Y + index);

        throw new InvalidOperationException("edges must be horizontally or vertically");
    }
}

internal class Simulator
{
    private readonly InstructionSet _instructionSet;
    private readonly Map _map;
    private readonly IWrapStrategy _wrapStrategy;

    public Simulator(Map map, InstructionSet instructionSet, IWrapStrategy wrapStrategy)
    {
        _map = map;
        _instructionSet = instructionSet;
        _wrapStrategy = wrapStrategy;
    }

    public PlayerState Simulate()
    {
        var startPosition = _map.GetStartPosition();
        var playerState = new PlayerState(startPosition, Direction.Right);

        foreach (var instruction in _instructionSet.Instructions)
        {
            if (instruction is TurnInstruction turnInstruction)
            {
                playerState = Turn(playerState, turnInstruction.Turn);
                continue;
            }

            if (instruction is MoveInstruction moveInstruction) playerState = Move(playerState, moveInstruction.Steps);
        }

        return playerState;
    }

    private PlayerState Move(PlayerState playerState, int moveInstructionSteps)
    {
        var currentPosition = (playerState.Position, playerState.Direction);

        for (var i = 0; i < moveInstructionSteps; i++)
        {
            (Position Position, Direction Direction) nextPosition =
                GetNextPosition(currentPosition.Position, currentPosition.Direction);
            if (nextPosition.Position.Y < 0)
            {
            }

            if (_map.GetTypeAt(nextPosition.Position) == TileType.Wall)
                return new PlayerState(currentPosition.Position, currentPosition.Direction);
            currentPosition = nextPosition;
        }

        return new PlayerState(currentPosition.Position, currentPosition.Direction);
    }

    private (Position position, Direction direction) GetNextPosition(Position currentPosition, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                var nextPosTop = new Position(currentPosition.X, currentPosition.Y - 1);
                if (currentPosition.Y == 0 || _map.GetTypeAt(nextPosTop) == TileType.OutsideMap)
                    return _wrapStrategy.GetNextPosition(currentPosition, direction);
                return (nextPosTop, direction);
            case Direction.Down:
                var nextPosDown = new Position(currentPosition.X, currentPosition.Y + 1);
                if (currentPosition.Y == _map.MapHeight - 1 || _map.GetTypeAt(nextPosDown) == TileType.OutsideMap)
                    return _wrapStrategy.GetNextPosition(currentPosition, direction);
                return (nextPosDown, direction);
            case Direction.Left:
                var nextPosLeft = new Position(currentPosition.X - 1, currentPosition.Y);
                if (currentPosition.X == 0 || _map.GetTypeAt(nextPosLeft) == TileType.OutsideMap)
                    return _wrapStrategy.GetNextPosition(currentPosition, direction);
                return (nextPosLeft, direction);
            case Direction.Right:
                var nextPositionRight = new Position(currentPosition.X + 1, currentPosition.Y);
                if (currentPosition.X == _map.MapWidth - 1 || _map.GetTypeAt(nextPositionRight) == TileType.OutsideMap)
                    return _wrapStrategy.GetNextPosition(currentPosition, direction);
                return (nextPositionRight, direction);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }


    private PlayerState Turn(PlayerState playerState, Turn turn)
    {
        switch (turn)
        {
            case Day_22.Turn.Left:
                return new PlayerState(playerState.Position, playerState.Direction switch
                {
                    Direction.Right => Direction.Up,
                    Direction.Down => Direction.Right,
                    Direction.Left => Direction.Down,
                    Direction.Up => Direction.Left,
                    _ => throw new ArgumentOutOfRangeException()
                });
                break;
            case Day_22.Turn.Right:
                return new PlayerState(playerState.Position, playerState.Direction switch
                {
                    Direction.Right => Direction.Down,
                    Direction.Down => Direction.Left,
                    Direction.Left => Direction.Up,
                    Direction.Up => Direction.Right,
                    _ => throw new ArgumentOutOfRangeException()
                });
            default:
                throw new ArgumentOutOfRangeException(nameof(turn), turn, null);
        }
    }
}

internal class InstructionSet
{
    public InstructionSet(string instructionLine)
    {
        InstructionLine = instructionLine;

        var instructionList = new List<IInstruction>();
        for (var i = 0; i < instructionLine.Length; i++)
        {
            if (instructionLine[i] == 'R')
            {
                instructionList.Add(new TurnInstruction(Turn.Right));
                continue;
            }

            if (instructionLine[i] == 'L')
            {
                instructionList.Add(new TurnInstruction(Turn.Left));
                continue;
            }

            if (IsNumber(instructionLine[i]))
            {
                var ii = i;
                var parsedNumber = 0;
                while (ii < instructionLine.Length && IsNumber(instructionLine[ii]))
                {
                    parsedNumber = parsedNumber * 10 + int.Parse(instructionLine[ii].ToString());
                    ii++;
                }

                i = ii - 1;
                instructionList.Add(new MoveInstruction(parsedNumber));
            }
        }

        Instructions = instructionList;
    }

    public IReadOnlyList<IInstruction> Instructions { get; }
    public string InstructionLine { get; }
}

internal class TurnInstruction : IInstruction
{
    public TurnInstruction(Turn turn)
    {
        Turn = turn;
    }

    public Turn Turn { get; }
}

internal class MoveInstruction : IInstruction
{
    public MoveInstruction(int steps)
    {
        Steps = steps;
    }

    public int Steps { get; }
}

internal interface IInstruction
{
}

internal enum Turn
{
    Left,
    Right
}

internal class Map
{
    private readonly Tile[,] _tiles;

    public Map(string[] lines)
    {
        var emptyLineIndex = Array.IndexOf(lines, "");

        MapWidth = lines.Take(new Range(0, emptyLineIndex)).Select(x => x.Length).Max();
        MapHeight = emptyLineIndex;

        _tiles = new Tile[MapWidth, MapHeight];
        for (var y = 0; y < MapHeight; y++)
        {
            var line = lines[y];
            for (var x = 0; x < MapWidth; x++)
            {
                if (line.Length <= x)
                {
                    _tiles[x, y] = new Tile(TileType.OutsideMap);
                    continue;
                }

                _tiles[x, y] = new Tile(line[x] switch
                {
                    ' ' => TileType.OutsideMap,
                    '#' => TileType.Wall,
                    '.' => TileType.Free,
                    _ => throw new InvalidOperationException()
                });
            }
        }
    }

    public int MapWidth { get; }
    public int MapHeight { get; }


    public Position GetStartPosition()
    {
        for (var x = 0; x < MapWidth; x++)
            if (_tiles[x, 0].TileType == TileType.Free)
                return new Position(x, 0);
        throw new Exception("did not found any free space in line 0");
    }

    public TileType GetTypeAt(Position position)
    {
        return _tiles[position.X, position.Y].TileType;
    }
}

internal class Position
{
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }
}

internal enum TileType
{
    OutsideMap,
    Free,
    Wall
}

internal class Tile
{
    public Tile(TileType tileType)
    {
        TileType = tileType;
    }

    public TileType TileType { get; }
}

internal enum Direction
{
    Up,
    Down,
    Left,
    Right
}

internal class PlayerState
{
    public PlayerState(Position position, Direction direction)
    {
        Position = position;
        Direction = direction;
    }

    public Position Position { get; }
    public Direction Direction { get; }
}