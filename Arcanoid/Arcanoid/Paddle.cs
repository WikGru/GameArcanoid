using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcanoid
{
    class Paddle
    {
        private int sizeX;
        private int velocity;
        private int positionX;

        public Paddle(int sizeX, int velocity, int positionX)
        {
            this.sizeX = sizeX;
            this.velocity = velocity;
            this.positionX = positionX;
        }

        public int SizeX
        {
            get
            {
                return sizeX;
            }

            set
            {
                sizeX = value;
            }
        }


        public int Velocity
        {
            get
            {
                return velocity;
            }

            set
            {
                velocity = value;
            }
        }


        public int PositionX
        {
            get
            {
                return positionX;
            }

            set
            {
                positionX = value;
            }
        }
    }
}
