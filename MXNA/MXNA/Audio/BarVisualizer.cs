using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MXNA.Audio
{
    public class BarVisualizer
    {
        private SoundBar[] _bars;

        public BarVisualizer(Game parent, Vector2 position, string[] assetNames, Vector2 assetDimentions)
        {
            _bars = new SoundBar[16];

            for (int i = 0; i < 16; i++)
            {
                _bars[i] = new SoundBar(parent, new Vector2(position.X + (5 * i), position.Y), new int[3] { 10, 3, 3 }, new int[3] { 40, 20, 40 }, assetNames, assetDimentions);
            }           
        }

        public void Update(int[] frequencyData)
        {
            for (int i = 0; i < 16; i++)
            {
                _bars[i].FrequencyPower = frequencyData[i];
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (SoundBar bar in _bars)
            {
                bar.Draw(spriteBatch);
            }
        }
    }
}
