﻿using System;
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
    //aiBotBase is the base class for all ai objects. It encapsulates the position, movement animation, and validation 
    //of new positions, but leaves the actual behaviour function as an abstract member that must be implmented by a 
    //dervied class 



    abstract class AiBotBase
    {
        //lists
        public List<node> open = new List<node>();
        public List<node> closed = new List<node>();
        public List<Coord2> path = new List<Coord2>();
        //gridPosition is the current location of the bot.
        //targetPosition is the next chosen location of the bot = the position it is moving towards
        //screen position is the position the sprite is drawn to - somewhere between gridlocation and targetlocation
        private Coord2 gridPosition; //X position, Y position on grid
        private Coord2 startPosition;
        private Coord2 targetPosition; //X position, Y position on grid
        private Coord2 screenPosition; //X position, Y position on screen
        int timerMs;
        const int moveTime = 400; //miliseconds

        protected bool diag = true;
        protected float weight = 1f;

        protected float[,] heuristic;

        //accessors
        public Coord2 GridPosition
        {
            get { return gridPosition; }
        }
        public Coord2 ScreenPosition
        {
            get { return screenPosition; }
        }

        //constructor: requires initial position
        public AiBotBase(int x, int y)
        {
            gridPosition = new Coord2(x, y);
            targetPosition = new Coord2(x, y);
            screenPosition = new Coord2(x, y);
            startPosition = gridPosition;
            timerMs = moveTime;
        }
        public void resetPos() { gridPosition = startPosition; targetPosition = gridPosition; }
        //sets target position: the next grid location to move to
        //need to validate this position - so must be within 1 cell of current position(in x and y directions)
        //and must also be valid on the map: greater than 0, less than mapsize, and not a wall
        public bool SetNextGridPosition(Coord2 pos, Level level)
        {
            if (pos.X < (gridPosition.X - 1)) return false;
            if (pos.X > (gridPosition.X + 1)) return false;
            if (pos.Y < (gridPosition.Y - 1)) return false;
            if (pos.Y > (gridPosition.Y + 1)) return false;
            if (!level.ValidPosition(pos)) return false;
            targetPosition = pos;
            return true;
        }

        //Handles animation moving from current grid position (gridLocation) to next grid position (targetLocation)
        //When target location is reached, sets grid location to targetLocation, and then calls ChooseNextGridLocation
        //and resets animation timer
        public virtual void Update(GameTime gameTime, Level level, Player plr)
        {
            timerMs -= gameTime.ElapsedGameTime.Milliseconds;
            if (timerMs <= 0)
            {
                gridPosition = targetPosition;
                ChooseNextGridLocation(level, plr);
                timerMs = moveTime;
            }
            //calculate screen position
            screenPosition = (gridPosition * 15) + ((((targetPosition * 15) - (gridPosition * 15)) * (moveTime - timerMs)) / moveTime);
        }
        public virtual void Draw(SpriteBatch spritebatch, Texture2D texture, Level lvl)
        {

        }
        //this function is filled in by a derived class: must use SetNextGridLocation to actually move the bot
        protected abstract void ChooseNextGridLocation(Level level, Player plr);

        public virtual void Setup(Level lvl, Player plr){}

        public void CalcHeuristic(Level level, Coord2 player)
        {
            if (diag)
            {
                for (int i = 0; i < level.gridX; i++)
                {
                    for (int j = 0; j < level.gridY; j++)
                    {
                        float D2 = (float)Math.Sqrt(2.0f) * weight;
                        float dx = Math.Abs(i - player.X);
                        float dy = Math.Abs(j - player.Y);
                        heuristic[i, j] = weight * (dx + dy) + (D2 - 2 * weight) * Math.Min(dx, dy);
                    }
                }
            }
            else
                for (int i = 0; i < level.gridX; i++)
                {
                    for (int j = 0; j < level.gridX; j++)
                    {
                        float dx = Math.Abs(i - player.X);
                        float dy = Math.Abs(j - player.Y);
                        heuristic[i, j] = weight * (dx + dy);
                    }
                }
        }
    }
}
