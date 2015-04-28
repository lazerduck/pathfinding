#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
using System.Threading;

#endregion

namespace Pathfinder
{
    
    public struct node
    {
        public Coord2 pos;
        public Coord2 prev;
        public float cost;
        public float f_score;
        public node(node other)
        {
            pos = other.pos;
            prev = other.prev;
            cost = other.cost;
            f_score = other.f_score;
        }
    };

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //sprite texture for tiles, player, and ai bot
        Texture2D tile1Texture;
        Texture2D tile2Texture;
        Texture2D aiTexture;
        Texture2D playerTexture;
        Texture2D open_tex;
        Texture2D closed_tex;
        Texture2D path_tex;
        //level size

        //objects representing the level map, bot, and player 
        private Level level;
        //private AiBotBase bot;
        private List<AiBotBase> bot = new List<AiBotBase>();
        private Player player;

        //screen size and frame rate
        private const int TargetFrameRate = 50;
        private const int BackBufferWidth = 600;
        private const int BackBufferHeight = 600;


        public Game1()
        {
            //use only the second core
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);
            //set the priority to be high
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            //initialise the testing
            MultiActors.state = MultiActors.TestState.E20;
            MultiActors.algo = MultiActors.TestAlgorithm.Astar;
            MultiActors.Metrics = "Dijksta\n20x20 empty";
            //constructor
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = BackBufferHeight;
            graphics.PreferredBackBufferWidth = BackBufferWidth;
            Window.Title = "Pathfinder";
            Content.RootDirectory = "../../../Content";
            //set frame rate
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
            //load level map
            level = new Level();
            level.Loadmap("../../../Content/50H.txt");
            Console.WriteLine("map = " + level.gridX + " by " + level.gridY);
            player = new Player(10, 10);
            //instantiate bot and player objects
            //for (int i = 5; i < 6; i++)
            {
                bot.Add(new Dijkstra(1, 1));
            }
            foreach (AiBotBase b in bot)
            {
                b.Setup(level, player);
            }
            //bot.generate(level);
            //bot.LoadMap("../../../Content/map4.txt", level.GridSize);
            //make mouse visable
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //load the sprite textures
            Content.RootDirectory = "../../../Content";
            tile1Texture = Content.Load<Texture2D>("tile1");
            tile2Texture = Content.Load<Texture2D>("tile2");
            aiTexture = Content.Load<Texture2D>("ai");
            playerTexture = Content.Load<Texture2D>("target");
            path_tex = Content.Load<Texture2D>("path_tile");
            open_tex = Content.Load<Texture2D>("open_tile");
            closed_tex = Content.Load<Texture2D>("closed_tile");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //player movement: read keyboard
            KeyboardState keyState = Keyboard.GetState();
            Coord2 currentPos = new Coord2();
            currentPos = player.GridPosition;
            if (keyState.IsKeyDown(Keys.Up))
            {
                currentPos.Y -= 1;
                player.SetNextLocation(currentPos, level);
            }
            else if (keyState.IsKeyDown(Keys.Down))
            {
                currentPos.Y += 1;
                player.SetNextLocation(currentPos, level);
            }
            else if (keyState.IsKeyDown(Keys.Left))
            {
                currentPos.X -= 1;
                player.SetNextLocation(currentPos, level);
            }
            else if (keyState.IsKeyDown(Keys.Right))
            {
                currentPos.X += 1;
                player.SetNextLocation(currentPos, level);
            }
            if(keyState.IsKeyDown(Keys.L))
            {
                level.Loadmap("../../../Content/20E.txt");
                foreach (AiBotBase b in bot)
                {
                    b.resetPos();
                    b.Setup(level, player);
                }
            }

            //update bot and player
            foreach (AiBotBase b in bot)
            {
                b.Update(gameTime, level, player);
            }
            player.Update(gameTime, level);
            //update the test
            if (MultiActors.AlgFinished)
            {
                bot.Clear();
                switch (MultiActors.algo)
                {
                    case MultiActors.TestAlgorithm.Dijkstra:
                        bot.Add(new Dijkstra(1, 1));
                        MultiActors.algo = MultiActors.TestAlgorithm.Astar;
                        MultiActors.Metrics += "\n\nDijkstra ";
                        break;
                        
                    case MultiActors.TestAlgorithm.Astar:
                        bot.Add(new Astar(1, 1));
                        MultiActors.algo = MultiActors.TestAlgorithm.Precalculated;
                        MultiActors.Metrics += "\n\nAstar ";
                        break;
                    case MultiActors.TestAlgorithm.Precalculated:
                         bot.Add(new DijkstraPrecalculated(1, 1));
                         MultiActors.algo = MultiActors.TestAlgorithm.AvP;
                         MultiActors.Metrics += "\n\nPre-Calculated ";
                        break;
                    case MultiActors.TestAlgorithm.AvP:
                         bot.Add(new AiBotAvP(1, 1));
                         MultiActors.algo = MultiActors.TestAlgorithm.RSR;
                         MultiActors.Metrics += "\n\nAvP ";
                        break;
                    case MultiActors.TestAlgorithm.RSR:
                         bot.Add(new AiBotRSR(1, 1));
                         MultiActors.Metrics += "\n\nRSR ";
                        break;
                }
                MultiActors.AlgFinished = false;
                foreach (AiBotBase b in bot)
                {
                    b.resetPos();
                    b.Setup(level, player);
                }
                
            }
            if (MultiActors.TestFinished)
            {
                switch (MultiActors.state)
                {
                    case MultiActors.TestState.E20:
                        level.Loadmap("../../../Content/20L.txt");
                        MultiActors.state = MultiActors.TestState.L20;
                        MultiActors.Metrics += "\n20x20 low ";
                        break;
                    case MultiActors.TestState.L20:
                        level.Loadmap("../../../Content/20H.txt");
                        MultiActors.state = MultiActors.TestState.H20;
                        MultiActors.Metrics += "\n20x20 high ";
                        break;
                    case MultiActors.TestState.H20:
                        level.Loadmap("../../../Content/50E.txt");
                        MultiActors.state = MultiActors.TestState.E50;
                        MultiActors.Metrics += "\n50x50 empty ";
                        break;
                    case MultiActors.TestState.E50:
                        level.Loadmap("../../../Content/50L.txt");
                        MultiActors.state = MultiActors.TestState.L50;
                        MultiActors.Metrics += "\n50x50 low ";
                        break;
                    case MultiActors.TestState.L50:
                        level.Loadmap("../../../Content/50H.txt");
                        MultiActors.state = MultiActors.TestState.H50;
                        MultiActors.Metrics += "\n50x50 high ";
                        break;
                    case MultiActors.TestState.H50:
                        level.Loadmap("../../../Content/100E.txt");
                        MultiActors.state = MultiActors.TestState.E100;
                        MultiActors.Metrics += "\n100x100 empty ";
                        break;
                    case MultiActors.TestState.E100:
                        level.Loadmap("../../../Content/100L.txt");
                        MultiActors.state = MultiActors.TestState.L100;
                        MultiActors.Metrics += "\n100x100 low ";
                        break;
                    case MultiActors.TestState.L100:
                        level.Loadmap("../../../Content/100H.txt");
                        MultiActors.state = MultiActors.TestState.H100;
                        MultiActors.Metrics += "\n100x100 high ";
                        break;
                    case MultiActors.TestState.H100:
                        level.Loadmap("../../../Content/20E.txt");
                        MultiActors.state = MultiActors.TestState.E20;
                        Console.Write(MultiActors.Metrics);
                        MultiActors.AlgFinished = true;
                        break;
                }
                MultiActors.TestFinished = false;
                foreach (AiBotBase b in bot)
                {
                    b.resetPos();
                    b.Setup(level, player);
                }
                MultiActors.map = null;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            //draw level map
            DrawGrid();
            foreach (AiBotBase b in bot)
            {
                b.Draw(spriteBatch, open_tex, level);
            }
            //draw bot
            foreach (AiBotBase b in bot)
            {
                spriteBatch.Draw(aiTexture, b.ScreenPosition, Color.White );
            }
            //drawe player
            spriteBatch.Draw(playerTexture, player.ScreenPosition, Color.White * 0.3f);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGrid()
        {
            //draws the map grid
            int sz = level.gridX;

            for (int x = 0; x < level.gridX; x++)
            {
                for (int y = 0; y < level.gridY; y++)
                {
                    Coord2 pos = new Coord2((x * 15), (y * 15));
                    Coord2 posOrig = new Coord2((x), (y));
                    if (level.tiles[x, y] == 0) spriteBatch.Draw(tile1Texture, pos, Color.White);
                    else spriteBatch.Draw(tile2Texture, pos, Color.White);
                    //foreach (AiBotBase b in bot)
                    //{
                    //    foreach (node n in b.open)
                    //    {
                    //        if (n.pos == posOrig)
                    //        {
                    //            spriteBatch.Draw(open_tex, pos, Color.White * 0.5f);
                    //        }
                    //    }
                    //}
                    //foreach (AiBotBase b in bot)
                    //{
                    //    foreach (node n in b.closed)
                    //    {
                    //        if (n.pos == posOrig)
                    //        {
                    //            spriteBatch.Draw(closed_tex, pos, Color.White * 0.5f);
                    //        }
                    //    }
                    //}
                    //foreach (AiBotBase b in bot)
                    //{
                    //    foreach (Coord2 n in b.path)
                    //    {
                    //        if (n == posOrig)
                    //        {
                    //            spriteBatch.Draw(path_tex, pos, Color.White * 0.5f);
                    //        }
                    //    }

                    //}
                }
            }
        }
    }
}
