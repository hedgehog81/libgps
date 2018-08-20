using System;
using System.IO.Ports;
using System.IO;

namespace Location.Gps
{
   
    public delegate void GpsUpdateDelegate();

    public class LocationInfo
    {
        private double m_latitude;
        private double m_longitude;
        private double m_speed;
        private double m_altitude;

        internal LocationInfo(double latitude, double longitude, double speed, double altitude)
        {
            m_latitude = latitude;
            m_longitude = longitude;
            m_speed = speed;
            m_altitude = altitude;
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

        public double Altitude
        {
            get { return m_altitude; }
        }
 
    }

    public class SateliteInfo
    {
        private int m_activeSatelites;
        private FixQuality m_fixQuality;

        internal SateliteInfo(int activeSatelites, FixQuality fixQuality)
        {
            m_activeSatelites = activeSatelites;
            m_fixQuality = fixQuality;
        }

        public int ActiveSatelites
        {
            get { return m_activeSatelites; }
        }

        public FixQuality m_Quality
        {
            get { return m_fixQuality; }
        }

    }


    public class GpsLocator
    {
        private SerialPort m_serialPort = new SerialPort();
        private RxState m_rxState;
        private MemoryStream m_sentence = new MemoryStream(90);
        private SentenceReader m_sentenceReader = new SentenceReader();
        private RingBuffer m_rxBuffer = new RingBuffer(256);

        private Object m_lock = new object();

        private double m_latitude;
        private double m_longitude;
        private double m_speed;
        private double m_altitude;
        private int m_activeSatelites;
        private FixQuality m_fixQuality;
        private DateTime m_sampleTime;
        private bool m_isValid;

       
        enum RxState
        {
            WaitDollar,
            WaitG,
            WaitP,
            WaitEnd
        };


        public event GpsUpdateDelegate GpsDataUpdated;

        public GpsLocator(int port)
        {
            m_serialPort.PortName = String.Format("COM{0}:", port);
            m_serialPort.BaudRate = 9600;
            m_serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
        }

        private void FillBuffer()
        {
            m_rxBuffer.Put(m_serialPort.BaseStream);
        }

        private byte GetCh()
        {
            return m_rxBuffer.Get();
        }
      
        private void ProcessBuffer()
        {
            while (m_rxBuffer.BytesToRead != 0)
            {
                byte ch = GetCh();
                
                switch (m_rxState)
                {
                    case RxState.WaitDollar:
                        if (ch == '$')
                        {
                            m_rxState = RxState.WaitG;
                        }
                        break;

                    case RxState.WaitG:
                        if (ch == 'G')
                        {
                            m_rxState = RxState.WaitP;
                        }
                        else
                        {
                            m_rxState = RxState.WaitDollar;
                        }
                        break;

                    case RxState.WaitP:
                        if (ch == 'P')
                        {
                            m_rxState = RxState.WaitEnd;
                            m_sentence.SetLength(0);
                        }
                        else
                        {
                            m_rxState = RxState.WaitDollar;
                        }
                        break;

                    case RxState.WaitEnd:

                        if (ch == '\r')
                        {
                            m_sentence.Seek(0, SeekOrigin.Begin);
                            DecodeSentence(m_sentence);
                            
                            m_sentence.SetLength(0);
                            m_rxState = RxState.WaitDollar;
                        }
                        else
                        {
                            if (m_sentence.Length > 80)
                            {
                                m_sentence.SetLength(0);
                                m_rxState = RxState.WaitDollar;
                            }
                            else
                            {
                                m_sentence.WriteByte(ch);
                            }
                        }
                        break;
                }
            }
        }


        private void DecodeSentence(Stream strm)
        {

            StreamReader rdr = new StreamReader(strm);
            IGpsData obj = (IGpsData)m_sentenceReader.Decode(rdr);

            if (obj != null)
            {
                if (obj is RMC)
                {
                    HandleSentence((RMC)obj);
                }
                else if (obj is GGA)
                {
                    HandleSentence((GGA)obj);
                }

                if (GpsDataUpdated != null)
                {
                    GpsDataUpdated();
                }
            }
            
        }

        private void HandleSentence(GGA data)
        {
            lock(m_lock)
            {
                m_altitude = data.Altitude;
                m_activeSatelites = data.ActiveSatellites;
                m_fixQuality = data.Quality;
            }
        }

        private void HandleSentence(RMC data)
        {
            lock(m_lock)
            {
                m_latitude = data.Latitude;
                m_longitude = data.Longitude;
                m_speed = data.Speed;
                m_sampleTime = data.SampleDate;
                m_isValid = data.IsActive;
            }
        }

        

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            FillBuffer();
            ProcessBuffer();
        }

        public void Open()
        {
            m_serialPort.Open();
        }

        public void Close()
        {
            m_serialPort.Close();
        }

        public LocationInfo LocationInfo
        {
            get 
            {
                lock(m_lock)
                {
                    return new LocationInfo(m_latitude, m_longitude, m_speed, m_altitude);
                }
            }
        }

        public SateliteInfo SatInfo
        {
            get
            {
                lock(m_lock)
                {
                    return new SateliteInfo(m_activeSatelites, m_fixQuality);
                }
            }
        }

        public DateTime LastSampleTimeStamp
        {
            get { return m_sampleTime; }
        }

        public bool Valid
        {
            get { return m_isValid; }
        }
    }
}
