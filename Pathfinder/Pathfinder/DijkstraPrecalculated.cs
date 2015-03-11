using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder
{
    class DijkstraPrecalculated: AiBotBase
    {
        public Coord2[,] map;
        node[,] Carray;
        public DijkstraPrecalculated(int x, int y)
            : base(x, y)
        {
        }
        public void generate(Level level)
        {
            map = new Coord2[(int)Math.Pow(level.GridSize, 2), (int)Math.Pow(level.GridSize, 2)];
            long time = DateTime.Now.Ticks;
            //from
            for (int i = 0; i < level.GridSize; i++)
            {
                for (int j = 0; j < level.GridSize; j++)
                {
                    calcpath(level, new Coord2(j,i));
                    //to
                    for (int ii = 0; ii < level.GridSize; ii++)
                    {
                        for (int jj = 0; jj < level.GridSize; jj++)
                        {
                            if (level.ValidPosition(new Coord2(jj, ii)))
                            {
                                if (ii == i && jj == j)
                                {
                                    map[(i * level.GridSize) + j, (ii * level.GridSize) + jj] = new Coord2(j, i);
                                }
                                else
                                {
                                    //find the node we want to path to
                                    node temp = new node();
                                    temp = Carray[jj, ii];
                                    CreatePath(temp);
                                    map[(ii * level.GridSize) + jj, (i * level.GridSize) + j] = path[path.Count - 2];
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("time taken = " + ((DateTime.Now.Ticks-time) / 10000000) + " seconds");
        }
        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            SetNextGridPosition(map[(plr.GridPosition.Y * level.GridSize) + plr.GridPosition.X, (GridPosition.Y * level.GridSize) + GridPosition.X], level);
        }
        void calcpath(Level level, Coord2 startPos)
        {
            Carray= new node[level.GridSize,level.GridSize];
            for (int i = 0; i < level.GridSize; i++)
            {
                for (int j = 0; j < level.GridSize; j++)
                {
                    Carray[j, i].cost = -1;
                }
            }
            //clear arrays
            open.Clear();
            path.Clear();
            //add start to open list
            node start = new node();
            start.pos = startPos;
            start.prev = new Coord2(-1, -1);
            start.cost = 0;
            open.Add(start);
            //begin the loop
            bool found = false;
            //loop until the whole map is searched
            while (!found)
            {
                //get best node
                node curr = new node(); ;
                curr.cost = int.MaxValue;
                int count = 0;
                int rem = 0;
                if (open.Count == 0)
                {
                    //finished
                    found = true;
                    break;
                }
                foreach (node n in open)
                {
                    if (n.cost < curr.cost)
                    {
                        curr = n;
                        rem = count;
                    }
                    count++;
                }
                //remove from open list
                open.RemoveAt(rem);
                Carray[curr.pos.X, curr.pos.Y] = curr;
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
            }
        }
        void CheckSpace(node curr, Level level)
        {
            if (level.ValidPosition(curr.pos))
            {
                //is it in the closed list already
                if (Carray[curr.pos.X, curr.pos.Y].cost != -1)
                {
                    return;
                }

                //is there a worse route in the open list
                foreach (node n in open)
                {
                    if (n.pos == curr.pos)
                    {
                        if (n.cost > curr.cost)
                        {
                            open.Remove(n);
                            open.Add(curr);
                            return;
                        }
                        return;
                    }
                }
                open.Add(curr);
            }
        }
        void CreatePath(node end)
        {
            path.Clear();
            path.Add(end.pos);
            while (end.prev != new Coord2(-1, -1))
            {
                if (end.prev == end.pos)
                {
                    path.Add(end.prev);
                    break;
                }
                end = Carray[end.prev.X, end.prev.Y];
                path.Add(end.pos);
            }
        }
    }
}
