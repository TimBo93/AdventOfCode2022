namespace Day_06
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            var firstLine = lines.First();
            var index1 = firstLine.ToCharArray().Select((_, i) => new {  index = i }).First(x => firstLine.Substring(x.index, 4).ToCharArray().Distinct().Count() == 4).index + 4;
            var index2 = firstLine.ToCharArray().Select((_, i) => new { index = i }).First(x => firstLine.Substring(x.index, 14).ToCharArray().Distinct().Count() == 14).index + 14;
            var x = 0;
        }
    }
}