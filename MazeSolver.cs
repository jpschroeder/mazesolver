using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;

namespace MazeProgram
{
    // Find a path through the maze
    public class MazeSolver
    {
        private static Random rand = new Random();

        public static ImmutableList<Tuple<int,int>> SolveMaze(Maze grid)
        {
            var path = ImmutableList.Create<Tuple<int,int>>(grid.start);
            var visited = new HashSet<Tuple<int,int>>();
            visited.Add(grid.start);
            return SolveMaze(grid, path, visited);
        }

        private static ImmutableList<Tuple<int,int>> SolveMaze(
            Maze grid,
            ImmutableList<Tuple<int,int>> currentPath,
            HashSet<Tuple<int,int>> visited)
        {
            MazeRenderer.RenderMaze(grid, currentPath);
            int currentX = currentPath.Last().Item1;
            int currentY = currentPath.Last().Item2;
            var dir = (Direction[])Enum.GetValues(typeof(Direction));
            var directions = dir.Where(d => grid.DoorExists(currentY, currentX, d)).OrderBy(x => rand.Next());

            foreach(var direction in directions)
            {
                int nextX = currentX + Maze.OffsetX(direction);
                int nextY = currentY + Maze.OffsetY(direction);
                var next = new Tuple<int,int>(nextX, nextY);

                if (!grid.ValidX(nextX) || !grid.ValidY(nextY))
                {
                    continue;
                }

                if (nextX == grid.finishx && nextY == grid.finishy)
                {
                    return currentPath.Add(next);
                }

                if (visited.Contains(next))
                {
                    continue;
                }
                visited.Add(next);

                var ret = SolveMaze(grid, currentPath.Add(next), visited);
                if (ret != null)
                {
                    return ret;
                }
            }
            return null;
        }
    }
}