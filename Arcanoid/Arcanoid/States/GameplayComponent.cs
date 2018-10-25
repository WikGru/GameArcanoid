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
            //Pole Gry
            gameSpace = new Rectangle(30, 40, 340, 580);
            gameSpaceLeft = new Rectangle(gameSpace.Left, gameSpace.Top, 0, gameSpace.Height);
            gameSpaceTop = new Rectangle(gameSpace.Left, gameSpace.Top, gameSpace.Width, 0);
            gameSpaceRight = new Rectangle(gameSpace.Right, gameSpace.Top, 0, gameSpace.Height);

            //paddle
            paddle = new Paddle(72, 3, (gameSpace.Width / 2));
            paddleBounds = new Rectangle(paddle.PositionX, Globals.graphics.PreferredBackBufferHeight - 40, paddle.SizeX, 15);

            //ball
            ball = new Ball(5, 15, 0, 1, gameSpace.Center.X - 10, 300);
            ballBounds = new Rectangle(ball.PositionX, ball.PositionY, ball.Size, ball.Size);

            //tekstury 
            backgroundTexture = Globals.contentManager.Load<Texture2D>("background");
            boundsTexture = Globals.contentManager.Load<Texture2D>("bounds");
            paddleTexture = Globals.contentManager.Load<Texture2D>("paddle");
            ballTexture = Globals.contentManager.Load<Texture2D>("ball");

            //flaga do loadContent
            isLoaded = !isLoaded;
        }

        public override void Update(GameTime gameTime)
        {
            if (!isLoaded) LoadContent();

            // ta petle zostaw na razie bo tu ma byc liczone z kwantu czasu coś jeszcze nwm jak xD
            for (int i = 0; i < 1; i++)
            {
                MoveBall(1);
                GameSpaceCollision();
                if (ball.DirectionY > 0) PaddleCollision();
            }
            MovePaddle();


            //Aktualizacja pozycji fragmentów paddle
            paddleSectionLeft.Size = new Point(paddleBounds.Width / 3, 0);
            paddleSectionLeft.Location = new Point(paddleBounds.Left, paddleBounds.Top);

            paddleSectionCenter.Size = new Point(paddleBounds.Width / 3, 0);
            paddleSectionCenter.Location = new Point(paddleBounds.Left + paddleBounds.Width / 3, paddleBounds.Top);

            paddleSectionRight.Size = new Point(paddleBounds.Width / 3, 0);
            paddleSectionRight.Location = new Point(paddleBounds.Left + paddleBounds.Width / 3 * 2, paddleBounds.Top);

            Draw();

        }

        public override void Draw()
        {
            Globals.spriteBatch.Begin();

            //BACKGROUND
            Globals.spriteBatch.Draw(backgroundTexture, new Rectangle(20, 30, 360, 580), Color.White);
            Globals.spriteBatch.Draw(boundsTexture, new Rectangle(10, 20, 380, 580), Color.White);

            //BALL
            Globals.spriteBatch.Draw(ballTexture, ballBounds, Color.White);
            //PADDLE
            Globals.spriteBatch.Draw(paddleTexture, paddleBounds, Color.White);

            //Wizualizacja sekcji paddle
            //Globals.spriteBatch.Draw(paddleTexture, paddleSectionLeft, Color.Red);
            //Globals.spriteBatch.Draw(paddleTexture, paddleSectionCenter, Color.Green);
            //Globals.spriteBatch.Draw(paddleTexture, paddleSectionRight, Color.Magenta);

            Globals.spriteBatch.End();
        }

        public void GameSpaceCollision()
        {
            if (ballBounds.Intersects(gameSpaceLeft) || ballBounds.Intersects(gameSpaceRight))
            {
                ball.DirectionX *= -1;
            }
            else if (ballBounds.Intersects(gameSpaceTop))
            {
                ball.DirectionY *= -1;
            }
        }

        public void PaddleCollision()
        {
            if (ballBounds.Intersects(paddleSectionLeft))
            {
                if (ball.DirectionX == 0)
                {
                    ball.DirectionX = -0.5f;
                }
                else if (ball.DirectionX > 0)
                {
                    ball.DirectionX -= 0.25f;
                }
                else
                {
                    ball.DirectionX = -0.5f;
                }
                ball.DirectionY *= -1;
            }
            else if (ballBounds.Intersects(paddleSectionCenter))
            {
                if (ball.DirectionX == 0)
                {
                    ball.DirectionX -= 0.5f;
                }
                ball.DirectionY *= -1;
            }
            else if (ballBounds.Intersects(paddleSectionRight))
            {
                if (ball.DirectionX == 0)
                {
                    ball.DirectionX = 0.5f;
                }
                else if (ball.DirectionX < 0)
                {
                    ball.DirectionX += 0.25f;
                }
                else
                {
                    ball.DirectionX = 0.5f;
                }
                ball.DirectionY *= -1;
            }
        }

        public void MoveBall(float multiplier)
        {
            ball.PositionX += (int)(ball.Velocity * ball.DirectionX * multiplier);
            ball.PositionY += (int)(ball.Velocity * ball.DirectionY * multiplier);

            ballBounds = new Rectangle(ball.PositionX, ball.PositionY, ball.Size, ball.Size);
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
                if (paddle.PositionX + paddleBounds.Width / 2 >= gameSpace.Width - 5)
                {
                    paddle.PositionX = gameSpace.Width - paddleBounds.Width / 2 - 5;
                }
            }
            paddleBounds = new Rectangle(paddle.PositionX, Globals.graphics.PreferredBackBufferHeight - 40, paddle.SizeX, 10);
        }
    }
}
