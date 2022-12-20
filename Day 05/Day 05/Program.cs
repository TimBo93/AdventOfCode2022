using System.Collections;

namespace Day_05
{
    internal class Program
    {
        private const int numStacks = 9;
        private static readonly Stack<char>[] stacks = new Stack<char>[numStacks];

        private static int part = 2;

        static void Main(string[] args)
        {
            for (int i = 0; i < Program.numStacks; i++)
            {
                stacks[i] = new Stack<char>();
            }

            var lines = File.ReadAllLines("input.txt");
            foreach (var line in lines)
            {
                if (line.Contains("["))
                {
                    for (int s = 0; s < numStacks; s++)
                    {
                        char c = line[1+4*s];
                        if (char.IsLetter(c))
                        {
                            stacks[s].Push(c);
                        }
                    }
                    continue;
                }

                if (string.IsNullOrEmpty(line))
                {
                    for (int s = 0; s < numStacks; s++)
                    {
                        stacks[s] = new (stacks[s]);
                    }
                }

                if (line.Contains("move"))
                {
                    var split = line.Split(" ");
                    var number = int.Parse(split[1]);
                    var from = int.Parse(split[3]);
                    var to = int.Parse(split[5]);

                    if (part == 1)
                    {
                        for (int i = 0; i < number; i++)
                        {
                            stacks[to - 1].Push(stacks[from - 1].Pop());
                        }
                    }

                    if (part == 2)
                    {
                        var list = new List<char>();
                        for (int i = 0; i < number; i++)
                        {
                            list.Add(stacks[from-1].Pop());
                        }

                        list.Reverse();
                        foreach (var c in list)
                        {
                            stacks[to - 1].Push(c);
                        }
                    }
                }
            }

            var result = "";
            for (int i = 0; i < Program.numStacks; i++)
            {
                result += stacks[i].Pop();
            }
            Console.WriteLine(result);
        }
    }

}