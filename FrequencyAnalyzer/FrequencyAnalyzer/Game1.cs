using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using FrequencyAnalyzer.Modules;

namespace FrequencyAnalyzer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Song[] songs;
        int trackNumber = 0;
        VisualizationData visualizationData;

        short[][] lineListIndicies;
        VertexPositionColor[][] pointList;

        VertexDeclaration vertexDeclaration;

        Matrix worldMatrix;
        Matrix viewMatrix;
        Matrix projectionMatrix;

        BasicEffect basicEffect;

        int sampleNumber = 256;
        string songTitle = "";

        SpriteFont font;

        PlayerInput Player1;

        int frameCounter;
        int currentFrameRate;
        int frameTime;     

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800; //1920;
            graphics.PreferredBackBufferHeight = 600; //1080;
            graphics.ApplyChanges();

            frameCounter = 0;
            currentFrameRate = 0;
            frameTime = 0;

            InitializeEffect();
            InitializeLists();

            visualizationData = new VisualizationData();
            songs = new Song[2];

            Player1 = new PlayerInput(PlayerIndex.One);

            base.Initialize();
        }

        protected void InitializeLists()
        {
            pointList = new VertexPositionColor[sampleNumber][];
            lineListIndicies = new short[sampleNumber][];

            for (int i = 0; i < sampleNumber; i++)
            {
                VertexPositionColor[] vpc = new VertexPositionColor[60];
                lineListIndicies[i] = new short[2] { 0, 1 };

                for (int j = 0; j < 60; j++)
                {
                    if (j < 25)
                    {
                        vpc[j] = new VertexPositionColor(new Vector3(00 + (i * 3), 500 + (-j), 0), Color.Green);
                    }
                    else if (j < 45)
                    {
                        vpc[j] = new VertexPositionColor(new Vector3(00 + (i * 3), 500 + (-j), 0), Color.Yellow);
                    }
                    else
                    {
                        vpc[j] = new VertexPositionColor(new Vector3(00 + (i * 3), 500 + (-j), 0), Color.Red);
                    }
                }

                pointList[i] = vpc;
            }
        }

        protected void InitializeEffect()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, (float)GraphicsDevice.Viewport.Width, (float)GraphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f);
           
            vertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);

            basicEffect = new BasicEffect(GraphicsDevice, null);
            basicEffect.VertexColorEnabled = true;

            //worldMatrix = Matrix.CreateTranslation(GraphicsDevice.Viewport.Width / 2f - 150, GraphicsDevice.Viewport.Height / 2f - 50, 0);
            worldMatrix = Matrix.CreateTranslation(0, 0, 0);

            basicEffect.World = worldMatrix;
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Nikona");

            MediaPlayer.Stop();
            songs[0] = Content.Load<Song>("twilight");
            songs[1] = Content.Load<Song>("orange");            
            MediaPlayer.IsVisualizationEnabled = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            MediaPlayer.Stop();                    
        }

        protected void CheckPlayerInput()
        {
#if XBOX
            if (Player1.IsPressed(Buttons.Back))
#elif WINDOWS
            if (Player1.isPressed(Keys.Escape))
#endif
            {
                this.Exit();
            }

            if (Player1.isPressed(Keys.Enter))
            {
                if (MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Pause();
                }
                else if (MediaPlayer.State == MediaState.Paused)
                {
                    MediaPlayer.Resume();
                }
            }

            if (Player1.isPressed(Keys.Right))
            {
                MediaPlayer.Stop();

                if (trackNumber + 1 > songs.Length - 1)
                {
                    trackNumber = 0;
                }
                else
                {
                    trackNumber++;
                }
                songTitle = songs[trackNumber].ToString();
                MediaPlayer.Play(songs[trackNumber]);
            }

            if (Player1.isPressed(Keys.Left))
            {
                MediaPlayer.Stop();

                if (trackNumber - 1 < 0)
                {
                    trackNumber = songs.Length - 1;
                }
                else
                {
                    trackNumber--;
                }
                songTitle = songs[trackNumber].ToString();
                MediaPlayer.Play(songs[trackNumber]);
            }

            if(Player1.isHeld(Keys.Down))
            {
                if(MediaPlayer.Volume - 0.01f > 0.0f)
                {
                    MediaPlayer.Volume -= 0.01f;
                }
            }

            if (Player1.isHeld(Keys.Up))
            {
                if (MediaPlayer.Volume + 0.01f < 1.0f)
                {
                    MediaPlayer.Volume += 0.01f;
                }
            }
        }

        protected void CalculateFPS(GameTime gameTime)
        {
            frameCounter++;
            frameTime += gameTime.ElapsedGameTime.Milliseconds;

            if (frameTime >= 1000)
            {
                currentFrameRate = frameCounter;
                frameTime = 0;
                frameCounter = 0;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Player1.Poll();

            CheckPlayerInput();
            CalculateFPS(gameTime);            

            MediaPlayer.GetVisualizationData(visualizationData);

            if (MediaPlayer.State != MediaState.Paused)
            {
                float freq = 0.0f;
                int count = 0;
                short[] indicies;

                for (int i = 0; i < sampleNumber; i++)
                {

                    if (sampleNumber == 256)
                    {
                        freq = visualizationData.Frequencies[i];
                    }
                    else
                    {
                        int splitValue = 256 / sampleNumber;

                        for (int x = i * splitValue; x < (i * splitValue) + splitValue; x++)
                        {
                            freq += visualizationData.Frequencies[x];
                        }

                        freq = freq / (splitValue+1);
                    }



                    count = Convert.ToInt32(freq * 100);
                    //count /= 2;
                    indicies = new short[2] { 0, 1 };

                    count = (int)(count * MediaPlayer.Volume);

                    if ((count * 2) - 2 > 0)
                    {
                        indicies = new short[(count * 2) - 2];

                        for (int j = 0; j < count - 1; j++)
                        {
                            indicies[j * 2] = (short)j;
                            indicies[(j * 2) + 1] = (short)(j + 1);
                        }
                    }
                    lineListIndicies[i] = indicies;
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.VertexDeclaration = vertexDeclaration;

            basicEffect.Begin();

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                
                DrawLineList();

                GraphicsDevice.RenderState.FillMode = FillMode.Solid;

                pass.End();
            }

            basicEffect.End();

            spriteBatch.Begin();
            DrawText();
            spriteBatch.End();


            base.Draw(gameTime);
        }

        protected void DrawText()
        {
            spriteBatch.DrawString(font, songTitle, new Vector2(40,40), Color.White);
            spriteBatch.DrawString(font, "Width: " + GraphicsDevice.Viewport.Width, new Vector2(40, 80), Color.White);
            spriteBatch.DrawString(font, "Height: " + GraphicsDevice.Viewport.Height, new Vector2(40, 120), Color.White);
            spriteBatch.DrawString(font, "FPS: " + currentFrameRate, new Vector2(40, 160), Color.White);            
        }

        protected void DrawLineList()
        {
            for (int i = 0; i < sampleNumber; i++)
            {
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,
                    pointList[i],
                    0,
                    60,
                    lineListIndicies[i],
                    0,
                    lineListIndicies[i].Length / 2);
            }
        }
    }
}
