using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Arcanoid.States
{
    class GameplayComponent : StateTemplate
    {
        private bool isLoaded = false;

        Paddle paddle;
        //Ball ball;
        Texture2D backgroundTexture;
        Texture2D boundsTexture;
        Texture2D paddleTexture;
        Texture2D ballTexture;
        Rectangle gameSpace = new Rectangle(30, 20, 370, 580);
        Rectangle paddleBounds;

        public void LoadContent()
        {
            paddle = new Paddle(60, 5, (gameSpace.Width/ 2) - 30);
            paddleBounds = new Rectangle(paddle.PositionX, Globals.graphics.PreferredBackBufferHeight - 40, paddle.SizeX, 10);

            backgroundTexture = Globals.contentManager.Load<Texture2D>("background");
            boundsTexture = Globals.contentManager.Load<Texture2D>("bounds");
            paddleTexture = Globals.contentManager.Load<Texture2D>("paddle");
            ballTexture = Globals.contentManager.Load<Texture2D>("ball");
            isLoaded = !isLoaded;
        }

        public override void Update(GameTime gameTime)
        {
            if (!isLoaded) LoadContent();

            MovePaddle();

            Draw();
        }

        public override void Draw()
        {
            Globals.spriteBatch.Begin();
            Globals.spriteBatch.Draw(backgroundTexture, new Rectangle(20, 30, 360, 580), Color.White);
            Globals.spriteBatch.Draw(boundsTexture, new Rectangle(10, 20, 380, 580), Color.White);
            Globals.spriteBatch.Draw(paddleTexture, paddleBounds, Color.White);
            Globals.spriteBatch.End();
        }


        public void MovePaddle()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                paddle.PositionX -= paddle.Velocity;
                if (paddle.PositionX <= gameSpace.Location.X)
                {
                    paddle.PositionX = 5 + gameSpace.Location.X;
                }
                paddleBounds = new Rectangle(paddle.PositionX, Globals.graphics.PreferredBackBufferHeight - 40, paddle.SizeX, 10);

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                paddle.PositionX += paddle.Velocity;
                if (paddle.PositionX + paddleBounds.Width >= gameSpace.Width)
                {
                    paddle.PositionX = gameSpace.Width - 5 - paddleBounds.Width;
                }
                paddleBounds = new Rectangle(paddle.PositionX, Globals.graphics.PreferredBackBufferHeight - 40, paddle.SizeX, 10);

            }


        }
    }
}
