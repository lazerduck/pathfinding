﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Pathfinder
{
    class DijkstraShared : AiBotBase
    {
        Coord2 target;
        bool calc = true;
        public DijkstraShared(int x, int y)
            : base(x, y)
        {
            if(MultiActors.closed.Count == 0)
            {
                MultiActors.closed.Add(new node());
            }
            else
            {
                calc = false;
            }
        }
        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            if (target != plr.GridPosition)
            {
                target = plr.GridPosition;
                calcpath(level);
            }
            if (path.Count != 0)
            {
                SetNextGridPosition(path[0], level);
                path.RemoveAt(0);
            }
        }
        void calcpath(Level level)
        {
            if (calc)
            {
                //clear arrays
                open.Clear();
                closed.Clear();
                path.Clear();
                //add start to open list
                node start = new node();
                start.pos = target;
                start.prev = new Coord2(-1, -1);
                start.cost = 0;
                open.Add(start);
                //begin the loop
                bool finished = false;
                while (!finished)
                {
                    //get best node
                    node curr = new node(); ;
                    curr.cost = int.MaxValue;
                    int count = 0;
                    int rem = 0;
                    if (open.Count == 0)
                    {
                        //no path 
                        finished = true;
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
                    closed.Add(curr);
                    //check nodes
                    curr.prev = curr.pos;
                    curr.cost += 1;
                    //right
                    curr.pos.X += 1;
                    CheckSpace(curr, level);
                    //left
                    curr.pos.X -= 2;
                    CheckSpace(curr, level);
                    //down
                    curr.pos.X += 1;
                    curr.pos.Y += 1;
                    CheckSpace(curr, level);
                    //up
                    curr.pos.Y -= 2;
                    CheckSpace(curr, level);
                    curr.cost += 0.4142f;
                    //up left
                    curr.pos.X -= 1;
                    CheckSpace(curr, level);
                    //up right
                    curr.pos.X += 2;
                    CheckSpace(curr, level);
                    ////down right
                    curr.pos.Y += 2;
                    CheckSpace(curr, level);
                    //down left
                    curr.pos.X -= 2;
                    CheckSpace(curr, level);
                }
                MultiActors.closed.Clear();
                MultiActors.closed = new List<node>(closed);
            }
            else
            {
                closed.Clear();
                path.Clear();
                closed = MultiActors.closed;
            }
            //find the path
            findEnd();
        }
        void CheckSpace(node curr, Level level)
        {
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
                            open.Add(new node(curr));
                            return;
                        }
                        return;
                    }
                }
                open.Add(curr);
            }
        }
        void findEnd()
        {
            foreach(node n in closed)
            {
                if(n.pos == GridPosition)
                {
                    CreatePath(n);
                    return;
                }
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
