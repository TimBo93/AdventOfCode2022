using System.Globalization;
using System.Threading;

namespace Day_21
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");

            var compiler = new Compiler();
            var executionContext = compiler.Compile(lines);
            executionContext.Init();
            var root = executionContext.GetExpression("root");

            executionContext.RefreshAllNumberValues();
            Console.WriteLine($"Solution Part 1: {root.LastValue}");




            var executionContextPart2 = compiler.Compile(lines);
            
            var newRoot = new SubtractExpression("gqjg", "rpjv");
            executionContextPart2.AddExpression("root", newRoot);
            executionContextPart2.Init();
            executionContextPart2.RefreshAllNumberValues();

            var human = (executionContextPart2.GetExpression("humn") as NumberExpression)!;
            var currentPosition = 0d;

            while (true)
            {
                var currentHeight = GetHeight(currentPosition, human, newRoot);
                if (currentHeight == 0)
                {
                    Console.WriteLine($"Solution Part 2: {currentPosition}");
                    return;
                }

                currentPosition = Newton(currentPosition, human, newRoot, currentHeight);
                Console.WriteLine($"{currentPosition} .. {currentHeight}");
            }
        }

        private static double Newton(double i, NumberExpression human, SubtractExpression root, double currentHeight)
        {
            var derivate = CalculateDerivate(i, human, root, currentHeight);
            return Math.Floor( i - currentHeight / derivate);
        }

        private static double CalculateDerivate(double i, NumberExpression human, SubtractExpression root, double currentHeight)
        {
            var height1 = GetHeight(i + 1, human, root);
            var derivate = height1 - currentHeight;
            return derivate;
        }

        private static double GetHeight(double i, NumberExpression human, SubtractExpression root)
        {
            human.OverrideValue(i);
            return root.LastValue!.Value;
        }
    }

    internal class Compiler
    {
        public ExecutionContext Compile(string[] lines)
        {
            var context = new ExecutionContext();
            foreach (var tokens in lines.Select(x => x.Split(" ")))
            {
                IExpression expression = tokens switch
                {
                    [_, var number] when int.TryParse(number, out var parsed) => new NumberExpression(parsed),
                    [_, var var1, "+", var var2] => new AddExpression(var1, var2),
                    [_, var var1, "-", var var2] => new SubtractExpression(var1, var2),
                    [_, var var1, "*", var var2] => new MultiplyExpression(var1, var2),
                    [_, var var1, "/", var var2] => new DivideExpression(var1, var2),
                    _ => throw new InvalidOperationException("Syntax error")
                };
                context.AddExpression(tokens[0].Substring(0, tokens[0].Length -1), expression);
            }
            return context;
        }
        
    }

    internal class NumberExpression : IExpression
    {
        private readonly int _initValue;

        public NumberExpression(int initValue)
        {
            _initValue = initValue;
            LastValue = initValue;
        }

        public void PublishValue()
        {
            LastValue = _initValue;
            Publish?.Invoke(this, LastValue.Value);
        }

        public double? LastValue { get; private set; }

        public void Setup(ExecutionContext context)
        {
            // no dependencies
        }

        public event EventHandler<double>? Publish;

        public void OverrideValue(double testNumber)
        {
            LastValue = testNumber;
            Publish?.Invoke(this, LastValue.Value);
        }
    }
    internal class AddExpression : IExpression
    {
        private readonly string _varName1;
        private readonly string _varName2;

        private double? _var1;
        public double? Var1
        {
            get => _var1;
            set
            {
                if (_var1 == value)
                {
                    return;
                }
                _var1 = value;
                OnPropertyChanged();
            }
        }

        private double? _var2;
        public double? Var2
        {
            get => _var2;
            set
            {
                if (_var2 == value)
                {
                    return;
                }
                _var2 = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged()
        {
            if (Var1 != null && Var2 != null)
            {
                LastValue = Var1.Value + Var2.Value;
                Publish?.Invoke(this, LastValue.Value);
            }
        }

        public AddExpression(string varName1, string varName2)
        {
            _varName1 = varName1;
            _varName2 = varName2;
        }

        public double? LastValue { get; private set; }

        public void Setup(ExecutionContext context)
        {
            context.GetExpression(_varName1).Publish += (_, i) =>
            {
                Var1 = i;
            };
            context.GetExpression(_varName2).Publish += (_, i) =>
            {
                Var2 = i;
            };
        }

        public event EventHandler<double>? Publish;
    }
    internal class SubtractExpression : IExpression
    {
        private readonly string _varName1;
        private readonly string _varName2;

        private double? _var1;
        public double? Var1
        {
            get => _var1;
            set
            {
                if (_var1 == value)
                {
                    return;
                }
                _var1 = value;
                OnPropertyChanged();
            }
        }

        private double? _var2;
        public double? Var2
        {
            get => _var2;
            set
            {
                if (_var2 == value)
                {
                    return;
                }
                _var2 = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged()
        {
            if (Var1 != null && Var2 != null)
            {
                LastValue = Var1.Value - Var2.Value;
                Publish?.Invoke(this, LastValue.Value);
            }
        }

        public SubtractExpression(string varName1, string varName2)
        {
            _varName1 = varName1;
            _varName2 = varName2;
        }

        public double? LastValue { get; private set; }

        public void Setup(ExecutionContext context)
        {
            context.GetExpression(_varName1).Publish += (_, i) =>
            {
                Var1 = i;
            };
            context.GetExpression(_varName2).Publish += (_, i) =>
            {
                Var2 = i;
            };
        }

        public event EventHandler<double>? Publish;
    }
    internal class MultiplyExpression : IExpression
    {
        private readonly string _varName1;
        private readonly string _varName2;

        private double? _var1;
        public double? Var1
        {
            get => _var1;
            set
            {
                if (_var1 == value)
                {
                    return;
                }
                _var1 = value;
                OnPropertyChanged();
            }
        }

        private double? _var2;
        public double? Var2
        {
            get => _var2;
            set
            {
                if (_var2 == value)
                {
                    return;
                }
                _var2 = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged()
        {
            if (Var1 != null && Var2 != null)
            {
                LastValue = Var1.Value * Var2.Value;
                Publish?.Invoke(this, LastValue.Value);
            }
        }

        public MultiplyExpression(string varName1, string varName2)
        {
            _varName1 = varName1;
            _varName2 = varName2;
        }

        public double? LastValue { get; private set; }

        public void Setup(ExecutionContext context)
        {
            context.GetExpression(_varName1).Publish += (_, i) =>
            {
                Var1 = i;
            };
            context.GetExpression(_varName2).Publish += (_, i) =>
            {
                Var2 = i;
            };
        }

        public event EventHandler<double>? Publish;
    }
    internal class DivideExpression : IExpression
    {
        private readonly string _varName1;
        private readonly string _varName2;

        private double? _var1;
        public double? Var1
        {
            get => _var1;
            set
            {
                if (_var1 == value)
                {
                    return;
                }
                _var1 = value;
                OnPropertyChanged();
            }
        }

        private double? _var2;
        public double? Var2
        {
            get => _var2;
            set
            {
                if (_var2 == value)
                {
                    return;
                }
                _var2 = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged()
        {
            if (Var1 != null && Var2 != null)
            {
                LastValue = Var1.Value / Var2.Value;
                Publish?.Invoke(this, LastValue.Value);
            }
        }

        public DivideExpression(string varName1, string varName2)
        {
            _varName1 = varName1;
            _varName2 = varName2;
        }

        public double? LastValue { get; private set; }

        public void Setup(ExecutionContext context)
        {
            context.GetExpression(_varName1).Publish += (_, i) =>
            {
                Var1 = i;
            };
            context.GetExpression(_varName2).Publish += (_, i) =>
            {
                Var2 = i;
            };
        }

        public event EventHandler<double>? Publish;
    }
    

    internal class ExecutionContext
    {
        private readonly Dictionary<string, IExpression> _expressions = new();

        public IExpression GetExpression(string name)
        {
            return _expressions[name];
        }

        public void AddExpression(string name, IExpression expression)
        {
            if (_expressions.ContainsKey(name))
            {
                _expressions.Remove(name);
            }
            _expressions.Add(name, expression);
        }

        public void Init()
        {
            foreach (var expression in _expressions.Select(x => x.Value))
            {
                expression.Setup(this);
            }
        }

        public void RefreshAllNumberValues()
        {
            foreach (var expression in _expressions.Select(x => x.Value))
            {
                if (expression is NumberExpression numberExpression)
                {
                    numberExpression.PublishValue();
                }
            }
        }
    }

    internal interface IExpression
    {
        public double? LastValue { get; }

        void Setup(ExecutionContext context);

        public event EventHandler<double> Publish;
    }
}