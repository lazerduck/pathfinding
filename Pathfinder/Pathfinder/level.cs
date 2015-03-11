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
    //loads a map from an ascii text file
    class Level
    {
        private const int gridSize = 40; //set the map grid size
        public int [,] tiles; //a 2d array of 0's and 1's: 0 = free cell, 1 = blocked cell
        public int gridX = 40;
        public int gridY = 40;
        //constructor initialises the grid array
        public Level()
        {
            tiles = new int[gridX, gridY];
            for (int i = 0; i < gridX; i++)
                for (int j = 0; j < gridY; j++)
                    tiles[i,j] = 0;
        }

        //accessors
        public int GridSize
        {
            get { return gridSize; }
        }

        //validates a grid position (passed as a 2d vector): returns false if position is blocked, or if x or y 
        //positions are greater than grid size, or less than 0
        public bool ValidPosition(Coord2 pos)
        {
            if (pos.X < 0) return false;
            if (pos.X >= gridX) return false;
            if (pos.Y < 0) return false;
            if (pos.Y >= gridY) return false;
            return (tiles[pos.X,pos.Y] == 0);
        }

        //loads the map from a text file
        public void Loadmap(string path)
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line = reader.ReadLine();
                gridX = line.Length;
                //Debug.Assert(line.Length == gridSize, "loaded map string line width must be 30");
                Debug.Assert(line.Length == gridX, String.Format("loaded map string line width must be {0}.", gridSize));
                while (line != null)
                {
                    lines.Add(line);
                    line = reader.ReadLine();
                }
                gridY = lines.Count;
            }
            //Debug.Assert(lines.Count == gridX, String.Format("loaded map string must have {0} lines.",gridSize));

            // Loop over every tile position,
            for (int y = 0; y < gridY; ++y)
            {
                for (int x = 0; x < gridX; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    if (tileType == '.') tiles[x, y] = 0;
                    else tiles[x, y] = 1;
                }
            }
        }
    }
}
