using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using MXNA;
using MXNA.Input;
using MXNA.Drawing;


namespace MXNA.Test.Modules
{
    public class ScreenTest : ManagedGameComponent
    {
        ManagedGameComponent screen1 = new ScreenTest2(Color.Yellow, 20, 200);
        ManagedGameComponent screen2 = new ScreenTest2(Color.Blue, 40, 220);
        ManagedGameComponent screen3 = new ScreenTest2(Color.Blue, 40, 220);


        PlayerInput playerInput;


        public override void Initialize()
        {
            base.Initialize();
            screen3.Initialize();

            this.ID = "base screen";
            screen1.ID = "screen 1";
            screen2.ID = "screen 2";
            screen3.ID = "screen 3";

            double transitionTime = 0.5;

            screen1.TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            screen1.TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            screen2.TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            screen2.TransitionOnTime = TimeSpan.FromSeconds(transitionTime);
            screen3.TransitionOffTime = TimeSpan.FromSeconds(transitionTime);
            screen3.TransitionOnTime = TimeSpan.FromSeconds(transitionTime);

            this.Add(screen1);
            this.Add(screen2);

            playerInput = new PlayerInput(PlayerIndex.One);
            this.Game.Components.Add(playerInput);

            playerInput.BindAction("Pop", Keys.Z );
            playerInput.BindAction("Push", Keys.X);
            playerInput.BindAction("Stay", Keys.C);
            playerInput.BindAction("Load", Keys.A);

            for (int i = 0; i < 1; i++)
            {
                ParticleTest effect = new ParticleTest();
                effect.Initialize();
                this.Add(effect);
            }
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

            if (playerInput.IsPressed("Pop"))
            {
                screen2.ExitScreen();
            }

            if (playerInput.IsPressed("Push"))
            {
                this.Add(screen2);
            }

            if (playerInput.IsPressed("Stay"))
            {
                screen1.IsPersistant = !screen1.IsPersistant;
            }

            if (playerInput.IsPressed("Load"))
            {
                Loader.Load(this, new ScreenTest2(Color.Red,0,0), screen3);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            

            G.SpriteBatch.Begin();
            DrawDebugInfo(0, 0);
            G.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class ScreenTest2 : ManagedGameComponent
    {
        int i;
        int x, y;
        Color c;

        public ScreenTest2(Color c, int x, int y)
            : base()
        {
            this.c = c;
            this.x = x;
            this.y = y;
        }

        public override void Update(GameTime gameTime)
        {

            i++;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            float translate = G.Viewport.Width * (float)Math.Pow(TransitionPosition,2);

            G.SpriteBatch.Transform = Matrix.CreateTranslation(translate, 0, 0);
            G.SpriteBatch.Begin();
            
            G.SpriteBatch.DrawString(Game.Content.Load<SpriteFont>(@"SpriteFonts\Arial"), i.ToString() , new Vector2(x, y), c);


            G.SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}