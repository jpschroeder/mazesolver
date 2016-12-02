
using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeProgram
{
    public enum Direction
    {
        North = 1,
        South = 2,
        East = 4,
        West = 8
    }

    public class Maze
    {
        public static bool WallExists(int gridelem, Direction direction)
        {
            return !DoorExists(gridelem, direction);
        }
        public static bool DoorExists(int gridelem, Direction direction)
        {
            return ((gridelem & (int)direction) != 0);
        }

        public static bool ValidX(int val, int[,] grid)
        {
            return val >= 0 && val <= grid.GetUpperBound(1);
        }

        public static bool ValidY(int val, int[,] grid)
        {
            return val >= 0 && val <= grid.GetUpperBound(0);
        }

        public static Direction Opposite(Direction direction)
        {
            switch(direction)
            {
                case Direction.North: return Direction.South;
                case Direction.South: return Direction.North;
                case Direction.East: return Direction.West;
                case Direction.West: return Direction.East;
                default: throw new Exception($"Invalid direction: {direction}");
            }
        }

        public static int OffsetX(Direction direction)
        {
            switch(direction)
            {
                case Direction.North: return 0;
                case Direction.South: return 0;
                case Direction.East: return 1;
                case Direction.West: return -1;
                default: throw new Exception($"Invalid direction: {direction}");
            }
        }
        public static int OffsetY(Direction direction)
        {
            switch(direction)
            {
                case Direction.North: return -1;
                case Direction.South: return 1;
                case Direction.East: return 0;
                case Direction.West: return 0;
                default: throw new Exception($"Invalid direction: {direction}");
            }
        }
    }

}