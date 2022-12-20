using System.Security.Cryptography.X509Certificates;

namespace Day_09
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            var executionContext = new ExecutionContext()
            {
                X = 1,
                Checkpoints = new [] { 20, 60, 100, 140, 180, 220 }
            };

            foreach (var line in lines)
            {
                var tokens = line.Split(' ');
                ICommand command = tokens switch
                {
                    ["addx", var value] when int.TryParse(value, out int parsed) => new AddCommand(executionContext,
                        parsed),
                    ["noop"] => new NoopCommand(executionContext),
                    _ => throw new InvalidOperationException()
                };
                command.Execute();
            }

            Console.WriteLine(executionContext.Sum);
        }
    }

    class ExecutionContext
    {
        public int Cycle { get; private set; } = 1;
        public int X { get; set; }
        public int[] Checkpoints { get; set; }

        public int Sum { get; private set; }

        public void IncreaseCycle()
        {
            Render();
            Cycle++;
            if (Checkpoints.Contains(Cycle))
            {
                Sum += Cycle * X;
            }
        }

        private void Render()
        {
            var position = (Cycle) % 40;
            if (position == 0)
            {
                Console.WriteLine();
            }

            if (Math.Abs(position - X) <= 1)
            {
                Console.Write("#");
                return;
            }
            Console.Write(" ");
        }
    }

    interface ICommand
    {
        void Execute();
    }

    class NoopCommand : ICommand
    {
        private readonly ExecutionContext _executionContext;

        public NoopCommand(ExecutionContext executionContext)
        {
            _executionContext = executionContext;
        }

        public void Execute()
        {
            this._executionContext.IncreaseCycle();
        }
    }

    class AddCommand : ICommand
    {
        private readonly ExecutionContext _executionContext;
        private readonly int _valToAdd;

        public AddCommand(ExecutionContext executionContext, int valToAdd)
        {
            _executionContext = executionContext;
            _valToAdd = valToAdd;
        }

        public void Execute()
        {
            this._executionContext.IncreaseCycle();
            this._executionContext.X += _valToAdd;
            this._executionContext.IncreaseCycle();
        }
    }
}