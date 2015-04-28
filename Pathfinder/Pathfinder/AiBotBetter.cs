using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pathfinder
{
    class AiBotBetter : AiBotBase
    {
        public override void Setup(Level lvl, Player plr) { }
        public int direction = 0;
        /* 0
         *3 1
         * 2
         */
        bool walling = false;
        public AiBotBetter(int x, int y)
            : base(x, y)
        {

        }
        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            Coord2 pos;
            if (!walling)
            {
                //move towards the player
                if (GridPosition.X > plr.GridPosition.X)
                {
                    direction = 3;
                }
                else if (GridPosition.X < plr.GridPosition.X)
                {
                    direction = 1;
                }
                else if (GridPosition.Y > plr.GridPosition.Y)
                {
                    direction = 2;
                }
                else if (GridPosition.Y < plr.GridPosition.Y)
                {
                    direction = 0;
                }

                pos = GridPosition;
                pos = move(direction);
                if (!level.ValidPosition(pos))
                {
                    walling = true;
                    Random rnd = new Random();
                    int dir = rnd.Next(0, 2);
                    if (dir == 0)
                    {
                        direction--;
                    }
                    else
                    {
                        direction++;
                    }
                    if (direction < 0)
                        direction = 3;
                    if (direction > 3)
                        direction = 0;
                }
            }
            if(0==nextToWall(level))
            {
                walling = false;
            }
            if (2 == nextToWall(level))
            {
                Random rnd = new Random();
                    int dir = rnd.Next(0, 2);
                    if (dir == 0)
                    {
                        direction--;
                    }
                    else
                    {
                        direction++;
                    }
                    if (direction < 0)
                        direction = 3;
                    if (direction > 3)
                        direction = 0;
                
            }
            pos = move(direction);
            SetNextGridPosition(pos, level);
        }
        private Coord2 move(int direction)
        {
            Coord2 pos = GridPosition;
            switch (direction)
            {
                case 0:
                    pos.Y++;
                    break;
                case 1:
                    pos.X++;
                    break;
                case 2:
                    pos.Y--;
                    break;
                case 3:
                    pos.X--;
                    break;
            }
            return pos;
        }
        int nextToWall(Level level)
        {
            int ntw = 0;
            Coord2 pos = GridPosition;
            pos.X++;
            if (!level.ValidPosition(pos))
            {
                ntw ++;
            }
            pos.X -= 2;
            if (!level.ValidPosition(pos))
            {
                ntw ++;
            }
            pos.X++;
            pos.Y++;
            if (!level.ValidPosition(pos))
            {
                ntw ++;
            }
            pos.Y -= 2;
            if (!level.ValidPosition(pos))
            {
                ntw ++;
            }
            return ntw;
        }
    }
}
