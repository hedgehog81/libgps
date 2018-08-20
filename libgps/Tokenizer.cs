using System;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Location.Gps
{
    internal class Tokenizer
    {
        private TextReader m_rdr;
        private Char m_token;

        public Tokenizer(TextReader rdr, Char token)
        {
            Debug.Assert(rdr != null);
            m_rdr = rdr;
            m_token = token;
        }


        public string GetString()
        {
            StringBuilder bldr = new StringBuilder();

            while (true)
            {
                int ret = m_rdr.Read();

                if (ret == -1)
                    break;

                Char ch = (Char)ret;

                if (ch != m_token)
                {
                    bldr.Append(ch);
                }
                else
                {
                    break;
                }
            }

            return bldr.ToString();
        }

        public void Skip()
        {
            GetString();
        }

        public DateTime GetDate()
        {
            string date = GetString();

            if (String.IsNullOrEmpty(date))
                return new DateTime();
            try
            {
                int day = int.Parse(date.Substring(0, 2));
                int month = int.Parse(date.Substring(2, 2));
                int year = int.Parse(date.Substring(4, 2)) + 2000;

                return new DateTime(year, month, day);
            }
            catch
            {
                return new DateTime();
            }            
        }

        public DateTime GetTime()
        {
            string time = GetString();

            if (String.IsNullOrEmpty(time))
                return new DateTime();

            try
            {
                int hour = int.Parse(time.Substring(0, 2));
                int minute = int.Parse(time.Substring(2, 2));
                int second = int.Parse(time.Substring(4, 2));
                int millisecond = int.Parse(time.Substring(7));

                return new DateTime(1, 1, 1, hour, minute, second, millisecond, DateTimeKind.Utc);
            }
            catch (Exception)
            {
                return new DateTime();
            }            
        }

        public int GetInt()
        {
            string param = GetString();
            
            if (String.IsNullOrEmpty(param))
                return 0;

            try
            {
                return int.Parse(param);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public double GetDouble()
        {
            string param = GetString();

            if (String.IsNullOrEmpty(param))
                return 0;

            try
            {
                return double.Parse(param);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public double GetLatitude()
        {
            string lat = GetString();
            if (string.IsNullOrEmpty(lat))
            {
                return 0;
            }

            try
            {
                int neg = GetString() == "N" ? 1 : -1;
                int deg = int.Parse(lat.Substring(0, 2));
                double min = double.Parse(lat.Substring(2));

                return (deg + min / 60) * neg;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public double GetLongitude()
        {
            string lng = GetString();
            if (string.IsNullOrEmpty(lng))
            {
                return 0;
            }

            try
            {
                int neg = GetString() == "E" ? 1 : -1;
                int deg = int.Parse(lng.Substring(0, 3));
                double min = double.Parse(lng.Substring(3));

                return (deg + min / 60) * neg;
            }
            catch (Exception)
            {
                return 0;
            }
        }


    }
}
