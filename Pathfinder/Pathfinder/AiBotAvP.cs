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
        bool calc = true;
        public int[,] buffer1;
        public int[,] buffer2;
        public AiBotAvP(int x, int y)
            : base(x, y)
        {
        }
        public override void Setup(Level lvl, Player plr)
        {
            if (MultiActors.buffer1 == null)
            {
                calc = true;
                buffer1 = new int[lvl.gridX, lvl.gridY];
                MultiActors.buffer1 = buffer1;
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
            else
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
                calc = false;
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
                if (buffer1[pos.X, pos.Y] > curr)
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
            pos.X += 1;
            if (level.ValidPosition(pos))
            {
                if (buffer1[pos.X, pos.Y] > curr)
                {
                    curr = buffer1[pos.X, pos.Y];
                    next = pos;
                }
            }
            //check up left
            pos.X -= 2;
            if (level.ValidPosition(pos))
            {
                if (buffer1[pos.X, pos.Y] > curr)
                {
                    curr = buffer1[pos.X, pos.Y];
                    next = pos;
                }
            }
            //check down left
            pos.Y += 2;

            if (level.ValidPosition(pos))
            {
                if (buffer1[pos.X, pos.Y] > curr)
                {
                    curr = buffer1[pos.X, pos.Y];
                    next = pos;
                }
            }
            //check down right
            pos.X += 2;
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
                if (calc)
                {
                //set the player pos to be the current scent
                buffer1[plr.GridPosition.X, plr.GridPosition.Y] = playerscent;
                //increment the scent
                playerscent++;
                for (int i = 0; i < level.gridX - 1; i++)
                {
                    for (int j = 0; j < level.gridY - 1; j++)
                    {
                        buffer2[i, j] = buffer1[i, j];
                    }
                }
                //buffer2 = buffer1;
                //Update buffer 1 based on buffer 2
                for (int i = 0; i < level.gridX; i++)
                {
                    for (int j = 0; j < level.gridY; j++)
                    {
                        UpdateSpace(new Coord2(i, j), level);
                    }
                }
            }
            else
            {
                buffer1 = MultiActors.buffer1;
            }
        }
        private void UpdateSpace(Coord2 pos, Level level)
        {
            if (level.ValidPosition(pos))
            {
                int curr = buffer2[pos.X, pos.Y];
                //check right
                pos.X += 1;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                //check left
                pos.X -= 2;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                //check down
                pos.X += 1;
                pos.Y += 1;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                //check up
                pos.Y -= 2;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                //check up right
                pos.X += 1;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                //check up left
                pos.X -= 2;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                //check down left
                pos.Y += 2;

                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                //check down right
                pos.X += 2;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] > curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                //return to current pos
                pos.X -= 1;
                pos.Y -= 1;
                //update scent
                buffer1[pos.X, pos.Y] = curr - 1;
            }
        }
    }
}
