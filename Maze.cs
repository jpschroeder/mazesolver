using System;

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
        private int[,] grid;

        public int height { get {
            return this.grid.GetUpperBound(0) + 1;
        } }
        public int width { get {
            return this.grid.GetUpperBound(1) + 1;
        } }

        public Maze(int height, int width)
        {
            this.grid = new int[height,width];
        }

        public bool NoDoors(int y, int x)
        {
            return this.grid[y,x] == 0;
        }
        public bool HasDoors(int y, int x)
        {
            return this.grid[y,x] != 0;
        }

        public void CreateDoor(int y, int x, Direction direction)
        {
            this.grid[y, x] |= (int)direction;
        }

        public bool ValidX(int val)
        {
            return val >= 0 && val <= this.grid.GetUpperBound(1);
        }
        public bool ValidY(int val)
        {
            return val >= 0 && val <= this.grid.GetUpperBound(0);
        }

        public bool WallExists(int y, int x, Direction direction)
        {
            return WallExists(this.grid[y,x], direction);
        }
        public bool DoorExists(int y, int x, Direction direction)
        {
            return DoorExists(this.grid[y,x], direction);
        }

        public static bool WallExists(int gridelem, Direction direction)
        {
            return !DoorExists(gridelem, direction);
        }
        public static bool DoorExists(int gridelem, Direction direction)
        {
            return ((gridelem & (int)direction) != 0);
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