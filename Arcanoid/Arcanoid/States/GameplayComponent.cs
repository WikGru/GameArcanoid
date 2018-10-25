﻿using System;
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

        //PADDLE
        Paddle paddle;
        Rectangle paddleBounds;
        Texture2D paddleTexture;
        //PADDLE SECTIONS
        Rectangle paddleSectionLeft;
        Rectangle paddleSectionCenter;
        Rectangle paddleSectionRight;
        //BALL
        Ball ball;
        Rectangle ballBounds;
        Vector2 ballBoundsPrecise;
        Texture2D ballTexture;
        //BLOCK
        List<Block> blockList = new List<Block>();
        Block block;
        Texture2D blockTexture;
        //GAMESPACE
        Texture2D backgroundTexture;
        Texture2D boundsTexture;
        Rectangle gameSpace;
        Rectangle gameSpaceLeft;
        Rectangle gameSpaceTop;
        Rectangle gameSpaceRight;

        public void LoadContent()
        {
            LoadGameSpace();
            LoadDynamics();
            LoadTextures();

            //flaga do loadContent
            isLoaded = !isLoaded;
        }

        public override void Update(GameTime gameTime)
        {
            if (!isLoaded) LoadContent();

            // ta petle zostaw na razie bo tu ma byc liczone z kwantu czasu coś jeszcze nwm jak xD
            for (int i = 0; i < 2; i++)
            {
                MoveBall(0.5f);
                GameSpaceCollision();
                BlocksCollision();
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

            //BLOCK TEST
            //Globals.spriteBatch.Draw(blockTexture, blockBounds, block.Color);

            foreach (Block block in blockList)
            {
                if (block.State != 0)
                {
                    Globals.spriteBatch.Draw(blockTexture, block.Bounds, block.Colour[block.State]);
                }

                //Wizualizacja rysowanych bloków
                //Globals.spriteBatch.Draw(blockTexture, block.Bottom, Color.Magenta);
                //Globals.spriteBatch.Draw(blockTexture, block.Left, Color.Magenta);
                //Globals.spriteBatch.Draw(blockTexture, block.Right, Color.Magenta);
                //Globals.spriteBatch.Draw(blockTexture, block.Top, Color.Magenta);
            }
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
            if (ballBounds.Intersects(gameSpaceLeft))
            {
                if (ball.DirectionX < 0) ball.DirectionX *= -1;

            }
            else if (ballBounds.Intersects(gameSpaceRight))
            {
                if (ball.DirectionX > 0) ball.DirectionX *= -1;
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

        public void BlocksCollision()
        {
            foreach (Block block in blockList)
            {
                if (ballBounds.Intersects(block.Bounds))
                {
                    if (ballBounds.Intersects(block.Top))
                    {
                        if (block.State != 0)
                        {
                            if (block.State != 3)
                            {
                                block.State--;
                            }
                            ball.DirectionY *= -1;
                        }
                    }
                    else if (ballBounds.Intersects(block.Bottom))
                    {
                        if (block.State != 0)
                        {
                            if (block.State != 3)
                            {
                                block.State--;
                            }
                            ball.DirectionY *= -1;
                        }
                    }
                    else if (ballBounds.Intersects(block.Left))
                    {
                        if (block.State != 0)
                        {
                            if (block.State != 3)
                            {
                                block.State--;
                            }
                            ball.DirectionX *= -1;
                        }
                    }
                    else if (ballBounds.Intersects(block.Right))
                    {
                        if (block.State != 0)
                        {
                            if (block.State != 3)
                            {
                                block.State--;
                            }
                            ball.DirectionX *= -1;
                        }
                    }
                    if (block.State == 0) blockList.Remove(block);
                    break;
                }
            }
        }

        public void MoveBall(float multiplier)
        {
            ballBoundsPrecise = new Vector2(ball.Velocity * ball.DirectionX * multiplier, ball.Velocity * ball.DirectionY * multiplier);
            ball.PositionX += (int)ballBoundsPrecise.X;
            ball.PositionY += (int)ballBoundsPrecise.Y;

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

        public void LoadGameSpace()
        {
            //Pole Gry
            gameSpace = new Rectangle(30, 40, 340, 580);
            gameSpaceLeft = new Rectangle(gameSpace.Left, gameSpace.Top, 0, gameSpace.Height);
            gameSpaceTop = new Rectangle(gameSpace.Left, gameSpace.Top, gameSpace.Width, 0);
            gameSpaceRight = new Rectangle(gameSpace.Right, gameSpace.Top, 0, gameSpace.Height);
        }

        public void LoadDynamics()
        {
            //paddle
            paddle = new Paddle(200/*72*/, 4, (gameSpace.Width / 2));
            paddleBounds = new Rectangle(paddle.PositionX, Globals.graphics.PreferredBackBufferHeight - 40, paddle.SizeX, 15);

            //ball
            ball = new Ball(5, 20, 0, 1, gameSpace.Center.X - 10, 300);
            ballBounds = new Rectangle(ball.PositionX, ball.PositionY, ball.Size, ball.Size);

            //block test
            block = new Block(1, 1, 1);
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (Globals.BlockMesh[i, j] != 0)
                    {
                        blockList.Add(new Block(Globals.BlockMesh[i, j], i, j));
                    }
                }
            }


            //juz calkiem sensowne stwianie bloku na podstawie obiektu
            //blockBounds = new Rectangle(block.Column * 30, 60 + block.Row * 15, (int)Globals.blockSize.X, (int)Globals.blockSize.Y);
        }

        public void LoadTextures()
        {
            //tekstury 
            backgroundTexture = Globals.contentManager.Load<Texture2D>("background");
            boundsTexture = Globals.contentManager.Load<Texture2D>("bounds");
            paddleTexture = Globals.contentManager.Load<Texture2D>("paddle");
            ballTexture = Globals.contentManager.Load<Texture2D>("ball");

            blockTexture = Globals.contentManager.Load<Texture2D>("block");
        }


    }
}
