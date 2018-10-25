using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcanoid
{
    class Block
    {
        //0 - non existant
        //1 - orange - one hit to destroy
        //2 - red - two hits to destroy
        //3 - gray - indestructible
        private int state;
        private int row;
        private int column;
        private Color color;
        private Rectangle bounds;

        public Block(int state, int row, int column)
        {
            this.state = state;
            this.row = row + 1;
            this.column = column + 1;
            bounds = new Rectangle(column * 30+35, 60 + row * 15, (int)Globals.blockSize.X, (int)Globals.blockSize.Y);
            switch (state)
            {
                case 1:
                    color = Color.Orange;
                    break;
                case 2:
                    color = Color.Red;
                    break;
                case 3:
                    color = Color.White;
                    break;
            }
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

        public Color Color
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
    }
}
