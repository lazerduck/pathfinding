using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Pathfinder
{
    class AiBotAvPReverse : AiBotBase
    {
        public int playerscent = 1;
        public int[,] buffer1;
        public int[,] buffer2;
        public AiBotAvPReverse(int x, int y, bool search)
            : base(x, y)
        {
            buffer1 = new int[40, 40];
            buffer2 = new int[40, 40];
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    buffer1[i, j] = 0;
                    buffer2[i, j] = 0;
                }
            }
        }
        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            int curr = int.MaxValue;
            Coord2 pos = GridPosition;
            Coord2 next = new Coord2(0, 0); ;
            pos.X += 1;
            if (level.ValidPosition(pos))
            {
                if (buffer1[pos.X, pos.Y] < curr)
                {
                    curr = buffer1[pos.X, pos.Y];
                    next = pos;
                }
            }
            pos.X += -2;
            if (level.ValidPosition(pos))
            {
                if (buffer1[pos.X, pos.Y] < curr)
                {
                    curr = buffer1[pos.X, pos.Y];
                    next = pos;
                }
            }
            pos.X += 1;
            pos.Y += 1;
            if (level.ValidPosition(pos))
            {
                if (buffer1[pos.X, pos.Y] < curr)
                {
                    curr = buffer1[pos.X, pos.Y];
                    next = pos;
                }
            }
            pos.Y -= 2;
            if (level.ValidPosition(pos))
            {
                if (buffer1[pos.X, pos.Y] < curr)
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
            buffer2 = buffer1;
            //Update buffer 1 based on buffer 2
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    if(i!= plr.GridPosition.X && j!= plr.GridPosition.Y)
                    UpdateSpace(new Coord2(i, j), level);
                }
            }
            KeyboardState keys = Keyboard.GetState();
            if(keys.IsKeyDown(Keys.P))
            {
                for(int i = 0; i<40;i++)
                {
                    for(int j = 0 ; j< 40;j++)
                    {
                        if(buffer1[j,i] >= 10)
                        Console.Write(buffer1[j,i] + ",");
                        else
                        Console.Write(buffer1[j,i] + " ,");
                    }
                    Console.Write("\n");
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
                    if (buffer2[pos.X, pos.Y] < curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                pos.X -= 2;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] < curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                pos.X += 1;
                pos.Y += 1;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] < curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                pos.Y -= 2;
                if (level.ValidPosition(pos))
                {
                    if (buffer2[pos.X, pos.Y] < curr)
                    {
                        curr = buffer2[pos.X, pos.Y];
                    }
                }
                pos.Y += 1;
                //diagonals
                //pos.X += 1;
                //if (level.ValidPosition(pos))
                //{
                //    if (buffer2[pos.X, pos.Y] < curr)
                //    {
                //        curr = buffer2[pos.X, pos.Y];
                //    }
                //}
                //pos.X -= 2;
                //if (level.ValidPosition(pos))
                //{
                //    if (buffer2[pos.X, pos.Y] < curr)
                //    {
                //        curr = buffer2[pos.X, pos.Y];
                //    }
                //}
                //pos.Y += 2;

                //if (level.ValidPosition(pos))
                //{
                //    if (buffer2[pos.X, pos.Y] < curr)
                //    {
                //        curr = buffer2[pos.X, pos.Y];
                //    }
                //}
                //pos.X += 2;
                //if (level.ValidPosition(pos))
                //{
                //    if (buffer2[pos.X, pos.Y] < curr)
                //    {
                //        curr = buffer2[pos.X, pos.Y];
                //    }
                //}
                //pos.X -= 1;
                //pos.Y -= 1;
                //if(curr != buffer1[pos.X,pos.Y])
                buffer1[pos.X, pos.Y] = curr + 1;
            }
        }
    }
}
