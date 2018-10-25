using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Arcanoid
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        Manager manager;
        Random rand = new Random();
        public Game1()
        {
            Globals.currentState = Globals.EnStates.SPLASH;
            Globals.contentManager = Content;
            Globals.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Globals.graphics.PreferredBackBufferWidth = 600;  // set this value to the desired width of your window
            Globals.graphics.PreferredBackBufferHeight = 600;   // set this value to the desired height of your window
            Globals.graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Globals.spriteFontBig = Content.Load<SpriteFont>("font");
            Globals.spriteFontSmall = Content.Load<SpriteFont>("Font1");
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);
            //for(int i = 0; i < 22; i++)
            //{
            //    for(int j = 0; j < 11; j++)
            //    {
            //        Globals.BlockMesh[i, j] = (rand.Next() % 4);
            //    }
            //}
            manager = new Manager();
            //spriteFont = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Globals.exit) Exit();
            manager.Update();
            base.Update(gameTime);
        }
    }
}
