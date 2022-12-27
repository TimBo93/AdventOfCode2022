using System.Net.Sockets;
using System.Text;

namespace Day_25;

internal class Program
{
    private static void Main(string[] args)
    {
        var converter = new SNAUFConverter();
        var sumAsSnauf =  converter.Convert(File.ReadAllLines("input.txt").Select(x => converter.Convert(x)).Sum());
        Console.WriteLine(sumAsSnauf);
    }
}

internal class SNAUFConverter
{
    public long Convert(string snaufNumber)
    {
        var result = 0L;
        for (var i = 0; i < snaufNumber.Length; i++)
        {
            var digit = snaufNumber[snaufNumber.Length - i - 1];
            var exponent = (long)Math.Pow(5, i);

            result += digit switch
            {
                '=' => -2 * exponent,
                '-' => -1 * exponent,
                '0' => 0,
                '1' => 1 * exponent,
                '2' => 2 * exponent,
                _ => throw new InvalidOperationException()
            };
        }

        return result;
    }

    public string Convert(long number)
    {
        var currentProbe = "";

        while (true)
        {
            var snaufStartingWith1 = "1" + currentProbe;
            if (GetLowerLimit(snaufStartingWith1) <= number &&
                number <= GetUpperLimit(snaufStartingWith1))
            {
                return Resolve(snaufStartingWith1, number);
            }

            var snaufStartingWith2 = "2" + currentProbe;
            if (GetLowerLimit(snaufStartingWith2) <= number &&
                number <= GetUpperLimit(snaufStartingWith2))
            {
                return Resolve(snaufStartingWith2, number);
            }

            currentProbe += "?";
        }

        return "?";
    }

    private string Resolve(string unresolvedSnaufNumber, long number)
    {
        while (unresolvedSnaufNumber.Contains("?"))
        {
            var indexOfFirstUnresolved = unresolvedSnaufNumber.IndexOf("?");

            StringBuilder builder = new StringBuilder(unresolvedSnaufNumber);
            builder[indexOfFirstUnresolved] = '=';
            if (GetLowerLimit(builder.ToString()) <= number &&
                number <= GetUpperLimit(builder.ToString()))
            {
                unresolvedSnaufNumber = builder.ToString();
                continue;
            }

            builder[indexOfFirstUnresolved] = '-';
            if (GetLowerLimit(builder.ToString()) <= number &&
                number <= GetUpperLimit(builder.ToString()))
            {
                unresolvedSnaufNumber = builder.ToString();
                continue;
            }

            builder[indexOfFirstUnresolved] = '0';
            if (GetLowerLimit(builder.ToString()) <= number &&
                number <= GetUpperLimit(builder.ToString()))
            {
                unresolvedSnaufNumber = builder.ToString();
                continue;
            }

            builder[indexOfFirstUnresolved] = '1';
            if (GetLowerLimit(builder.ToString()) <= number &&
                number <= GetUpperLimit(builder.ToString()))
            {
                unresolvedSnaufNumber = builder.ToString();
                continue;
            }

            builder[indexOfFirstUnresolved] = '2';
            if (GetLowerLimit(builder.ToString()) <= number &&
                number <= GetUpperLimit(builder.ToString()))
            {
                unresolvedSnaufNumber = builder.ToString();
                continue;
            }
        }

        return unresolvedSnaufNumber;
    }

    private long GetLowerLimit(string unresolvedSnaufNumber)
    {
        return Convert(unresolvedSnaufNumber.Replace("?", "="));
    }

    private long GetUpperLimit(string unresolvedSnaufNumber)
    {
        return Convert(unresolvedSnaufNumber.Replace("?", "2"));
    }
}