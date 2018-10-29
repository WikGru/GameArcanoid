using Microsoft.Xna.Framework;
using System;

namespace Arcanoid
{
    class PowerUp
    {
        private Globals.enPowerUpType powerUp;
        Random rand;
        private readonly Color[] color;

        private Rectangle bounds;
        private int positionY;

        public PowerUp(Rectangle tileBounds)
        {
            color = new Color[5] { Color.Lime, Color.Red, Color.Magenta, Color.Cyan, Color.Gold };

            rand = new Random();
            powerUp = (Globals.enPowerUpType)rand.Next(0, 5);

            bounds = new Rectangle(tileBounds.Center.X - 12, tileBounds.Center.Y - 6, 24, 12);
            positionY = bounds.Top;
        }

        public Globals.enPowerUpType Powerup
        {
            get { return powerUp; }
        }

        public Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        public Color[] Colour
        {
            get { return color; }
        }

        public int PositionY
        {
            get { return positionY; }
            set { positionY = value; }
        }
    }
}
