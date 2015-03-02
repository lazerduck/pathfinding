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
        Coord2 prevPos = new Coord2(-1,-1);
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
        public void ExpandRectangles(Level level)
        {
            pruned = new int[level.GridSize, level.GridSize];
            for (int i = 0; i < level.GridSize; i++)
            {
                for (int j = 0; j < level.GridSize; j++)
                {
                    pruned[i, j] = 1;
                    Coord2 temp = new Coord2(j, i);
                    Expand(temp, level);
                }
            }
            Prune(level);
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
        public void Prune(Level level)
        {
            for (int x = 0; x < level.GridSize; x++)
            {
                for (int y = 0; y < level.GridSize; y++)
                {
                    pruned[x, y] = 1;
                }
            }
            //check through each point
            for (int x = 0; x < level.GridSize; x++)
            {
                for (int y = 0; y < level.GridSize; y++)
                {
                    //check if it is on the edge of a rectangle
                    Coord2 temp = new Coord2(x, y);
                    foreach (Rectangle r in RectList)
                    {
                        if (r.isEdge(temp))
                        {
                            pruned[x, y] = 0;
                        }
                    }
                }
            }
        }
        public void DrawRectangles(SpriteBatch spritebatch, Texture2D texture)
        {
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 40; j++)
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
                Prune(level);
                prevPos = plr.GridPosition;
                CalculateHeuristic(level, plr);
                Astar(level, plr);

            }
        }
        void CalculateHeuristic(Level level, Player plr)
        {
            heuristic = new float[level.GridSize, level.GridSize];
            for (int i = 0; i < level.GridSize; i++)
            {
                for (int j = 0; j < level.GridSize; j++)
                {
                    float dx = Math.Abs(j - plr.GridPosition.X);
                    float dy = Math.Abs(i - plr.GridPosition.Y);
                    heuristic[j, i] = 1 * (dx + dy);
                }
            }
        }
        void Astar(Level level, Player plr)
        {
            Carray = new node[level.GridSize, level.GridSize];
            for (int i = 0; i < level.GridSize; i++)
            {
                for (int j = 0; j < level.GridSize; j++)
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
            //is in the range?
            if (curr.pos.Y - 1 >= 0)
            {
                //is it valid
                if (level.ValidPosition(new Coord2(curr.pos.X, curr.pos.Y - 1)))
                {
                    if (Carray[curr.pos.X, curr.pos.Y - 1].cost == -1)
                    {
                        //is it pruned?
                        if (pruned[curr.pos.X, curr.pos.Y - 1] == 0)
                        {
                            //not pruned
                            node temp = new node();
                            temp.prev = curr.pos;
                            temp.pos = new Coord2(curr.pos.X, curr.pos.Y - 1);
                            temp.cost = curr.cost + 1;
                            temp.f_score = temp.cost + heuristic[temp.pos.X, temp.pos.Y];
                            bool inuse = false;
                            for(int i = 0; i <open.Count;i++)
                            {
                                if (open[i].pos == temp.pos)
                                {
                                    inuse = true;
                                    if (open[i].cost > temp.cost)
                                    {
                                        inuse = true;
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
                            temp.prev = curr.pos;
                            temp.pos = new Coord2(curr.pos.X, curr.pos.Y - 1);
                            temp.cost = curr.cost + 1;
                            while (pruned[temp.pos.X, temp.pos.Y] != 0)
                            {
                                temp.pos.Y--;
                                temp.cost++;
                            }
                            temp.f_score = temp.cost + heuristic[temp.pos.X, temp.pos.Y];
                            bool inuse = false;
                            for (int i = 0; i < open.Count; i++)
                            {
                                if (open[i].pos == temp.pos)
                                {
                                    if (open[i].cost > temp.cost)
                                    {
                                        inuse = true;
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
                    }
                }
            }
            //check down
            //is in the range?
            if (curr.pos.Y + 1 <= level.GridSize-1)
            {
                //is it valid
                if (level.ValidPosition(new Coord2(curr.pos.X, curr.pos.Y + 1)))
                {
                    if (Carray[curr.pos.X, curr.pos.Y + 1].cost == -1)
                    {
                        //is it pruned?
                        if (pruned[curr.pos.X, curr.pos.Y + 1] == 0)
                        {
                            //not pruned
                            node temp = new node();
                            temp.prev = curr.pos;
                            temp.pos = new Coord2(curr.pos.X, curr.pos.Y + 1);
                            temp.cost = curr.cost + 1;
                            temp.f_score = temp.cost + heuristic[temp.pos.X, temp.pos.Y];
                            bool inuse = false;
                            for (int i = 0; i < open.Count; i++)
                            {
                                if (open[i].pos == temp.pos)
                                {
                                    if (open[i].cost > temp.cost)
                                    {
                                        inuse = true;
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
                            temp.prev = curr.pos;
                            temp.pos = new Coord2(curr.pos.X, curr.pos.Y + 1);
                            temp.cost = curr.cost + 1;
                            while (pruned[temp.pos.X, temp.pos.Y] != 0)
                            {
                                temp.pos.Y++;
                                temp.cost++;
                            }
                            temp.f_score = temp.cost + heuristic[temp.pos.X, temp.pos.Y];
                            bool inuse = false;
                            for (int i = 0; i < open.Count; i++)
                            {
                                if (open[i].pos == temp.pos)
                                {
                                    if (open[i].cost > temp.cost)
                                    {
                                        inuse = true;
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
                    }
                }
            }
            //check left
            //is in the range?
            if (curr.pos.X - 1 >= 0)
            {
                //is it valid
                if (level.ValidPosition(new Coord2(curr.pos.X-1, curr.pos.Y)))
                {
                    if (Carray[curr.pos.X - 1, curr.pos.Y].cost == -1)
                    {
                        //is it pruned?
                        if (pruned[curr.pos.X - 1, curr.pos.Y] == 0)
                        {
                            //not pruned
                            node temp = new node();
                            temp.prev = curr.pos;
                            temp.pos = new Coord2(curr.pos.X - 1, curr.pos.Y);
                            temp.cost = curr.cost + 1;
                            temp.f_score = temp.cost + heuristic[temp.pos.X, temp.pos.Y];
                            bool inuse = false;
                            for (int i = 0; i < open.Count; i++)
                            {
                                if (open[i].pos == temp.pos)
                                {
                                    if (open[i].cost > temp.cost)
                                    {
                                        inuse = true;
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
                            temp.prev = curr.pos;
                            temp.pos = new Coord2(curr.pos.X - 1, curr.pos.Y);
                            temp.cost = curr.cost + 1;
                            while (pruned[temp.pos.X, temp.pos.Y] != 0)
                            {
                                temp.pos.X--;
                                temp.cost++;
                            }
                            temp.f_score = temp.cost + heuristic[temp.pos.X, temp.pos.Y];
                            bool inuse = false;
                            for (int i = 0; i < open.Count; i++)
                            {
                                if (open[i].pos == temp.pos)
                                {
                                    if (open[i].cost > temp.cost)
                                    {
                                        inuse = true;
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
                    }
                }
            }
            //check right
            //is in the range?
            if (curr.pos.X+1 <= level.GridSize-1)
            {
                //is it valid
                if (level.ValidPosition(new Coord2(curr.pos.X + 1, curr.pos.Y)))
                {
                    if (Carray[curr.pos.X + 1, curr.pos.Y].cost == -1)
                    {
                        //is it pruned?
                        if (pruned[curr.pos.X + 1, curr.pos.Y] == 0)
                        {
                            //not pruned
                            node temp = new node();
                            temp.prev = curr.pos;
                            temp.pos = new Coord2(curr.pos.X + 1, curr.pos.Y);
                            temp.cost = curr.cost + 1;
                            temp.f_score = temp.cost + heuristic[temp.pos.X, temp.pos.Y];
                            bool inuse = false;
                            for (int i = 0; i < open.Count; i++)
                            {
                                if (open[i].pos == temp.pos)
                                {
                                    if (open[i].cost > temp.cost)
                                    {
                                        inuse = true;
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
                            temp.prev = curr.pos;
                            temp.pos = new Coord2(curr.pos.X + 1, curr.pos.Y);
                            temp.cost = curr.cost + 1;
                            while (pruned[temp.pos.X, temp.pos.Y] != 0)
                            {
                                temp.pos.X++;
                                temp.cost++;
                            }
                            temp.f_score = temp.cost + heuristic[temp.pos.X, temp.pos.Y];
                            bool inuse = false;
                            for (int i = 0; i < open.Count; i++)
                            {
                                if (open[i].pos == temp.pos)
                                {
                                    if (open[i].cost > temp.cost)
                                    {
                                        inuse = true;
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
                    }
                }
            }
        }
        void createPath(node curr)
        {
            path.Add(curr.pos);
            while(curr.prev.X != -1)
            {
                curr = Carray[curr.prev.X, curr.prev.Y];
                path.Add(curr.pos);
            }
        }
    }
}
