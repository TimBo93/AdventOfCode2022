namespace Day_09;

internal class Program
{
    private static void Main(string[] args)
    {
        var simulator = new Simulator(10);
        var lines = File.ReadAllLines("input.txt");

        foreach (var line in lines.Select(x => x.Split(" ")))
            if (line is ["R", var stepRight] && int.TryParse(stepRight, out var parsedStepRight))
                simulator.Move(1, 0, parsedStepRight);
            else if (line is ["L", var stepLeft] && int.TryParse(stepLeft, out var parsedStepLeft))
                simulator.Move(-1, 0, parsedStepLeft);
            else if (line is ["U", var stepUp] && int.TryParse(stepUp, out var parsedStepUp))
                simulator.Move(0, 1, parsedStepUp);
            else if (line is ["D", var stepDown] && int.TryParse(stepDown, out var parsedStepDown))
                simulator.Move(0, -1, parsedStepDown);

        Console.WriteLine(simulator.visitedTailPositions.Count);
    }
}

internal class ChainLink
{
    public (int x, int y) Position { get; set; } = (0, 0);
};

internal class Simulator
{
    private readonly List<ChainLink> _chainLinks;
    public HashSet<(int x, int y)> visitedTailPositions = new() { (0, 0) };

    public Simulator(int numLinks)
    {
        this._chainLinks = new List<ChainLink>();
        for (var i = 0; i < numLinks; i++)
        {
            _chainLinks.Add(new ChainLink());
        }
    }

    public void Move(int x, int y, int steps)
    {
        for (var i = 0; i < steps; i++)
        {
            var head = _chainLinks.First();
            head.Position = (head.Position.x + x, head.Position.y + y);

            for (var tailLink = 1; tailLink < _chainLinks.Count; tailLink++)
            {
                UpdateTailPositions(_chainLinks[tailLink - 1], _chainLinks[tailLink]);
            }
            visitedTailPositions.Add(_chainLinks.Last().Position);
        }
    }

    private void UpdateTailPositions(ChainLink head, ChainLink tail)
    {
        var headPosition = head.Position;
        var tailPosition = tail.Position;

        if (headPosition.y == tailPosition.y && Math.Abs(headPosition.x - tailPosition.x) > 1)
        {
            tailPosition.x += headPosition.x > tailPosition.x ? 1 : -1;
        }
        else if (headPosition.x == tailPosition.x && Math.Abs(headPosition.y - tailPosition.y) > 1)
        {
            tailPosition.y += headPosition.y > tailPosition.y ? 1 : -1;
        }
        else if (Math.Abs(headPosition.x - tailPosition.x) > 1 || Math.Abs(headPosition.y - tailPosition.y) > 1)
        {
            tailPosition.x += headPosition.x > tailPosition.x ? 1 : -1;
            tailPosition.y += headPosition.y > tailPosition.y ? 1 : -1;
        }

        tail.Position = tailPosition;
    }
}