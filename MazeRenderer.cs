using System;
using System.Collections.Immutable;

namespace MazeProgram
{
    public class MazeRenderer
    {
        public static void RenderMaze(Maze grid)
        {
            RenderMaze(grid, null);
        }
        public static void RenderMaze(Maze grid, int x, int y)
        {
            RenderMaze(grid, ImmutableList.Create<Tuple<int,int>>(new Tuple<int,int>(x,y)));
        }
        public static void RenderMaze(Maze grid, ImmutableList<Tuple<int,int>> highlightList)
        {
            var highlight = highlightList == null? null : highlightList.ToImmutableHashSet();
            Console.SetCursorPosition(0,0);
            Console.Write(" ");
            for(int x = 0; x < grid.width * 2 - 1; x++)
            {
                Console.Write("_");
            }
            Console.Write("\n");

            for(int y = 0; y < grid.height; y++)
            {
                Console.Write("|");
                for(int x = 0; x < grid.width; x++)
                {
                    var mid = grid.WallExists(y, x, Direction.South)? "_" : " ";
                    bool hl = (highlight != null && highlight.Contains(new Tuple<int,int>(x,y)));
                    if (hl)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.Write(mid);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(mid);
                    }

                    if (grid.WallExists(y, x, Direction.East))
                    {
                        Console.Write("|");
                    }
                    else
                    {
                        var s = grid.WallExists(y, x, Direction.South)? "_" : " ";
                        if (hl)
                        {
                            Console.BackgroundColor = ConsoleColor.Green;
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