using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

using SharpAIS;
namespace AISApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int MaxLines = 20000;
            DateTime currentTime = DateTime.Now;
            CultureInfo culture = new CultureInfo("en-US");

            SharpAIS.Parser parser = new SharpAIS.Parser();
            if (File.Exists(args[0]))
            {
                int currentpart = 1;
                int currentline = 0;
                FileInfo inComing = new FileInfo(args[0]);
                string filename = string.Format("{0}-{1:00}{2}", inComing.Name.Replace(inComing.Extension, string.Empty), currentpart, inComing.Extension);
                Console.WriteLine(string.Format("Writing: {0}", filename));
                TextWriter outComing = new StreamWriter(filename);

                using (StreamReader sr = new StreamReader(args[0]))
                {
                    string textline;
                    while ((textline = sr.ReadLine()) != null)
                    {
                        if (currentline > MaxLines)
                        {
                            outComing.Flush();
                            outComing.Close();

                            currentpart++;
                            currentline = 0;
                            filename = string.Format("{0}-{1:00}{2}", inComing.Name.Replace(inComing.Extension, string.Empty) ,currentpart ,inComing.Extension);
                            Console.WriteLine(string.Format("Writing: {0}", filename));
                            outComing = new StreamWriter(filename);
                        }


                        Hashtable rs = parser.Parse(textline);
                        if (rs != null)
                        {
                            if (rs.ContainsKey("MessageID"))
                            {
                                switch ((int)rs["MessageID"])
                                {
                                    case 1:
                                    case 2:
                                    case 3:
                                        outComing.Write("{0}", currentTime.ToString("yyyy!MM!dd!HH!mm!ss!"));
                                        outComing.Write("{0}!{1}!{2}!{3}!{4}!{5}!{6}!{7}!{8}!{9}!{10}!{11}!{12}!{13}!{14}",
                                            1,
                                            rs["UserID"],
                                            rs["RepeatIndicator"],
                                            rs["NavigationalStatus"],
                                            rs["RateOfTurn"],
                                            rs["SpeedOverGround"],
                                            rs["PositionAccuracy"],
                                            string.Format(culture.NumberFormat, "{0:####.00000}", rs["Longitude"]),
                                            string.Format(culture.NumberFormat, "{0:####.00000}", rs["Latitude"]),
                                            string.Format(culture.NumberFormat, "{0:####.0}", rs["CourseOverGround"]),
                                            rs["TrueHeading"],
                                            rs["TimeStamp"],
                                            rs["SpecialManeuvreIndicator"],
                                            rs["RAIMFlag"],
                                            rs["Spare"]);
                                        outComing.WriteLine();
                                        currentline++;
                                        break;

                                    case 4:
                                    case 11:
                                        currentTime = new DateTime((int)rs["UTCYear"], (int)rs["UTCMonth"], (int)rs["UTCDay"], (int)rs["UTCHour"], (int)rs["UTCMinute"], (int)rs["UTCSecond"]);
                                        break;

                                    case 5:
                                        outComing.Write("{0}", currentTime.ToString("yyyy!MM!dd!HH!mm!ss!"));
                                        outComing.Write("{0}!{1}!{2}!{3}!{4}!{5}!{6}!{7}!{8}!{9}!{10}!{11}!{12}!{13}!{14}!{15}!{16}",
                                            rs["MessageID"],
                                            rs["UserID"],
                                            rs["RepeatIndicator"],
                                            rs["DSI"],
                                            rs["IMONumber"],
                                            ((string)rs["CallSign"]).Trim(),
                                            ((string)rs["Name"]).Trim(),
                                            rs["TypeOfShipAndCargoType"],
                                            rs["LengthFore"],
                                            rs["LengthAft"],
                                            rs["WidthPort"],
                                            rs["WidthStarboard"],
                                            rs["TypeOfElectronicPositionFixingDevice"],
                                            rs["MaximumPresentStaticDraught"],
                                            ((string)rs["Destination"]).Trim(),
                                            rs["DTE"],
                                            rs["Spare"]);
                                        outComing.WriteLine();
                                        currentline++;
                                        break;


                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                //outComing.WriteLine("{Error}");
                            }
                        }
                        else
                        {
                            //outComing.WriteLine("{No parse} " + textline);
                        }
                    }
                }

                outComing.Flush();
                outComing.Close();
                //Console.ReadKey();
            }
        }
    }
}
