using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpAIS
{
    public class Parser
    {

        public Hashtable Parse(string NMEASentence)
        {
            if (ChecksumIsCorrect(NMEASentence))
            {
                string[] sentencedata = NMEASentence.Split('*');
                string commandword = sentencedata[0].Substring(0, 6);
                switch (commandword)
                {
                    case "$GPRMC":
                        return GPRMCParser.ParseSentence(sentencedata[0]);

                    case "!AIVDM":
                    case "!AIVDO":
                    case "!BSVDM":
                    case "!BSVDO":
                        return AISParser.Instance.ParseSentence(sentencedata[0]);

                    default:
                        break;
                }
            }

            return null;
        }

        private bool ChecksumIsCorrect(string NMEASentence)
        {
            int iStart = 0;
            int iEnd = NMEASentence.IndexOf('*');

            //If start/stop isn't found it probably doesn't contain a checksum,
            //or there is no checksum after *. In such cases just return true.
            if (iStart >= iEnd || iEnd + 3 > NMEASentence.Length)
                return true;

            byte result = 0;
            for (int i = iStart + 1; i < iEnd; i++)
                result ^= (byte)NMEASentence[i];

            string check = result.ToString("X");
            if (check.Length == 1)
                check = "0" + check;

            return (check == NMEASentence.Substring(iEnd + 1, 2));
        }



 
    }
}
