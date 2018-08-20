using System;
using System.Collections.Generic;
using System.IO;


namespace Location.Gps
{
    
    internal class SentenceReader
    {
        private static Dictionary<string, Type> s_decoderMap = new Dictionary<string,Type>();

        static SentenceReader()
        {
            RegisterType("GGA", typeof(GGA));
            //RegisterType("GLL", typeof(GLL));
            //RegisterType("GSA",  typeof(GSA));
            RegisterType("RMC", typeof(RMC));
        }


        public static void RegisterType(string sentence, Type type)
        {
            //Debug.Assert(type.IsSubclassOf(typeof(IGpsData)));
            
            s_decoderMap[sentence] = type;
        }

        public static void UnregisterType(string sentence)
        {
            s_decoderMap.Remove(sentence);
        }


        public IGpsData Decode(TextReader reader)
        {
            Tokenizer tok = new Tokenizer(reader, ',');

            string type = tok.GetString();

            if (s_decoderMap.ContainsKey(type))
            {
                IGpsData obj = (IGpsData)Activator.CreateInstance(s_decoderMap[type]);
                obj.Decode(tok);

                return obj;
            }

            return null;

        }

        public IGpsData Decode(string str)
        {
            StringReader source = new StringReader(str);
            return Decode(source);
        }


    };
}
