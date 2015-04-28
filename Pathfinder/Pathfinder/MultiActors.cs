using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder
{

    public static class  MultiActors
    {
        public enum TestState {E20, L20, H20, E50, L50, H50, E100, L100, H100};
        public enum TestAlgorithm { Dijkstra, Astar, Precalculated, AvP, RSR };
        public static bool TestFinished = false;
        public static bool AlgFinished = false;
        public static TestState state;
        public static TestAlgorithm algo;
        public static string Metrics;
        public static Coord2[,] map;
        public static int[,] buffer1;
        public static List<node> closed = new List<node>();
    }
}
