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

        private int lvlNumber;
        private int maxLvl;
        private bool endGame;
        private bool isBallGlued;

        private bool isLoaded = false;

        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;

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
        List<Tile> blockList = new List<Tile>();
        Texture2D tileTexture;
        //GAMESPACE
        Song levelStart;
        Texture2D backgroundTexture;
        Texture2D boundsTexture;
        Rectangle gameSpace;
        Rectangle gameSpaceLeft;
        Rectangle gameSpaceTop;
        Rectangle gameSpaceRight;

        

        private bool CheckKey(Keys theKey)
        {
            return keyboardState.IsKeyUp(theKey) &&
                oldKeyboardState.IsKeyDown(theKey);
        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            if (CheckKey(Keys.Tab)) blockList.Clear(); //TESTING ONLY finish level on TAB

            if (!isLoaded) LoadContent();                                               // on first encounter load content
            if (CheckKey(Keys.Space)) ReleaseBall();                                     //release ball from paddle
            if (isBallGlued) ball.PositionX = paddleBounds.Center.X - ball.Size / 2;    //glue ball to paddle

            //if only indestructible blocks are left => finish level
            if (blockList.All(x => x.State == 3)) 
            {
                if (lvlNumber == 99)
                {
                    //finish game => return to menu
                    Globals.currentState = Globals.EnStates.MENU;
                    isLoaded = false;
                }
                else LoadLevel();
            }
            
            // ta petle zostaw na razie bo tu ma byc liczone z kwantu czasu coś jeszcze nwm jak xD
            for (int i = 0; i < 2; i++)
            {
                MoveBall(1);
                DetectCollision();
            }
            MovePaddle();

            oldKeyboardState = keyboardState;

            Draw();
        }

        public override void Draw()
        {
            Globals.spriteBatch.Begin();

            //BACKGROUND
            Globals.spriteBatch.Draw(tileTexture, new Rectangle(0, 0, Globals.graphics.PreferredBackBufferWidth, Globals.graphics.PreferredBackBufferHeight), Color.Black);
            Globals.spriteBatch.Draw(backgroundTexture, new Rectangle(20, 30, gameSpace.Width + 20, gameSpace.Height), Color.White);
            Globals.spriteBatch.Draw(boundsTexture, new Rectangle(10, 20, 380, 580), Color.White);

            //TILES (level)
            foreach (Tile block in blockList)
            {
                if (block.State != 0)
                {
                    Globals.spriteBatch.Draw(tileTexture, block.Bounds, block.Colour[block.State]);
                }
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

        public void ReleaseBall()
        {
            isBallGlued = false;
            ball.DirectionY = -1;
        }

        private void PlayLevelSound()
        {
            MediaPlayer.Play(levelStart);
            MediaPlayer.IsRepeating = false;
        }

        #region ContentLoadMethods
        private void LoadVariables()
        {
            lvlNumber = 1;
            maxLvl = 3;
            endGame = false;
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
            tileTexture = Globals.contentManager.Load<Texture2D>("block");

            // dzwieki
            levelStart = Globals.contentManager.Load<Song>("levelStart");
        }
        private void LoadFromFile(string fileName)
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
                        blockList.Add(new Tile(Globals.BlockMesh[i, j], i, j));
                    }
                }
            }
        }
        private void LoadLevel()
        {
            PlayLevelSound();

            ball.DirectionY = 0;
            ball.PositionY = paddleBounds.Center.Y - 15;

            isBallGlued = true;
            LoadFromFile("Content/lvl" + lvlNumber.ToString() + ".txt");

            if (++lvlNumber > maxLvl) lvlNumber = 99;
        }
        public void LoadContent()
        {
            LoadVariables();        //set variables to default
            LoadGameSpace();        //gameBounds, background
            LoadDynamics();         //pad, ball
            LoadTextures();         // textures
            LoadLevel();            //load tiles (level)

            isLoaded = !isLoaded;   //flaga do loadContent
        }
        #endregion

        #region CollisionMethods
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
            if (ballBounds.Intersects(paddleSectionLeft) && (ball.DirectionY > 0))
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
            foreach (Tile block in blockList)
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
        public void DetectCollision()
        {
            GameSpaceCollision();
            BlocksCollision();
            PaddleCollision();
        }
        #endregion

        #region ObjectMovingMethods
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
        #endregion
    }
}
