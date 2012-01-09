using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using MXNA;
using MXNA.Input;
using MXNA.Drawing;


namespace MXNA.Test.Modules
{
    public class ScreenTest : ManageableGameComponent
    {
        
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void UnloadContent()
        {

            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {   
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteFont Font = Game.Content.Load<SpriteFont>(@"SpriteFonts\Arial");

            Globals.GameResources.SpriteBatch.DrawString(Font, "Test", new Vector2(200,200), Color.White);

            base.Draw(gameTime);
        }

    }
}