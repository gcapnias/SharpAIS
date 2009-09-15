using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace AISSplit
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime currentTime = DateTime.Now;
            CultureInfo culture = new CultureInfo("en-US");

            if (File.Exists(args[0]))
            {
                FileInfo inComing = new FileInfo(args[0]);

                TextWriter outComingA = new StreamWriter(inComing.Name + "-A" + inComing.Extension);
                TextWriter outComingB = new StreamWriter(inComing.Name + "-B" + inComing.Extension);

                using (StreamReader sr = new StreamReader(args[0]))
                {
                    string textline;
                    while ((textline = sr.ReadLine()) != null)
                    {
                        if (textline.StartsWith("!A"))
                            outComingA.WriteLine(textline);
                        else if (textline.StartsWith("!B"))
                            outComingB.WriteLine(textline);
                        else
                        {
                            outComingA.WriteLine(textline);
                            outComingB.WriteLine(textline);
                        }
                    }
                }

                outComingA.Flush();
                outComingA.Close();
                outComingB.Flush();
                outComingB.Close();

                //Console.ReadKey();
            }
        }
    }
}
