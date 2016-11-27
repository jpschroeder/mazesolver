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
            //while(true)
            {
                var maze = Maze.GenerateMaze(dimen, dimen);
                var path = Maze.SolveMaze(maze);
            }
        }
    }

    public enum Direction
    {
        North = 1,
        South = 2,
        East = 4,
        West = 8
    }

    public class Maze
    {
        private static Random rand = new Random();

        public static ImmutableList<Tuple<int,int>> SolveMaze(int[,] grid)
        {
            int startx = 0;
            int starty = rand.Next(grid.GetUpperBound(0));
            var start = new Tuple<int,int>(startx, starty);

            int finishx = grid.GetUpperBound(1);
            int finishy = rand.Next(grid.GetUpperBound(0));
            var finish = new Tuple<int,int>(finishx, finishy);

            var path = ImmutableList.Create<Tuple<int,int>>(start);
            var visited = new HashSet<Tuple<int,int>>();
            visited.Add(start);
            return SolveMaze(grid, path, visited, finishx, finishy);
        }

        public static ImmutableList<Tuple<int,int>> SolveMaze(
            int[,] grid,
            ImmutableList<Tuple<int,int>> currentPath,
            HashSet<Tuple<int,int>> visited,
            int finishX, int finishY)
        {
            RenderMaze(grid, currentPath.Add(new Tuple<int,int>(finishX, finishY)));
            int currentX = currentPath.Last().Item1;
            int currentY = currentPath.Last().Item2;
            var dir = (Direction[])Enum.GetValues(typeof(Direction));
            var directions = dir.Where(d => DoorExists(grid[currentY,currentX], d)).OrderBy(x => rand.Next());

            foreach(var direction in directions)
            {
                int nextX = currentX + OffsetX(direction);
                int nextY = currentY + OffsetY(direction);
                var next = new Tuple<int,int>(nextX, nextY);

                if (!ValidX(nextX, grid) || !ValidY(nextY, grid))
                {
                    continue;
                }

                if (nextX == finishX && nextY == finishY)
                {
                    return currentPath.Add(next);
                }

                if (visited.Contains(next))
                {
                    continue;
                }
                visited.Add(next);

                var ret = SolveMaze(grid, currentPath.Add(next), visited, finishX, finishY);
                if (ret != null)
                {
                    return ret;
                }
            }
            return null;
        }
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
        public static void RenderMaze(int[,] grid)
        {
            RenderMaze(grid, null);
        }
        public static void RenderMaze(int[,] grid, int x, int y)
        {
            RenderMaze(grid, ImmutableList.Create<Tuple<int,int>>(new Tuple<int,int>(x,y)));
        }
        public static void RenderMaze(int[,] grid, ImmutableList<Tuple<int,int>> highlightList)
        {
            var highlight = highlightList == null? null : highlightList.ToImmutableHashSet();
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
                    var mid = WallExists(grid[y,x], Direction.South)? "_" : " ";
                    bool hl = (highlight != null && highlight.Contains(new Tuple<int,int>(x,y)));
                    if (hl)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.Write(mid);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(mid);
                    }

                    if (WallExists(grid[y,x], Direction.East))
                    {
                        Console.Write("|");
                    }
                    else
                    {
                        var s = WallExists(grid[y,x], Direction.South)? "_" : " ";
                        if (hl)
                        {
                            Console.BackgroundColor = ConsoleColor.Blue;
                            Console.Write(s);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.Write(s);
                        }
                    }
                }
                Console.Write("\n");
            }
        }

        public static bool WallExists(int gridelem, Direction direction)
        {
            return !DoorExists(gridelem, direction);
        }
        public static bool DoorExists(int gridelem, Direction direction)
        {
            return ((gridelem & (int)direction) != 0);
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
