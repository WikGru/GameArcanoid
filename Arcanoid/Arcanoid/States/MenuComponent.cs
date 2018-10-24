using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
        Color highLight = Color.Yellow;

        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;

        public enum enMenuItems
        {
            Play,
            Quit
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

        private void MeasureMenu()
        {
            height = 0;
            width = 0;
            foreach (enMenuItems item in (enMenuItems[])Enum.GetValues(typeof(enMenuItems)))
            {
                Vector2 size = Globals.spriteFont.MeasureString(item.ToString());
                if (size.X > width)
                    width = size.X;
                height += Globals.spriteFont.LineSpacing + 5;
            }
            position = new Vector2(
                (Globals.graphics.PreferredBackBufferWidth - width) / 2,
                (Globals.graphics.PreferredBackBufferHeight - height) / 2);
        }

        private bool CheckKey(Keys theKey)
        {
            return keyboardState.IsKeyUp(theKey) &&
                oldKeyboardState.IsKeyDown(theKey);
        }

        public override void Update(GameTime gameTime)
        {
            if (!isLoaded)
            {
                MeasureMenu();
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

            if (CheckKey(Keys.Enter))
            {
                switch (selectedItem)
                {
                    case enMenuItems.Play:
                        Globals.currentState = Globals.EnStates.START;
                        return;
                    case enMenuItems.Quit:
                        Globals.currentState = Globals.EnStates.EXIT;
                        return;
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
                    Globals.spriteFont,
                    i.ToString(),
                    location,
                    tint);
                location.Y += Globals.spriteFont.LineSpacing + 5;
            }
            Globals.spriteBatch.End();
        }

    }
}
