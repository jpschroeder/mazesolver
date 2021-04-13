using System;

namespace MazeProgram
{
    // A program to generate and solve a Maze of arbitrary size
    public class Program
    {
        public static void Main(string[] args)
        {
            int dimen = 25;
            Console.Clear();
            Console.CursorVisible = false;
            //while(true)
            {
                var maze = MazeGenerator.GenerateMaze(dimen, dimen);
                var path = MazeSolver.SolveMaze(maze);
                MazeRenderer.RenderMaze(maze, path);
            }
        }
    }
    
}
