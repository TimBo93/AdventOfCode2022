using System.Text.RegularExpressions;

namespace Day_19;

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var ruleSet = lines.Select(x => Regex.Matches(x, @"\d+")).Select(x => x.Select(y => int.Parse(y.Value)).ToList())
            .Select(x => new RuleSet(x[1], x[2], x[3], x[4], x[5], x[6], 32)).ToList();

        var sum = 0;
        for (var i = 0; i < ruleSet.Count; i++)
        {
            var solver = new Solver();
            var bestSolution = solver.Solve(ruleSet[i]);
            Console.WriteLine($"Found best solution for {i+1} = {bestSolution}");
            sum += (i+1) * bestSolution;
        }

        Console.WriteLine($"Solution Part 1: {sum}");
    }
}

internal class Solver
{
    public int Solve(RuleSet rules)
    {
        var initState = GameState.Init();
        var assumptions = new Assumptions(rules);
        return FindBestSolution(initState, rules, assumptions);
    }

    public int FindBestSolution(GameState gameState, RuleSet rules, Assumptions assumptions)
    {
        if (gameState.Minute > rules.NumMoves)
        {

        }

        if (gameState.Minute == rules.NumMoves)
        {
            return gameState.GeodeAccount;
        }

        var possibleTransactions = new List<GameState>();

        // Option 1 Buy Ore
        var afterBoughtOre = gameState.BuyOreRobot(rules);
        if (afterBoughtOre.Minute <= rules.NumMoves && gameState.NumOreRobots < assumptions.MaxNumOreRobots)
        {
            
            possibleTransactions.Add(afterBoughtOre);
        }

        // Option 2 Buy Clay
        var afterBoughtClay = gameState.BuyClayRobot(rules);
        if (afterBoughtClay.Minute <= rules.NumMoves && gameState.NumClayRobots < assumptions.MaxNumClayRobots)
        {
            possibleTransactions.Add(afterBoughtClay);
        }

        // Option 3 Buy Obsidion
        if (gameState.CanBuyObsidianRobot && gameState.NumObsidianRobots < assumptions.MaxNumObsidianRobots)
        {
            var afterBoughtObsidion = gameState.BuyObsidianRobot(rules);
            if (afterBoughtObsidion.Minute <= rules.NumMoves)
            {
                possibleTransactions.Add(afterBoughtObsidion);
            }
        }

        // Option 4 Buy Geode
        if (gameState.CanBuyGeodeRobot)
        {
            var afterBoughtGeode = gameState.BuyGeodeRobot(rules);
            if (afterBoughtGeode.Minute <= rules.NumMoves)
            {
                possibleTransactions.Add(afterBoughtGeode);
            }
        }

        if (possibleTransactions.Count == 0)
        {
            possibleTransactions.Add(gameState.WaitUntilEnd(rules));
        }

        // default -> WaitUntilEnd
        return possibleTransactions.Max(x => FindBestSolution(x, rules, assumptions));
    }
}

internal class Assumptions
{
    public Assumptions(RuleSet rules)
    {
        MaxNumOreRobots = new[] { rules.OrePerClayRobot, rules.OrePerGeodeRobot, rules.OrePerObsidianRobot, rules.OrePerOreRobot }.Max();
        MaxNumClayRobots = new[] { rules.ClayPerObsidianRobot }.Max();
        MaxNumObsidianRobots = new[] { rules.ObsidianPerGeodeRobot }.Max();
    }

    public int MaxNumOreRobots { get; }
    public int MaxNumClayRobots { get; }
    public int MaxNumObsidianRobots { get; }
}

internal class GameState
{
    public GameState(int numOreRobots, int numClayRobots, int numObsidianRobots, int numGeodeRobots, int oreAccount,
        int clayAccount, int obsidianAccount, int geodeAccount, int minute)
    {
        NumOreRobots = numOreRobots;
        NumClayRobots = numClayRobots;
        NumObsidianRobots = numObsidianRobots;
        NumGeodeRobots = numGeodeRobots;

        OreAccount = oreAccount;
        ClayAccount = clayAccount;
        ObsidianAccount = obsidianAccount;
        GeodeAccount = geodeAccount;

        Minute = minute;
    }

    public int NumOreRobots { get; }
    public int NumClayRobots { get; }
    public int NumObsidianRobots { get; }
    public int NumGeodeRobots { get; }

    public int OreAccount { get; }
    public int ClayAccount { get; }
    public int ObsidianAccount { get; }
    public int GeodeAccount { get; }

    public int Minute { get; }

    public bool CanBuyObsidianRobot => NumClayRobots > 0;
    public bool CanBuyGeodeRobot => NumObsidianRobots > 0;

    public static GameState Init()
    {
        return new GameState(1, 0, 0, 0, 0, 0, 0, 0, 0);
    }

    public GameState BuyOreRobot(RuleSet ruleSet)
    {
        if (OreAccount >= ruleSet.OrePerOreRobot)
        {
            var wait = this.Wait(1);
            return new GameState(wait.NumOreRobots + 1, wait.NumClayRobots, wait.NumObsidianRobots, wait.NumGeodeRobots,
                wait.OreAccount - ruleSet.OrePerOreRobot,
                wait.ClayAccount, wait.ObsidianAccount, wait.GeodeAccount, wait.Minute);
        }

        var numRoundsToWait = (int)Math.Ceiling((ruleSet.OrePerOreRobot - OreAccount) / (float)NumOreRobots);
        var afterWaiting = Wait(numRoundsToWait).Wait(1);
        return new GameState(afterWaiting.NumOreRobots + 1, afterWaiting.NumClayRobots, afterWaiting.NumObsidianRobots, afterWaiting.NumGeodeRobots, afterWaiting.OreAccount - ruleSet.OrePerOreRobot,
            afterWaiting.ClayAccount, afterWaiting.ObsidianAccount, afterWaiting.GeodeAccount, afterWaiting.Minute);
    }

    public GameState BuyClayRobot(RuleSet ruleSet)
    {
        if (OreAccount >= ruleSet.OrePerClayRobot)
        {
            // we can buy imidiately -> no time to increase
            var wait = this.Wait(1);
            return new GameState(wait.NumOreRobots, wait.NumClayRobots + 1, wait.NumObsidianRobots, wait.NumGeodeRobots,
                wait.OreAccount - ruleSet.OrePerClayRobot,
                wait.ClayAccount, wait.ObsidianAccount, wait.GeodeAccount, wait.Minute);
        }

        var numRoundsToWait = (int)Math.Ceiling((ruleSet.OrePerClayRobot - OreAccount) / (float)NumOreRobots);
        var afterWaiting = Wait(numRoundsToWait).Wait(1);
        return new GameState(afterWaiting.NumOreRobots, afterWaiting.NumClayRobots + 1, afterWaiting.NumObsidianRobots, afterWaiting.NumGeodeRobots, afterWaiting.OreAccount - ruleSet.OrePerClayRobot,
            afterWaiting.ClayAccount, afterWaiting.ObsidianAccount, afterWaiting.GeodeAccount, afterWaiting.Minute);
    }

    public GameState BuyObsidianRobot(RuleSet ruleSet)
    {
        if (OreAccount >= ruleSet.OrePerObsidianRobot && ClayAccount >= ruleSet.ClayPerObsidianRobot)
        {
            // we can buy imidiately -> no time to increase
            var wait = this.Wait(1);
            return new GameState(wait.NumOreRobots, wait.NumClayRobots, wait.NumObsidianRobots + 1, wait.NumGeodeRobots,
                wait.OreAccount - ruleSet.OrePerObsidianRobot,
                wait.ClayAccount - ruleSet.ClayPerObsidianRobot, wait.ObsidianAccount, wait.GeodeAccount, wait.Minute);
        }

        var numRoundsToWaitForOre = (int)Math.Ceiling((ruleSet.OrePerObsidianRobot - OreAccount) / (float)NumOreRobots);
        var numRoundsToWaitForClay = (int)Math.Ceiling((ruleSet.ClayPerObsidianRobot - ClayAccount) / (float)NumClayRobots);

        var waitTime = Math.Max(numRoundsToWaitForOre, numRoundsToWaitForClay);
        var afterWaiting = Wait(waitTime).Wait(1);

        return new GameState(afterWaiting.NumOreRobots, afterWaiting.NumClayRobots, afterWaiting.NumObsidianRobots + 1, afterWaiting.NumGeodeRobots, afterWaiting.OreAccount - ruleSet.OrePerObsidianRobot,
            afterWaiting.ClayAccount - ruleSet.ClayPerObsidianRobot, afterWaiting.ObsidianAccount, afterWaiting.GeodeAccount, afterWaiting.Minute);
    }

    public GameState BuyGeodeRobot(RuleSet ruleSet)
    {
        if (OreAccount >= ruleSet.OrePerGeodeRobot && ObsidianAccount >= ruleSet.ObsidianPerGeodeRobot)
        {
            // we can buy imidiately -> no time to increase
            var wait = this.Wait(1);
            return new GameState(wait.NumOreRobots, wait.NumClayRobots, wait.NumObsidianRobots, wait.NumGeodeRobots + 1,
                wait.OreAccount - ruleSet.OrePerGeodeRobot,
                wait.ClayAccount, wait.ObsidianAccount - ruleSet.ObsidianPerGeodeRobot, wait.GeodeAccount, wait.Minute);
        }

        var numRoundsToWaitForOre = (int)Math.Ceiling((ruleSet.OrePerObsidianRobot - OreAccount) / (float)NumOreRobots);
        var numRoundsToWaitForObsidion = (int)Math.Ceiling((ruleSet.ObsidianPerGeodeRobot - ObsidianAccount) / (float)NumObsidianRobots);

        var waitTime = Math.Max(numRoundsToWaitForOre, numRoundsToWaitForObsidion);
        var afterWaiting = Wait(waitTime).Wait(1);

        return new GameState(afterWaiting.NumOreRobots, afterWaiting.NumClayRobots, afterWaiting.NumObsidianRobots, afterWaiting.NumGeodeRobots + 1, afterWaiting.OreAccount - ruleSet.OrePerGeodeRobot,
            afterWaiting.ClayAccount, afterWaiting.ObsidianAccount - ruleSet.ObsidianPerGeodeRobot, afterWaiting.GeodeAccount, afterWaiting.Minute);
    }

    public GameState WaitUntilEnd(RuleSet rules)
    {
        var timeToWait = rules.NumMoves - Minute;
        return Wait(timeToWait);
    }

    private GameState Wait(int numRoundsToWait)
    {
        return new GameState(NumOreRobots, NumClayRobots, NumObsidianRobots, NumGeodeRobots,
            OreAccount + numRoundsToWait * NumOreRobots, 
            ClayAccount + numRoundsToWait * NumClayRobots,
            ObsidianAccount + numRoundsToWait * NumObsidianRobots, 
            GeodeAccount + +numRoundsToWait * NumGeodeRobots,
            Minute + numRoundsToWait);
    }
}

internal class RuleSet
{
    public RuleSet(int orePerOreRobot, int orePerClayRobot, int orePerObsidianRobot, int clayPerObsidianRobot,
        int orePerGeodeRobot, int obsidianPerGeodeRobot, int numMoves)
    {
        OrePerOreRobot = orePerOreRobot;
        OrePerClayRobot = orePerClayRobot;
        OrePerObsidianRobot = orePerObsidianRobot;
        ClayPerObsidianRobot = clayPerObsidianRobot;
        OrePerGeodeRobot = orePerGeodeRobot;
        ObsidianPerGeodeRobot = obsidianPerGeodeRobot;
        NumMoves = numMoves;
    }

    public int OrePerOreRobot { get; }
    public int OrePerClayRobot { get; }
    public int OrePerObsidianRobot { get; }
    public int ClayPerObsidianRobot { get; }
    public int OrePerGeodeRobot { get; }
    public int ObsidianPerGeodeRobot { get; }
    public int NumMoves { get; }
}