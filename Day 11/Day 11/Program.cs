using System.Collections;

namespace Day_11;

internal class Program
{
    private static void Main(string[] args)
    {
        var demoMonkeys = new List<Monkey>
        {
            new(0, new List<int> { 79, 98 }.Select(x => new Item(x)).ToList(), old => old * 19, x=>x.DivisibleByChecker23.IsDivisableBy(), 2, 3),
            new(1, new List<int> { 54, 65, 75, 74 }.Select(x => new Item(x)).ToList(), old => old + 6, x=> x.DivisibleByChecker19.IsDivisableBy(), 2, 0),
            new(2, new List<int> { 79, 60, 97 }.Select(x => new Item(x)).ToList(), old => old * old, x=>x.DivisibleByChecker13.IsDivisableBy(), 1, 3),
            new(3, new List<int> { 74 }.Select(x => new Item(x)).ToList(), old => old + 3, x=>x.DivisibleByChecker17.IsDivisableBy(), 0, 1)
        };

        var puzzleInputMonkeys = new List<Monkey>
        {
            new(0, new List<int> { 65, 58, 93, 57, 66 }.Select(x => new Item(x)).ToList(), old => old * 7, x=>x.DivisibleByChecker19.IsDivisableBy(), 6, 4),
            new(1, new List<int> { 76, 97, 58, 72, 57, 92, 82 }.Select(x => new Item(x)).ToList(), old => old + 4, x=>x.DivisibleByChecker3.IsDivisableBy(), 7, 5),
            new(2, new List<int> { 90, 89, 96 }.Select(x => new Item(x)).ToList(), old => old * 5, x=>x.DivisibleByChecker13.IsDivisableBy(), 5, 1),
            new(3, new List<int> { 72, 63, 72, 99 }.Select(x => new Item(x)).ToList(), old => old * old, x=>x.DivisibleByChecker17.IsDivisableBy(), 0, 4),
            new(4, new List<int> { 65 }.Select(x => new Item(x)).ToList(), old => old + 1, x=>x.DivisibleByChecker2.IsDivisableBy(), 6, 2),
            new(5, new List<int> { 97, 71 }.Select(x => new Item(x)).ToList(), old => old + 8, x=>x.DivisibleByChecker11.IsDivisableBy(), 7, 3),
            new(6, new List<int> { 83, 68, 88, 55, 87, 67 }.Select(x => new Item(x)).ToList(), old => old + 2, x=>x.DivisibleByChecker5.IsDivisableBy(), 2, 1),
            new(7, new List<int> { 64, 81, 50, 96, 82, 53, 62, 92 }.Select(x => new Item(x)).ToList(), old => old + 5, x=>x.DivisibleByChecker7.IsDivisableBy(), 3, 0),
        };

        var simulator = new Simulator(puzzleInputMonkeys);
        const int numRounds = 10_000;
        for (int i = 0; i < numRounds; i++)
        {
            simulator.SimulateRound();
        }

        var sorted = puzzleInputMonkeys.OrderByDescending(x => x.Inspections).ToList();
        var a = sorted[0].Inspections * sorted[1].Inspections;
        var t = 0;
    }
}

internal class Simulator
{
    private readonly List<Monkey> _monkeys;

    public Simulator(List<Monkey> monkeys)
    {
        _monkeys = monkeys;
    }

    public void SimulateRound()
    {
        for (var i = 0; i < _monkeys.Count; i++)
        {
            SimulateMonkey(_monkeys[i]);
        }
    }

    private void SimulateMonkey(Monkey monkey)
    {
        while (monkey.Items.TryDequeue(out var item))
        {
            monkey.Inspections++;

            //var newValue = (int)Math.Floor(monkey.Operation(item) / 3.0f);
            item.Apply(monkey.Operation);
            if (monkey.DivisibleByTest(item))
            {
                _monkeys[monkey.TrueMonkey].Items.Enqueue(item);
                //Console.WriteLine($"throw {newValue} to monkey {monkey.TrueMonkey}");
                continue;
            }

            _monkeys[monkey.FalseMonkey].Items.Enqueue(item);
            //Console.WriteLine($"throw {newValue} to monkey {monkey.FalseMonkey}");
        }
    }
}

internal class Item
{

    public DivisibleByChecker DivisibleByChecker3 { get; }
    public DivisibleByChecker DivisibleByChecker19 { get; }
    public DivisibleByChecker DivisibleByChecker13 { get; }
    public DivisibleByChecker DivisibleByChecker11 { get; }
    public DivisibleByChecker DivisibleByChecker2 { get; }
    public DivisibleByChecker DivisibleByChecker17 { get; }
    public DivisibleByChecker DivisibleByChecker7 { get; }
    public DivisibleByChecker DivisibleByChecker5 { get; }
    public DivisibleByChecker DivisibleByChecker23 { get; }

    public readonly IReadOnlyList<DivisibleByChecker> AllCheckers;

    public Item(int initialValue)
    {
        DivisibleByChecker19 = new DivisibleByChecker(initialValue, 19);
        DivisibleByChecker3 = new DivisibleByChecker(initialValue, 3);
        DivisibleByChecker13 = new DivisibleByChecker(initialValue, 13);
        DivisibleByChecker17 = new DivisibleByChecker(initialValue, 17);
        DivisibleByChecker2 = new DivisibleByChecker(initialValue, 2);
        DivisibleByChecker11 = new DivisibleByChecker(initialValue, 11);
        DivisibleByChecker5 = new DivisibleByChecker(initialValue, 5);
        DivisibleByChecker7 = new DivisibleByChecker(initialValue, 7);
        DivisibleByChecker23 = new DivisibleByChecker(initialValue, 23);

        AllCheckers = new List<DivisibleByChecker>()
        {
            DivisibleByChecker19,
            DivisibleByChecker3,
            DivisibleByChecker13,
            DivisibleByChecker17,
            DivisibleByChecker2,
            DivisibleByChecker11,
            DivisibleByChecker5,
            DivisibleByChecker7,
            DivisibleByChecker23,
        };
    }

    public void Apply(Func<int, int> operation)
    {
        foreach (var divisibleByChecker in AllCheckers)
        {
            divisibleByChecker.Apply(operation);
        }
    }
}

internal class DivisibleByChecker
{
    private int value;
    private readonly int _divisableBy;

    public DivisibleByChecker(int initialValue, int divisableBy)
    {
        value = initialValue % divisableBy;
        _divisableBy = divisableBy;
    }

    public void Apply(Func<int, int> operation)
    {
        value = operation(value) % _divisableBy;
    }

    public bool IsDivisableBy()
    {
        return value == 0;
    }
}

internal class Monkey
{
    public int Number { get; }
    public Queue<Item> Items { get; }
    public Func<int, int> Operation { get; }
    public Func<Item, bool> DivisibleByTest { get; }
    public int TrueMonkey { get; }
    public int FalseMonkey { get; }

    public int Inspections { get; set; } = 0;

    public Monkey(int number, List<Item> startingItems, Func<int, int> operation, Func<Item, bool> divisibleByTest, int trueMonkey,
        int falseMonkey)
    {
        Number = number;
        Items = new Queue<Item>(startingItems);
        Operation = operation;
        DivisibleByTest = divisibleByTest;
        TrueMonkey = trueMonkey;
        FalseMonkey = falseMonkey;
    }
}