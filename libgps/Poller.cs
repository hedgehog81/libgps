using System;
using System.Threading;

namespace Location.Gps
{
    public class GpsPoller
    {
        public event Action<LocationInfo> TransmitGpsLocation;

        private AutoResetEvent _evtStop;
        private Thread _thWorker;
        private GpsLocator _gpsLocator;
        private int _pollingInterval;
        private int _transmitInterval;

        public GpsPoller(GpsLocator gpsLocator, int pollingInterval, int transmitInterval)
        {
            if (gpsLocator == null)
            {
                throw new ArgumentNullException("The argument cannot be null", "gpsLocator");
            }

            if (pollingInterval == 0)
            {
                throw new ArgumentException("The argument cannot be zero", "pollTimeOut");
            }

            if (transmitInterval < _pollingInterval)
            {
                throw new ArgumentException("The argument 'transmitInterval' cannot be less than 'pollTimeOut'");
            }
            _gpsLocator = gpsLocator;
            _pollingInterval = pollingInterval;
            _transmitInterval = transmitInterval;
            _evtStop = new AutoResetEvent(false);
        }

        public void Start()
        {
            if (_thWorker == null)
            {
                _evtStop.Reset();
                _thWorker = new Thread(new ThreadStart(Worker));
                _thWorker.Start();
            }
        }

        public void Stop()
        {
            if (_thWorker != null)
            {
                _evtStop.Set();

                if (!_thWorker.Join(10000))
                {
                    _thWorker.Abort();
                }
                _thWorker = null;
            }
        }

        public GpsLocator GpsLocator
        {
            get { return _gpsLocator; }
        }

        private void Worker()
        {
            DateTime dtStartCounting = DateTime.Now;

            while (!_evtStop.WaitOne(_pollingInterval, false))
            {
                LocationInfo li = _gpsLocator.LocationInfo;

                if (li.Latitude != 0 && li.Longitude != 0)
                {
                    TimeSpan diff = DateTime.Now - dtStartCounting;
                    if (diff.TotalMilliseconds >= _transmitInterval)
                    {
                        OnTransmitGpsLocation(li);

                        dtStartCounting = DateTime.Now;
                    }
                }                
            }
        }

        private void OnTransmitGpsLocation(LocationInfo li)
        {
            if (TransmitGpsLocation != null)
            {
                TransmitGpsLocation(li);
            }
        }
    }
}
