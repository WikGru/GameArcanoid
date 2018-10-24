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
        Rectangle paddleBounds;
        Rectangle paddleSectionLeft;
        Rectangle paddleSectionCenter;
        Rectangle paddleSectionRight;


        Ball ball;
        Rectangle ballBounds;

        Texture2D backgroundTexture;
        Texture2D boundsTexture;
        Texture2D paddleTexture;
        Texture2D ballTexture;
        Rectangle gameSpace;
        Rectangle gameSpaceLeft;
        Rectangle gameSpaceTop;
        Rectangle gameSpaceRight;

        public void LoadContent()
        {
            gameSpace = new Rectangle(30, 40, 340, 580);
            gameSpaceLeft = new Rectangle(gameSpace.Left, gameSpace.Top, 0, gameSpace.Height);
            gameSpaceTop = new Rectangle(gameSpace.Left, gameSpace.Top, gameSpace.Width, 0);
            gameSpaceRight = new Rectangle(gameSpace.Right, gameSpace.Top, 0, gameSpace.Height);

            paddle = new Paddle(60, 3, (gameSpace.Width / 2));
            paddleBounds = new Rectangle(paddle.PositionX, Globals.graphics.PreferredBackBufferHeight - 40, paddle.SizeX, 10);

           
            ball = new Ball(4, 15, 0, 1, gameSpace.Center.X - 10, 300);
            ballBounds = new Rectangle(ball.PositionX, ball.PositionY, ball.Size, ball.Size);

            backgroundTexture = Globals.contentManager.Load<Texture2D>("background");
            boundsTexture = Globals.contentManager.Load<Texture2D>("bounds");
            paddleTexture = Globals.contentManager.Load<Texture2D>("paddle");
            ballTexture = Globals.contentManager.Load<Texture2D>("ball");
            isLoaded = !isLoaded;
        }

        public override void Update(GameTime gameTime)
        {
            if (!isLoaded) LoadContent();


            for (int i = 0; i < 2; i++)
            {
                MoveBall(0.5f);
                ballBounds = new Rectangle(ball.PositionX, ball.PositionY, ball.Size, ball.Size);
                GameSpaceCollision();
                PaddleCollision();
            }
            MovePaddle();

            paddleBounds = new Rectangle(paddle.PositionX, Globals.graphics.PreferredBackBufferHeight - 40, paddle.SizeX, 10);
            
            //Aktualizacja pozycji fragmentów paddle
            paddleSectionLeft.Size = new Point(20, 0);
            paddleSectionLeft.Location = new Point(paddleBounds.Left, paddleBounds.Top);

            paddleSectionCenter.Size = new Point(20, 0);
            paddleSectionCenter.Location = new Point(paddleBounds.Left + 20, paddleBounds.Top);

            paddleSectionRight.Size = new Point(20, 0);
            paddleSectionRight.Location = new Point(paddleBounds.Left + 40, paddleBounds.Top);
            
            Draw();

        }

        public override void Draw()
        {
            Globals.spriteBatch.Begin();
            Globals.spriteBatch.Draw(backgroundTexture, new Rectangle(20, 30, 360, 580), Color.White);
            Globals.spriteBatch.Draw(boundsTexture, new Rectangle(10, 20, 380, 580), Color.White);

            //BALL
            Globals.spriteBatch.Draw(ballTexture, ballBounds, Color.White);
            //PADDLE
            Globals.spriteBatch.Draw(paddleTexture, paddleBounds, Color.White);


            Globals.spriteBatch.End();
        }

        public void GameSpaceCollision()
        {
            if (ballBounds.Intersects(gameSpaceLeft) || ballBounds.Intersects(gameSpaceRight))
            {
                ball.DirectionX *= -1;
            }else if (ballBounds.Intersects(gameSpaceTop))
            {
                ball.DirectionY *= -1;
            }
        }

        public void PaddleCollision()
        {
            if (ballBounds.Intersects(paddleSectionLeft))
            {
                ball.DirectionX -= 0.5f;
                ball.DirectionY *= -1;
            }
            else if (ballBounds.Intersects(paddleSectionCenter))
            {
                ball.DirectionY *= -1;
            }
            else if (ballBounds.Intersects(paddleSectionRight))
            {
                ball.DirectionX += 0.5f;
                ball.DirectionY *= -1;
            }
        }

        public void MoveBall(float multiplier)
        {
            ball.PositionX += (int)(ball.Velocity * ball.DirectionX * multiplier);
            ball.PositionY += (int)(ball.Velocity * ball.DirectionY * multiplier);
        }

        public void MovePaddle()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                paddle.PositionX -= paddle.Velocity;
                if (paddle.PositionX <= gameSpace.Location.X + 5)
                {
                    paddle.PositionX = 5 + gameSpace.Location.X;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                paddle.PositionX += paddle.Velocity;
                if (paddle.PositionX + paddleBounds.Width/2 >= gameSpace.Width - 5)
                {
                    paddle.PositionX = gameSpace.Width - paddleBounds.Width/2 -5;
                }
            }
        }
    }
}
