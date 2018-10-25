﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Arcanoid
{
    static public class Globals
    {
        public static bool exit = false;

        public static SpriteFont spriteFont = null;
        public static SpriteBatch spriteBatch = null;
        public static ContentManager contentManager = null;
        public static GraphicsDeviceManager graphics = null;

        public static EnStates currentState;
        public enum EnStates
        {
            SPLASH,
            MENU,
            START,
            EXIT
        }


        public static int[,] BlockMesh = new int[11, 11];
        public static Vector2 firstBlock = new Vector2(30, 60);
        public static Vector2 blockSize = new Vector2(30, 15);

    }
}
