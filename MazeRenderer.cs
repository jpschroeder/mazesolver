using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;

namespace MazeProgram
{
    public class MazeRenderer
    {
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
                    var mid = Maze.WallExists(grid[y,x], Direction.South)? "_" : " ";
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

                    if (Maze.WallExists(grid[y,x], Direction.East))
                    {
                        Console.Write("|");
                    }
                    else
                    {
                        var s = Maze.WallExists(grid[y,x], Direction.South)? "_" : " ";
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

    }
}