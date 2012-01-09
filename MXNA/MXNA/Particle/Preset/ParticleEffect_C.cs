using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MXNA.Drawing
{

    public class ParticleEffect_C : ParticleEffect
    {

        float _SourceX = 800;
        float _SourceY = 400;
        double ExplodeTime = 0.5;
        Color _Color = Color.White;

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
            Particle p = new Particle();

            p.TimeToLive = ExplodeTime;
            p.X = _SourceX;
            p.Y = _SourceY;
            p.Scale = 128 / 200.0f;
            p.Color = _Color;

            p.Transform += TrackToTarget;

            Animation frame = AnimationFactory.GenerateAnimation(@"Explosion", 200, 200, 6, 0.06);

            p.Sprite = new Sprite(frame, new Vector2(-200, -200));

            this.Add(p);
        }

        public ParticleEffect_C()
            : base()
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void TrackToTarget(IParticle p)
        {
            if (p.Sprite.Animation.CurrentFrame > 2)
                p.Sprite.Animation.CurrentFrame = 2;

            if (p.Age > 0.2)
            {
                p.Color = AdjustOpacity(p.Color, 255 - (int)((p.Age / 0.5) * 255));
            }
        
        }

        private Color AdjustOpacity(Color color, int opacity)
        {
            if (opacity < 0)
                opacity = 0;
            color.A = (byte)(opacity % 255);

            return color;
        }


    }

}
