using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SharpAIS
{
    // Based on: http://home.mira.net/~gnb/gps/nmea.html
    public class GPRMCParser
    {
        private static string[] pattern = new string[] { 
            "Command:string",
            "Time:time",
            "SatelliteFixStatus:string",
            "LatitudeDecimalDegrees:double:100",
            "LatitudeHemisphere:string",
            "LongitudeDecimalDegrees:double:100",
            "LongitudeHemisphere:string",
            "Speed:double",
            "Bearing:double",
            "UTCDate:date",
            "MagneticVariationDegrees:double",
            "MagneticVariation:string",
            "ModeIndicator:string"
        };

        static GPRMCParser()
        { }

        public static Hashtable ParseSentence(string sentence)
        {
            CultureInfo culture = new CultureInfo("en-US");
            Hashtable returnData = new Hashtable();
            string[] data = sentence.Split(',');
            for (int i = 0; i < data.Length; i++)
            {
                string[] metadata = pattern[i].Split(':');
                string fieldName = metadata[0];
                string fieldType = metadata[1];
                double divider = 1.0;
                if (metadata.Length > 2)
                    divider = double.Parse(metadata[2], culture.NumberFormat);

                switch (fieldType)
                {
                    case "string":
                        returnData.Add(fieldName, data[i]);
                        break;

                    case "time":
                        returnData.Add(fieldName, new TimeSpan(int.Parse(data[i].Substring(0, 2)), int.Parse(data[i].Substring(2, 2)), int.Parse(data[i].Substring(4, 2))));
                        break;

                    case "double":
                        returnData.Add(fieldName, double.Parse(data[i], culture.NumberFormat) / divider);
                        break;

                    case "date":
                        returnData.Add(fieldName, new DateTime(2000  + int.Parse(data[i].Substring(4, 2)), int.Parse(data[i].Substring(2, 2)), int.Parse(data[i].Substring(0, 2))));
                        break;

                    default:
                        returnData.Add(fieldName, data[i]);
                        break;
                }
            }
            return returnData;
        }
    }
}
