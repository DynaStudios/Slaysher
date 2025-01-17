﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Slaysher.Game.GUI.Screens;

namespace Slaysher.Game.GUI.Components
{
    public class Label : IGuiItem
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public int ZIndex { get; set; }
        public SoundEffect HoverSound { get; set; }
        public SoundEffect ClickSound { get; set; }

        public string Text { get; set; }
        public Color Color { get; set; }

        public Label(string text)
        {
            Text = text;

            //Defaul Values
            Size = new Vector2(150f,0);
            Color = Color.White;
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
            //Nothing to do here
        }

        public void Draw(GameScreen gameScreen, GameTime gameTime)
        {
            SpriteBatch spriteBatch = gameScreen.ScreenManager.SpriteBatch;
            SpriteFont font = gameScreen.ScreenManager.Font;

            if (Size.X == 150f)
            {
                Size = font.MeasureString(Text);
            }

            spriteBatch.DrawString(font, Text, Position, Color);

        }

        public void HandleInput(InputState input)
        {
            
        }
    }
}