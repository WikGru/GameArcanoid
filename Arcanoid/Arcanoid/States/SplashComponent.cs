using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcanoid.States
{
    class SplashComponent : StateTemplate
    {
        Texture2D splash;
        //keep on screen before fadeout
        int i = 0;
        //fadeout 
        int alpha = 0;

        public SplashComponent()
        {
            splash = Globals.contentManager.Load<Texture2D>("splash");
        }

        public override void Update(GameTime gameTime)
        {
            if(alpha >= 255)
            {
                Globals.currentState = Globals.EnStates.MENU;
            }
            if (i++ >= 40) alpha += 3;
            Draw();
        }

        public override void Draw()
        {
            Globals.spriteBatch.Begin();
            Globals.spriteBatch.Draw(splash, new Rectangle(100, 50, 400, 500), Color.White);
            Globals.spriteBatch.Draw(splash, new Rectangle(0, 0, 600, 600), new Color(Color.Black,alpha));
            Globals.spriteBatch.End();
        }

    }
}
