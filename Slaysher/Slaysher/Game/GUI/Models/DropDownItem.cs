using Microsoft.Xna.Framework;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI.Models
{
    public class DropDownItem
    {
        public string Label { get; set; }
        public int Value { get; set; }

        public DropDownItem()
        {
            
        }

        public DropDownItem(int value, string label)
        {
            Label = label;
            Value = value;
        }

        public void Draw(GameScreen gameScreen, Vector2 position, Vector2 dropdownSize, bool odd = true)
        {
            Vector2 labelSize = gameScreen.ScreenManager.Font.MeasureString(Label);
            labelSize.X = dropdownSize.X;

            Rectangle rec = new Rectangle((int) position.X, (int) position.Y, (int) labelSize.X, (int) labelSize.Y);
            //Draw Rectangle
            gameScreen.ScreenManager.SpriteBatch.Draw(gameScreen.ScreenManager.BlankTexture, rec, Color.White);
            //Draw Font
            gameScreen.ScreenManager.SpriteBatch.DrawString(gameScreen.ScreenManager.Font, Label, position, Color.White);
        }
    }
}