using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int dimen = 20;
            var board = MazeRunner.GenerateBoard(dimen, dimen);
            var str = MazeRunner.RenderBoard(board);
            Console.WriteLine(str);
        }
    }

    public class MazeRunner
    {
        public static Board GenerateBoard(int xsize, int ysize)
        {
            var rand = new Random();
            var board = new Board();
            board.rooms = GenerateRooms(xsize, ysize);

            int randy = rand.Next(ysize);
            board.start = new Address(0, randy);
            randy = rand.Next(ysize);
            board.finish = new Address(ysize-1, randy);

            var visited = new List<Address>();
            var seen = new Stack<Address>();
            seen.Push(board.start);
            while(seen.Any())
            {
                var currentAddr = seen.Pop();
                var currentRoom = board.GetRoom(currentAddr);
                var str = RenderBoard(board, currentAddr);
                Console.Clear();
                Console.WriteLine(str);
                System.Threading.Thread.Sleep(200);
                if (visited.Contains(currentAddr))
                {
                    continue;
                }

                if (visited.Count() > 0)
                {
                    var previousAddr = visited.Last();
                    var previousRoom = board.GetRoom(previousAddr);
                    currentRoom.adjacent.Clear();
                    currentRoom.adjacent.Add(previousAddr);
                    previousRoom.adjacent.Add(currentAddr);
                }

                visited.Add(currentAddr);

                var toadd = currentRoom.adjacent.Where(x => !visited.Contains(x) && !seen.Contains(x)).ToList();
                if (toadd.Count() > 0)
                {
                    toadd.Shuffle();
                    toadd.ForEach(x => seen.Push(x));
                }
                else
                {
                    seen.Shuffle();
                }
            }

            return board;
        }

        public static string RenderBoard(Board board, Address current = null)
        {
            var sb = new StringBuilder();
            for(var y = 0; y <= board.rooms.GetUpperBound(1); y++)
            {
                if (y == 0)
                {
                    for(var x = 0; x <= board.rooms.GetUpperBound(0); x++)
                    {
                        if (x == 0)
                        {
                            sb.Append("+");
                        }
                        sb.Append(board.rooms[x, y].topwall()? "-" : " ");
                        sb.Append("+");
                    }
                    sb.Append("\n");
                }
                for(var x = 0; x <= board.rooms.GetUpperBound(0); x++)
                {
                    if (x == 0)
                    {
                        sb.Append(board.rooms[x,y].leftwall()? "|" : " ");
                    }
                    var addr = new Address(x, y);
                    //Console.WriteLine($"current: {current}, addr: {addr}");
                    if (addr == board.start)
                    {
                        sb.Append("S");
                    }
                    else if (addr == board.finish)
                    {
                        sb.Append("E");
                    }
                    else if (current != null && addr == current)
                    {
                        sb.Append("X");
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                    sb.Append(board.rooms[x,y].rightwall()? "|" : " ");
                }
                sb.Append("\n");
                for(var x = 0; x <= board.rooms.GetUpperBound(0); x++)
                {
                    if (x == 0)
                    {
                        sb.Append("+");
                    }
                    sb.Append(board.rooms[x, y].bottomwall()? "-" : " ");
                    sb.Append("+");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        private static Room[,] GenerateRooms(int xsize, int ysize)
        {
            var rooms = new Room[xsize, ysize];
            for(int y = 0; y < ysize; y++)
            {
                for(int x = 0; x < xsize; x++)
                {
                    var room = new Room(new Address(x, y));
                    if (x <= 0)
                    {
                        room.adjacent.Add(room.address.right());
                    }
                    else if (x >= xsize - 1)
                    {
                        room.adjacent.Add(room.address.left());
                    }
                    else
                    {
                        room.adjacent.Add(room.address.left());
                        room.adjacent.Add(room.address.right());
                    }

                    if (y <= 0)
                    {
                        room.adjacent.Add(room.address.bottom());
                    }
                    else if (y >= ysize - 1)
                    {
                        room.adjacent.Add(room.address.top());
                    }
                    else
                    {
                        room.adjacent.Add(room.address.top());
                        room.adjacent.Add(room.address.bottom());
                    }

                    rooms[x,y] = room;
                }
            }
            return rooms;
        }
    }

    public class Address
    {
        public int xvalue { get; set; }
        public int yvalue { get; set; }

        public Address(int _xvalue, int _yvalue)
        {
            this.xvalue = _xvalue;
            this.yvalue = _yvalue;
        }

        public override bool Equals(Object obj) 
        {
            if (obj == null || GetType() != obj.GetType()) 
                return false;
            Address p = (Address)obj;
            return this.Equals(p);
        }
        public bool Equals(Address p)
        {
            return (xvalue == p.xvalue) && (yvalue == p.yvalue);
        }
        public static bool operator ==(Address a, Address b)
        {
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            return a.Equals(b);
        }
        public static bool operator !=(Address a, Address b)
        {
            return !(a == b);
        }


        public override int GetHashCode() 
        {
            return xvalue ^ yvalue;
        }

        public override string ToString()
        {
            return $"x: {this.xvalue}, y: {this.yvalue}";
        }

        public Address left()
        {
            return new Address(this.xvalue - 1, this.yvalue);
        }
        public Address top()
        {
            return new Address(this.xvalue, this.yvalue - 1);
        }
        public Address right()
        {
            return new Address(this.xvalue + 1, this.yvalue);
        }
        public Address bottom()
        {
            return new Address(this.xvalue, this.yvalue + 1);
        }
    }

    public class Room
    {
        public Address address { get; }
        public List<Address> adjacent { get; }
        public Room(Address _address)
        {
            this.address = _address;
            this.adjacent = new List<Address>();
        }

        public void AddAdjacent(Address address)
        {
            this.adjacent.Add(address);
        }

        public bool leftwall()
        { 
            return !this.adjacent.Contains(this.address.left());
        }
        public bool rightwall()
        {
            return !this.adjacent.Contains(this.address.right());
        }
        public bool topwall()
        {
            return !this.adjacent.Contains(this.address.top());
        }
        public bool bottomwall()
        {
            return !this.adjacent.Contains(this.address.bottom());
        }
    }

    public class Board
    {
        public Room[,] rooms { get; set; }

        public Address start { get; set; }

        public Address finish { get; set; }

        public Room GetRoom(Address address)
        {
            if (address.xvalue < rooms.GetLowerBound(0) ||
                address.xvalue > rooms.GetUpperBound(0) ||
                address.yvalue < rooms.GetLowerBound(1) ||
                address.yvalue > rooms.GetUpperBound(1))
            {
                Console.WriteLine($"Out of Range: {address}");
                return null;
            }
            var ret = rooms[address.xvalue, address.yvalue];
            return ret;
        }
    }

    public static class RandomHelpers
    {
        private static Random rng = new Random();  
        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }
        /*
        public static Stack<T> Shuffle<T>(this Stack<T> stack)
        {
            return new Stack<T>(stack.OrderBy(x => rng.Next()));
        }
        */
        public static void Shuffle<T>(this Stack<T> stack)
        {
            var values = stack.ToArray();
            stack.Clear();
            foreach (var value in values.OrderBy(x => rng.Next()))
                stack.Push(value);
        }
        private static bool NextBool(this Random rand)
        {
            return rand.NextDouble() >= 0.5;
        }
    }
    
}
