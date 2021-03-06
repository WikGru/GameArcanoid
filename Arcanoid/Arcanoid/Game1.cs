﻿using Microsoft.Xna.Framework;
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
            Globals.spriteFontMenu = Content.Load<SpriteFont>("Fonts/fontMenu");
            Globals.spriteFontScore = Content.Load<SpriteFont>("Fonts/fontScore");
            Globals.spriteFontScoreSmall = Content.Load<SpriteFont>("Fonts/fontScoreSmall");
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);
            manager = new Manager();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Globals.exit) Exit();
            manager.Update();
            base.Update(gameTime);
        }
    }
}
