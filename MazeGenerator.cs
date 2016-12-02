using System;
using System.Linq;

namespace MazeProgram
{
    public class MazeGenerator
    {
        private static Random rand = new Random();
        public static Maze GenerateMaze(int height, int width)
        {
            var grid = new Maze(height, width);
            CarvePassages(grid, rand.Next(width), rand.Next(height));
            return grid;
        }
        public static void CarvePassages(Maze grid, int currentX, int currentY)
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