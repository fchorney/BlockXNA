using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MXNA.Drawing
{
    public interface IParticle : ITransformable
    {
        double Age { get; }
        double TimeToLive { get; }
        double ElapsedTime { get; }
        bool Enabled { get; set; }

        Sprite Sprite { get; set; }
        Color Color { get; set; }
        TransformFunction Transform { get; }
        double[] Params { get; }
    }

    public delegate void TransformFunction(IParticle p);

    public class Particle : IParticle
    {
        double _Age;
        double _TimeToLive;
        double _ElapsedTime;

        double[] _Params;

        private Vector2 _position;
        private float _scale;
        private float _rotation;
        private Sprite _sprite;

        TransformFunction _Function;

        private Color _color = Color.White;

        public double Age
        {
            get { return _Age; }
        }

        public double TimeToLive
        {
            get { return _TimeToLive; }
            set { _TimeToLive = value; }
        }

        public double ElapsedTime
        {
            get { return _ElapsedTime; }
        }

        public bool Enabled { get; set; }

        public TransformFunction Transform
        {
            get { return _Function; }
            set { _Function = value; }
        }

        public double[] Params
        {
            get { return _Params; }
        }

        public float X
        {
            get { return _position.X; }
            set { _position.X = value; }
        }

        public float Y
        {
            get { return _position.Y; }
            set { _position.Y = value; }
        }

        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Sprite Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public Particle(params double[] Parameters)
        {
            Init(Parameters);
        }

        public void Init(params double[] Parameters)
        {
            _Age = 0;
            _Params = Parameters;
            _position = new Vector2();
            _scale = 1;
            _Function = null;
        }

        public void Update(GameTime gameTime)
        {
            _Age += gameTime.ElapsedGameTime.TotalSeconds;
            _ElapsedTime = gameTime.ElapsedGameTime.TotalSeconds;

            Sprite.Update(gameTime);
            _Function(this);
            
        }

        public void Draw()
        {
            Sprite.Draw(X, Y, Scale, Rotation, Color);
        }

    }

}