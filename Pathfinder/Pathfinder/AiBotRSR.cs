using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pathfinder
{
    class AiBotRSR : AiBotBase
    {
        struct Rectangle
        {
            public Coord2 TopLeft;
            public Coord2 BotRight;
            public Rectangle(Coord2 start)
            {
                TopLeft = start;
                BotRight = new Coord2();
            }
            public Rectangle(Coord2 start, Coord2 end)
            {
                TopLeft = start;
                BotRight = end;
            }
            public bool isContained(Coord2 pos)
            {
                if (pos.X >= TopLeft.X && pos.X <= BotRight.X)
                {
                    if (pos.Y >= TopLeft.Y && pos.Y <= BotRight.Y)
                    {
                        return true;
                    }
                }
                return false;
            }
            public bool isEdge(Coord2 pos)
            {
                if (pos.X >= TopLeft.X && pos.X <= BotRight.X)
                {
                    if (pos.Y == TopLeft.Y || pos.Y == BotRight.Y)
                    {
                        return true;
                    }
                }
                if (pos.Y >= TopLeft.Y && pos.Y <= BotRight.Y)
                {
                    if (pos.X == TopLeft.X || pos.X == BotRight.X)
                    {
                        return true;
                    }
                }
                return false;
            }
        };
        node[,] Carray;
        float weight = 0.7f;
        Coord2 prevPos = new Coord2(-1,-1);
        Coord2 moveto = new Coord2(0, 0);
        //created rectangle list
        List<Rectangle> RectList = new List<Rectangle>();
        //0 = in use, 1 = pruned
        int[,] pruned;
        //Astar stuff
        float[,] heuristic;
        public AiBotRSR(int x, int y)
            : base(x, y)
        {

        }
        //create the rectangles
        public void ExpandRectangles(Level level, Player plr)
        {
            pruned = new int[level.gridX, level.gridY];
            for (int i = 0; i < level.gridX; i++)
            {
                for (int j = 0; j < level.gridY; j++)
                {
                    pruned[i, j] = 1;
                    Coord2 temp = new Coord2(j, i);
                    Expand(temp, level);
                }
            }
            Prune(level, plr);
        }
        public void Expand(Coord2 start, Level level)
        {
            //check this is a valid position
            if (level.ValidPosition(start))
            {
                foreach (Rectangle r in RectList)
                {
                    if (r.isContained(start))
                    {
                        return;
                    }
                }
            }
            else
            {
                return;
            }
            //create new rectangle
            Rectangle temp = new Rectangle(start);
            //so we know when we cant expand further in a certain direction
            bool xLimit = false;
            bool yLimit = false;
            //current bottom right corent
            Coord2 ExpandPoint = start;
            //while we can still expand
            while (!xLimit || !yLimit)
            {
                Coord2 TestCase = ExpandPoint;
                if (!yLimit)
                {
                    //increase the y
                    ExpandPoint.Y += 1;
                    //temp coord2 for checking
                    TestCase = ExpandPoint;
                    //check that there are no conflicts along the newly expanded edge
                    for (int x = start.X; x <= ExpandPoint.X; x++)
                    {
                        TestCase.X = x;
                        if (level.ValidPosition(TestCase) && TestCase.Y < level.GridSize)
                        {
                            foreach (Rectangle r in RectList)
                            {
                                if (r.isContained(TestCase))
                                {
                                    yLimit = true;
                                    ExpandPoint.Y -= 1;
                                    goto end1;
                                }
                            }
                        }
                        else
                        {
                            yLimit = true;
                            ExpandPoint.Y -= 1;
                            break;
                        }
                    }
                }
            end1:
                if (!xLimit)
                {
                    //increase the x
                    ExpandPoint.X += 1;
                    //temp coord2 for checking
                    TestCase = ExpandPoint;
                    //check that there are no conflicts along the newly expanded edge
                    for (int y = start.Y; y <= ExpandPoint.Y; y++)
                    {
                        TestCase.Y = y;
                        if (level.ValidPosition(TestCase) && TestCase.Y < level.GridSize)
                        {
                            foreach (Rectangle r in RectList)
                            {
                                if (r.isContained(TestCase))
                                {
                                    xLimit = true;
                                    ExpandPoint.X -= 1;
                                    goto startPoint;
                                }
                            }
                        }
                        else
                        {
                            xLimit = true;
                            ExpandPoint.X -= 1;
                            break;
                        }
                    }
                }
            startPoint:
                TestCase = new Coord2(0, 0);
            }
            //add temp to our list of rectangles
            temp.BotRight = ExpandPoint;
            RectList.Add(temp);
        }
        public void Prune(Level level ,Player plr)
        {
            for (int x = 0; x < level.gridX; x++)
            {
                for (int y = 0; y < level.gridY; y++)
                {
                    pruned[x, y] = 1;
                }
            }
            //check through each point
            for (int x = 0; x < level.gridX; x++)
            {
                for (int y = 0; y < level.gridY; y++)
                {
                    //check if it is on the edge of a rectangle
                    Coord2 temp = new Coord2(x, y);
                    foreach (Rectangle r in RectList)
                    {
                        if (r.isEdge(temp))
                        {
                            pruned[x, y] = 0;
                        }
                        if (r.isContained(plr.GridPosition)&&r.isContained(temp))
                        {
                            pruned[x, y] = 0;
                        }
                    }
                }
            }
        }
        public void DrawRectangles(SpriteBatch spritebatch, Texture2D texture, Level lvl)
        {
            for (int i = 0; i < lvl.gridX; i++)
            {
                for (int j = 0; j < lvl.gridY; j++)
                {
                    if (pruned[i, j] == 0)
                    {
                        spritebatch.Draw(texture, new Coord2(i * 15, j * 15), Color.White * 0.2f);
                    }
                }
            }
        }
        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            if (prevPos != plr.GridPosition)
            {
                Prune(level, plr);
                prevPos = plr.GridPosition;
                CalculateHeuristic(level, plr);
                Astar(level, plr);

            }
            if (path.Count != 0)
            {
                if (moveto == GridPosition && path.Count != 1)
                {
                    path.RemoveAt(path.Count - 1);
                }
                moveto = path.Last<Coord2>();
                Coord2 next = GridPosition;
                if (moveto.X > GridPosition.X)
                {
                    next.X += 1;
                }
                if (moveto.X < GridPosition.X)
                {
                    next.X -= 1;
                }
                if (moveto.Y > GridPosition.Y)
                {
                    next.Y += 1;
                }
                if (moveto.Y < GridPosition.Y)
                {
                    next.Y -= 1;
                }
                SetNextGridPosition(next, level);
            }
        }
        void CalculateHeuristic(Level level, Player plr)
        {
            heuristic = new float[level.gridX, level.gridY];
            
            for (int i = 0; i < level.gridY; i++)
            {
                for (int j = 0; j < level.gridX; j++)
                {
                    float D2 = (float)Math.Sqrt(2.0f) * weight;
                    float dx = Math.Abs(j - plr.GridPosition.X);
                    float dy = Math.Abs(i - plr.GridPosition.Y);
                    heuristic[j, i] = weight * (dx + dy) + (D2 - 1 * weight) * Math.Min(dx, dy);
                }
            }
        }
        void Astar(Level level, Player plr)
        {
            Carray = new node[level.gridX, level.gridY];
            for (int i = 0; i < level.gridY; i++)
            {
                for (int j = 0; j < level.gridX; j++)
                {
                    Carray[j, i].cost = -1;
                }
            }
            open.Clear();
            path.Clear();
            //add start and end to the unpruned
            pruned[GridPosition.X, GridPosition.Y] = 0;
            pruned[plr.GridPosition.X, plr.GridPosition.Y] = 0;
            //add start node
            node start = new node();
            start.cost = 0;
            start.prev = new Coord2(-1, -1);
            start.pos = GridPosition;
            start.f_score = heuristic[start.pos.X, start.pos.Y];
            open.Add(start);
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
                Carray[curr.pos.X, curr.pos.Y] = curr;

                if (curr.pos == plr.GridPosition)
                {
                    found = true;
                    createPath(curr);
                    return;
                }
                checkNodes(curr, level);
            }
        }
        void checkNodes(node curr,Level level)
        {
            //check up
            
            checkNode(level,curr, new Coord2(0,-1));
           
            checkNode(level, curr, new Coord2(0, 1));
            
            checkNode(level, curr, new Coord2(-1, 0));
            
            checkNode(level, curr, new Coord2(1, 0));

            checkNode(level, curr, new Coord2(1, 1));
            checkNode(level, curr, new Coord2(1, -1));
            checkNode(level, curr, new Coord2(-1, -1));
            checkNode(level, curr, new Coord2(-1, 1));

            
           
        }
        void checkNode(Level level, node curr, Coord2 CheckVector)
        {
            curr.pos += CheckVector;
            if (curr.pos != curr.prev)
            {
                //is in the range?
                if (curr.pos.Y <= level.gridY - 1 && curr.pos.X <= level.gridX - 1 && curr.pos.Y >= 0 && curr.pos.X >= 0)
                {
                    //is it valid
                    if (level.ValidPosition(new Coord2(curr.pos.X, curr.pos.Y)))
                    {
                        if (Carray[curr.pos.X, curr.pos.Y].cost == -1)
                        {
                            //is it pruned?
                            if (pruned[curr.pos.X, curr.pos.Y] == 0)
                            {
                                //not pruned
                                node temp = new node();
                                temp.prev = curr.pos - CheckVector;
                                temp.pos = curr.pos;
                                float cost = (float)Math.Sqrt(Math.Pow(CheckVector.X, 2) + Math.Pow(CheckVector.Y, 2));
                                temp.cost = curr.cost + cost;
                                temp.f_score = temp.cost + heuristic[temp.pos.X, temp.pos.Y];
                                bool inuse = false;
                                for (int i = 0; i < open.Count; i++)
                                {
                                    if (open[i].pos == temp.pos)
                                    {
                                        inuse = true;
                                        if (open[i].cost > temp.cost)
                                        {
                                            
                                            open.RemoveAt(i);
                                            open.Add(temp);
                                            break;
                                        }
                                    }
                                }
                                if (!inuse)
                                {
                                    open.Add(temp);
                                }
                            }
                            else
                            {
                                //pruned - got to next edge
                                node temp = new node();
                                temp.prev = curr.pos - CheckVector;
                                temp.pos = new Coord2(curr.pos.X, curr.pos.Y);
                                float cost = (float)Math.Sqrt(Math.Pow(CheckVector.X, 2) + Math.Pow(CheckVector.Y, 2));
                                temp.cost = curr.cost + cost;
                                while (pruned[temp.pos.X, temp.pos.Y] != 0)
                                {
                                    temp.pos += CheckVector;
                                    temp.cost += cost;
                                }
                                temp.f_score = temp.cost + heuristic[temp.pos.X, temp.pos.Y];
                                bool inuse = false;
                                if (Carray[temp.pos.X, temp.pos.Y].cost == -1)
                                {
                                    for (int i = 0; i < open.Count; i++)
                                    {
                                        if (open[i].pos == temp.pos)
                                        {
                                            inuse = true;
                                            if (open[i].cost > temp.cost)
                                            {
                                                open.RemoveAt(i);
                                                open.Add(temp);
                                                break;
                                            }
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    //in the closed
                                    inuse = true;
                                }
                                if (!inuse)
                                {
                                    open.Add(temp);
                                }
                            }
                        }
                    }
                }
            }
        }
        void createPath(node curr)
        {
            path.Add(curr.pos); 
            while(curr.prev.X != -1)
            //for(int i = 0; i < 100; i ++)
            {
                curr = Carray[curr.prev.X, curr.prev.Y];
                path.Add(curr.pos);
                if (curr.prev == GridPosition)
                {
                    Console.WriteLine("found self");
                    path.Add(curr.prev);
                    break;
                }
                
            }
            Console.WriteLine("path length = " + path.Count);
        }
    }
}
