using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MXNA.Drawing
{

    public class ParticleEffect_A : ParticleEffect
    {
        Limiter _Limiter;
        public int x, y;

        public ParticleEffect_A()
            : base(10000)
        {
            _Limiter = new Limiter(0);
            x = 0;
            y = 0;
        }

        public override void Update(GameTime gameTime)
        {
            _Limiter.Update(gameTime);
            if (_Limiter.Ready && true)
            {
                //Params:
                //0: x-speed
                //1: y-speed
                Particle p = CreateParticle(RNG.Next(-200, 200), RNG.Next(-100, 100), RNG.Next(-100,100));
                p.TimeToLive = 20;
                p.X = x;
                p.Y = y;
                p.Scale = 0.1f;

                p.Transform = null;
                p.Transform += SinXFunction;
                p.Transform += SinYFunction;

                Animation frame = AnimationFactory.GenerateAnimation(@"Lensflare", 256, 256, 1, 0);

                p.Sprite = new Sprite(frame, new Vector2(100, 100));
               // p.Sprite.Animation.CurrentFrame = RNG.Next(5);

                this.Add(p);
            }

            base.Update(gameTime);
        }

        public void SinXFunction(IParticle p)
        {
            p.X = 300 + ((float)(p.Age * p.Params[1]) % 1000) + (float)(Math.Cos(p.Age) * p.Params[0]);
        }

        public void SinYFunction(IParticle p)
        {
            p.Y = 200 + ((float)(p.Age * p.Params[2]) % 1000) + (float)(Math.Sin(p.Age) * p.Params[0]);
            //p.Rotation = (float)p.Age * 10;
        }

    }

}
