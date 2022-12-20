using System.Runtime.CompilerServices;

namespace Day_13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");

            var parser = new Parser();

            var index = 1;
            var sum = 0;
            for (var i = 0; i < lines.Length; i+=3)
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

        
    }

    class ElementComparer
    {
        public int CompareElements(IElement firstElement, IElement secondElement)
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

        ListElement EnsureIsListElement(IElement element)
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

    interface IElement
    {
    }

    class IntegerElement : IElement
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

    class ListElement : IElement
    {
        public List<IElement> Elements { get; init;  } = new();

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