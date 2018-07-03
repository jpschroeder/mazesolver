using System;
using System.Linq;

namespace MazeProgram
{
    // Generate a random solvable maze 
    public class MazeGenerator
    {
        private static Random rand = new Random();

        public static Maze GenerateMaze(int height, int width)
        {
            int startx = 0;
            int starty = rand.Next(height - 1);
            int finishx = width - 1;
            int finishy = rand.Next(height - 1);

            var grid = new Maze(height, width, starty, startx, finishy, finishx);
            CarvePassages(grid, rand.Next(width), rand.Next(height));
            return grid;
        }

        private static void CarvePassages(Maze grid, int currentX, int currentY)
        {
            MazeRenderer.RenderMaze(grid, currentX, currentY);
            var dir = (Direction[])Enum.GetValues(typeof(Direction));
            var directions = dir.OrderBy(x => rand.Next());

            foreach(var direction in directions)
            {
                int nextX = currentX + Maze.OffsetX(direction);
                int nextY = currentY + Maze.OffsetY(direction);

                if (!grid.ValidX(nextX) || !grid.ValidY(nextY))
                {
                    continue;
                }

                if (grid.HasDoors(nextY, nextX))
                {
                    continue;
                }

                grid.CreateDoor(currentY, currentX, direction);
                grid.CreateDoor(nextY, nextX, Maze.Opposite(direction));
                CarvePassages(grid, nextX, nextY);
            }
        }
    }
}