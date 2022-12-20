public class Day3
{
    public static void Main()
    {
        var sum = File.ReadAllLines("input.txt").Select(x =>
            {
                var length = x.Length;
                return (x.Substring(0, length / 2), x.Substring(length / 2, length - length / 2));
            })
            .Select(x => x.Item1.ToCharArray().First(c => x.Item2.Contains(c)))
            .Select(x =>
            {
                if (char.IsAsciiLetterLower(x)) return x - 'a' + 1;

                return x - 'A' + 27;
            })
            .Sum();
        Console.WriteLine(sum);

        var sum2 = File
            .ReadAllLines("input.txt")
            .SlidingWindow(3)
            .Select(x => x[0].ToCharArray().First(c => x[1].Contains(c) && x[2].Contains(c)))
            .Select(x =>
            {
                if (char.IsAsciiLetterLower(x)) return x - 'a' + 1;

                return x - 'A' + 27;
            })
            .Sum();
        Console.WriteLine(sum2);
    }
}

public static class SlidingWindowExtension
{
    public static IEnumerable<List<T>> SlidingWindow<T>(this IEnumerable<T> collection, int slidingWindowSize)
    {
        var list = new List<T>();
        int index = 0;
        foreach (var x in collection)
        {
            if (index == 0)
            {
                list = new List<T>();
            }
            list.Add(x);
            index++;
            
            if (index != slidingWindowSize) continue;
            index = 0;
            yield return list;
        }
    }
}
