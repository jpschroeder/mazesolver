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
            => RenderMaze(grid, ImmutableList.Create<Tuple<int,int>>(new Tuple<int,int>(x,y)));
                
        // Each room in the maze is represented by two characters (South and East)
        // "_|", "__", "_ ", " |", "  "
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
                Console.Write(y != grid.starty? "|" : " ");
                for(int x = 0; x < grid.width; x++)
                {
                    bool highlighted = (highlight != null && highlight.Contains(new Tuple<int,int>(x,y)));

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
                        Write(" ", false);
                    }
                    else if (grid.WallExists(y, x, Direction.East))
                    {
                        Write("|", false);
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

        private static void Write(string txt, bool highlighted)
        {
            if (highlighted)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.Write(txt);
                Console.ResetColor();
            }
            else
            {
                Console.Write(txt);
            }
        }

    }
}