using System;


namespace Location.Gps
{
    internal class GLL : IGpsData
    {
        private double m_latitude;
        private double m_longtitude;
        private DateTime m_time;
        private bool m_dataActive;

        public double Latitude
        {
            get { return m_latitude; }
        }

        public double Longtitude
        {
            get { return m_longtitude; }
        }

        public DateTime SampleTime
        {
            get { return m_time; }
        }

        public bool IsDataActive
        {
            get { return m_dataActive; }
        }


        public void Decode(Tokenizer tok)
        {
            m_latitude = tok.GetLatitude();
            m_longtitude = tok.GetLongitude();
            m_time = tok.GetTime();
            m_dataActive = (tok.GetString() == "A");
        }
    }
    
}
