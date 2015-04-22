using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Pathfinder
{
    class Astar : AiBotBase
    {
        Coord2 target;
        public Astar(int x, int y)
            : base(x, y)
        {

        }
        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            heuristic = new float[level.GridSize,level.GridSize];
            if (target != plr.GridPosition)
            {
                target = plr.GridPosition;
                CalcHeuristic(level, plr.GridPosition);
                calcpath(level);
            }
            if (path.Count != 0)
            {
                SetNextGridPosition(path.Last<Coord2>(), level);
                path.RemoveAt(path.Count - 1);
            }
        }
        void calcpath(Level level)
        {
            //clear arrays
            open.Clear();
            closed.Clear();
            path.Clear();
            //add start to open list
            node start = new node();
            start.pos = GridPosition;
            Console.WriteLine(GridPosition.X + " " + GridPosition.Y);
            start.prev = new Coord2(-1, -1);
            start.cost = 0;
            start.f_score = heuristic[start.pos.X, start.pos.Y];
            open.Add(start);
            //begin the loop
            bool found = false;
            while (!found)
            {
                //get best node
                node curr = new node(); ;
                curr.f_score = float.MaxValue;
                int count = 0;
                int rem = 0;
                if (open.Count == 0)
                {
                    //no path 
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
                    break;
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
                    curr.pos.X+=1;
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
            if (curr.pos.X >= level.gridX|| curr.pos.Y >= level.gridY)
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
