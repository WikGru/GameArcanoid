using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcanoid
{
    class MenuComponent : StateTemplate
    {
        private Game game;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        public MenuComponent(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch =this.spriteBatch;
            spriteFont = this.spriteFont; 
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }

    }
}
