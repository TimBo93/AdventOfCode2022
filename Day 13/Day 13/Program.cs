using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Day_13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            Part1(lines);
            Part2(lines);

        }

        static void Part1(string[] lines)
        {
            var parser = new Parser();

            var index = 1;
            var sum = 0;
            for (var i = 0; i < lines.Length; i += 3)
            {
                var firstLineParsed = parser.ParseLine(lines[i]);
                var secondLineParsed = parser.ParseLine(lines[i + 1]);

                if (new ElementComparer().CompareElements(firstLineParsed, secondLineParsed) == -1)
                {
                    Console.WriteLine(index);
                    sum += index;
                }

                index++;
            }
            Console.WriteLine($"Solution Part 1: {sum}");
        }

        static void Part2(string[] lines)
        {
            var parser = new Parser();

            List<ListElement> listElements = new(); 
            foreach (var line in lines.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                listElements.Add(parser.ParseLine(line));
            }

            var marker1 = parser.ParseLine("[[2]]");
            var marker2 = parser.ParseLine("[[6]]");

            listElements.Add(marker1);
            listElements.Add(marker2);

            listElements.Sort();

            var index1 = listElements.IndexOf(marker1) + 1;
            var index2 = listElements.IndexOf(marker2) + 1;

            Console.WriteLine($"Solution Part 2: {index1*index2}");
        }
    }

    class ElementComparer
    {
        public int CompareElements(Element firstElement, Element secondElement)
        {
            if (firstElement is IntegerElement firstIntegerElement &&
                secondElement is IntegerElement secondIntegerElement)
            {
                return Math.Sign(firstIntegerElement.Value - secondIntegerElement.Value);
            }

            if (firstElement is ListElement firstListElement && secondElement is ListElement secondListElement)
            {
                var length1 = firstListElement.Elements.Count;
                var length2 = secondListElement.Elements.Count;
                var maxLength = Math.Min(length1, length2);
                for (var i = 0; i < maxLength; i++)
                {
                    var comparison = CompareElements(firstListElement.Elements[i], secondListElement.Elements[i]);
                    if (comparison == 0)
                    {
                        continue;
                    }
                    return comparison;
                }
                return Math.Sign(length1 - length2);
            }

            var firstListElementCreated = EnsureIsListElement(firstElement);
            var secondListElementCreated = EnsureIsListElement(secondElement);
            return CompareElements(firstListElementCreated, secondListElementCreated);
        }

        ListElement EnsureIsListElement(Element element)
        {
            if (element is ListElement le)
            {
                return le;
            }
            return ListElement.FromIntegerElement((element as IntegerElement)!);
        }
    }

    class Parser
    {
        public ListElement ParseLine(string line)
        {
            var charArray = line.ToCharArray();
            var lineElement = ListElement.Parse(charArray, 0);
            return lineElement.listElement;
        }
    }

    abstract class Element : IComparable<Element>
    {
        public int CompareTo(Element? other)
        {
            return new ElementComparer().CompareElements(this, other!);
        }
    }

    class IntegerElement : Element
    {
        public int Value { get; set; }

        public static (IntegerElement listElement, int endPositionInclusive) Parse(char[] line, int startPosition)
        {
            var value = 0;
            var index = startPosition;
            while (true)
            {
                if (char.IsNumber(line[index]))
                {
                    value = value * 10 + line[index] - '0';
                    index++;
                    continue;
                }
                return (new IntegerElement() { Value = value }, index - 1);
            }
        }
    }

    class ListElement : Element
    {
        public List<Element> Elements { get; init;  } = new();

        public static (ListElement listElement, int endPositionInclusive) Parse(char[] line, int startPosition)
        {
            var listElement = new ListElement();
            for (var i = startPosition+1; i < line.Length; i++)
            {
                if (line[i] == ']')
                {
                    return (listElement, i);
                }

                if (line[i] == ',')
                {
                    continue;
                }

                if (char.IsNumber(line[i]))
                {
                    var (integerElement, endPositionInclusive) = IntegerElement.Parse(line, i);
                    listElement.Elements.Add(integerElement);
                    i = endPositionInclusive;
                }

                if (line[i] == '[')
                {
                    var (nestedListElement, endPositionInclusive) = ListElement.Parse(line, i);
                    listElement.Elements.Add(nestedListElement);
                    i = endPositionInclusive;
                }
            }

            throw new FormatException("invalid format exception");
        }

        public static ListElement FromIntegerElement(IntegerElement integerElement)
        {
            return new ListElement()
            {
                Elements = new () {integerElement}
            };
        }
    }
}