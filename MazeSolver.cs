using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;

namespace MazeProgram
{
    public class MazeSolver
    {

        private static Random rand = new Random();

        public static ImmutableList<Tuple<int,int>> SolveMaze(Maze grid)
        {
            int startx = 0;
            int starty = rand.Next(grid.height -1);
            var start = new Tuple<int,int>(startx, starty);

            int finishx = grid.width -1;
            int finishy = rand.Next(grid.height -1);
            var finish = new Tuple<int,int>(finishx, finishy);

            var path = ImmutableList.Create<Tuple<int,int>>(start);
            var visited = new HashSet<Tuple<int,int>>();
            visited.Add(start);
            return SolveMaze(grid, path, visited, finishx, finishy);
        }

        public static ImmutableList<Tuple<int,int>> SolveMaze(
            Maze grid,
            ImmutableList<Tuple<int,int>> currentPath,
            HashSet<Tuple<int,int>> visited,
            int finishX, int finishY)
        {
            MazeRenderer.RenderMaze(grid, currentPath.Add(new Tuple<int,int>(finishX, finishY)));
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

                if (nextX == finishX && nextY == finishY)
                {
                    return currentPath.Add(next);
                }

                if (visited.Contains(next))
                {
                    continue;
                }
                visited.Add(next);

                var ret = SolveMaze(grid, currentPath.Add(next), visited, finishX, finishY);
                if (ret != null)
                {
                    return ret;
                }
            }
            return null;
        }
    }
}