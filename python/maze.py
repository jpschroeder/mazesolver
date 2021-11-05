import os
import stat
from dataclasses import dataclass
from random import randrange, shuffle
from math import floor
from time import sleep
from argparse import ArgumentParser

# MAZE

@dataclass(frozen=True)
class Point:
    x: int
    y: int

    def add(self, other):
        return Point(self.x + other.x, self.y + other.y)

@dataclass(frozen=True)
class Direction:
    bitval: int
    offset: Point

NORTH = Direction(1, Point(0, -1))
SOUTH = Direction(2, Point(0, 1))
EAST = Direction(4, Point(1, 0))
WEST = Direction(8, Point(-1, 0))

DIRECTIONS = [NORTH, SOUTH, EAST, WEST]

OPPOSITE = {NORTH: SOUTH, SOUTH: NORTH, EAST: WEST, WEST: EAST}

class Maze:
    def __init__(self, max, offset):
        self.max = max
        self.offset = offset
        self.start = Point(0, randrange(max.y))
        self.end = Point(max.x - 1, randrange(max.y))
        self.grid = [[0] * max.x for i in range(max.y)]

    def has_doors(self, point):
        return self.grid[point.y][point.x] != 0

    def create_door(self, point, direction):
        self.grid[point.y][point.x] = self.grid[point.y][point.x] | direction.bitval

    def is_valid(self, point):
        return (
            point.x >= 0
            and point.x < self.max.x
            and point.y >= 0
            and point.y < self.max.y
        )

    def door_exists(self, point, direction):
        return (self.grid[point.y][point.x] & direction.bitval) != 0

    def wall_exists(self, point, direction):
        return (self.grid[point.y][point.x] & direction.bitval) == 0


# GENERATOR

def generate_maze(maze):
    # carve passages starting at a random point
    path = [Point(randrange(maze.max.x), randrange(maze.max.y))]

    # pick a random direction to go
    # carve passages until you can't go anymore
    # backtrack on your path and go another random direction
    while len(path) > 0:
        current = path[-1]
        (dir, next) = next_available_passage(maze, current)
        if dir is None:
            path.pop()
            continue

        maze.create_door(current, dir)
        maze.create_door(next, OPPOSITE[dir])
        path.append(next)
        animate_delay()
        print_animate(render_point(maze, current))
        print_animate(render_point(maze, next))

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

def solve_maze(maze):
    path = [maze.start]
    visited = [maze.start]

    while len(path) > 0:
        current = path[-1]
        print_animate(set_color(PATHCOLOR, render_point(maze, current)))

        if current == maze.end:
            return path

        (dir, next) = next_passage(maze, current, visited)
        if dir is None:
            path.pop()
            print_animate(set_color(VISITEDCOLOR, render_point(maze, current)))
        else:
            animate_delay()
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

PATHCOLOR = "\033[44m"  # blue
VISITEDCOLOR = "\033[48;5;238m"  # gray
STARTCOLOR = "\033[41m"  # red
ENDCOLOR = "\033[0m"
CLEAR_SCREEN = "\033[2J"

def set_cursor(y, x=0):
    return f"\033[{y};{x}H"

def set_color(color, str):
    if config.raw:
        return str

    return color + str + ENDCOLOR

XSEP = " " * 3

def join_cols(string_arr):
    return XSEP.join(string_arr) + "\n"

def render_maze_cols(mazes):
    strvalue = []

    headers = []
    for maze in mazes:
        headers.append(render_header(maze))

    strvalue.append(join_cols(headers))

    for y in range(mazes[0].max.y):
        rows = []
        for maze in mazes:
            rows.append(render_row(maze, y))

        strvalue.append(join_cols(rows))

    return "".join(strvalue)

def render_maze(maze):
    strvalue = []
    strvalue.append(render_header(maze))
    strvalue.append("\n")
    for y in range(maze.max.y):
        strvalue.append(render_row(maze, y))
        strvalue.append("\n")
    return "".join(strvalue)

def render_row(maze, y):
    strvalue = []
    if y == maze.start.y:
        strvalue.append(set_color(STARTCOLOR, " "))
    else:
        strvalue.append("|")

    for x in range(maze.max.x):
        point = Point(x, y)
        strvalue.extend(cell_value(maze, point))
    return "".join(strvalue)

def render_header(maze):
    return " " + "_" * maze.max.x * 2

def render_point(maze, point):
    strvalue = []
    strvalue.append(
        set_cursor(maze.offset.y + point.y + 2, maze.offset.x + (point.x * 2) + 2)
    )
    strvalue.extend(cell_value(maze, point))
    return "".join(strvalue)

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

def animate_delay():
    if config.delay > 0:
        sleep(config.delay)

def print_animate(str, **kwargs):
    if not config.raw:
        print(str, **kwargs)


# MAIN

@dataclass(frozen=True)
class Config:
    maze_size: Point
    frame_size: Point
    delay: int
    raw: bool

def characters_to_cells(columns, lines):
    width = floor(columns / 2) - 1
    height = lines - 3
    return Point(width, height)

def parse_size_arg(size):
    if size == "p":
        page_size = Point(37, 59)
        return page_size

    if size == "t":
        tsize = os.get_terminal_size()
        return characters_to_cells(tsize.columns, tsize.lines)

    parts = size.split(",")
    if len(parts) > 2:
        print("Invalid size")
        exit(1)

    width = int(parts[0])
    height = width
    if len(parts) == 2:
        height = int(parts[1])

    return Point(width, height)

def is_stdout_redirected():
    stdout_fd = 1
    return not stat.S_ISCHR(os.stat(stdout_fd).st_mode)

def parse_args():
    is_redirected = is_stdout_redirected()
    parser = ArgumentParser()
    parser.add_argument(
        "-s",
        "--size",
        default="p" if is_redirected else "t",
        type=str,
        help="maze size - x,y | t (terminal size) | p (page size)",
    )
    parser.add_argument(
        "-f",
        "--frame-size",
        default="m",
        type=str,
        dest="frame_size",
        help="frame size - x,y | m (maze size) | t (terminal size) | p (page size)",
    )
    parser.add_argument(
        "-d",
        "--delay",
        default=0,
        type=int,
        help="add a delay (in ms) to the processing",
    )
    parser.add_argument(
        "-r",
        "--raw",
        default=is_redirected,
        action="store_true",
        help="don't animate the generation/solution",
    )
    args = parser.parse_args()

    maze_size = parse_size_arg(args.size)

    frame_size = None
    if args.frame_size == "m":
        frame_size = maze_size
    else:
        frame_size = parse_size_arg(args.frame_size)

    if args.size == "p" or args.frame_size == "p":
        args.raw = True

    if frame_size.x < maze_size.x or frame_size.y < maze_size.y:
        print("Frame size cannot be less than maze size")
        exit()

    return Config(
        maze_size=maze_size,
        frame_size=frame_size,
        delay=args.delay / 1000,
        raw=args.raw,
    )

def maze():
    global config
    config = parse_args()

    print_animate(CLEAR_SCREEN, end="")
    print_animate(set_cursor(1, 1), end="")

    rows = int((config.frame_size.y + 2) / (config.maze_size.y + 2))
    cols = int((config.frame_size.x + 2) / (config.maze_size.x + 2))

    mazerows = []
    for row in range(rows):
        mazecols = []
        for col in range(cols):
            offset = Point(
                (config.maze_size.x * 2 + len(XSEP) + 1) * col,
                (config.maze_size.y + 2) * row,
            )
            mazecols.append(Maze(config.maze_size, offset))
        mazerows.append(mazecols)

    for mazes in mazerows:
        if not config.raw:
            print(render_maze_cols(mazes))

    for mazes in mazerows:
        for maze in mazes:
            generate_maze(maze)

    for mazes in mazerows:
        for maze in mazes:
            if not config.raw:
                solve_maze(maze)

    for mazes in mazerows:
        if config.raw:
            print(render_maze_cols(mazes))

    print_animate(set_cursor((config.maze_size.y + 2) * rows, 0))

if __name__ == "__main__":
    maze()
