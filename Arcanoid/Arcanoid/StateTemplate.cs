using Microsoft.Xna.Framework;
namespace Arcanoid
{
    abstract class StateTemplate
    {
        abstract public void Update(GameTime gameTime);
        abstract public void Draw();
    }
}
