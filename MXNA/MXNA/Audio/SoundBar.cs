using System;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MXNA.Drawing;

namespace MXNA.Audio
{
    public class SoundBar
    {
        private Sprite _greenOn, _yellowOn, _redOn;
        private Game _parent;
        private Vector2 _position;
        private int _greenAmount, _yellowAmount, _redAmount;
        private int _greenPercent, _yellowPercent, _redPercent;
        private int _freqPower;
        private int _gInterval, _yInterval, _rInterval;
        private int _offset;

        public int FrequencyPower
        {
            set
            {
                if (value > 100)
                {
                    _freqPower = 100;
                }
                else
                {
                    _freqPower = value;
                }
            }
        }

        public SoundBar(Game parent, Vector2 position, int[] amounts, int[] percentages, string[] spriteLocations, Vector2 spriteDimentions)
        {
            _parent = parent;
            _position = position;

            Debug.Assert((amounts.Length == 3), "Amounts Length MUST be 3!");
            Debug.Assert((percentages.Length == 3), "Percentages Length MUST be 3!");

            _greenAmount = amounts[0];
            _yellowAmount = amounts[1];
            _redAmount = amounts[2];

            _greenPercent = percentages[0];
            _yellowPercent = percentages[1];
            _redPercent = percentages[2];

            Debug.Assert((_greenPercent + _yellowPercent + _redPercent) == 100, "Percentages MUST add up to 100!");

            Initialize(spriteLocations, spriteDimentions);
        }

        private void Initialize(string[] spriteLocations,Vector2 spriteDimentions)
        {
            _greenOn = new Sprite(new Frame(spriteLocations[0], new Rectangle(0, 0, Convert.ToInt32(spriteDimentions.X), Convert.ToInt32(spriteDimentions.Y))), _position);
            _yellowOn = new Sprite( new Frame(spriteLocations[1], new Rectangle(0, 0, Convert.ToInt32(spriteDimentions.X), Convert.ToInt32(spriteDimentions.Y))), _position);
            _redOn = new Sprite(new Frame(spriteLocations[2], new Rectangle(0, 0, Convert.ToInt32(spriteDimentions.X), Convert.ToInt32(spriteDimentions.Y))), _position);

            _freqPower = 0;
            _offset = _greenOn.Height * 2;
            
            _gInterval = _greenPercent / _greenAmount;
            _yInterval = _yellowPercent / _yellowAmount;
            _rInterval = _redPercent / _redAmount;
        }

        private void ResetPosition(float X, float Y)
        {
            _greenOn.X = X;
            _greenOn.Y = Y;

            _yellowOn.X = X;
            _yellowOn.Y = Y;

            _redOn.X = X;
            _redOn.Y = Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int currInterval = 0;
            int nextInterval = _gInterval;

            float X = _position.X;
            float Y = _position.Y;

            bool keepGoing = true;
            ResetPosition(X, Y);

            for (int i = 1; keepGoing && i <= _greenAmount; i++)
            {
                if (_freqPower >= currInterval && _freqPower <= nextInterval)
                {
                    _greenOn.Draw();
                    keepGoing = false;
                }
                else if (_freqPower > nextInterval)
                {
                    _greenOn.Draw();
                }
                else
                {
                    keepGoing = false;
                }

                Y -= _offset;
                ResetPosition(X, Y);

                currInterval = nextInterval;
                nextInterval += _gInterval;
            }

            for (int i = 1; keepGoing && i <= _yellowAmount; i++)
            {
                if (_freqPower >= currInterval && _freqPower <= nextInterval)
                {
                    _yellowOn.Draw();
                    keepGoing = false;
                }
                else if (_freqPower > nextInterval)
                {
                    _yellowOn.Draw();
                }
                else
                {
                    keepGoing = false;
                }

                Y -= _offset;
                ResetPosition(X, Y);

                currInterval = nextInterval;
                nextInterval += _yInterval;
            }

            for (int i = 1; keepGoing && i <= _redAmount; i++)
            {
                if (_freqPower >= currInterval && _freqPower <= nextInterval)
                {
                    _redOn.Draw();
                    keepGoing = false;
                }
                else if (_freqPower > nextInterval)
                {
                    _redOn.Draw();
                }
                else
                {
                    keepGoing = false;
                }

                Y -= _offset;
                ResetPosition(X, Y);

                currInterval = nextInterval;
                nextInterval += _rInterval;
            }
        }
    }
}
