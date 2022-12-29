using System.Collections.Immutable;

namespace Day_17;

internal class Program
{
    private static void Main(string[] args)
    {
        var line = File.ReadAllLines("input.txt")[0];
        var directions = line.ToCharArray().Select(x => x switch
        {
            '<' => Direction.Left,
            '>' => Direction.Right,
            _ => throw new()
        }).ToImmutableList();

        var shape1 = new[,]
        {
            { true, true, true, true }
        };

        var shape2 = new[,]
        {
            { false, true, false },
            { true, true, true },
            { false, true, false}
        };

        var shape3 = new[,]
        {
            { false, false, true },
            { false, false, true },
            { true, true, true}
        };

        var shape4 = new[,]
        {
            { true },
            { true },
            { true },
            { true }
        };

        var shape5 = new[,]
        {
            { true, true },
            { true, true }
        };

        var shapeList = new List<bool[,]> { shape1, shape2, shape3, shape4, shape5 }.ToImmutableList();
        var map = new Map(7);

        var commandIndex = 0;
        var shapeIndex = 0;
        var currentShape = shapeList[shapeIndex];
        var shapePosX = 2;
        var shapePosY = 4;

        var numShapesSpawned = 1;

        while (true)
        {
            // Apply Gas
            var command = directions[commandIndex];
            commandIndex = (commandIndex + 1) % directions.Count;

            switch (command)
            {
                case Direction.Left when map.CanMoveTo(currentShape, shapePosX - 1, shapePosY):
                    Console.WriteLine($"Move left to {shapePosX}");
                    shapePosX -= 1;
                    break;
                case Direction.Right when map.CanMoveTo(currentShape, shapePosX + 1, shapePosY):
                    Console.WriteLine($"Move right to {shapePosX}");
                    shapePosX += 1;
                    break;
                case Direction.Left:
                    Console.Write("cannot move left");
                    break;
                case Direction.Right:
                    Console.Write("cannot move right");
                    break;
                default:
                    break;
            }

            if (map.CheckCollision(currentShape, shapePosX, shapePosY - 1))
            {
                Console.WriteLine("Collision ! Materialize:");
                map.Materialize(currentShape, shapePosX, shapePosY);

                //Render(map);

                shapeIndex = (shapeIndex + 1) % shapeList.Count;
                currentShape = shapeList[shapeIndex];
                shapePosX = 2;
                shapePosY = map.MaxHeight() + 4;

                if (numShapesSpawned == 2022)
                {
                    // 3195 too low
                    Console.WriteLine($"Solution Part 1 {map.MaxHeight()}");
                    return;
                }

                numShapesSpawned += 1;
            }
            else
            {
                shapePosY -= 1;
                Console.WriteLine($"fall to height {shapePosY}");
            }
        }
    }

    private static void Render(Map map)
    {
        Console.WriteLine("====================");
        for (int y = map.MaxHeight() - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.Width; x++)
            {
                if (map._heightMap[x][y])
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(".");
                }
            }

            Console.WriteLine();
        }
    }
}


class Map
{
    public readonly List<bool>[] _heightMap;

    public Map(int width)
    {
        Width = width;
        _heightMap = new List<bool>[Width];
        for (int i = 0; i < Width; i++)
        {
            _heightMap[i] = new();
        }
    }
    
    public int Width { get; }

    public void Materialize(bool[,] shape, int x, int y)
    {
        var shapeHeight = shape.GetLength(0);
        var shapeWidth = shape.GetLength(1);
        var currentMapMaxHeight = MaxHeight();
        var missingRowCount = (y + shapeHeight - 1) - currentMapMaxHeight;

        ExtendMap(missingRowCount);

        for (int sx = 0; sx < shapeWidth; sx++)
        {
            for (int sy = 0; sy < shapeHeight; sy++)
            {
                if (shape[shapeHeight - sy - 1, sx])
                {
                    _heightMap[sx + x][y + sy - 1] = true;
                }
            }
        }
    }

    private void ExtendMap(int count)
    {
        if (count <= 0)
        {
            return;
        }

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < count; j++)
            {
                _heightMap[i].Add(false);
            }
        }
    }

    public bool CheckCollision(bool[,] shape, int x, int y)
    {
        if (y == 0)
        {
            return true;
        }

        var shapeHeight = shape.GetLength(0);
        var shapeWidth = shape.GetLength(1);

       for (int sy = 0; sy < shapeHeight; sy++)
        {
            if (y + sy > MaxHeight())
            {
                return false;
            }

            for (int sx = 0; sx < shapeWidth; sx++)
            {
                if (_heightMap [sx + x][y + sy - 1] && shape[(shapeHeight - sy) - 1, sx])
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int MaxHeight()
    {
        return this._heightMap[0].Count;
    }

    public bool CanMoveTo(bool[,] shape, int x, int y)
    {
        var shapeWidth = shape.GetLength(1);
        if (x < 0 || x + shapeWidth > _heightMap.Length)
        {
            return false;
        }

        var collision = CheckCollision(shape, x, y);

        if (collision)
        {
            Console.WriteLine("Can not move, since collision");
            return false;
        }

        return true;

        //if (y > _heightMap.Length)
        //{
        //    return true;
        //}

        //for (int i = 0; i < shape.Slices.Count; i++)
        //{
        //    var currentHeight = _heightMap[i + x];
        //    if (currentHeight >= y + shape.Slices[i].ProbeOffset)
        //    {
        //        return false;
        //    }
        //}

        //return true;
    }
}

internal enum Direction
{
    Left,
    Right
}
