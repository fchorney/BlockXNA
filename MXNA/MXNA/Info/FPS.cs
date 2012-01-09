using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MXNA.Info
{
    public class FPS : GameComponent
    {
        private int frameCounter;          
        private double currentFrameTime;
        private double prevFrameTime;

        /// <summary>
        /// Creates a new FPS Drawable Game Component. Calculates and displays the current FPS
        /// </summary>
        /// <param name="parent">Game Object that this component will be paired with</param>
        /// <param name="fontName">Name of the font to load from the main Content Pipeline</param>
        /// <param name="x">X Coordinate</param>
        /// <param name="y">Y Coordinate</param>
        public FPS() : base(G.Game){ }

        public override void Initialize()
        {
            frameCounter = 0;
            currentFrameTime = 0;
            prevFrameTime = 0;
            TextWriter.Add("FPS");
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            frameCounter++;
            currentFrameTime = gameTime.TotalGameTime.TotalSeconds - prevFrameTime;
            if (currentFrameTime >= 1)
            {
                TextWriter.Update("FPS", frameCounter.ToString());
                frameCounter = 0;
                prevFrameTime = gameTime.TotalGameTime.TotalSeconds;
            }
            base.Update(gameTime);
        }
    }
}
