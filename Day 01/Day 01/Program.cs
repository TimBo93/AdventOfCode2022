// See https://aka.ms/new-console-template for more information

using System.Runtime.CompilerServices;

var lines = await File.ReadAllLinesAsync("input.txt");
List<int> groups = new() {0};

foreach (var line in lines)
{
    if (string.IsNullOrEmpty(line))
    {
        groups.Add(0);
        continue;
    }
    groups[groups.Count-1] = groups[groups.Count - 1] + int.Parse(line);
}

Console.Write("Solution 1: ");
Console.WriteLine(groups.MaxBy(x => x));
Console.Write("Solution 2: ");
Console.WriteLine(groups.OrderByDescending(x => x).Take(3).Sum());
