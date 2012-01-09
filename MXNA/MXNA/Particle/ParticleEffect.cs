using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MXNA.Drawing
{

    public class ParticleEffect : DrawableGameComponent 
    {
        private List<Particle> Particles;
        private List<Particle> ParticleBuffer;

        public ParticleEffect(int bufferSize = 0) : base(G.Game)
        {
            Particles = new List<Particle>();
            ParticleBuffer = new List<Particle>();

            for (int i = 0; i < bufferSize; i++)
            {
                Particle p = new Particle();
                ParticleBuffer.Add(p);
            }
        }

        public Particle CreateParticle(params double[] Parameters)
        {
            Particle p;

            if (ParticleBuffer.Count > 0)
            {
                p = ParticleBuffer[0];
                ParticleBuffer.RemoveAt(0);
                p.Init(Parameters);
            }
            else
            {
                p = new Particle(Parameters);
            }
            
            return p;
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                Particle p = Particles[i];
                if (p.Enabled)
                {
                    p.Update(gameTime);

                    if (p.TimeToLive > 0 && p.Age > p.TimeToLive)
                    {
                        p.Enabled = false;
                        ParticleBuffer.Add(p);
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            G.SpriteBatch.Begin();
            for (int i = 0; i < Particles.Count; i++)
            {
                Particle p = Particles[i];
                if (p.Enabled)
                {
                    p.Draw();
                }
            }
            G.SpriteBatch.End();
        }

        public virtual void Add(Particle p)
        {
            p.Enabled = true;
            Particles.Add(p);
        }

        public virtual void Start() { }
        public virtual void Stop() { }

    }

}