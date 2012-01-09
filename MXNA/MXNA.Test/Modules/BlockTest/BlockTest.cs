using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using MXNA;
using MXNA.Input;
using MXNA.Drawing;


namespace MXNA.Test.Modules
{
    public class BlockTest : ManagedGameComponent
    {
        private const int HORIZONTAL = 0;
        private const int VERTICAL = 1;

        private PlayerInput playerInput;
        private BlockGrid blockGrid;

        private Limiter[] InputLimiter;
        private double InputLimiterInterval = 0.15;

        private Limiter dropLimiter;

        ParticleEffect_B burstEffect;
        ParticleEffect_C explodeEffect;


        public override void Initialize()
        {

            playerInput = new PlayerInput(PlayerIndex.One);
            this.Game.Components.Add(playerInput);

            playerInput.BindAction("Quit", Keys.Escape);
            playerInput.BindAction("Up", Keys.Up);
            playerInput.BindAction("Down", Keys.Down);
            playerInput.BindAction("Left", Keys.Left);
            playerInput.BindAction("Right", Keys.Right);
            playerInput.BindAction("Swap", Keys.Space);
            playerInput.BindAction("Explode", Keys.Z);
            playerInput.BindAction("ExplodeAll", Keys.X);

            playerInput.BindAction("Reset", Keys.Enter);

            playerInput.BindAction("Up", Buttons.DPadUp);
            playerInput.BindAction("Down", Buttons.DPadDown);
            playerInput.BindAction("Left", Buttons.DPadLeft);
            playerInput.BindAction("Right", Buttons.DPadRight);

            blockGrid = new BlockGrid(48, 48, 12, 10);

            blockGrid.X = 48;
            blockGrid.Y = 64;

            BlockGroup bg = new BlockGroup(3, 3, blockGrid);
            bg.Add(new Block(BlockType.White), 0, 1);
            bg.Add(new Block(BlockType.White), 1, 1);
            bg.Add(new Block(BlockType.White), 2, 1);
            bg.Add(new Block(BlockType.White), 2, 2);

            //blockGrid.Drop(bg, 2);
            blockGrid.Drop(bg, 4);

            InputLimiter = new Limiter[2]{new Limiter(InputLimiterInterval),new Limiter(InputLimiterInterval)};
            dropLimiter = new Limiter(0.15);

            burstEffect = new ParticleEffect_B();
            this.Game.Components.Add(burstEffect);

            explodeEffect = new ParticleEffect_C();
            this.Game.Components.Add(explodeEffect);

            blockGrid.BurstEffect = burstEffect;
            blockGrid.ExplodeEffect = explodeEffect;

            this.Game.Components.Add(blockGrid);
   
            base.Initialize();

            this.Enabled = true;
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

            InputLimiter[HORIZONTAL].Update(gameTime);
            InputLimiter[VERTICAL].Update(gameTime);
            HandleInput(gameTime);

            //dropLimiter.Reset();
            //dropLimiter.Enabled = false;
            dropLimiter.Update(gameTime);
            if (dropLimiter.Ready)
            {
                BlockGroup bg = new BlockGroup(1, 1, blockGrid);
                BlockType bt = (BlockType)RNG.Next(4);
                bg.Add(new Block(bt), 0, 0);
                blockGrid.Drop(bg, RNG.Next(blockGrid.NumCols));
            }

        }

        public override void Draw(GameTime gameTime)
        {
            float translate = G.Viewport.Width * (float)Math.Pow(TransitionPosition, 2);
            G.SpriteBatch.Transform = Matrix.CreateTranslation(translate, 0, 0);
            G.SpriteBatch.Begin();

            //G.SpriteBatch.DrawString(Game.Content.Load<SpriteFont>(@"SpriteFonts\Arial"), i.ToString(), new Vector2(x, y), c);


            base.Draw(gameTime);

            G.SpriteBatch.End();
            
        }

        #region Update Methods

        private void HandleInput(GameTime gameTime)
        {

            if (playerInput.IsPressed("Quit"))
            {
                Game.Exit();
            }

            if (playerInput.IsPressed("Explode"))
            {
                blockGrid.ExplodeBlock(blockGrid.SelectorRow,blockGrid.SelectorCol);
            }

            if (playerInput.IsHeld("ExplodeAll"))
            {
                for (int r=0; r < blockGrid.NumRows; r++)
                for (int c = 0; c < blockGrid.NumCols; c++)
                    blockGrid.ExplodeBlock(r, c);
            }

            if (playerInput.IsPressed("Swap"))
            {
                blockGrid.DoSwap();
            }

            if (playerInput.IsPressed("Left") || playerInput.IsPressed("Right"))
            {
                InputLimiter[HORIZONTAL].Allow();
            }

            if (playerInput.IsPressed("Up") || playerInput.IsPressed("Down"))
            {
                InputLimiter[VERTICAL].Allow();
            }

            if (playerInput.IsHeld("Up"))
            {
                if (InputLimiter[VERTICAL].Ready)
                {
                    blockGrid.SelectorRow--;
                }
            }

            if (playerInput.IsHeld("Down"))
            {
                if (InputLimiter[VERTICAL].Ready)
                {
                    blockGrid.SelectorRow++;
                }
            }

            if (playerInput.IsHeld("Left"))
            {
                if (InputLimiter[HORIZONTAL].Ready)
                {
                    blockGrid.SelectorCol--;
                }
            }

            if (playerInput.IsHeld("Right"))
            {
                if (InputLimiter[HORIZONTAL].Ready)
                {
                    blockGrid.SelectorCol++;
                }
            }

            if (playerInput.IsPressed("Reset"))
            {
                for (int r = 0; r < blockGrid.NumRows; r++)
                for (int c = 0; c < blockGrid.NumCols; c++)
                {
                    blockGrid.Blocks[r,c] = null;
                }
            }

        }

        #endregion
    }
}