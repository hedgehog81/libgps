using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Ort.Location.Gps
{
    public enum FixType
    {
        NoFix,
        Fix2D,
        Fix3D,
    };

    public class GSA : IGpsData
    {
      
        private bool m_autoSelection;
        private FixType m_fixType;
        private int m_nSatellites;
        private double m_precisionDilution;
        private double m_hDop;
        private double m_vDop;

        public bool IsAutoSelection
        {
            get { return m_autoSelection; }
        }

        public FixType Fix
        {
            get { return m_fixType; }
        }

        public int ActiveSatellites
        {
            get { return m_nSatellites; }
        }

        public double PrecisionDilution
        {
            get { return m_precisionDilution; }
        }

        public double HorizontalDop
        {
            get { return m_hDop; }
        }

        public double VerticalDop
        {
            get { return m_vDop; }
        }


        public void Decode(Tokenizer tok)
        {
            string param = tok.GetString();
            m_autoSelection = (param == "A");
            m_fixType = (FixType)tok.GetInt();
            m_nSatellites = tok.GetInt();
            m_precisionDilution = tok.GetDouble();
            m_hDop = tok.GetDouble();
            m_vDop = tok.GetDouble(); 
        }
    }
}
