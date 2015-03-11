using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;

namespace Pathfinder
{
    class AiBotAvP : AiBotBase
    {
        public int playerscent = 20;
        public int[,] buffer1;
        public int[,] buffer2;
        public AiBotAvP(int x, int y, bool search, Level lvl)
            : base(x, y)
        {
            buffer1 = new int[lvl.gridX, lvl.gridY];
            buffer2 = new int[lvl.gridX, lvl.gridY];
            for (int i = 0; i < lvl.gridX; i++)
            {
                for (int j = 0; j < lvl.gridY; j++)
                {
                    buffer1[i, j] = 0;
                    buffer2[i, j] = 0;
                }
            }
        }
        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            int curr = 0;
            Coord2 pos = GridPosition;
            Coord2 next = new Coord2(0, 0); ;
            pos.X += 1;
            if (level.ValidPosition(pos))
            {
                if(buffer1[pos.X, pos.Y] > curr)
                {
                    curr = buffer1[pos.X, pos.Y];
                    next = pos;
                }
            }
            pos.X += -2;
            if (level.ValidPosition(pos))
            {
                if (buffer1[pos.X, pos.Y] > curr)
                {
                    curr = buffer1[pos.X, pos.Y];
                    next = pos;
                }
            }
            pos.X += 1;
            pos.Y += 1;
            if (level.ValidPosition(pos))
            {
                if (buffer1[pos.X, pos.Y] > curr)
                {
                    curr = buffer1[pos.X, pos.Y];
                    next = pos;
                }
            }
            pos.Y -= 2;
            if (level.ValidPosition(pos))
            {
                if (buffer1[pos.X, pos.Y] > curr)
                {
                    curr = buffer1[pos.X, pos.Y];
                    next = pos;
                }
            }


            SetNextGridPosition(next, level);
        }
        public override void Update(GameTime gameTime, Level level, Player plr)
        {
            base.Update(gameTime, level, plr);
            //set the player pos to be the current scent
            buffer1[plr.GridPosition.X, plr.GridPosition.Y] = playerscent;
            //increment the scent
            //playerscent++;
            buffer2 = buffer1;
            //Update buffer 1 based on buffer 2
            for (int i = 0; i < level.gridX; i++)
            {
                for (int j = 0; j < level.gridY; j++)
                {
                    UpdateSpace(new Coord2(i, j), level);
                }
            }

        }
        private void UpdateSpace(Coord2 pos, Level level)
        {
            if (level.ValidPosition(pos))
            {
                int curr = buffer2[pos.X, pos.Y];
                pos.X += 1;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                pos.X -= 2;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                pos.X += 1;
                pos.Y += 1;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                pos.Y -= 2;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }

                pos.X += 1;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                pos.X -= 2;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                pos.Y += 2;

                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                pos.X += 2;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                pos.X -= 1;
                pos.Y -= 1;
                buffer1[pos.X, pos.Y] = curr - 1;
            }
        }
    }
}
