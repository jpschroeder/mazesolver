using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int dimen = 20;
            Console.Clear();
            Console.CursorVisible = false;
            var maze = MazeGenerator.GenerateMaze(dimen, dimen);
            MazeGenerator.RenderMaze(maze);
        }
    }

    public enum Direction
    {
        North = 1,
        South = 2,
        East = 4,
        West = 8
    }

    public class MazeGenerator
    {
        private static Random rand = new Random();
        public static int[,] GenerateMaze(int height, int width)
        {
            var grid = new int[height,width];
            CarvePassages(grid, rand.Next(width), rand.Next(height));
            return grid;
        }

        public static void CarvePassages(int[,] grid, int currentX, int currentY)
        {
            RenderMaze(grid, currentX, currentY);
            //System.Threading.Thread.Sleep(50);
            var dir = (Direction[])Enum.GetValues(typeof(Direction));
            var directions = dir.OrderBy(x => rand.Next());

            foreach(var direction in directions)
            {
                int nextX = currentX + OffsetX(direction);
                int nextY = currentY + OffsetY(direction);

                if (!ValidX(nextX, grid) || !ValidY(nextY, grid))
                {
                    continue;
                }

                if (grid[nextY, nextX] != 0)
                {
                    continue;
                }

                grid[currentY, currentX] |= (int)direction;
                grid[nextY, nextX] |= (int)Opposite(direction);
                CarvePassages(grid, nextX, nextY);
            }
        }

        public static void RenderMaze(int[,] grid, int currentX = -1, int currentY = -1)
        {
            int height = grid.GetUpperBound(0) + 1;
            int width = grid.GetUpperBound(1) + 1;
            Console.SetCursorPosition(0,0);
            Console.Write(" ");
            for(int x = 0; x < width * 2 - 1; x++)
            {
                Console.Write("_");
            }
            Console.Write("\n");

            for(int y = 0; y < height; y++)
            {
                Console.Write("|");
                for(int x = 0; x < width; x++)
                {
                    var mid = DirectionExists(grid[y,x], Direction.South)? "_" : " ";
                    if (currentX != -1 && currentY != -1 && x == currentX && y == currentY)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write(mid);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(mid);
                    }

                    if (DirectionExists(grid[y,x], Direction.East))
                    {
                        Console.Write("|");
                    }
                    else
                    {
                        Console.Write(DirectionExists(grid[y,x], Direction.South)? "_" : " ");
                    }
                }
                Console.Write("\n");
            }
        }

        public static bool DirectionExists(int gridelem, Direction direction)
        {
            return !((gridelem & (int)direction) != 0);
        }

        public static bool ValidX(int val, int[,] grid)
        {
            return val >= 0 && val <= grid.GetUpperBound(1);
        }

        public static bool ValidY(int val, int[,] grid)
        {
            return val >= 0 && val <= grid.GetUpperBound(0);
        }

        public static Direction Opposite(Direction direction)
        {
            switch(direction)
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.East: return Direction.West;
                case Direction.West: return Direction.East;
                default: throw new Exception($"Invalid direction: {direction}");
            }
        }

        public static int OffsetX(Direction direction)
        {
            switch(direction)
            {
                case Direction.North: return 0;
                case Direction.South: return 0;
                case Direction.East: return 1;
                case Direction.West: return -1;
                default: throw new Exception($"Invalid direction: {direction}");
            }
        }
        public static int OffsetY(Direction direction)
        {
            switch(direction)
            {
                case Direction.North: return -1;
                case Direction.South: return 1;
                case Direction.East: return 0;
                case Direction.West: return 0;
                default: throw new Exception($"Invalid direction: {direction}");
            }
        }
    }


    
}
