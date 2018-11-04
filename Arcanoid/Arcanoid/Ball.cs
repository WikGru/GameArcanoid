using Microsoft.Xna.Framework;

namespace Arcanoid
{
    class Ball
    {
        private float velocity;
        private int size;
        private Vector2 direction;
        private int positionX;
        private int positionY;

        public Ball(float velocity, int size, Vector2 direction, int positionX, int positionY)
        {
            this.velocity = velocity;
            this.size = size;
            this.direction = direction;
            this.positionX = positionX;
            this.positionY = positionY;
        }

        public float Velocity { get { return velocity; } set { velocity = value; } }
        public int Size { get { return size; } set { size = value; } }
        public Vector2 Direction { get { return direction; } set { direction = value; } }
        public int PositionX { get { return positionX; } set { positionX = value; } }
        public int PositionY { get { return positionY; } set { positionY = value; } }
    }
}
