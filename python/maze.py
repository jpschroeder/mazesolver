from dataclasses import dataclass
from random import randrange, shuffle
from os import get_terminal_size
from math import floor
from time import sleep
from argparse import ArgumentParser

# MAZE

@dataclass(frozen = True)
class Point:
    x: int
    y: int
    def add(self, other):
        return Point(self.x + other.x, self.y + other.y)

@dataclass(frozen = True)
class Direction:
    bitval: int
    offset: Point

NORTH = Direction(1, Point(0, -1))
SOUTH = Direction(2, Point(0, 1))
EAST = Direction(4, Point(1, 0))
WEST = Direction(8, Point(-1, 0))

DIRECTIONS = [NORTH, SOUTH, EAST, WEST]

OPPOSITE = {
    NORTH: SOUTH,
    SOUTH: NORTH,
    EAST: WEST,
    WEST: EAST
}

class Maze:
    def __init__(self, max, start, end):
        self.max = max
        self.start = start
        self.end = end
        self.grid = [ [0] * max.x for i in range(max.y) ]
    
    def has_doors(self, point):
        return self.grid[point.y][point.x] != 0

    def create_door(self, point, direction):
        self.grid[point.y][point.x] = self.grid[point.y][point.x] | direction.bitval
    
    def is_valid(self, point):
        return point.x >= 0 and point.x < self.max.x and point.y >= 0 and point.y < self.max.y
    
    def door_exists(self, point, direction):
        return (self.grid[point.y][point.x] & direction.bitval) != 0

    def wall_exists(self, point, direction):
        return (self.grid[point.y][point.x] & direction.bitval) == 0

# GENERATOR

def generate_maze(max, delay=0):
    start = Point(0, randrange(max.y))
    finish = Point(max.x-1, randrange(max.y))
    
    maze = Maze(max, start, finish)
    print(render_maze(maze))

    # carve passages starting at a random point
    path = [Point(randrange(max.x), randrange(max.y))]

    # pick a random direction to go
    # carve passages until you can't go anymore
    # backtrack on your path and go another random direction

    while len(path) > 0:
        current = path[-1]
        (dir, next) = next_available_passage(maze, current)
        if dir is None:
            path.pop()
            continue
        if delay > 0:
            sleep(delay)
        maze.create_door(current, dir)
        maze.create_door(next, OPPOSITE[dir])
        print(render_point(maze, current))
        print(render_point(maze, next))
        path.append(next)

    return maze

def next_available_passage(maze, current):
    shuffle(DIRECTIONS)
    for dir in DIRECTIONS:
        next = current.add(dir.offset)
        if not maze.is_valid(next):
            continue
        if maze.has_doors(next):
            continue
        return (dir, next)
    return (None, None)

# SOLVER

def solve_maze(maze, delay=0):
    path = [maze.start]
    visited = [maze.start]

    while len(path) > 0:
        current = path[-1]
        print(set_color(PATHCOLOR, render_point(maze, current)))

        if current == maze.end:
            return path

        (dir, next) = next_passage(maze, current, visited)
        if dir is None:
            path.pop()
            print(set_color(VISITEDCOLOR, render_point(maze, current)))
        else:
            if delay > 0:
                sleep(delay)
            visited.append(next)
            path.append(next)
    
    return None

def next_passage(maze, current, visited):
    shuffle(DIRECTIONS)
    for dir in DIRECTIONS:
        if not maze.door_exists(current, dir):
            continue
        next = current.add(dir.offset)
        if not maze.is_valid(next):
            continue
        if next in visited:
            continue
        return (dir, next)
    return (None, None)

# RENDERING

PATHCOLOR = '\033[44m' # blue
VISITEDCOLOR = '\033[48;5;238m' # gray
STARTCOLOR = '\033[41m' # red
ENDCOLOR = '\033[0m'
CLEAR_SCREEN = '\033[2J'

def set_cursor(y, x = 0):
    return f'\033[{y};{x}H'

def set_color(color, str):
    return color + str + ENDCOLOR

def render_maze(maze):
    strvalue = []
    strvalue.append(set_cursor(1, 1))
    strvalue.append(" " + "_" * maze.max.x * 2 + "\n")
    for y in range(maze.max.y):
        if y == maze.start.y:
            strvalue.append(set_color(STARTCOLOR, " "))
        else:
            strvalue.append("|")
        
        for x in range(maze.max.x):
            point = Point(x, y)
            strvalue.extend(cell_value(maze, point))
        strvalue.append("\n")
    return ''.join(strvalue)

def render_point(maze, point):
    strvalue = []
    strvalue.append(set_cursor(point.y + 2, point.x * 2 + 2))
    strvalue.extend(cell_value(maze, point))
    return ''.join(strvalue)

def cell_value(maze, point):
    strvalue = []

    if maze.wall_exists(point, SOUTH):
        strvalue.append("_")
    else:
        strvalue.append(" ")

    if point == maze.end:
        strvalue.append(set_color(STARTCOLOR, " "))
    elif maze.wall_exists(point, EAST):
        strvalue.append("|")
    elif maze.wall_exists(point, SOUTH):
        strvalue.append("_")
    else:
        strvalue.append(" ")

    return strvalue
    
# MAIN

def maze():
    size = get_terminal_size()
    default_width = floor(size.columns/2)-1
    default_height = size.lines-3

    parser = ArgumentParser()
    parser.add_argument("--width", default=default_width, type=int, help="set the width of the maze")
    parser.add_argument("--height", default=default_height, type=int, help="set the height of the maze")
    parser.add_argument("--delay", default=0, type=int, help="add a delay (in ms) to the processing")
    args = parser.parse_args()

    print(CLEAR_SCREEN, end='')
    delay = args.delay / 1000
    dimens = Point(args.width, args.height)
    maze = generate_maze(dimens, delay)
    path = solve_maze(maze, delay)
    print(set_cursor(maze.max.y+1, maze.max.x+1))

if __name__ == "__main__":
    maze()