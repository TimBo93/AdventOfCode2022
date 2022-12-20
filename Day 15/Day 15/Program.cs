using System.Text.RegularExpressions;

namespace Day_15;

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("input.txt");
        var pattern = @"\d+";

        var sensorBeacons = new List<SensorBeacon>();

        foreach (var line in lines)
        {
            var numbers = Regex.Matches(line, pattern, RegexOptions.IgnoreCase).Select(x => int.Parse(x.Value))
                .ToList();
            sensorBeacons.Add(new SensorBeacon((numbers[0], numbers[1]), (numbers[2], numbers[3])));
        }

        var yToCalculatePart1 = 2000000;
        var items = ExtractExclusions(yToCalculatePart1, sensorBeacons);
        Console.WriteLine($"Sum Part 1: {items.Select(x => x.max - x.min + 1 - NumSensorsAndBeaconsInRange(x, sensorBeacons, yToCalculatePart1)).Sum()}");



        for (var yToCalculate = 0; yToCalculate < 4_000_000; yToCalculate++)
        {
            items = ExtractExclusions(yToCalculate, sensorBeacons);
            if (items.Any(x => x.max < 4_000_000))
            {
                Console.WriteLine($"Frequency Part 2: {(long) (items.First(x => x.max < 4_000_000).max + 1) * 4000000 + yToCalculate}");
                return;
            }
        }
    }

    private static List<(int min, int max)> ExtractExclusions(int yToCalculate, List<SensorBeacon> sensorBeacons)
    {
        var projected = sensorBeacons
            .Select(x => x.GetExclusionAt(yToCalculate))
            .Where(x => x != null)
            .Select(x => x.Value)
            .OrderBy(x => x.minX)
            .ToList();

        var items = new List<(int min, int max)> { projected.First() };
        foreach (var projection in projected)
        {
            var lastItem = items.Last();
            if (lastItem.max >= projection.minX)
            {
                lastItem.max = Math.Max(lastItem.max, projection.maxX);
                items[^1] = lastItem;
            }
            else
            {
                items.Add(projection);
            }
        }

        return items;
    }

    private static int NumSensorsAndBeaconsInRange((int min, int max) range, List<SensorBeacon> sensorBeacons, int yToCalculate)
    {
        var numSensorsInRange = sensorBeacons.Where(x =>
                x._sensorPosition.y == yToCalculate && range.min <= x._sensorPosition.x &&
                x._sensorPosition.x <= range.max)
            .Select(x => x._sensorPosition)
            .Distinct()
            .Count();

        var numBeaconsInRange = sensorBeacons.Where(x =>
                    x._beaconPosition.y == yToCalculate && range.min <= x._beaconPosition.x &&
                    x._beaconPosition.x <= range.max)
                .Select(x => x._beaconPosition)
                .Distinct()
                .Count();

        return numSensorsInRange + numBeaconsInRange;
    }
}

internal class SensorBeacon
{
    public readonly (int x, int y) _beaconPosition;
    public readonly (int x, int y) _sensorPosition;

    private readonly int _distance;

    public SensorBeacon((int x, int y) sensorPosition, (int x, int y) beaconPosition)
    {
        _sensorPosition = sensorPosition;
        _beaconPosition = beaconPosition;
        _distance = Math.Abs(sensorPosition.x - beaconPosition.x) + Math.Abs(sensorPosition.y - beaconPosition.y);
    }

    public (int minX, int maxX)? GetExclusionAt(int y)
    {
        var yDistance = Math.Abs(_sensorPosition.y - y);

        if (yDistance > _distance) return null;

        var xDistance = _distance - yDistance;
        return (_sensorPosition.x - xDistance, _sensorPosition.x + xDistance);
    }
}