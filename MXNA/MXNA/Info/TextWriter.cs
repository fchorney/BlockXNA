using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MXNA.Info
{
    public class TextWriter : DrawableGameComponent
    {
        private SpriteFont Font;            // Sprite Font to use
        private string FontName;            // Name of font to load
        private static int X, Y;                   // Current Position
        private static Dictionary<string, TextObject> text = new Dictionary<string,TextObject>();

        public TextWriter(string assetName)
            : base(G.Game)
        {
            FontName = assetName;
            X = Y = 20;
        }

        protected override void LoadContent()
        {
            Font = Game.Content.Load<SpriteFont>(FontName);

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {

            foreach (KeyValuePair<string, TextObject> pair in text)
            {
                G.SpriteBatch.DrawString(Font, pair.Key + ": " + pair.Value.Data, pair.Value.Position, Color.White);
            }

            base.Draw(gameTime);
        }

        public static void Add(string label)
        {
            text.Add(label, new TextObject(new Vector2(X, Y)));            
            Y += 25;
        }

        public static void Update(string label, string data)
        {
            text[label].Data = data;
        }

    }

    public class TextObject
    {
        private string _data;
        private Vector2 _position;

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
        }

        public TextObject()
        {
            _data = "";
            _position = new Vector2(0, 0);
        }

        public TextObject(Vector2 position)
        {
            _data = "";
            _position = position;
        }
    }
}
