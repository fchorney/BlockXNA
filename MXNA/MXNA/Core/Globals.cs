using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MXNA
{

    public static class G
    {
        private static Game _Game = null;
        private static GameTime _GameTime = null;
        private static ManagedSpriteBatch _SpriteBatch = null;

        public static void SetGame(Game game)
        {
            if(_Game == null) _Game = game;
        }

        public static Game Game
        {
            get { return _Game; }
        }

        public static Viewport Viewport
        {
            get { return _Game.GraphicsDevice.Viewport; }
        }

        public static GameTime GameTime
        {
            get { return _GameTime; }
            set { _GameTime = value; }
        }

        public static ContentManager Content
        {
            get { return _Game.Content; }
        }

        public static ManagedSpriteBatch SpriteBatch
        {
            get
            {
                if (_SpriteBatch == null)
                {
                    _SpriteBatch = new ManagedSpriteBatch();
                }
                return _SpriteBatch;
            }
        }
    }
}