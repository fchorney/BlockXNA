using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MXNA.Drawing
{

    public class ParticleEffect_B : ParticleEffect
    {
        Limiter limiter;

        float _TargetX = 1000;
        float _TargetY = 100;
        float _SourceX = 800;
        float _SourceY = 400;
        double TrackingTime = 1;
        Color _Color = Color.White;

        public float TargetX
        {
            get { return _TargetX; }
            set { _TargetX = value; }
        }

        public float TargetY
        {
            get { return _TargetY; }
            set { _TargetY = value; }
        }

        public float SourceX
        {
            get { return _SourceX; }
            set { _SourceX = value; }
        }

        public float SourceY
        {
            get { return _SourceY; }
            set { _SourceY = value; }
        }

        public Color Color
        {
            get { return _Color; }
            set { _Color = value; }
        }

        public void Burst()
        {
            for (int i = 0; i < 4; i++)
            {
                //Params:
                //0: x-speed initial
                //1: y-speed initial
                //2: x-accel
                //3: y-accel
                //4: x source
                //5: y source
                Particle p = new Particle(RNG.Next(-1000, 1000), RNG.Next(-1000, 1000), 0, 0, _SourceX, _SourceY);

                p.Params[2] = 2 * ((_TargetX - _SourceX) - p.Params[0] * TrackingTime) / (TrackingTime * TrackingTime);
                p.Params[3] = 2 * ((_TargetY - _SourceY) - p.Params[1] * TrackingTime) / (TrackingTime * TrackingTime);

                p.TimeToLive = TrackingTime;
                p.X = _SourceX;
                p.Y = _SourceY;
                p.Scale = 64 / 256.0f;
                p.Color = _Color;

                p.Transform += TrackToTarget;

                Animation frame = AnimationFactory.GenerateAnimation(@"Lensflare", 256, 256, 1, 0);

                p.Sprite = new Sprite(frame, new Vector2(200, 200));
                //p.Sprite.Animation.CurrentFrame = RNG.Next(5);

                this.Add(p);

            }
        }

        public ParticleEffect_B()
            : base()
        {
            limiter = new Limiter(1);
        }

        public override void Update(GameTime gameTime)
        {
            //limiter.Update(gameTime);

            base.Update(gameTime);
        }

        public void TrackToTarget(IParticle p)
        {
            double dX = (p.Params[0] * p.Age) + (p.Params[2] * p.Age * p.Age / 2);
            double dY = (p.Params[1] * p.Age) + (p.Params[3] * p.Age * p.Age / 2);

            p.X = (float)(p.Params[4] + dX);
            p.Y = (float)(p.Params[5] + dY);
        }

    }

}
