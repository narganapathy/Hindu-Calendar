using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;
using HtmlAgilityPack;


namespace CalendarData
{
    
    class Program
    {
        enum FieldType {
             None,
             Sunrise,
             Sunset,
             Moonrise,
             TamilYear,
             NorthYear,
             GujaratYear,
             Ayana,
             Ritu,
             VedicRitu,
             TamilMonth,
             SanskritMonth,
             Paksha,
             Tithi,
             Nakshatra,
             Yoga,
             Karana,
             SunRasi,
             MoonRasi,
             RahuKalam,
             YamaGandam,
             Gulikai,
             Festival,
             MaxFields
        };



        [DataContract(Name = "PanchangData", Namespace = "http://www.jyotishcalendar.com")]
        class PanchangData
        {
            [DataMember(Name="Year")]
            public int Year;
            [DataMember(Name="Month")]
            public int Month;
            [DataMember(Name="Day")]
            public int Day;
            [DataMember(Name="FieldValues")]
            public String [] _fieldValues;
            public PanchangData(int year, int month, int day)
            {
                _fieldValues = new String[(int)FieldType.MaxFields];
                Year = year;
                Day = day;
                Month = month;
            }
        };

        [DataContract(Name ="YearlyPanchangData", Namespace = "http://www.jyotishcalendar.com")]
        [KnownType(typeof(PanchangData))]
        class YearlyPanchangData
        {
            [DataMember(Name="PanchangDataArray")]
            public PanchangData [] _panchangData;
        }

        struct PatternToScan
        {
            public String _RegExp; // Regexp to apply to this line
            public bool _Skip; // Skip this entry
            public FieldType _FieldType; // Type to initialize
            public PatternToScan(String regExp, bool skip, FieldType fieldType)
            {
                _RegExp = regExp;
                _Skip = skip;
                _FieldType = fieldType;
            }
        };

        static PatternToScan[] scanPatterns;
        static FestivalData[] festivalData;
        static void GetCalendarDataPerCityAndYear(int Year, String City, String UrlToken, TimeZoneValues timeZone)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();
            YearlyPanchangData yearPanchangData = new YearlyPanchangData();
            PanchangData[] panchangData = new PanchangData[12*31];
            yearPanchangData._panchangData = panchangData;
            int day = 0;
            String fileName = String.Format("IndianCalendar-{0}-{1}.dat", UrlToken, Year);
            Console.WriteLine("Filename is {0}", fileName);

            if (File.Exists(fileName))
            {
                Console.WriteLine(fileName + " Exists");
                return;
            }

            YearlyPanchangData yearPanchangDataCopy;
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Create);
                DataContractSerializer ser = new DataContractSerializer(typeof(YearlyPanchangData));

                // Get the calendar data for every month
                for (int month = 1; month <= 12; month++)
                {
                    String url = String.Format("http://www.mypanchang.com/phppanchang.php?yr={0}&cityhead=&cityname={2}&monthtype=0&mn={1}", Year, month-1, UrlToken);

                    HtmlDocument document = null;
                    try
                    {
                        document = web.Load(url);
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine("Load Failed "  + url + "\n" + exp.Message);
                        return;
                    }

                    day = 0;
                    PanchangData pData = null;
                    String log = null;
                    foreach (HtmlNode node in document.DocumentNode.SelectNodes("//table"))
                    {
                        HtmlNodeCollection coll = node.ChildNodes;
                        foreach (HtmlNode data in coll)
                        {
                            log += data.InnerText.Trim();
                            log += "\n";
                            log += "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++";
                            log += "\n";
                            //Console.WriteLine(log);
                            String input = data.InnerText;
                            String pattern1 = @"Panchang for";
                            Match match = Regex.Match(input, pattern1);
                            if (match.Success)
                            {
                                if (pData != null)
                                {
                                //    Console.WriteLine("Year {0} Month {1} Day {2}", Year, month, day);
                                    for (int j = 0; j < (int)(FieldType.MaxFields); j++)
                                    {
                                        if ((j != (int)FieldType.None) && (j != (int)FieldType.TamilYear) && (j != (int)FieldType.Festival))
                                        {
                                            if (pData._fieldValues[j] == null)
                                            {
                                                Console.WriteLine("Field Value {0} is null", ((FieldType)j).ToString());
                                                Console.WriteLine(log);
                                            }
                                        }

                                        //Console.WriteLine("{0}: {1}", (FieldType)j, pData._fieldValues[j]);
                                    }
                                    // Stash away the old data
                                    panchangData[(month - 1) * 31 + (day-1)] = pData;
                                };
                                // A new day got started.
                                pData = new PanchangData(Year, month, day + 1);
                                day++;
                                log = null;
                                //if (day > 30) break;
                            }

                            pattern1 = @"Shalivahan Shaka: (\d\d\d\d) \((\w+) Samvatsara\),\&nbsp;(\w+) Year \(North India\) (\d\d\d\d),\&nbsp;(\w+ \w+) \(Gujarat\) (\d\d\d\d),\&nbsp; Ayana:(\w+) \&nbsp;Ritu:(\w+),  Vedic Ritu:(\w+), Amavasyant   (\w+) (\w+) Paksha,\&nbsp;Tamil Month: (\w+)";
                            match = Regex.Match(input, pattern1);
                            currentIndex = 0;
                            if (match.Success)
                            {
                                pData._fieldValues[(int)FieldType.TamilYear] = match.Groups[2].Value + " " + match.Groups[1].Value;
                                pData._fieldValues[(int)FieldType.NorthYear] = match.Groups[3].Value + " " + match.Groups[4].Value;
                                pData._fieldValues[(int)FieldType.GujaratYear] = match.Groups[5].Value + " " + match.Groups[6].Value;
                                pData._fieldValues[(int)FieldType.Ayana] = match.Groups[7].Value;
                                pData._fieldValues[(int)FieldType.Ritu] = match.Groups[8].Value;
                                pData._fieldValues[(int)FieldType.VedicRitu] = match.Groups[9].Value;
                                pData._fieldValues[(int)FieldType.SanskritMonth] = match.Groups[10].Value;
                                pData._fieldValues[(int)FieldType.Paksha] = match.Groups[11].Value;
                                pData._fieldValues[(int)FieldType.TamilMonth] = match.Groups[12].Value;
                                pData._fieldValues[(int)FieldType.Festival] = null;
                            }

                            // @"Sunrise:07:36:10Sunset:17:08:38Moonrise:23:54:46";
                            String pattern2 = @"Sunrise:([\d:]+)Sunset:([\d:]+)Moonrise:([\d:]+)";
                            match = Regex.Match(input, pattern2);
                            if (match.Success)
                            {
                                pData._fieldValues[(int)FieldType.Sunrise] = match.Groups[1].Value;
                                pData._fieldValues[(int)FieldType.Sunset] = match.Groups[2].Value;
                                pData._fieldValues[(int)FieldType.Moonrise] = match.Groups[3].Value;
                            }

                            pattern2 = @"Sunrise:([\d:]+)Sunset:([\d:]+)Moonrise:None";
                            match = Regex.Match(input, pattern2);
                            if (match.Success)
                            {
                                pData._fieldValues[(int)FieldType.Sunrise] = match.Groups[1].Value;
                                pData._fieldValues[(int)FieldType.Sunset] = match.Groups[2].Value;
                                pData._fieldValues[(int)FieldType.Moonrise] = "None";
                            }

                            String pattern3 = @"Sun:(\w+)Entering";
                            match = Regex.Match(input, pattern3);
                            if (match.Success)
                            {
                                pData._fieldValues[(int)FieldType.SunRasi] = match.Groups[1].Value;
                            }

                            String pattern4 = @"Chandra:(\w+)Entering";
                            match = Regex.Match(input, pattern4);
                            if (match.Success)
                            {
                                pData._fieldValues[(int)FieldType.MoonRasi] = match.Groups[1].Value;
                            }

                            String pattern5 = @"Chandra:(\w+)Entering";
                            match = Regex.Match(input, pattern5);
                            if (match.Success)
                            {
                                pData._fieldValues[(int)FieldType.MoonRasi] = match.Groups[1].Value;
                            }

                            pattern1 = @"Tithi:([\w ]+)End time:[\w \d:+]+Nakshatra:([\w\.]+) ";
                            match = Regex.Match(input, pattern1);
                            if (match.Success)
                            {
                                pData._fieldValues[(int)FieldType.Tithi] = match.Groups[1].Value;
                                pData._fieldValues[(int)FieldType.Nakshatra] = match.Groups[2].Value;
                            }
                            //Rahukalam:11:10:50-12:22:24Yamagandam:14:45:31-15:57:05Gulikai:08:47:43-09:59:17Abhijit Muhurta:12:03:19-12:41:29
                            pattern1 = @"Rahukalam:([\d:-]+)Yamagandam:([\d:-]+)Gulikai:([\d:-]+)";
                            match = Regex.Match(input, pattern1);
                            if (match.Success)
                            {
                                pData._fieldValues[(int)FieldType.RahuKalam] = match.Groups[1].Value;
                                pData._fieldValues[(int)FieldType.YamaGandam] = match.Groups[2].Value;
                                pData._fieldValues[(int)FieldType.Gulikai] = match.Groups[2].Value;
                            }

                            pattern1 = @"Yoga:(\w+)End";
                            match = Regex.Match(input, pattern1);
                            if (match.Success)
                            {
                                pData._fieldValues[(int)FieldType.Yoga] = match.Groups[1].Value;
                            }

                            pattern1 = @"Karana:(\w+)End";
                            match = Regex.Match(input, pattern1);
                            if (match.Success)
                            {
                                pData._fieldValues[(int)FieldType.Karana] = match.Groups[1].Value;
                            }

                        }
                    }
                    //Console.WriteLine("Final day Year {0} Month {1} Day {2}", Year, month, day);
                    panchangData[(month - 1) * 31 + (day-1)] = pData;
                    for (int j = 0; j < (int)(FieldType.MaxFields); j++)
                    {
                        if ((j != (int)FieldType.None) && (j != (int)FieldType.TamilYear) && (j != (int)FieldType.Festival))
                        {
                            if (pData._fieldValues[j] == null)
                            {
                                Console.WriteLine("Field Value {0} is null", ((FieldType)j).ToString());
                                Console.WriteLine(log);
                            }
                        }

                        //Console.WriteLine("{0}: {1}", (FieldType)j, pData._fieldValues[j]);
                    }
                }
                ser.WriteObject(fs, yearPanchangData);
                fs.Close();

                FileStream fs1 = new FileStream(fileName, FileMode.Open);
                DataContractSerializer ser1 = new DataContractSerializer(typeof(YearlyPanchangData));
                yearPanchangDataCopy = (YearlyPanchangData)ser1.ReadObject(fs1);
            }
            catch (Exception e)
            {
                Console.Write("Web load problem " + e.Message);
            }

            // PrintPanchangData(panchangData);
        }

        static void OldGetCalendarDataPerCityAndYear(int Year, String City, String UrlToken, TimeZoneValues timeZone)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();
            YearlyPanchangData yearPanchangData = new YearlyPanchangData();
            PanchangData[] panchangData = new PanchangData[12*31];
            yearPanchangData._panchangData = panchangData;
            int day = 0;
            String fileName = String.Format("IndianCalendar-{0}-{1}.dat", UrlToken, Year);
            Console.WriteLine("Filename is {0}", fileName);

            if (File.Exists(fileName))
            {
                Console.WriteLine(fileName + " Exists");
                return;
            }

            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Create);
                DataContractSerializer ser = new DataContractSerializer(typeof(YearlyPanchangData));

                // Get the calendar data for every month
                for (int month = 1; month <= 12; month++)
                {
                    String url = String.Format("http://www.mypanchang.com/vcalformat.php?cityname={2}&yr={0}&mn={1}&monthtype=0", Year, month, UrlToken);

                    HtmlDocument document = null;
                    try
                    {
                        document = web.Load(url);
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine("Load Failed "  + url + "\n" + exp.Message);
                        return;
                    }

                    day = 0;
                    foreach (HtmlNode node in document.DocumentNode.SelectNodes("//table"))
                    {
                        HtmlNodeCollection coll = node.ChildNodes;
                        foreach (HtmlNode data in coll)
                        {
                            Console.WriteLine(data.InnerText.Trim());
                            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                            String input = data.InnerText;
                            String pattern = @"panData\[(\d\d)\] = ""([^""]+)"";";
                            foreach (Match match in Regex.Matches(input, pattern, RegexOptions.IgnoreCase))
                            {
                                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                                Console.WriteLine(match.Groups[0].Value);
                                Console.WriteLine(match.Groups[1].Value);
                                HtmlDocument doc = new HtmlDocument();
                                String dayData = match.Groups[2].Value;
                                 // remove the bold elements to make it easier to parse
                                String dayData1 = Regex.Replace(dayData, "<b>", " ");
                                String dayData2 = Regex.Replace(dayData1, "</b>", " ");
                                Console.WriteLine(dayData2);
                                doc.LoadHtml(dayData2);
                                PanchangData pData = new PanchangData(Year, month, day + 1);
                                currentIndex = 0;
                                CollectData(doc.DocumentNode, pData, scanPatterns);
                                panchangData[(month - 1) * 31 + day] = pData;
                                day++;
                                pData._fieldValues[(int)FieldType.Festival] = GetFestivalData(Year, month, day, timeZone);
                                if (day > 30) break;
                            }
                            if (day > 30) break;
                        }
                        if (day > 30) break;
                    }
                }
                //ser.WriteObject(fs, yearPanchangData);
                //writer.Close();
                //fs.Close();

                //FileStream fs1 = new FileStream(fileName, FileMode.Open);
                //DataContractSerializer ser1 = new DataContractSerializer(typeof(YearlyPanchangData));
                //YearlyPanchangData yearPanchangDataCopy;
                //yearPanchangDataCopy = (YearlyPanchangData)ser1.ReadObject(fs1);
                //Console.WriteLine("Done");
            }
            catch (Exception e)
            {
                Console.Write("Web load problem " + e.Message);
            }
            // PrintPanchangData(panchangData);
        }
        static String GetFestivalData(int year, int month, int day, TimeZoneValues timeZone)
        {
            if (timeZone == TimeZoneValues.Unknown)
            {
                return null;
            }
            Console.WriteLine("Timezone is + {0}", timeZone.ToString());
            DateTime date = new DateTime(year, month, day);
            foreach (FestivalData fest in festivalData)
            {
                String s = fest._Date[(int)timeZone];
                if (DateTime.Parse(s) == date)
                {
                    Console.WriteLine(fest._Festival +  " " + date.ToString());
                    return fest._Festival;
                }
            }
            return null;
        }

        static void PrintPanchangData(PanchangData[,] panchangData)
        {
            for (int month = 0; month < 12; month++)
            {
                for (int day = 0; day < 31; day++)
                {
                    Console.WriteLine("Month {1} Day {0} +++++++++++++++++++++++++++++++++++++++++++", day, month);
                    for (int j = 0; j < (int)(FieldType.MaxFields); j++)
                    {
                        Console.WriteLine("{0}: {1}", (FieldType)j, (panchangData[month,day] != null) ? panchangData[month,day]._fieldValues[j]: "None");
                    }
                }
            }
        }

        static int currentIndex = 0;
        static void CollectData(HtmlNode nodeToPrint, PanchangData panchangData, PatternToScan[] scanPatterns)
        {
            if (nodeToPrint.HasChildNodes)
            {
                foreach (HtmlNode subNode in nodeToPrint.ChildNodes)
                {
                    CollectData(subNode, panchangData, scanPatterns);
                }
            }
            else
            {
                // If we have looked at all the patterns we are interested in skip.
                if (currentIndex >= scanPatterns.Length)
                {
                    return;
                }

                PatternToScan scanPattern = scanPatterns[currentIndex];
                //Console.WriteLine("currentIndex {0}: {1} \n skip {2} pattern {3} fieldType {4}",
                 //   currentIndex, nodeToPrint.InnerText, scanPattern._Skip, scanPattern._RegExp, scanPattern._FieldType.ToString());
                if (scanPattern._Skip == false)
                {
                    String input = nodeToPrint.InnerText;
                    String pattern = scanPattern._RegExp;
                    String output;
                    output = null;
                    if (pattern == null)
                    {
                        output = input;
                    }
                    else
                    {
                        Match match = Regex.Match(input, pattern);
                        if (match.Success)
                        {
                            output = match.Groups[1].Value;
                        }
                    }

                    if (output != null)
                    {
                        //Console.WriteLine(output);
                        panchangData._fieldValues[(int)scanPattern._FieldType] = output;
                        currentIndex++;
                    }
                    //Console.WriteLine("========================================================================");
                    // if we get an invalid pattern. Skip and go to the next node and try for the same match
                }
                else
                {
                    currentIndex++;
                }
                
            }
        }

        static void Main(string[] args)
        {
            scanPatterns = new PatternToScan[] {
                                new PatternToScan(@"Sunrise:\s+([\d:]+)", false, FieldType.Sunrise),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Sunset:\s+([\d:]+)", false, FieldType.Sunset),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Moonrise:\s+([\w\d:]+)", false, FieldType.Moonrise),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.TamilYear),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.NorthYear),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.GujaratYear),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Ayana:(\w+)", false, FieldType.Ayana),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Ritu:(\w+)", false, FieldType.Ritu),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Vedic Ritu:(\w+)", false, FieldType.VedicRitu),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.TamilMonth),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.SanskritMonth),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.Paksha),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"T:\s+(\w+)", false, FieldType.Tithi),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"N:\s+(\S+)", false, FieldType.Nakshatra),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Y:\s+(\w+)", false, FieldType.Yoga),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"K:\s+(\w+)", false, FieldType.Karana),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Sun\s+:\s(\w+)", false, FieldType.SunRasi),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Moon\s+:\s(\w+)", false, FieldType.MoonRasi),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"RK:\s+([\d:-]+)", false, FieldType.RahuKalam),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"YM:\s+([\d:-]+)", false, FieldType.YamaGandam),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"GK:\s+([\d:-]+)", false, FieldType.Gulikai),
                                // spare values
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None)
                            };

            SubContinent[] subContinents = CityData.GetCityData();
            festivalData = FestivalDataGetter.GetFestivalData();
            	
            //GetCalendarDataPerCityAndYear(2013, "Seattle, WA", "Seattle-WA-USA", TimeZoneValues.PST);

            // Uncomment to generate latlong data
            //CalendarData.CityData.FindClosestCity(0, 0);
            //CalculateLatLong(subContinents);

            foreach (SubContinent subcontinent in subContinents)
            {
                TimeZoneValues timeZone;
                if (subcontinent._stateOrCityList[0] is State)
                {
                    State[] stateList = subcontinent._stateOrCityList as State[];
                    foreach (State state in stateList)
                    {
                        foreach (City city in state._cities)
                        {
                            if (subcontinent._timeZone != TimeZoneValues.Unknown)
                            {
                                timeZone = subcontinent._timeZone;
                            }
                            else if (state._timeZone != TimeZoneValues.Unknown)
                            {
                                timeZone = state._timeZone;
                            }
                            else
                            {
                                timeZone = city._timeZone;
                            }

                            GetCalendarDataPerCityAndYear(2013, city._Name, city._UrlToken, timeZone);
                        }
                    }
                }
                else
                {
                    City[] cityList = subcontinent._stateOrCityList as City[];
                    foreach (City city in cityList)
                    {
                        if (subcontinent._timeZone != TimeZoneValues.Unknown)
                        {
                            timeZone = subcontinent._timeZone;
                        }
                        else
                        {
                            timeZone = city._timeZone;
                        }
                        GetCalendarDataPerCityAndYear(2013, city._Name, city._UrlToken, timeZone);
                    }
                }
            }
        }

        static void OldMain(string[] args)
        {

            scanPatterns = new PatternToScan[] {
                                new PatternToScan(@"Sunrise:\s+([\d:]+)", false, FieldType.Sunrise),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Sunset:\s+([\d:]+)", false, FieldType.Sunset),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Moonrise:\s+([\w\d:]+)", false, FieldType.Moonrise),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.TamilYear),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.NorthYear),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.GujaratYear),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Ayana:(\w+)", false, FieldType.Ayana),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Ritu:(\w+)", false, FieldType.Ritu),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Vedic Ritu:(\w+)", false, FieldType.VedicRitu),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.TamilMonth),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.SanskritMonth),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, false, FieldType.Paksha),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"T:\s+(\w+)", false, FieldType.Tithi),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"N:\s+(\S+)", false, FieldType.Nakshatra),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Y:\s+(\w+)", false, FieldType.Yoga),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"K:\s+(\w+)", false, FieldType.Karana),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Sun\s+:\s(\w+)", false, FieldType.SunRasi),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"Moon\s+:\s(\w+)", false, FieldType.MoonRasi),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"RK:\s+([\d:-]+)", false, FieldType.RahuKalam),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"YM:\s+([\d:-]+)", false, FieldType.YamaGandam),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(@"GK:\s+([\d:-]+)", false, FieldType.Gulikai),
                                // spare values
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None),
                                new PatternToScan(null, true, FieldType.None)
                            };
            SubContinent[] subContinents = CityData.GetCityData();
            festivalData = FestivalDataGetter.GetFestivalData();
            	

            //CalendarData.CityData.FindClosestCity(0, 0);
            //CalculateLatLong(subContinents);

            foreach (SubContinent subcontinent in subContinents)
            {
                TimeZoneValues timeZone;
                if (subcontinent._stateOrCityList[0] is State)
                {
                    State[] stateList = subcontinent._stateOrCityList as State[];
                    foreach (State state in stateList)
                    {
                        foreach (City city in state._cities)
                        {
                            if (subcontinent._timeZone != TimeZoneValues.Unknown)
                            {
                                timeZone = subcontinent._timeZone;
                            }
                            else if (state._timeZone != TimeZoneValues.Unknown)
                            {
                                timeZone = state._timeZone;
                            }
                            else
                            {
                                timeZone = city._timeZone;
                            }

                            GetCalendarDataPerCityAndYear(2013, city._Name, city._UrlToken, timeZone);
                        }
                    }
                }
                else
                {
                    City[] cityList = subcontinent._stateOrCityList as City[];
                    foreach (City city in cityList)
                    {
                        if (subcontinent._timeZone != TimeZoneValues.Unknown)
                        {
                            timeZone = subcontinent._timeZone;
                        }
                        else
                        {
                            timeZone = city._timeZone;
                        }
                        GetCalendarDataPerCityAndYear(2013, city._Name, city._UrlToken, timeZone);
                    }
                }
            }
        }

        class CountryCode
        {
            public String _countryName;
            public String _countryCode;
            public CountryCode(String countryName, String countryCode)
            {
                _countryCode = countryCode;
                _countryName = countryName;
            }
        }

        static void CalculateLatLong(SubContinent[] subContinents)
        {

            CountryCode[] countryCodes = {
                                    new CountryCode("Canada", "CA"),
                                    new CountryCode("Columbia", "CO"),
                                    new CountryCode("Argentina","AR"),
                                    new CountryCode("Surinam","SR"),
                                    new CountryCode("Congo/Zaire", "CG"),
                                    new CountryCode("Egypt", "EG"),
                                    new CountryCode("Tanzania", "TZ"),
                                    new CountryCode("Kenya", "KE"),
                                    new CountryCode("South Africa", "ZA"),
                                    new CountryCode("Zambia", "ZM"),
                                    new CountryCode("Mauritius","MU"),
                                    new CountryCode("Mahe", "SC"),
                                    new CountryCode("Barbados","BB"),
                                    new CountryCode("Guyana","GY"),
                                    new CountryCode("Jamaica","JM"),
                                    new CountryCode("Bahamas","BS"),
                                    new CountryCode("Trinidad and Tobago","TT"),
                                    new CountryCode("Australia","AU"),
                                    new CountryCode("India","IN"),
                                    new CountryCode("Pakistan","PK"),
                                    new CountryCode("New Zealand","NZ"),
                                    new CountryCode("Fiji","FJ"),
                                    new CountryCode("Scotland (UK)","B"),
                                    new CountryCode("Germany","DE"),
                                    new CountryCode("Poland","PL"),
                                    new CountryCode("Netherlands","NL"),
                                    new CountryCode("Ireland","IE"),
                                    new CountryCode("UK", "GB"),
                                    new CountryCode("Ukraine","UA"),
                                    new CountryCode("Finland","FI"),
                                    new CountryCode("Italy", "IT"),
                                    new CountryCode("Spain","ES"),
                                    new CountryCode("Norway","NO"),
                                    new CountryCode("Sweden", "SE"),
                                    new CountryCode("Switzerland","CH"),
                                    new CountryCode("Denmark","DK"),
                                    new CountryCode("Qatar","QA"),
                                    new CountryCode("UAE","AE"),
                                    new CountryCode("Bahrain", "BH"),
                                    new CountryCode("Oman","OM"),
                                    new CountryCode("Yemen", "YE"),
                                    new CountryCode("Iran","IR"),
                                    new CountryCode("Kuwait","KW"),
                                    new CountryCode("Saudi Arabia","SA"),
                                    new CountryCode("SriLanka","LK"),
                                    new CountryCode("Philippines","PH"),
                                    new CountryCode("Indonesia","ID"),
                                    new CountryCode("Bangladesh","BD"),
                                    new CountryCode("Vietnam", "VN"),
                                    new CountryCode("China", "CN"),
                                    new CountryCode("Malaysia","MY"),
                                    new CountryCode("Singapore", "SG"),
                                    new CountryCode("Japan", "JP"),
                                    new CountryCode("Mongolia","MN"),
                                    new CountryCode("Martinique","MQ"),
                };


            String patternToMatch = @"([\w\s]+),([\w\s]+)";
            foreach (SubContinent subcontinent in subContinents)
            {
                if (subcontinent._stateOrCityList[0] is State)
                {
                    State[] stateList = subcontinent._stateOrCityList as State[];
                    foreach (State state in stateList)
                    {
                        foreach (City city in state._cities)
                        {
                            Match match = Regex.Match(city._Name, patternToMatch);
                            if (match.Success)
                            {
                                GetLatLong("US", match.Groups[2].ToString().Trim(), match.Groups[1].ToString().Trim(), city._UrlToken);
                            }
                            else
                            {
                                Console.WriteLine("No match for " + city._Name);
                            }
                        }
                    }
                }
                else
                {
                    City[] cityList = subcontinent._stateOrCityList as City[];
                    foreach (City city in cityList)
                    {
                        Match match = Regex.Match(city._Name, patternToMatch);
                        if (match.Success)
                        {
                            if (String.Equals(subcontinent._Name, "India", StringComparison.CurrentCultureIgnoreCase))
                            {
                                GetLatLong("IN", String.Empty, match.Groups[1].ToString(), city._UrlToken);
                            }
                            else
                            {
                                foreach (CountryCode code in countryCodes)
                                {
                                    if (String.Equals(code._countryName, match.Groups[2].ToString().Trim(), StringComparison.CurrentCultureIgnoreCase) == true)
                                    {
                                        GetLatLong(code._countryCode, String.Empty, match.Groups[1].ToString(), city._UrlToken);
                                    }
                                }
                            }
                        }
                        else
                        {
                            String secondPattern = @"([\w]+)";
                            Match match1 = Regex.Match(city._Name, secondPattern);
                            if (match1.Success)
                            {
                                if (String.Equals(subcontinent._Name, "India", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    GetLatLong("IN", String.Empty, match1.Groups[1].ToString(), city._UrlToken);
                                }
                                else
                                {
                                    Console.WriteLine("No match" + city._Name);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No match" + city._Name);
                            }
                        }
                    }
                }
            }
            //GetLatLong("US", "CA", "Los Angeles", "LosAngeles-CA");
            //GetLatLong("CN", String.Empty, "HongKong", "HongKong-China");
        }

        static void GetLatLong(String country, String state, String city, String UrlToken)
        {
            //Console.WriteLine("{0} {1} {2} {3}", country, state, city, UrlToken);

            String bingMapsKey = "As4YEtofwNVwMPinIlG_LQlwb-rYVVvrnqZ25ZRivjdb8rEMPyBToNFVhRlHuABZ";
            String url;
            if (String.Empty != state)
            {
                url = String.Format("http://dev.virtualearth.net/REST/v1/Locations?countryRegion={0}&adminDistrict={1}&locality={2}&o=xml&key={3}",
                                country, state, city, bingMapsKey);
            }
            else
            {
                url = String.Format("http://dev.virtualearth.net/REST/v1/Locations?countryRegion={0}&locality={1}&o=xml&key={2}",
                                country, city, bingMapsKey);
            }
            //Console.WriteLine(url);
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            Stream respStream = wr.GetResponse().GetResponseStream();
            StreamReader r = new StreamReader(respStream);
            String xmlString = r.ReadToEnd();
            //Console.WriteLine(xmlString);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);
            XmlNodeList list = xmlDoc.GetElementsByTagName("EstimatedTotal");
            String total = list[0].InnerXml;
            if (int.Parse(total) == 0)
            {
                Console.WriteLine("No data for {0}", UrlToken);
                return;
            }
            list = xmlDoc.GetElementsByTagName("Latitude");
            String latitude = list[0].InnerXml;
            list = xmlDoc.GetElementsByTagName("Longitude");
            String longtitude = list[1].InnerXml;
            Console.WriteLine(" new LatLong(\"{0}\", {1}, {2});", UrlToken, latitude, longtitude);
        }
    }
}
