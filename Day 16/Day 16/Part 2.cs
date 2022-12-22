using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using static System.String;

namespace Day_16;

internal class Program
{
    private static void Main(string[] args)
    {
        var nodes = new List<Node>();

        var lines = File.ReadAllLines("input.txt");
        var id = 0;
        foreach (var line in lines)
        {
            var split = line.Split(" ");
            var nodeName = split[1];
            var flowRate = int.Parse(Regex.Match(split.First(x => x.StartsWith("rate=")), @"\d+").Value);
            var targets = split.Where(x => x.EndsWith(",")).Select(x => x.Substring(0, x.Length - 1)).ToList();
            targets.Add(split.Last());

            nodes.Add(new Node(id, nodeName, flowRate, targets));
            ++id;
        }

        var map = new Map(nodes);
        foreach (var node in nodes) node.AccelerateRelation(map);

        var solver = new Solver();

        var numNodes = nodes.Count;

        var openedNodes = new bool[numNodes];
        for (var i = 0; i < numNodes; i++)
        {
            openedNodes[i] = nodes[i].FlowRate == 0;
        }

        var startVariant = new Variant(26, 0, openedNodes, map.GetNodeByName("AA"), map.GetNodeByName("AA"), 0, map);

        solver.Solve(startVariant);

        Console.WriteLine("finished iteration");
    }
}

internal class Solver
{
    private readonly PriorityQueue<Variant, int> _priorityQueue = new();
    private readonly NeighborsEnumerator _neighborsEnumerator = new();
    private readonly Dictionary<string, Variant> alreadyVisitedVariants = new();

    public void Solve(Variant variant)
    {
        _priorityQueue.Enqueue(variant, variant.WastedPoints);
        alreadyVisitedVariants.Add(variant.Serialize(), variant);
        Solve();
    }

    private void Solve()
    {
        int bestScore = 0;
        int minRemaining = 50;
        while (_priorityQueue.TryDequeue(out var variant, out int priority))
        {
            bestScore = Math.Max(bestScore, variant.Score);

            if (variant.RemainingTime < minRemaining)
            {
                minRemaining = variant.RemainingTime;
                Console.WriteLine(minRemaining);
            }

            if (variant.RemainingTime == 1)
            {
                Console.WriteLine($"Solution Part 2: {variant.Score}");
                return;
            }

            if (variant.AllValvesOpen())
            {
                var remaining = variant.Map.Nodes.Sum(x => x.FlowRate) * (variant.RemainingTime - 1);
                Console.WriteLine($"Solution Part 2: {variant.Score + remaining}");
                return;
            }

            foreach (var neighbor in _neighborsEnumerator.GetAllNeighbors(variant))
            {
                var serializedNeighbor = neighbor.Serialize();
                if (alreadyVisitedVariants.ContainsKey(serializedNeighbor))
                {
                    continue;
                }

                alreadyVisitedVariants.Add(serializedNeighbor, neighbor);
                _priorityQueue.Enqueue(neighbor, neighbor.WastedPoints);
            }
        }
    }
}

internal class NeighborsEnumerator
{
    public IEnumerable<Variant> GetAllNeighbors(Variant variant)
    {
        var allTransitions1 = GetAllTransitions1(variant).ToList();
        foreach (var transition1 in allTransitions1)
        {
            var variantAfterTransition1 = transition1.Apply(variant);
            var allTransitions2 = GetAllTransitions2(variantAfterTransition1).ToList();
            foreach (var transition2 in allTransitions2)
            {
                var variantAfterTransition2 = transition2.Apply(variantAfterTransition1);
                var invariant = variantAfterTransition2.UpdateRemainingTimeScoreAndWastedPoints().ToInvariant();
                yield return invariant;
            }
        }
    }

    private IEnumerable<ITransition> GetAllTransitions1(Variant variant)
    {
        if (variant is { CanOpenValve1: true, ValveAtCurrentPositionOpened1: false })
            yield return new OpenValve1();

        foreach (var neighborNode in variant.CurrentlyAtNode1.ReferencedNodes)
            yield return new MovePlayer1(neighborNode);
    }

    private IEnumerable<ITransition> GetAllTransitions2(Variant variant)
    {
        if (variant is { CanOpenValve2: true, ValveAtCurrentPositionOpened2: false })
            yield return new OpenValve2();

        foreach (var neighborNode in variant.CurrentlyAtNode2.ReferencedNodes)
            yield return new MovePlayer2(neighborNode);
    }
}

interface ITransition
{
    Variant Apply(Variant variant);
}

class OpenValve1 : ITransition
{
    public Variant Apply(Variant variant)
    {
        return variant.OpenCurrentValve1();
    }
}

class OpenValve2 : ITransition
{
    public Variant Apply(Variant variant)
    {
        return variant.OpenCurrentValve2();
    }
}

class MovePlayer1 : ITransition
{
    private readonly Node _targetNode;

    public MovePlayer1(Node targetNode)
    {
        _targetNode = targetNode;
    }

    public Variant Apply(Variant variant)
    {
        return variant.MoveToValve1(_targetNode);
    }
}

class MovePlayer2 : ITransition
{
    private readonly Node _targetNode;

    public MovePlayer2(Node targetNode)
    {
        _targetNode = targetNode;
    }

    public Variant Apply(Variant variant)
    {
        return variant.MoveToValve2(_targetNode);
    }
}

internal class Map
{
    public Map(List<Node> nodes)
    {
        Nodes = nodes;
    }

    public IReadOnlyList<Node> Nodes { get; }

    public Node GetNodeByName(string name)
    {
        return Nodes.First(x => x.NodeName == name);
    }

    public int GetWastedPoints(bool[] openedNodes)
    {
        int sum = 0;
        for (int i = 0; i < Nodes.Count; i++)
        {
            sum += !openedNodes[i] ? Nodes[i].FlowRate : 0;
        }
        return sum;
    }

    public int GetMovePoints(bool[] openedNodes)
    {
        int sum = 0;
        for (int i = 0; i < Nodes.Count; i++)
        {
            sum += openedNodes[i] ? Nodes[i].FlowRate : 0;
        }
        return sum;
    }
}

internal class Node
{
    private readonly IReadOnlyList<string> _targetNodeNames;

    public Node(int id, string nodeName, int flowRate, List<string> targetNodeNames)
    {
        Id = id;
        _targetNodeNames = targetNodeNames;
        NodeName = nodeName;
        FlowRate = flowRate;
    }

    public IReadOnlyList<Node> ReferencedNodes { get; private set; }

    public int Id { get; }
    public string NodeName { get; }
    public int FlowRate { get; }

    public void AccelerateRelation(Map map)
    {
        ReferencedNodes = _targetNodeNames.Select(map.GetNodeByName).ToList();
    }
}

internal class Variant
{
    public Variant(int remainingTime, int score, bool[] openedNodes, Node currentlyAtNode1, Node currentlyAtNode2, int wastedPoints, Map map)
    {
        RemainingTime = remainingTime;
        Score = score;
        OpenedNodes = openedNodes;
        CurrentlyAtNode1 = currentlyAtNode1;
        CurrentlyAtNode2 = currentlyAtNode2;
        WastedPoints = wastedPoints;
        Map = map;
    }

    public int RemainingTime { get; }
    public int Score { get; }
    public bool[] OpenedNodes { get; }
    public Node CurrentlyAtNode1 { get; }
    public Node CurrentlyAtNode2 { get; }
    public int WastedPoints { get; }
    public Map Map { get; }

    public bool CanOpenValve1 => CurrentlyAtNode1.FlowRate > 0;
    public bool CanOpenValve2 => CurrentlyAtNode2.FlowRate > 0;

    public bool ValveAtCurrentPositionOpened1 => OpenedNodes[CurrentlyAtNode1.Id];
    public bool ValveAtCurrentPositionOpened2 => OpenedNodes[CurrentlyAtNode2.Id];


    public Variant OpenCurrentValve1()
    {
        var openedValvesNew = (OpenedNodes.Clone() as bool[])!;
        openedValvesNew[CurrentlyAtNode1.Id] = true;
        return new Variant(RemainingTime, Score, openedValvesNew, CurrentlyAtNode1, CurrentlyAtNode2, WastedPoints, Map);
    }

    public Variant OpenCurrentValve2()
    {
        var openedValvesNew = (OpenedNodes.Clone() as bool[])!;
        openedValvesNew[CurrentlyAtNode2.Id] = true;
        return new Variant(RemainingTime, Score, openedValvesNew, CurrentlyAtNode1, CurrentlyAtNode2, WastedPoints, Map);
    }

    public Variant MoveToValve1(Node nodeToMoveTo)
    {
        return new Variant(RemainingTime, Score, OpenedNodes, nodeToMoveTo, CurrentlyAtNode2, WastedPoints, Map);
    }

    public Variant MoveToValve2(Node nodeToMoveTo)
    {
        return new Variant(RemainingTime, Score, OpenedNodes, CurrentlyAtNode1, nodeToMoveTo, WastedPoints, Map);
    }

    public string Serialize()
    {
        var stringbuilder = new StringBuilder();
        for (int i = 0; i < OpenedNodes.Length; i++)
        {
            if (OpenedNodes[i])
            {
                stringbuilder.Append("1");
            }
            else
            {
                stringbuilder.Append("0");
            }
        }
        stringbuilder.Append(CurrentlyAtNode1.NodeName);
        stringbuilder.Append(CurrentlyAtNode2.NodeName);
        return stringbuilder.ToString();
    }

    public Variant UpdateRemainingTimeScoreAndWastedPoints()
    {
        return new Variant(RemainingTime - 1, Score + Map.GetMovePoints(OpenedNodes), OpenedNodes, CurrentlyAtNode1,
            CurrentlyAtNode2, WastedPoints + Map.GetWastedPoints(OpenedNodes), Map);
    }

    public Variant ToInvariant()
    {
        // it does not matter if player 1 is elephant or human. So assume that node 1 is always the smaller one.
        if (Compare(CurrentlyAtNode1.NodeName, CurrentlyAtNode2.NodeName, StringComparison.InvariantCulture) < 0)
        {
            return this;
        }
        return new Variant(RemainingTime, Score, OpenedNodes, CurrentlyAtNode2, CurrentlyAtNode1, WastedPoints, Map);
    }

    public bool AllValvesOpen()
    {
        return OpenedNodes.All(x => x);
    }
}