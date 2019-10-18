using System;
using System.Collections.Immutable;

namespace MazeProgram
{
    // Render a maze with a start,end, and highlighted path to the console
    public class MazeRenderer
    {
        public static void RenderMaze(Maze grid)
            => RenderMaze(grid, null);

        public static void RenderMaze(Maze grid, int x, int y)
            => RenderMaze(grid, ImmutableList.Create<(int x, int y)>((x, y)));
                
        // Each room in the maze is represented by two characters (South and East)
        // "_|", "__", "_ ", " |", "  "
        public static void RenderMaze(Maze grid, ImmutableList<(int x, int y)> highlightList)
        {
            var highlight = highlightList?.ToImmutableHashSet();
            Console.SetCursorPosition(0,0);
            Console.Write(" ");
            for(int x = 0; x < grid.width * 2 - 1; x++)
            {
                Console.Write("_");
            }
            Console.Write("\n");

            for(int y = 0; y < grid.height; y++)
            {
                if (y != grid.starty)
                {
                    Write("|");
                }
                else
                {
                    Write(" ", ConsoleColor.Red);
                }

                for(int x = 0; x < grid.width; x++)
                {
                    bool highlighted = highlight?.Contains((x, y)) ?? false;

                    if (grid.WallExists(y, x, Direction.South))
                    {
                        Write("_", highlighted);
                    }
                    else
                    {
                        Write(" ", highlighted);
                    }

                    if (x == grid.finishx && y == grid.finishy)
                    {
                        Write(" ", ConsoleColor.Red);
                    }
                    else if (grid.WallExists(y, x, Direction.East))
                    {
                        Write("|");
                    }
                    else if (grid.WallExists(y, x, Direction.South))
                    {
                        Write("_", highlighted);
                    }
                    else
                    {
                        Write(" ", highlighted);
                    }
                }
                Console.Write("\n");
            }
        }

        private static void Write(string txt, bool highlighted = false)
        {
            if (highlighted)
            {
                Write(txt, ConsoleColor.Green);
            }
            else
            {
                Console.Write(txt);
            }
        }

        private static void Write(string txt, ConsoleColor color)
        {
            Console.BackgroundColor = color;
            Console.Write(txt);
            Console.ResetColor();
        }
    }
}