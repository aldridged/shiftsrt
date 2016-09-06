using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Threading;
using System.IO;

namespace ShiftSrt
{
    class Program
    {
        // Program
        static void Main(string[] args)
        {
            try
            {
                // Check args for value
                if (args.Length < 2)
                {
                    Console.WriteLine("Arguments Missing - shiftsrt.exe <filename> <delta value> {<start point>}");
                    Console.WriteLine("Start and Delta Format - hh:mm:ss.ffffff");
                    Environment.Exit(0);
                }

                // Parse in Filename
                string fname = args[0];

                // Parse in Delta
                TimeSpan delta = TimeSpan.Parse(args[1]);  // hh:mm:ss.ffffff
                TimeSpan zerotime = TimeSpan.Parse("00:00:00");
                TimeSpan twoseconds = TimeSpan.Parse("00:00:02");

                // Parse in Start (if present)
                TimeSpan start = TimeSpan.Parse("00:00:00");
                if (args.Length > 2)
                {
                    start = TimeSpan.Parse(args[2]); // hh:mm:ss.ffffff
                }

                // Loop though file
                using (StreamReader sr = File.OpenText(fname))
                {
                    string s = String.Empty;
                    while ((s = sr.ReadLine()) != null)
                    {
                        // If line contains --> then parse and adjust
                        if (s.Contains("-->"))
                        {
                            string[] ss = s.Split(' ');
                            ss[0] = ss[0].Replace(',', '.') + "000";
                            ss[2] = ss[2].Replace(',', '.') + "000";

                            // Check for and handle malformed times
                            TimeSpan val1 = TimeSpan.Parse("00:00:00");
                            TimeSpan val2 = TimeSpan.Parse("00:00:00");
                            if(ss[0].Length == 15) val1 = TimeSpan.Parse(ss[0]);
                            if(ss[2].Length == 15) val2 = TimeSpan.Parse(ss[2]);
                            if (val1.Equals(zerotime) && val2.CompareTo(zerotime) > 0) val1 = val2.Subtract(twoseconds);
                            if (val2.Equals(zerotime) && val1.CompareTo(zerotime) > 0) val2 = val1.Add(twoseconds);

                            // If after starting point then adjust
                            if (val1.CompareTo(start) >= 0)
                            {
                                val1 = val1.Add(delta);
                                val2 = val2.Add(delta);
                                ss[0] = val1.ToString().Replace('.', ',');
                                if (ss[0].Contains(","))
                                {
                                    ss[0] = ss[0].Substring(0, ss[0].Length - 4);
                                }
                                else
                                {
                                    ss[0] = ss[0] + ",000";
                                }
                                ss[2] = val2.ToString().Replace('.', ',');
                                if (ss[2].Contains(","))
                                {
                                    ss[2] = ss[2].Substring(0, ss[2].Length - 4);
                                }
                                else
                                {
                                    ss[2] = ss[2] + ",000";
                                }
                                s = string.Join(" ", ss);
                            }
                        }

                        // Output line
                        Console.WriteLine(s);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured - " + ex.ToString());
            }
        }
    }
}
