using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arcanoid.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Arcanoid
{
    class Manager
    {
        private GameTime gameTime;
        MenuComponent menu = new MenuComponent();
        SplashComponent splash = new SplashComponent();
        GameplayComponent game = new GameplayComponent();

        public Manager()
        {
            gameTime = new GameTime();
        }

        public void Update()
        {
            switch (Globals.currentState)
            {
                case Globals.EnStates.SPLASH:
                    splash.Update(gameTime);
                    break;
                case Globals.EnStates.MENU:
                    menu.Update(gameTime);
                    break;
                case Globals.EnStates.START:
                    game.Update(gameTime);
                    break;
                case Globals.EnStates.EXIT:
                    Globals.exit = true;
                    break;

            }
        }
    }
}
