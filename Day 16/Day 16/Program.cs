using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;

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
        var startVariant = new Variant(30, 0, new bool[numNodes], map.GetNodeByName("AA"), 0, map);

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
        this._priorityQueue.Enqueue(variant, variant.WastedPoints);
        this.alreadyVisitedVariants.Add(variant.Serialize(), variant);
        this.Solve();
    }

    private void Solve()
    {
        int minRemaining = 50;
        while (_priorityQueue.TryDequeue(out var variant, out int priority))
        {
            if (variant.RemainingTime < minRemaining)
            {
                minRemaining = variant.RemainingTime;
                Console.WriteLine(minRemaining);
            }

            if (variant.RemainingTime == 0)
            {
                // Solution here
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
        if (variant is { CanOpenValve: true, ValveAtCurrentPositionOpened: false })
            yield return variant.OpenCurrentValve();

        foreach (var neighborNode in variant.CurrentlyAtNode.ReferencedNodes)
            yield return variant.MoveToValve(neighborNode);
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
    public Variant(int remainingTime, int score, bool[] openedNodes, Node currentlyAtNode, int wastedPoints, Map map)
    {
        RemainingTime = remainingTime;
        Score = score;
        OpenedNodes = openedNodes;
        CurrentlyAtNode = currentlyAtNode;
        WastedPoints = wastedPoints;
        Map = map;  
    }

    public int RemainingTime { get; }
    public int Score { get; }
    public bool[] OpenedNodes { get; }
    public Node CurrentlyAtNode { get; }
    public int WastedPoints { get; }
    public Map Map { get; }

    public bool CanOpenValve => CurrentlyAtNode.FlowRate > 0;
    public bool ValveAtCurrentPositionOpened => OpenedNodes[CurrentlyAtNode.Id];


    public Variant OpenCurrentValve()
    {
        var openedValvesNew = (OpenedNodes.Clone() as bool[])!;
        openedValvesNew[CurrentlyAtNode.Id] = true;
        return new Variant(RemainingTime - 1, Score + (RemainingTime - 1) * CurrentlyAtNode.FlowRate,
            openedValvesNew, CurrentlyAtNode, WastedPoints + Map.GetWastedPoints(openedValvesNew), Map);
    }

    public Variant MoveToValve(Node nodeToMoveTo)
    {
        return new Variant(RemainingTime - 1, Score, (OpenedNodes.Clone() as bool[])!, nodeToMoveTo, WastedPoints + Map.GetWastedPoints(OpenedNodes), Map);
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
        stringbuilder.Append(this.CurrentlyAtNode.NodeName);
        return stringbuilder.ToString();
    }
}