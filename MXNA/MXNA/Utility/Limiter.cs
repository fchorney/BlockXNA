using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MXNA
{
    public class Limiter
    {
        double interval;
        double elapsedTime;
        bool enabled;

        public Limiter(double interval)
        {
            this.interval = interval;
            elapsedTime = double.MaxValue;
            enabled = true;
        }

        public double Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        public void Update(GameTime gameTime)
        {
            if (enabled)
                this.elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Reset()
        {
            this.elapsedTime = 0;
        }

        public void Allow()
        {
            this.elapsedTime = interval;
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = false; }
        }

        public bool Ready
        {
            get
            {
                bool result = elapsedTime >= interval;
                if (result && enabled) elapsedTime = 0;
                return result;
            }
        }
    }
}