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

        public Block(int state, int row, int column)
        {
            this.state = state;
            this.row = row;
            this.column = column;
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
    }
}
