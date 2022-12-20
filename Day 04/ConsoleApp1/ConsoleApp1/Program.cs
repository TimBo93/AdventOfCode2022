var count1 = File
    .ReadAllLines("input.txt")
    .Select(x =>
    {
        var split = x.Split(",");
        var firstSplit = split[0].Split("-");
        var secondSplit = split[1].Split("-");

        return (int.Parse(firstSplit[0]), int.Parse(firstSplit[1]), int.Parse(secondSplit[0]),
            int.Parse(secondSplit[1]));
    })
    .Count(x => (x.Item1 <= x.Item3 && x.Item4 <= x.Item2) || (x.Item3 <= x.Item1 && x.Item2 <= x.Item4));
Console.WriteLine(count1);


var count2 = File
    .ReadAllLines("input.txt")
    .Select(x =>
    {
        var split = x.Split(",");
        var firstSplit = split[0].Split("-");
        var secondSplit = split[1].Split("-");

        return (int.Parse(firstSplit[0]), int.Parse(firstSplit[1]), int.Parse(secondSplit[0]),
            int.Parse(secondSplit[1]));
    })
    .Count(x => Helper.InRange(x.Item1, x.Item2, x.Item3) 
                || Helper.InRange(x.Item1, x.Item2, x.Item4)
                || Helper.InRange(x.Item3, x.Item4, x.Item1)
                || Helper.InRange(x.Item3, x.Item4, x.Item2)
    );


Console.WriteLine(count2);

internal static class Helper
{
    public static bool InRange(int from, int to, int val)
    {
        return from <= val && val <= to;
    }
}


