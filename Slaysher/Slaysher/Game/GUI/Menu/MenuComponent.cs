using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slaysher.Game.Physics.Collisions;

namespace Slaysher.Game.GUI.Menu
{
    public class MenuComponent : DrawableGameComponent
    {
        public event EventHandler MouseClick;

        public Engine Engine { get; set; }

        public static Color Normal = Color.White;
        public static Color Highlight = Color.Orange;

        private SoundEffect _hoverSong;

        private readonly string[] _menuItems;
        private readonly SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;

        private int _selectedMenuItem = -1;
        private int _lastSelected = -1;

        private Vector2 _startLocation;
        private readonly List<MenuEntry> _entries;
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
            _hoverSong = Engine.Content.Load<SoundEffect>("Sounds/GUI/btnHover");

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
            bool foundMatch = false;
            _selectedMenuItem = -1;

            for (int i = 0; i < _entries.Count; i++)
            {
                Color color;

                if (!foundMatch && Box.Intersect(_entries[i], _mouseBox))
                {
                    //Highlight
                    _selectedMenuItem = i;
                    color = Highlight;
                    foundMatch = true;
                    if (_lastSelected != _selectedMenuItem)
                    {
                        _lastSelected = _selectedMenuItem;
                        _hoverSong.Play();
                    }
                }
                else
                {
                    color = Normal;
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
            _mouseBox = new Box(new Vector2(mouse.X, mouse.Y), new Vector2(125, 20));

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (_selectedMenuItem != -1 && MouseClick != null)
                {
                    MouseClick(this, new MenuSelectEvent(_selectedMenuItem));
                }
            }
        }
    }
}