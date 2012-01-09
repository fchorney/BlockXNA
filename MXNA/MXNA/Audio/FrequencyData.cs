using System;
using System.Collections.Generic;
using System.IO;

namespace MXNA.Audio
{
    public class FrequencyData
    {
        private Dictionary<string, int[]> _frequency;
        private int[] _last;
        public const int Resolution = 16;

        public FrequencyData(string assetName)
        {
            _frequency = new Dictionary<string, int[]>();
            _last = new int[16];
            LoadData(assetName);
        }

        private void LoadData(string assetName)
        {
            TextReader tr = new StreamReader(assetName);
            string line = "";
            string wtf = "";

            while ((line = tr.ReadLine()) != null)
            {
                string[] timeSplit = line.Split('-');
                string[] strData = timeSplit[1].Split(',');
                int[] data = new int[16];
                int counter = 0;

                foreach (string num in strData)
                {
                    data[counter++] = Convert.ToInt32(num);
                }
                wtf = timeSplit[0].Substring(0, timeSplit[0].Length - 1);
                insertData(wtf, data);
            }

            tr.Close();
        }

        public int[] getData(string timeStamp)
        {
            //string key = timeStamp.Substring(0,timeStamp.Length - 1) + "0";
            TimeStamp key = new TimeStamp(timeStamp);            
            int currms = key.Millisecond;
            int add = -1;

            while (!_frequency.ContainsKey(key.CurrentTime))
            {
                if (key.Millisecond == 0)
                {
                    key.Millisecond = currms;
                    add = 1;
                }
                key.Millisecond += add;
            }

            if (_frequency.ContainsKey(key.CurrentTime))
            {
                _last = _frequency[key.CurrentTime];
                return _frequency[key.CurrentTime];
            }
            else
            {
                return _last;
            }
        }

        private void insertData(string timeStamp, int[] frequencies)
        {
            _frequency.Add(timeStamp, frequencies);
        }
    }

    public class TimeStamp
    {
        private int _minute;
        private int _second;
        private int _millisecond;

        public int Minute
        {
            get { return _minute; }
            set { _minute = value; }
        }

        public int Second
        {
            get { return _second; }
            set
            {
                if (value < 60)
                {
                    _second = value;
                }
                else
                {
                    _second = 0;
                }
            }
        }

        public int Millisecond
        {
            get { return _millisecond; }
            set
            {
                if (value < 1000)
                {
                    _millisecond = value;
                }
                else
                {
                    _millisecond = 0;
                }
            }
        }

        public string CurrentTime
        {
            get { return _minute.ToString("#0") + ":" + _second.ToString("#0") + ":" + _millisecond.ToString("#000"); }
            set
            {
                string[] split = value.Split(':');
                _minute = Convert.ToInt32(split[0]);
                _second = Convert.ToInt32(split[1]);
                _millisecond = Convert.ToInt32(split[2]);
            }
        }

        public TimeStamp()
        {
            _minute = 0;
            _second = 0;
            _millisecond = 0;
        }

        public TimeStamp(int min, int sec, int milli)
        {
            _minute = min;
            _second = sec;
            _millisecond = milli;
        }

        public TimeStamp(string time)
        {
            string[] split = time.Split(':');
            _minute = Convert.ToInt32(split[0]);
            _second = Convert.ToInt32(split[1]);
            _millisecond = Convert.ToInt32(split[2]);
        }
    }
}
