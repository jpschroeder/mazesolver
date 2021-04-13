using System;
using System.Text;
using System.Collections.Immutable;

namespace MazeProgram
{
    // Render a maze with a start,end, and highlighted path to the console
    public class MazeRenderer
    {
        private const ConsoleColor _pathColor = ConsoleColor.DarkCyan;
        private const ConsoleColor _startColor = ConsoleColor.DarkRed;

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
            var console = new BufferredConsole();
            console.Write(" ");
            for(int x = 0; x < grid.width * 2 - 1; x++)
            {
                console.Write("_");
            }
            console.Write("\n");

            for(int y = 0; y < grid.height; y++)
            {
                if (y != grid.starty)
                {
                    console.Write("|");
                }
                else
                {
                    console.Write(" ", _startColor);
                }

                for(int x = 0; x < grid.width; x++)
                {
                    bool highlighted = highlight?.Contains((x, y)) ?? false;
                    var color = highlighted? (ConsoleColor?)_pathColor : null;

                    if (grid.WallExists(y, x, Direction.South))
                    {
                        console.Write("_", color);
                    }
                    else
                    {
                        console.Write(" ", color);
                    }

                    if (x == grid.finishx && y == grid.finishy)
                    {
                        console.Write(" ", _startColor);
                    }
                    else if (grid.WallExists(y, x, Direction.East))
                    {
                        console.Write("|");
                    }
                    else if (grid.WallExists(y, x, Direction.South))
                    {
                        console.Write("_", color);
                    }
                    else
                    {
                        console.Write(" ", color);
                    }
                }
                console.Write("\n");
            }
            console.Flush();
        }

        private class BufferredConsole
        {
            private ConsoleColor? _color;
            private StringBuilder _buffer = new StringBuilder();

            public void Flush()
            {
                if (_color.HasValue)
                {
                    Console.BackgroundColor = _color.Value;
                    Console.Write(_buffer);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(_buffer);
                }
                _buffer.Clear();
            }

            public void Write(string str, ConsoleColor? color = null)
            {
                if (color != _color)
                {
                    Flush();
                    _color = color;
                    _buffer.Clear();
                }
                _buffer.Append(str);
            }
        }
    }
}