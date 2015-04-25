using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder
{
    public static class  MultiActors
    {
        public static Coord2[,] map;
        public static int[,] buffer1;
        public static List<node> closed = new List<node>();
    }
}
