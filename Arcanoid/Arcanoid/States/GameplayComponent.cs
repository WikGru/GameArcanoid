using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Arcanoid.States
{
    class GameplayComponent : StateTemplate
    {
        string[] strs;

        Random rand = new Random();

        //VARIABLES TO SET ON EACH GAME
        private int lvlNumber;
        private bool isBallGlued;
        private bool isLoaded = false;
        private bool isGameOver;
        private bool isGameWin;


        Keys keyRight = Keys.Right;
        Keys keyLeft = Keys.Left;

        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;

        //LIFES
        private int lifes;
        //SCORE
        private int score;
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
        Texture2D ballTexture;
        Vector2 ballPrecisePosition;
        //BLOCK
        List<Tile> tileList = new List<Tile>();
        Texture2D tileTexture;
        //GAMESPACE
        Texture2D backgroundTexture;
        Texture2D boundsTexture;
        Rectangle gameSpace;
        Rectangle gameSpaceLeft;
        Rectangle gameSpaceTop;
        Rectangle gameSpaceRight;
        Rectangle gameSpaceBottom;
        //SOUNDS
        Song levelStart;
        SoundEffect tileDestroyed;
        SoundEffect objectBounce;
        SoundEffect lostLife;
        SoundEffect powerUp;
        SoundEffect powerDown;
        //POWER UPS
        PowerUp powerUpTemplate = new PowerUp(new Rectangle(1, 1, 1, 1));
        Texture2D powerUpTexture;
        List<PowerUp> powerUpList = new List<PowerUp>();


        float paddleHit = 0;

        private bool CheckKey(Keys theKey)
        {
            return keyboardState.IsKeyUp(theKey) &&
                oldKeyboardState.IsKeyDown(theKey);
        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            if (CheckKey(Keys.Escape))
            {
                Globals.currentState = Globals.EnStates.MENU; //TESTING ONLY finish level on TAB
                isLoaded = false;
            }
            if (CheckKey(Keys.Tab)) tileList.Clear(); ; //TESTING ONLY finish level on TAB

            if (!isLoaded) LoadContent();                                               // on first encounter load content
            if (CheckKey(Keys.Space) && ball.Direction.Y == 0) ReleaseBall();                                     //release ball from paddle
            if (isBallGlued)
            {
                ball.Direction = new Vector2(ball.Direction.X, 0);
                ballPrecisePosition.X = paddleBounds.Center.X - ball.Size / 2;    //glue ball to paddle
                ballPrecisePosition.Y = Globals.graphics.PreferredBackBufferHeight - 40 - ball.Size;
            }

            if (lifes < 0) GameOver(); //if lost all lives set gameover
            if (lvlNumber == 99) GameWin();
            //if only indestructible blocks are left => finish level
            if (tileList.All(x => x.State == 3)) LoadLevel();

            // ta petle zostaw na razie bo tu ma byc liczone z kwantu czasu coś jeszcze nwm jak xD
            for (int i = 0; i < Globals.physicsIterations; i++)
            {
                MoveBall(Globals.physicsIterations);
                DetectCollision();
            }
            MovePaddle();


            if (isGameOver)
            {
                if (CheckKey(Keys.Space))
                {
                    Globals.currentState = Globals.EnStates.MENU;
                    isLoaded = false;
                    isGameOver = false;
                }
            }
            else if (isGameWin)
            {
                if (CheckKey(Keys.Space))
                {
                    Globals.currentState = Globals.EnStates.MENU;
                    isLoaded = false;
                    isGameWin = false;
                }
            }
            else
            {
                //if game goes on, then continue drawing
                Draw();
            }

            oldKeyboardState = keyboardState;
        }

        public override void Draw()
        {
            Globals.spriteBatch.Begin();
            //BACKGROUND
            Globals.spriteBatch.Draw(tileTexture, new Rectangle(0, 0, Globals.graphics.PreferredBackBufferWidth, Globals.graphics.PreferredBackBufferHeight), Color.Black);
            Globals.spriteBatch.Draw(backgroundTexture, new Rectangle(20, 30, gameSpace.Width + 20, gameSpace.Height), Color.White);
            Globals.spriteBatch.Draw(boundsTexture, new Rectangle(10, 20, 380, 580), Color.White);
            //TILES
            foreach (Tile block in tileList)
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
            //POWERUPS
            foreach (PowerUp power in powerUpList)
            {
                Globals.spriteBatch.Draw(powerUpTexture, new Rectangle(power.Bounds.X, power.PositionY, 24, 12), power.Colour[(int)power.Powerup]);
                power.PositionY += 3;
            }
            //LIFES
            for (int life = 1; life <= lifes; life++)
            {
                Globals.spriteBatch.Draw(paddleTexture, new Rectangle(life * 35, Globals.graphics.PreferredBackBufferHeight - 15, 24, 5), Color.White);
            }
            //SCORE
            Globals.spriteBatch.DrawString(Globals.spriteFontScore, "SCORE", new Vector2(gameSpace.Right + 55, 90), Color.White);
            Globals.spriteBatch.DrawString(Globals.spriteFontScore, score.ToString(), new Vector2(gameSpace.Right + 60, 120), Color.White);

            //POWERUPS GUIDE
            Globals.spriteBatch.Draw(powerUpTexture, new Rectangle(new Point(gameSpace.Right + 30, 300), new Point(24, 12)), Color.Lime);
            Globals.spriteBatch.DrawString(Globals.spriteFontScoreSmall, "bigger paddle", new Vector2(gameSpace.Right + 60, 300-6), Color.White);

            Globals.spriteBatch.Draw(powerUpTexture, new Rectangle(new Point(gameSpace.Right + 30, 330), new Point(24, 12)), Color.Red);
            Globals.spriteBatch.DrawString(Globals.spriteFontScoreSmall, "smaller paddle", new Vector2(gameSpace.Right + 60, 330-6), Color.White);

            Globals.spriteBatch.Draw(powerUpTexture, new Rectangle(new Point(gameSpace.Right + 30, 360), new Point(24, 12)), Color.Magenta);
            Globals.spriteBatch.DrawString(Globals.spriteFontScoreSmall, "life up", new Vector2(gameSpace.Right + 60, 360-6), Color.White);

            Globals.spriteBatch.Draw(powerUpTexture, new Rectangle(new Point(gameSpace.Right + 30, 390), new Point(24, 12)), Color.Cyan);
            Globals.spriteBatch.DrawString(Globals.spriteFontScoreSmall, "invert controls", new Vector2(gameSpace.Right + 60, 390-6), Color.White);

            Globals.spriteBatch.Draw(powerUpTexture, new Rectangle(new Point(gameSpace.Right + 30, 420), new Point(24, 12)), Color.Gold);
            Globals.spriteBatch.DrawString(Globals.spriteFontScoreSmall, "bonus points", new Vector2(gameSpace.Right + 60, 420-6), Color.White);

            Globals.spriteBatch.End();
        }

        public void ReleaseBall()
        {
            isBallGlued = false;
            ball.Direction = new Vector2(ball.Direction.X, 1);
        }

        #region ContentLoadMethods
        private void LoadVariables()
        {
            keyRight = Keys.Right;
            keyLeft = Keys.Left;
            powerUpList.Clear();
            score = 0;
            lifes = 3;
            lvlNumber = 0;
        }
        public void LoadGameSpace()
        {
            //Pole Gry
            gameSpace = new Rectangle(30, 40, 340, 580);
            gameSpaceLeft = new Rectangle(gameSpace.Left, gameSpace.Top, 0, gameSpace.Height);
            gameSpaceTop = new Rectangle(gameSpace.Left, gameSpace.Top, gameSpace.Width, 0);
            gameSpaceRight = new Rectangle(gameSpace.Right, gameSpace.Top, 0, gameSpace.Height);
            gameSpaceBottom = new Rectangle(gameSpace.Left, gameSpace.Bottom, gameSpace.Width, 0);
        }
        public void LoadDynamics()
        {
            //paddle
            paddle = new Paddle(72, 6, (gameSpace.Width / 2));
            paddleBounds = new Rectangle(paddle.PositionX, Globals.graphics.PreferredBackBufferHeight - 40, paddle.SizeX, 15);

            //ball
            ball = new Ball(2.4f, 12, new Vector2(0, 0), paddleBounds.Center.X, paddleBounds.Top - 12);
            ballBounds = new Rectangle(ball.PositionX, ball.PositionY, ball.Size, ball.Size);
            ballPrecisePosition = new Vector2(ball.PositionX, ball.PositionY);
        }
        public void LoadTextures()
        {
            backgroundTexture = Globals.contentManager.Load<Texture2D>("background");
            boundsTexture = Globals.contentManager.Load<Texture2D>("bounds");
            paddleTexture = Globals.contentManager.Load<Texture2D>("paddle");
            ballTexture = Globals.contentManager.Load<Texture2D>("ball");
            tileTexture = Globals.contentManager.Load<Texture2D>("block");
            powerUpTexture = Globals.contentManager.Load<Texture2D>("powerUp");
        }
        private void LoadSounds()
        {
            // dzwieki
            MediaPlayer.IsRepeating = false;
            levelStart = Globals.contentManager.Load<Song>("levelStart");
            tileDestroyed = Globals.contentManager.Load<SoundEffect>("tileDestroyed");
            objectBounce = Globals.contentManager.Load<SoundEffect>("padBounce");
            lostLife = Globals.contentManager.Load<SoundEffect>("lostLife");
            powerUp = Globals.contentManager.Load<SoundEffect>("powerUpSound");
            powerDown = Globals.contentManager.Load<SoundEffect>("powerDownSound");
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
                        Globals.tileMesh[i, j] = Convert.ToInt32(strs[j]);
                    }
                    if (i++ == 21) break;
                } while (!reader.EndOfStream);
            }

            for (int i = 0; i < 22; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (Globals.tileMesh[i, j] != 0)
                    {
                        tileList.Add(new Tile(Globals.tileMesh[i, j], i, j));
                    }
                }
            }
        }
        private void LoadLevel()
        {
            tileList.Clear();
            MediaPlayer.Play(levelStart);
            //ball.Velocity = Globals.physicsIterations;
            keyRight = Keys.Right;
            keyLeft = Keys.Left;
            paddle.SizeX = 72;
            powerUpList.Clear();
            ball.Direction *= new Vector2(1, 0);
            ball.PositionY = paddleBounds.Top - ball.Size;
            lvlNumber++;
            isBallGlued = true;
            if (lvlNumber > Globals.maxLvl) lvlNumber = 99;
            LoadFromFile("Content/lvl" + lvlNumber.ToString() + ".txt");
        }
        public void LoadContent()
        {
            LoadVariables();        //set variables to default
            LoadGameSpace();        //gameBounds, background
            LoadDynamics();         //pad, ball
            LoadTextures();         // textures
            LoadSounds();
            LoadLevel();            //load tiles (level)

            isLoaded = !isLoaded;   //flaga do loadContent
        }
        #endregion

        #region CollisionMethods
        public void GameSpaceCollision()
        {
            if (ball.PositionX <= gameSpace.Left)
            {
                if (ball.Direction.X < 0) ball.Direction *= new Vector2(-1, 1);
            }
            else if (ball.PositionX + ball.Size >= gameSpace.Right)
            {
                if (ball.Direction.X > 0) ball.Direction *= new Vector2(-1, 1);
            }
            else if (ball.PositionY <= gameSpace.Top)
            {
                if (ball.Direction.Y < 0) ball.Direction *= new Vector2(1, -1);
            }
            else if (ball.PositionY + ball.Size >= gameSpace.Bottom)
            {
                ball.Direction *= new Vector2(1, -1);
                lostLife.Play(0.5f, 0, 0);
                isBallGlued = true;
                keyRight = Keys.Right;
                keyLeft = Keys.Left;
                powerUpList.Clear();
                paddle.SizeX = 72;
                LoseLife();
            }
        }
        public void PaddleCollision()
        {
            //POWER UP
            foreach (PowerUp power in powerUpList)
            {
                if (paddleBounds.Intersects(new Rectangle(power.Bounds.X, power.PositionY, 24, 12)))
                {
                    GivePowerUp(power);
                    powerUpList.Remove(power);
                    break;
                }
            }
            //BALL
            if (ball.Direction.Y > 0)
            {
                if (ballBounds.Intersects(paddleBounds))
                {
                    ball.Velocity += 0.02f;
                    if (ball.Velocity > 4.2f) ball.Velocity = 4.2f;
                    paddleHit = (ballBounds.Center.X - paddleBounds.Center.X) / ((paddle.SizeX / 2 + ball.Size) * 0.9f);
                    ball.Direction = new Vector2((float)Math.Sin(paddleHit) * ball.Velocity, -(float)Math.Cos(paddleHit) * ball.Velocity);
                }
            }

        }
        public void TileCollision()
        {
            foreach (Tile tile in tileList)
            {
                if (ballBounds.Intersects(tile.Bounds))
                {
                    if (ballBounds.Intersects(tile.Top) && ball.Direction.Y > 0)
                    {
                        if (tile.State != 3) tile.State--;
                        ball.Direction *= new Vector2(1, -1);
                    }
                    else if (ballBounds.Intersects(tile.Bottom) && ball.Direction.Y < 0)
                    {
                        if (tile.State != 3) tile.State--;
                        ball.Direction *= new Vector2(1, -1);
                    }
                    else if (ballBounds.Intersects(tile.Left) && ball.Direction.X > 0)
                    {
                        if (tile.State != 3) tile.State--;
                        ball.Direction *= new Vector2(-1, 1);
                    }
                    else if (ballBounds.Intersects(tile.Right) && ball.Direction.X < 0)
                    {
                        if (tile.State != 3) tile.State--;
                        ball.Direction *= new Vector2(-1, 1);
                    }

                    if (tile.State == 0)
                    {
                        if (rand.Next(1, 8) == 1)
                        {
                            powerUpList.Add(new PowerUp(tile.Bounds));
                        }
                        tileDestroyed.Play(0.3f, 0, 0);
                        AddPoints(10);
                        tileList.Remove(tile);
                    }
                    else if (tile.State < 3)
                    {
                        objectBounce.Play(0.3f, 0, 0);
                        AddPoints(2);
                    }
                    else
                    {
                        objectBounce.Play(0.3f, 0, 0);
                    }
                    break;
                }
            }
        }
        public void DetectCollision()
        {
            GameSpaceCollision();
            TileCollision();
            PaddleCollision();
        }
        #endregion

        #region ObjectMovingMethods
        public void MoveBall(float multiplier)
        {
            ballPrecisePosition.X += (ball.Velocity * ball.Direction.X) / multiplier;
            ballPrecisePosition.Y += (ball.Velocity * ball.Direction.Y) / multiplier;

            ball.PositionX = (int)ballPrecisePosition.X;
            ball.PositionY = (int)ballPrecisePosition.Y;

            ballBounds = new Rectangle(ball.PositionX, ball.PositionY, ball.Size, ball.Size);
        }
        public void MovePaddle()
        {
            if (Keyboard.GetState().IsKeyDown(keyLeft))
            {
                paddle.PositionX -= paddle.Velocity;
                if (paddle.PositionX <= gameSpace.Location.X + 5)
                {
                    paddle.PositionX = 5 + gameSpace.Location.X;
                }
            }
            if (Keyboard.GetState().IsKeyDown(keyRight))
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

        #region LifesMethods
        public void LoseLife()
        {
            lifes--;
            ball.Velocity = 2.4f;
        }
        public void AddLife()
        {
            if (++lifes > 9)
            {
                lifes = 9;
                score += 300;
            }
        }
        public void GameOver()
        {
            Vector2 size = Globals.spriteFontScore.MeasureString("Game Over");
            Globals.spriteBatch.Begin();
            Globals.spriteBatch.DrawString(Globals.spriteFontScore, "Game Over", new Vector2(gameSpace.Center.X - (size.X / 2), 400), Color.White);
            Globals.spriteBatch.End();
            isGameOver = true;
        }
        public void GameWin()
        {
            Vector2 size = Globals.spriteFontScore.MeasureString("You Win!");
            Globals.spriteBatch.Begin();
            Globals.spriteBatch.DrawString(Globals.spriteFontScore, "You Win!", new Vector2(gameSpace.Center.X - (size.X / 2), 400), Color.White);
            Globals.spriteBatch.End();
            isGameWin = true;
        }
        #endregion

        #region ScoreMethods
        public void AddPoints(int points)
        {
            score += points;
        }
        #endregion

        #region PowerUpMethods
        public void GivePowerUp(PowerUp power)
        {
            switch (power.Powerup)
            {
                case Globals.enPowerUpType.PADDLE_PLUS:
                    AddPoints(50);
                    powerUp.Play(0.5f, 0, 0);
                    paddle.SizeX += 25;
                    break;
                case Globals.enPowerUpType.PADDLE_MINUS:
                    AddPoints(100);
                    powerDown.Play(0.5f, 0, 0);
                    paddle.SizeX -= 35;
                    if (paddle.SizeX <= 20) paddle.SizeX = 25;
                    break;
                case Globals.enPowerUpType.LIFE:
                    AddPoints(50);
                    powerUp.Play(0.5f, 0, 0);
                    AddLife();
                    break;
                case Globals.enPowerUpType.BONUS_POINTS_BIG:
                    AddPoints(800);
                    break;
                case Globals.enPowerUpType.INVERT_CONTROLS:
                    AddPoints(100);
                    InverControls();
                    powerDown.Play(0.5f, 0, 0);
                    break;
            }
        }

        public void InverControls()
        {
            keyRight = Keys.Left;
            keyLeft = Keys.Right;
        }
        #endregion
    }
}
