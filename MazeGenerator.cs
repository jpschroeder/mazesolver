using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;

namespace MazeProgram
{
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
            MazeRenderer.RenderMaze(grid, currentX, currentY);
            var dir = (Direction[])Enum.GetValues(typeof(Direction));
            var directions = dir.OrderBy(x => rand.Next());

            foreach(var direction in directions)
            {
                int nextX = currentX + Maze.OffsetX(direction);
                int nextY = currentY + Maze.OffsetY(direction);

                if (!Maze.ValidX(nextX, grid) || !Maze.ValidY(nextY, grid))
                {
                    continue;
                }

                if (grid[nextY, nextX] != 0)
                {
                    continue;
                }

                grid[currentY, currentX] |= (int)direction;
                grid[nextY, nextX] |= (int)Maze.Opposite(direction);
                CarvePassages(grid, nextX, nextY);
            }
        }
    }
}