﻿using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeProgram
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int dimen = 20;
            Console.Clear();
            Console.CursorVisible = false;
            //while(true)
            {
                var maze = MazeGenerator.GenerateMaze(dimen, dimen);
                var path = MazeSolver.SolveMaze(maze);
            }
        }
    }
    
}