using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcanoid
{
    class Tile
    {
        //0 - non existant
        //1 - orange - one hit to destroy
        //2 - red - two hits to destroy
        //3 - gray - indestructible
        private int state;
        private int row;
        private int column;
        private readonly Color[] color;
        private Rectangle bounds;
        private Rectangle top;
        private Rectangle bottom;
        private Rectangle left;
        private Rectangle right;


        public Tile(int state, int row, int column)
        {
            this.state = state;
            this.row = row + 1;
            this.column = column + 1;
            bounds = new Rectangle(column * 30 + 35, 60 + row * 15, (int)Globals.blockSize.X, (int)Globals.blockSize.Y);
            color = new Color[4] { Color.Black, Color.Orange, Color.Red, Color.LightGray };

            top = new Rectangle(bounds.Left, bounds.Top, bounds.Width, 3);
            bottom = new Rectangle(bounds.Left, bounds.Bottom, bounds.Width, 3);
            left = new Rectangle(bounds.Left, bounds.Top, 3, bounds.Height);
            right = new Rectangle(bounds.Right, bounds.Top, 3, bounds.Height);
        }

        public int State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public int Row
        {
            get
            {
                return row;
            }
        }

        public int Column
        {
            get
            {
                return column;
            }
        }

        public Color[] Colour
        {
            get
            {
                return color;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }

        public Rectangle Top { get { return top; } set { top = value; } }
        public Rectangle Bottom { get { return bottom; } set { bottom = value; } }
        public Rectangle Left { get { return left; } set { left = value; } }
        public Rectangle Right { get { return right; } set { right = value; } }
    }
}
