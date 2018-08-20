using System;


namespace Location.Gps
{
    internal class RMC : IGpsData
    {
     
        private DateTime m_sampleDate;
        private bool m_isActive;
        private double m_latitude;
        private double m_longitude;
        private double m_speed;
        private double m_trackAngle;

        public DateTime SampleDate
        {
            get { return m_sampleDate; }
        }

        public bool IsActive
        {
            get { return m_isActive; }
        }

        public double Latitude
        {
            get { return m_latitude; }
        }

        public double Longitude
        {
            get { return m_longitude; }
        }

        public double Speed
        {
            get { return m_speed; }
        }

        public double TrackAngle
        {
            get { return m_trackAngle; }
        }

        public void Decode(Tokenizer tok)
        {
                DateTime time = tok.GetTime();
                m_isActive = (tok.GetString() == "A");
                m_latitude = tok.GetLatitude();
                m_longitude = tok.GetLongitude();
                m_speed = tok.GetDouble();
                m_trackAngle = tok.GetDouble();
                DateTime date = tok.GetDate();

                m_sampleDate = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Utc);       
        }
    }
}
