﻿using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slaysher.Game.Physics.Collisions;

namespace Slaysher.Game.GUI.Menu
{
    public class MenuComponent : DrawableGameComponent
    {
        public Engine Engine { get; set; }

        public static Color normal = Color.White;
        public static Color highlight = Color.Orange;

        private string[] _menuItems;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;

        private int _selectedMenuItem = -1;

        private Vector2 _startLocation;
        private List<MenuEntry> _entries;
        private Box _mouseBox;

        public MenuComponent(Engine engine, string[] menuItems, SpriteBatch spriteBatch, Vector2 location)
            : base(engine)
        {
            Engine = engine;
            _entries = new List<MenuEntry>(menuItems.Length);

            _startLocation = location;
            _menuItems = menuItems;
            _spriteBatch = spriteBatch;
        }

        public override void Initialize()
        {
            base.Initialize();
            _mouseBox = new Box(new Vector2(0, 0), new Vector2(0, 0));
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteFont = Engine.Content.Load<SpriteFont>("Fonts/menu");

            for (int i = 0; i < _menuItems.Length; i++)
            {
                Vector2 size = _spriteFont.MeasureString(_menuItems[i]);
                Vector2 location = new Vector2(_startLocation.X, _startLocation.Y + i * 50);
                _entries.Add(new MenuEntry(_menuItems[i], location, size));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            for (int i = 0; i < _entries.Count; i++)
            {
                Color color;

                if (AABB.Intersect(_entries[i], _mouseBox))
                {
                    //Highlight
                    _selectedMenuItem = i;
                    color = highlight;
                }
                else
                {
                    _selectedMenuItem = -1;
                    color = normal;
                }

                _spriteBatch.DrawString(_spriteFont, new StringBuilder(_entries[i].Text), _entries[i].Location, color);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Check Mouse Collisions
            MouseState mouse = Mouse.GetState();
            _mouseBox = new Box(new Vector2(mouse.X, mouse.Y), new Vector2(125, 24));
        }
    }
}