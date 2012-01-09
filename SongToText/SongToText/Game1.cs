using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Collections;

namespace SongToText
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int songIndex;
        VisualizationData visData;
        TextWriter tw;
        Song[] songs;
        string soundData;
        string soundBuffer;
        int splitvalue;
        int prevMin;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 1);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            visData = new VisualizationData();

            songs = new Song[5];        
            songs[0] = Content.Load<Song>(@"BG");
            songs[1] = Content.Load<Song>(@"Bass");
            songs[2] = Content.Load<Song>(@"Lead");
            songs[3] = Content.Load<Song>(@"Other");
            songs[4] = Content.Load<Song>(@"Vox");
            songIndex = 0;

            tw = new StreamWriter(songs[songIndex] + ".txt");

            MediaPlayer.IsVisualizationEnabled = true;
            //MediaPlayer.Volume = 0.0f;
            MediaPlayer.Stop();

            soundData = "";
            soundBuffer = "";
            prevMin = 0;
            splitvalue = 16;            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {}

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                //double freq = 0.0f;
                float freq = 0.0f;
                MediaPlayer.GetVisualizationData(visData);

                if (MediaPlayer.PlayPosition.Minutes > prevMin)
                {
                    prevMin = MediaPlayer.PlayPosition.Minutes;
                }

                soundData += MediaPlayer.PlayPosition.Minutes + ":" + MediaPlayer.PlayPosition.Seconds + ":" + MediaPlayer.PlayPosition.Milliseconds + " - ";
                for (int i = 0; i < 16; i++)
                {
                    for (int j = (i * splitvalue); j < (i * splitvalue) + splitvalue; j++)
                    {
                        //freq += (double)visData.Frequencies[j];
                        freq += visData.Frequencies[j];
                    }
                    freq = freq / splitvalue;
                    soundData += ((int)(freq * 100)) + ", ";
                }
                soundData = soundData.Substring(0, soundData.Length - 2) + "\r\n";
                //soundData += "\r\n";
            }

            if (soundData.Length > 400000)
            {
                soundBuffer += soundData;
                soundData = "";
            }

            // Allows the game to exit
            if (prevMin > MediaPlayer.PlayPosition.Minutes || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
            {
                if (soundData.Length > 0)
                {
                    soundBuffer += soundData;
                    soundData = "";
                }

                if (++songIndex == songs.Length)
                {
                    if (soundBuffer.Length > 0)
                    {
                        printOutData(soundBuffer);
                       
                        /*
                        TextWriter noob = new StreamWriter("noobulax.txt");
                        noob.Write(soundBuffer);
                        noob.Close();
                        */

                        soundData = "";
                    }
                    tw.Close();

                    this.Exit();
                }
                else
                {
                    if (soundBuffer.Length > 0)
                    {
                        printOutData(soundBuffer);
                        soundData = "";
                    }
                    tw.Close();

                    tw = new StreamWriter(songs[songIndex] + ".txt");
                    MediaPlayer.Stop();
                    soundData = "";
                    soundBuffer = "";
                    prevMin = 0;
                    MediaPlayer.Play(songs[songIndex]);
                }
            }

            // Start the process (Make sure the program is fully loaded first)
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.S))
            {
                MediaPlayer.Play(songs[songIndex]);
            }

            this.SuppressDraw();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Color clr = new Color(Convert.ToByte(visData.Frequencies[32] * 100), Convert.ToByte(visData.Frequencies[64] * 100), Convert.ToByte(visData.Frequencies[128] * 100));

            GraphicsDevice.Clear(clr);
            base.Draw(gameTime);
        }

        public void printOutData(string data)
        {
            string[] lines = data.Split('\n');
            int minute = 0;
            int second = 0;
            int milli = 0;
            int counter2 = 0;
            string timeStamp = "";
            int prevMilli = -1;
            FrequencyData freqData = new FrequencyData();

            foreach (string line in lines)
            {
                if (line != "")
                {
                    string[] lr = line.Split('-');
                    string[] time = lr[0].Split(':');
                    string[] freqs = lr[1].Split(',');
                    int[] fdata = new int[16];

                    minute = Convert.ToInt32(time[0]);
                    second = Convert.ToInt32(time[1]);
                    milli = Convert.ToInt32(time[2]) / 10;

                    if (milli != prevMilli)
                    {
                        prevMilli = milli;

                        counter2 = 0;
                        foreach (string fd in freqs)
                        {
                            fdata[counter2++] = Convert.ToInt32(fd);
                        }

                        if (milli == 0)
                        {
                            timeStamp = minute + ":" + second + ":" + (milli * 10) + "00";
                        }
                        else if (milli > 9)
                        {
                            timeStamp = minute + ":" + second + ":" + (milli) + "0";
                        }
                        else
                        {
                            timeStamp = minute + ":" + second + ":0" + (milli) + "0";
                        }

                        freqData.insertTime(timeStamp, fdata);
                    }                    
                }
            }

            bool print = true;

            foreach (KeyValuePair<string, int[]> pair in freqData.Frequency)
            {
                if (print)
                {
                    string str = "";
                    foreach (int i in pair.Value)
                    {
                        str += i + ",";

                    }
                    tw.Write(pair.Key + " - " + str.Substring(0, str.Length - 1) + "\r\n");
                }
                print = !print;
            }
        }
    }

    public class FrequencyData
    {
        private Dictionary<string, int[]> _frequency;
        public const int Resolution = 16;

        public Dictionary<string, int[]> Frequency
        {
            get { return _frequency; }
        }

        public FrequencyData()
        {
            _frequency = new Dictionary<string, int[]>();
        }

        public int[] getData(string timeStamp)
        {
            return _frequency[timeStamp];
        }

        public void insertTime(string timeStamp, int[] frequencies)
        {
            if (!_frequency.ContainsKey(timeStamp))
            {
                _frequency.Add(timeStamp, frequencies);
            }
        }   
    }
}
