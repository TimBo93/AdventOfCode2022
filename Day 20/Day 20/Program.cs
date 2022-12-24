using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Day_20
{
    // 8556
    // 8105
    // 4439
    // 21100 too high

    internal class Program
    {
        static void Main(string[] args)
        {
            var numbers = File.ReadLines("input.txt").Select(long.Parse).Select(x => x * 811589153).ToArray();
            
            Dictionary<long, LinkedListNode<long>> nodeLookup = new();
            LinkedList<long> linkedList = new();


            LinkedListNode<long> val0 = null;

            var i = 0;
            foreach (var number in numbers)
            {
                var node = new LinkedListNode<long>(number);
                nodeLookup.Add(i, node);
                linkedList.AddLast(node);
                i++;

                if (number == 0)
                {
                    val0 = node;
                }
            }

            Console.WriteLine(string.Join(", ", linkedList));
            //Console.WriteLine(string.Join(", ", linkedList));

            for (var z = 0; z < 10; z++)
            {
                for (var ii = 0; ii < linkedList.Count; ii++)
                {
                    Mix(ii, linkedList, nodeLookup);
                    //Console.WriteLine(string.Join(", ", linkedList));
                }
            }

            Console.WriteLine(string.Join(", ", linkedList));
            Console.WriteLine("finish");



            var iterator = val0;
            var sum = 0l;
            for (long z = 0; z < 3; z++)
            {
                for (long t = 0; t < 1000; t++)
                {
                    if (iterator == linkedList.Last)
                    {
                        iterator = linkedList.First;
                        continue;
                    }
                    iterator = iterator.Next;
                }

                sum += iterator.Value;
                Console.WriteLine(iterator.Value);
            }
            Console.WriteLine(sum);

        }

        private static void Mix(long index, LinkedList<long> linkedList, Dictionary<long, LinkedListNode<long>> nodes)
        {
            var originalNode = nodes[index];

            var numberToMix = originalNode.Value;
            if (numberToMix > 0)
            {
                var iteratorNode = GetNextNode(linkedList, originalNode);
                linkedList.Remove(originalNode);
                var targetNode = GetNextNodeSlow(linkedList, numberToMix - 1, iteratorNode);
                linkedList.AddAfter(targetNode, originalNode);
            }
            
            if (numberToMix < 0)
            {
                var iteratorNode = GetPrevNode(linkedList, originalNode);
                //Console.WriteLine($"prev node is {iteratorNode.Value}");
                linkedList.Remove(originalNode);
                var targetNode = GetPrevNodeSlow(linkedList, Math.Abs(numberToMix) - 1, iteratorNode);
                linkedList.AddBefore(targetNode, originalNode);
            }
        }

        private static LinkedListNode<long> GetNextNodeSlow(LinkedList<long> linkedList, long numberToMix, LinkedListNode<long> iteratorNode)
        {
            numberToMix %= linkedList.Count;
            for (long i = 0; i < numberToMix; i++)
            {
                iteratorNode = GetNextNode(linkedList, iteratorNode);
            }
            return iteratorNode;
        }

        private static LinkedListNode<long> GetPrevNodeSlow(LinkedList<long> linkedList, long numberToMix, LinkedListNode<long> iteratorNode)
        {
            numberToMix %= linkedList.Count;
            for (long i = 0; i < numberToMix; i++)
            {
                iteratorNode = GetPrevNode(linkedList, iteratorNode);
                //Console.WriteLine($"iterator node is now {iteratorNode.Value}");
            }
            return iteratorNode;
        }
        
        private static LinkedListNode<long> GetNextNode(LinkedList<long> linkedList, LinkedListNode<long> nodeIterator)
        {
            if (nodeIterator == linkedList.Last)
            {
                return linkedList.First;
            }
            return nodeIterator.Next;
        }

        private static LinkedListNode<long> GetPrevNode(LinkedList<long> linkedList, LinkedListNode<long> nodeIterator)
        {
            if (nodeIterator == linkedList.First)
            {
                return linkedList.Last;
            }
            return nodeIterator.Previous;
        }
    }

    internal static class IndexOfExtension {

        public static long IndexOf<T>(this LinkedList<T> linkedList, LinkedListNode<T> node)
        {
            var index = 0;
            var iterator = linkedList.First;

            while (iterator != node)
            {
                index++;
                iterator = iterator.Next;
            }

            return index;
        }

        public static LinkedListNode<T> NodeAt<T>(this LinkedList<T> linkedList, long index)
        {
            var iterator = linkedList.First;

            for (long i = 0; i < index; i++)
            {
                iterator = iterator.Next;
            }

            return iterator;
        }

    }
}