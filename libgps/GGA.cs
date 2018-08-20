using System;


namespace Location.Gps
{
    public enum FixQuality
    {
        Invalid,
        SPS,
        DGPS,
        PPS,
        RealTimeKinematic,
        FloatRTK,
        DeadReconing,
        ManualInputMode,
        SimulationMode
    };

    internal class GGA : IGpsData
    {

        private DateTime m_time;
        private double m_latitude;
        private double m_longtitude;
        private FixQuality m_fixQuality;
        private int m_nSatellites;
        private double m_horizontalDilution;
        private double m_altitude;
        private double m_heightOfGeoid;
        private double m_lastDGPSUpdate;
        private int m_dgpsStationId;



        public DateTime SampleTime
        {
            get { return m_time; }
        }

        public double Latitude
        {
            get { return m_latitude; }
        }

        public double Longtitude
        {
            get { return m_longtitude; }
        }

        public FixQuality Quality
        {
            get { return m_fixQuality; }
        }

        public int ActiveSatellites
        {
            get { return m_nSatellites; }
        }

        public double HorizontalDilution
        {
            get { return m_horizontalDilution; }
        }

        public double Altitude
        {
            get { return m_altitude; }
        }

        public double HeightOfGeoid
        {
            get { return m_heightOfGeoid; }
        }

        public double LastDGPSUpdate
        {
            get { return m_lastDGPSUpdate; }
        }

        public int DgpsStationId
        {
            get { return m_dgpsStationId; }
        }


        public void Decode(Tokenizer tok)
        {
            m_time = tok.GetTime();
            m_latitude = tok.GetLatitude();
            m_longtitude = tok.GetLongitude();
            m_fixQuality = (FixQuality)tok.GetInt();
            m_nSatellites = tok.GetInt();
            m_horizontalDilution = tok.GetDouble();
            m_altitude = tok.GetDouble();
            tok.Skip();
            m_heightOfGeoid = tok.GetDouble();
            tok.Skip();
            m_lastDGPSUpdate = tok.GetDouble();
            tok.Skip();
            m_dgpsStationId = tok.GetInt();

        }
    }
}
