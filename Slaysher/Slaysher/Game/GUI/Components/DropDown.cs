using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slaysher.Game.GUI.Models;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI.Components
{
    public class DropDown : IGuiItem
    {
        public Vector2 Position { get; set; }
        public int ZIndex { get; set; }
        public SoundEffect HoverSound { get; set; }
        public SoundEffect ClickSound { get; set; }

        private Vector2 Size { get; set; }

        //DropDown Properties
        public List<DropDownItem> Items { get; protected set; }
        public DropDownItem SelectedItem { get; protected set; }

        //Internal Properties
        private bool _collapsed;
        private bool _hovered;
        private bool _playedHoverSound;
        private Texture2D _dropDownBackground;

        public DropDown(List<DropDownItem> items, DropDownItem selectedItem)
        {
            Items = items;
            SelectedItem = selectedItem;

            _collapsed = true;
            Size = new Vector2(190, 30);
        }

        public float GetWidth(GameScreen gameScreen)
        {
            return Size.X;
        }

        public float GetHeight(GameScreen gameScreen)
        {
            return Size.Y;
        }

        public void Update(GameScreen gameScreen, GameTime gameTime)
        {
        }

        public void Draw(GameScreen gameScreen, GameTime gameTime)
        {
            SpriteBatch spriteBatch = gameScreen.ScreenManager.SpriteBatch;
            SpriteFont font = gameScreen.ScreenManager.Font;

            if (_dropDownBackground == null)
            {
                _dropDownBackground = gameScreen.ScreenManager.Game.Content.Load<Texture2D>("Images/Game/Menu/btnBg");
            }

            //Draw DropDown InnerBox
            Rectangle rec = new Rectangle((int) Position.X, (int) Position.Y, (int) Size.X, (int) Size.Y);
            spriteBatch.Draw(_dropDownBackground, rec, Color.Gray);

            //Draw the Text centered in the DropDown
            Vector2 textSize = font.MeasureString(SelectedItem.Label);
            Vector2 textPosition = new Vector2(rec.Center.X, rec.Center.Y) - textSize / 2f;
            textPosition.X = (int)textPosition.X;
            textPosition.Y = (int)textPosition.Y;
            spriteBatch.DrawString(font, SelectedItem.Label, textPosition, Color.White);

            //Draw DropDown Content
            if(!_collapsed && Items.Count != 0)
            {
                Vector2 position = Position;
                Vector2 size = font.MeasureString(Items[0].Label);

                //Initial Position
                position.Y += Size.Y;

                for (int i = 0; i < Items.Count; i++)
                {
                    DropDownItem item = Items[i];

                    //Enable Zebra Coloring
                    bool even = (i%2 == 0);

                    item.Draw(gameScreen, position, Size, even);
                    position.Y += size.Y;
                }
            }
        }

        public void HandleInput(InputState input)
        {
            var mousePosition = new Vector2(input.MouseState.X, input.MouseState.Y);

            if (_collapsed)
            {

                if (mousePosition.X >= Position.X && mousePosition.Y >= Position.Y && mousePosition.X <= Position.X + Size.X &&
                    mousePosition.Y <= Position.Y + Size.Y)
                {
                    _hovered = true;

                    if (!_playedHoverSound && HoverSound != null)
                    {
                        HoverSound.Play();
                        _playedHoverSound = true;
                    }

                    if (input.MouseState.LeftButton == ButtonState.Pressed)
                    {
                        _collapsed = false;
                    }
                }
                else
                {
                    _playedHoverSound = false;
                    _hovered = false;
                }
            }
            else
            {
                //Check if Item gets selected
            }
        }

        //DropDownEvents
        public event EventHandler<EventArgs> Clicked;

        public void OnClicked(EventArgs e)
        {
            EventHandler<EventArgs> handler = Clicked;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<EventArgs> SelectionChanged;

        public void OnSelectionChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = SelectionChanged;
            if (handler != null) handler(this, e);
        }

    }
}