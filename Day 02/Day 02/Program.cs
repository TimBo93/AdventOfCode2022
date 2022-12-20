// See https://aka.ms/new-console-template for more information

var lines = await File.ReadAllLinesAsync("input.txt");

var part1 = lines.Select(x =>
{
    var split = x.Split(" ");
    return StrategyGuideEntry.FromMove(split[0]!, split[1]!);
}).Select(x => x.GetPoints()).Sum();
Console.Write("Part 1: ");
Console.WriteLine(part1);

var part2 = lines.Select(x =>
{
    var split = x.Split(" ");
    return StrategyGuideEntry.FromResult(split[0]!, split[1]!);
}).Select(x => x.GetPoints()).Sum();
Console.Write("Part 2: ");
Console.WriteLine(part2);

internal class StrategyGuideEntry
{
    private readonly Move _myMove;
    private readonly Move _opponentsMove;

    private StrategyGuideEntry(Move myMove, Move opponentsMove)
    {
        _myMove = myMove;
        _opponentsMove = opponentsMove;
    }

    public static StrategyGuideEntry FromMove(string moveA, string moveB)
    {
        return new StrategyGuideEntry(MoveParser.Parse(moveB), MoveParser.Parse(moveA));
    }

    public static StrategyGuideEntry FromResult(string moveA, string result)
    {
        var move = MoveParser.Parse(moveA);
        return result switch
        {
            "X" =>
                // need to lose
                new StrategyGuideEntry(GetMoveWhichLosesAgainst(move), move),
            "Y" =>
                // draw
                new StrategyGuideEntry(move, move),
            "Z" =>
                // need to win
                new StrategyGuideEntry(GetMoveWhichWinsAgainst(move), move),
            _ => throw new ArgumentOutOfRangeException(nameof(result))
        };
    }

    public int GetPoints()
    {
        return WinPoints() + SelectionPoints();
    }

    private int WinPoints()
    {
        return (MyMove: _myMove, OpponentsMove: _opponentsMove) switch
        {
            { MyMove: var x, OpponentsMove: var y } when x == y => 3,
            { MyMove: var x, OpponentsMove: var y } when GetMoveWhichWinsAgainst(y) == x => 6,
            _ => 0
        };
    }
    

    private static Move GetMoveWhichWinsAgainst(Move move)
    {
        return move switch
        {
            Move.Rock => Move.Paper,
            Move.Paper => Move.Scissor,
            Move.Scissor => Move.Rock,
            _ => throw new ArgumentOutOfRangeException(nameof(move), move, null)
        };
    }

    private static Move GetMoveWhichLosesAgainst(Move move)
    {
        return new[] { Move.Rock, Move.Paper, Move.Scissor }.First(x =>
            x != move && x != GetMoveWhichWinsAgainst(move));
    }

    private int SelectionPoints()
    {
        return _myMove switch
        {
            Move.Rock => 1,
            Move.Paper => 2,
            Move.Scissor => 3,
            _ => throw new InvalidOperationException()
        };
    }
}

internal static class MoveParser
{
    public static Move Parse(string move)
    {
        return move switch
        {
            "A" or "X" => Move.Rock,
            "B" or "Y" => Move.Paper,
            "C" or "Z" => Move.Scissor,
            _ => throw new ArgumentOutOfRangeException(nameof(move))
        };
    }
}

internal enum Move
{
    Rock,
    Paper,
    Scissor
}