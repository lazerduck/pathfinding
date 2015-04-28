using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pathfinder
{
    class Precalculated : AiBotBase
    {
        public override void Setup(Level lvl, Player plr) { }
        bool diag = false;
        float weight = 1f;
        float[,] heuristic;
        //huge ass map to next node 
        Coord2[,] map;


        public Precalculated(int x, int y)
            : base(x, y)
        {

        }
        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            //SetNextGridPosition(map[(GridPosition.Y * level.gridY) + GridPosition.X, (plr.GridPosition.Y * level.gridY) + plr.GridPosition.X], level);
        }
        public void calcPaths(Level level)
        {
            heuristic = new float[level.gridX, level.gridY];
            map = new Coord2[(int)Math.Pow(level.gridX, 2), (int)Math.Pow(level.gridY, 2)];
            //to point
            //NOTE - i = y, j = x
            long time_sec = 0;
            long time_min = 0;
            for (int i = 0; i < level.gridY; i++)
            {
                for (int j = 0; j < level.gridX; j++)
                {
                    long time = DateTime.Now.Ticks;
                    if (level.ValidPosition(new Coord2(j, i)))
                    {
                        CalcHeuristic(level, new Coord2(j, i));
                        //from point 
                        for (int ii = 0; ii < level.gridY; ii++)
                        {
                            for (int jj = 0; jj < level.gridX; jj++)
                            {

                                if (ii == i && jj == j)
                                {
                                    map[(ii * level.gridX) + jj, (i * level.gridX) + j] = new Coord2(j, i);
                                }
                                else
                                {
                                    calcpath(level, new Coord2(j, i), new Coord2(jj, ii));
                                    //add last node to array
                                    map[(ii * level.gridX) + jj, (i * level.gridX) + j] = path.Last<Coord2>();
                                }
                            }
                        }
                    }
                    else
                        Console.WriteLine("was a wall");
                    long newTime = DateTime.Now.Ticks - time;
                    newTime /= 10000;
                    time_sec += newTime / 1000;
                    time_min = time_sec / 60;
                    Console.WriteLine("elapsed time since last: " + newTime + "ms seconds since start: " + time_sec + "s, mins since start: " + time_min + "mins. Currently at - " + j + ", " + i);
                }
            }
            //save the map
            StreamWriter output = new StreamWriter("map.txt");
            for (int i = 0; i < (int)Math.Pow(level.gridY, 2); i++)
            {
                for (int j = 0; j < (int)Math.Pow(level.gridX, 2); j++)
                {
                    output.Write(map[j,i].X+","+map[j,i].Y + " ");
                }
            }
            
           
            
        }
        public void LoadMap(string addr, Level lvl)
        {
            map = new Coord2[(int)Math.Pow(lvl.gridX, 2), (int)Math.Pow(lvl.gridY, 2)];
            StreamReader input = new StreamReader(addr);
            string raw = input.ReadToEnd();
            string[] split = raw.Split(' ');
            for (int i = 0; i < (int)Math.Pow(lvl.gridY, 2)-1; i++)
            {
                for (int j = 0; j < (int)Math.Pow(lvl.gridX, 2); j++)
                {
                    string []splitAgain = split[(i * lvl.gridY*lvl.gridY) + j].Split(',');
                    map[j, i].X = Convert.ToInt32(splitAgain[0]);
                    map[j, i].Y = Convert.ToInt32(splitAgain[1]);
                    
                }
            }
            
        }
        void calcpath(Level level, Coord2 target, Coord2 startpos)
        {
            //clear arrays
            open.Clear();
            closed.Clear();
            path.Clear();
            //add start to open list
            node start = new node();
            start.pos = startpos;
            start.prev = new Coord2(-1, -1);
            start.cost = 0;
            start.f_score = heuristic[start.pos.X, start.pos.Y];
            open.Add(start);
            //begin the loop
            bool found = false;
            while (!found)
            {
                //get best node
                node curr = new node();
                curr.pos = new Coord2(-1, -1);
                curr.f_score = float.MaxValue;
                int count = 0;
                int rem = 0;
                if (curr.pos == target)
                {
                    CreatePath(curr);
                    found = true;
                }
                foreach (node n in open)
                {
                    if (n.f_score < curr.f_score)
                    {
                        curr = n;
                        rem = count;
                    }
                    count++;
                }
                //remove from open list
                open.RemoveAt(rem);
                closed.Add(curr);
                //are we at the end
                if (curr.pos == target)
                {
                    CreatePath(curr);
                    found = true;
                }
                //check nodes
                curr.prev = curr.pos;
                curr.cost += 1;
                curr.pos.X += 1;
                CheckSpace(curr, level);
                curr.pos.X -= 2;
                CheckSpace(curr, level);
                curr.pos.X += 1;
                curr.pos.Y += 1;
                CheckSpace(curr, level);
                curr.pos.Y -= 2;
                CheckSpace(curr, level);
                if (diag)
                {
                    curr.cost += (float)Math.Sqrt(2) - 1;
                    curr.pos.X += 1;
                    CheckSpace(curr, level);
                    curr.pos.X -= 2;
                    CheckSpace(curr, level);
                    curr.pos.Y += 2;
                    CheckSpace(curr, level);
                    curr.pos.X += 2;
                    CheckSpace(curr, level);
                }
            }
        }
        void CheckSpace(node curr, Level level)
        {
            if (curr.pos.X >= level.gridX || curr.pos.Y >= level.gridY)
            {
                return;
            }
            if (curr.pos.X < 0 || curr.pos.Y < 0)
            {
                return;
            }
            if (level.ValidPosition(curr.pos))
            {
                //is it in the closed list already
                foreach (node n in closed)
                {
                    if (n.pos == curr.pos)
                    {
                        return;
                    }
                }

                //is there a worse route in the open list
                foreach (node n in open)
                {
                    if (n.pos == curr.pos)
                    {
                        if (n.cost > curr.cost)
                        {
                            open.Remove(n);
                            curr.f_score = curr.cost + heuristic[curr.pos.X, curr.pos.Y];
                            open.Add(curr);
                            return;
                        }
                        return;
                    }
                }
                curr.f_score = curr.cost + heuristic[curr.pos.X, curr.pos.Y];
                open.Add(curr);
            }
        }
        void CreatePath(node end)
        {
            path.Add(end.pos);
            while (end.prev != new Coord2(-1, -1))
            {
                path.Add(end.pos);
                foreach (node n in closed)
                {
                    if (n.pos == end.prev)
                    {
                        end = n;
                    }
                }
            }
        }
    }
}
