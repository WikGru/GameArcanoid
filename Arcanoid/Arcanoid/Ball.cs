using Microsoft.Xna.Framework;

namespace Arcanoid
{
    class Ball
    {
        private float velocity;
        private int size;
        private float directionX;
        private float directionY;
        private int positionX;
        private int positionY;

        public Ball(float velocity, int size, float directionX, float directionY, int positionX, int positionY)
        {
            this.velocity = velocity;
            this.size = size;
            this.directionX = directionX;
            this.directionY = directionY;
            this.positionX = positionX;
            this.positionY = positionY;
        }

        public float Velocity { get { return velocity; } set { velocity = value; } }
        public int Size { get { return size; } set { size = value; } }
        public float DirectionX { get { return directionX; } set { directionX = value; } }
        public float DirectionY { get { return directionY; } set { directionY = value; } }
        public int PositionX { get { return positionX; } set { positionX = value; } }
        public int PositionY { get { return positionY; } set { positionY = value; } }
    }
}
