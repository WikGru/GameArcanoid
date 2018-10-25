using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Arcanoid.States
{
    class GameplayComponent : StateTemplate
    {
        string[] strs;
        private int lvlNumber = 1;

        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;

        private bool isLoaded = false;
        private bool isStarted = false;
        private bool isLevelRunning = true;

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
        Song winnersong;
        Texture2D backgroundTexture;
        Texture2D boundsTexture;
        Rectangle gameSpace;
        Rectangle gameSpaceLeft;
        Rectangle gameSpaceTop;
        Rectangle gameSpaceRight;

        public void LoadContent()
        {
            LoadGameSpace();    //gameBounds, background
            LoadLevel("Content/lvl1.txt");  //blocks
            LoadDynamics(); //pad, ball
            LoadTextures(); // textures

            //flaga do loadContent
            isLoaded = !isLoaded;
        }

        public override void Update(GameTime gameTime)
        {
            if (!isLoaded) LoadContent(); // on first encounter load content

            keyboardState = Keyboard.GetState();

            if (CheckKey(Keys.Tab)) blockList.Clear(); //TESTING ONLY finish level on TAB

            StartGame(); //on SPACE
            IfLevelCompleted(); // go to next level on SPACE
           
            // ta petle zostaw na razie bo tu ma byc liczone z kwantu czasu coś jeszcze nwm jak xD
            for (int i = 0; i < 2; i++)
            {
                MoveBall(1);
                GameSpaceCollision();
                BlocksCollision();
                if (ball.DirectionY > 0) PaddleCollision();
            }
            MovePaddle();

            oldKeyboardState = keyboardState;

            if (isLevelRunning)
            {
                Draw();
            }
        }

        public override void Draw()
        {
            Globals.spriteBatch.Begin();

            //BACKGROUND
            Globals.spriteBatch.Draw(blockTexture, new Rectangle(0, 0, Globals.graphics.PreferredBackBufferWidth, Globals.graphics.PreferredBackBufferHeight), Color.Black);
            Globals.spriteBatch.Draw(backgroundTexture, new Rectangle(20, 30, gameSpace.Width + 20, gameSpace.Height), Color.White);
            Globals.spriteBatch.Draw(boundsTexture, new Rectangle(10, 20, 380, 580), Color.White);


           

            //BLOCKS
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
                    if (ballBounds.Intersects(block.Top) && ball.DirectionY > 0)
                    {
                        if (block.State != 3) block.State--;
                        ball.DirectionY *= -1;
                    }
                    else if (ballBounds.Intersects(block.Bottom) && ball.DirectionY < 0)
                    {
                        if (block.State != 3) block.State--;
                        ball.DirectionY *= -1;
                    }
                    else if (ballBounds.Intersects(block.Left) && ball.DirectionX > 0)
                    {
                        if (block.State != 3) block.State--;
                        ball.DirectionX *= -1;
                    }
                    else if (ballBounds.Intersects(block.Right) && ball.DirectionX < 0)
                    {
                        if (block.State != 3) block.State--;
                        ball.DirectionX *= -1;
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
                if (paddle.PositionX + paddleBounds.Width >= gameSpace.Right - 5)
                {
                    paddle.PositionX = gameSpace.Right - 5 - paddleBounds.Width;
                }
            }
            paddleBounds = new Rectangle(paddle.PositionX, Globals.graphics.PreferredBackBufferHeight - 40, paddle.SizeX, 10);

            //Aktualizacja pozycji fragmentów paddle
            paddleSectionLeft.Size = new Point(paddleBounds.Width / 8 * 3, 0);
            paddleSectionLeft.Location = new Point(paddleBounds.Left, paddleBounds.Top);

            paddleSectionCenter.Size = new Point(paddleBounds.Width / 8 * 2, 0);
            paddleSectionCenter.Location = new Point(paddleBounds.Left + paddleSectionLeft.Size.X, paddleBounds.Top);

            paddleSectionRight.Size = new Point(paddleBounds.Width / 8 * 3, 0);
            paddleSectionRight.Location = new Point(paddleSectionCenter.Location.X + paddleSectionCenter.Size.X, paddleBounds.Top);
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
            paddle = new Paddle(72, 6, (gameSpace.Width / 2));
            paddleBounds = new Rectangle(paddle.PositionX, Globals.graphics.PreferredBackBufferHeight - 40, paddle.SizeX, 15);

            //ball
            ball = new Ball(3, 12, 0, 0, paddleBounds.Center.X, paddleBounds.Top - 11);
            ballBounds = new Rectangle(ball.PositionX, ball.PositionY, ball.Size, ball.Size);
        }

        public void LoadTextures()
        {
            //tekstury

            backgroundTexture = Globals.contentManager.Load<Texture2D>("background");
            boundsTexture = Globals.contentManager.Load<Texture2D>("bounds");
            paddleTexture = Globals.contentManager.Load<Texture2D>("paddle");
            ballTexture = Globals.contentManager.Load<Texture2D>("ball");

            blockTexture = Globals.contentManager.Load<Texture2D>("block");

            // dzwieki
            winnersong = Globals.contentManager.Load<Song>("applause");
        }

        public void StartGame()
        {
            if (CheckKey(Keys.Space) && isLevelRunning == true) isStarted = true; //release ball from paddle
            if (isStarted && ball.DirectionY == 0) ball.DirectionY = -1; // release part 2
            if (!isStarted) ball.PositionX = paddleBounds.Center.X - ball.Size / 2; //starting position of ball sticked to paddle
        }

        public void IfLevelCompleted()
        {
            if (blockList.All(x => x.State == 3)) //if only indestructible blocks are left => finish level
            {
                
                LevelComplete();
            }
            if (CheckKey(Keys.Space))
            {
                MediaPlayer.Stop();
                isLevelRunning = true;   //enable drawing to simulate to start next level
            }
        }

        private bool CheckKey(Keys theKey)
        {
            return keyboardState.IsKeyUp(theKey) &&
                oldKeyboardState.IsKeyDown(theKey);
        }

        private void LevelComplete()
        {
            MediaPlayer.Play(winnersong);
            ball.DirectionY = 0;
            ball.PositionY = paddleBounds.Center.Y - 15;
            isLevelRunning = false;
            isStarted = false;
            lvlNumber++;
            Globals.spriteBatch.Begin();
            Globals.spriteBatch.DrawString(Globals.spriteFontBig, "You did it!", new Vector2(gameSpace.Center.X - 115, gameSpace.Center.Y - 100), Color.White);
            Globals.spriteBatch.DrawString(Globals.spriteFontSmall, "Go to the next LVL", new Vector2(gameSpace.Center.X - 125, gameSpace.Center.Y - 50), Color.White);
            Globals.spriteBatch.DrawString(Globals.spriteFontSmall, "Press Space", new Vector2(gameSpace.Center.X - 125, gameSpace.Center.Y - 20), Color.White);
            Globals.spriteBatch.End();
            LoadLevel("Content/lvl" + lvlNumber.ToString() + ".txt");
        }

        private void LoadLevel(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                int i = 0;
                do
                {
                    strs = reader.ReadLine().Split(',');
                    for (int j = 0; j < 11; j++)
                    {
                        Globals.BlockMesh[i, j] = Convert.ToInt32(strs[j]);
                    }
                    if (i++ == 21) break;
                } while (!reader.EndOfStream);
            }

            for (int i = 0; i < 22; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (Globals.BlockMesh[i, j] != 0)
                    {
                        blockList.Add(new Block(Globals.BlockMesh[i, j], i, j));
                    }
                }
            }
        }
    }
}
