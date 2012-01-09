using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MXNA
{
    public class ManagedSpriteBatch : SpriteBatch
    {
        private Boolean HasBegun { get; set; }

        public Matrix Transform { get; set; }

        public ManagedSpriteBatch()
            : base(G.Game.GraphicsDevice)
        {
            HasBegun = false;
            Transform = Matrix.Identity;
        }

        public new void Begin()
        {
            if (HasBegun)
                base.End();
                
            base.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Transform);
        }

        public new void End()
        {
            base.End();
            HasBegun = false;
            Transform = Matrix.Identity;
        }
    }
}
