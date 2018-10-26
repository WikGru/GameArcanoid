﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;

namespace Arcanoid
{
    class MenuComponent : StateTemplate
    {
        bool isLoaded = false;
        Vector2 position;
        float width = 0f;
        float height = 0f;

        int selectedIndex;
        public static enMenuItems selectedItem;

        Color normal = Color.White;
        Color highLight = new Color(255, 211, 5);
        Song menuSound;
        Texture2D backgroundtexture;

        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;

        public enum enMenuItems
        {
            Play,
            Quit
        }

        private bool CheckKey(Keys theKey)
        {
            return keyboardState.IsKeyUp(theKey) &&
                oldKeyboardState.IsKeyDown(theKey);
        }

        private void PlaySong()
        {
            MediaPlayer.Play(menuSound);
            MediaPlayer.IsRepeating = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (!isLoaded)
            {
                LoadMenu();
                PlaySong();
                isLoaded = true;
            }

            keyboardState = Keyboard.GetState();

            if (CheckKey(Keys.Down))
            {
                selectedIndex++;
                if (selectedIndex == Enum.GetNames(typeof(enMenuItems)).Length)

                    selectedIndex = 0;
            }
            if (CheckKey(Keys.Up))
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = Enum.GetNames(typeof(enMenuItems)).Length - 1;
            }

            if (CheckKey(Keys.Enter) || CheckKey(Keys.Space))
            {
                MediaPlayer.Stop();
                isLoaded = false;
                switch (selectedItem)
                {
                    case enMenuItems.Play:
                        Globals.currentState = Globals.EnStates.START;
                        break;
                    case enMenuItems.Quit:
                        Globals.currentState = Globals.EnStates.EXIT;
                        break ;
                }
            }

            oldKeyboardState = keyboardState;

            Draw();
        }


        public override void Draw()
        {
            Vector2 location = position;
            Color tint;

            Globals.spriteBatch.Begin();
            Globals.spriteBatch.Draw(backgroundtexture, new Rectangle(0, 0, 600, 600), Color.White);

            foreach (enMenuItems i in (enMenuItems[])Enum.GetValues(typeof(enMenuItems)))
            {
                if ((int)i == selectedIndex)
                {
                    tint = highLight;
                    selectedItem = i;
                }
                else
                    tint = normal;
                Globals.spriteBatch.DrawString(
                    Globals.spriteFontBig,
                    i.ToString(),
                    location,
                    tint);
                location.Y += Globals.spriteFontBig.LineSpacing + 5;
            }

            Globals.spriteBatch.End();
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                if (selectedIndex < 0)
                    selectedIndex = 0;
                if (selectedIndex >= Enum.GetNames(typeof(enMenuItems)).Length)
                    selectedIndex = Enum.GetNames(typeof(enMenuItems)).Length - 1;
            }
        }

        private void LoadMenu()
        {
            menuSound = Globals.contentManager.Load<Song>("menu");
            backgroundtexture = Globals.contentManager.Load<Texture2D>("back_menu");
            height = 0;
            width = 0;
            foreach (enMenuItems item in (enMenuItems[])Enum.GetValues(typeof(enMenuItems)))
            {
                Vector2 size = Globals.spriteFontBig.MeasureString(item.ToString());
                if (size.X > width)
                    width = size.X;
                height += Globals.spriteFontBig.LineSpacing + 5;
            }
            position = new Vector2(
                (Globals.graphics.PreferredBackBufferWidth - width) / 2,
                (Globals.graphics.PreferredBackBufferHeight - height) / 2);
        }

    }
}
