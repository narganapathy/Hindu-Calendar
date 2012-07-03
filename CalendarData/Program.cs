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

            FileStream fs = new FileStream(fileName, FileMode.Create);
            DataContractSerializer ser = new DataContractSerializer(typeof(YearlyPanchangData));

            // Get the calendar data for every month
            for (int month = 1; month <= 12; month++)
            {
                String url = String.Format("http://www.mypanchang.com/vcalformat.php?cityname={2}&yr={0}&mn={1}&monthtype=0", Year, month, UrlToken);
                Console.WriteLine("Url to look at is {0}", url);
                HtmlDocument document = web.Load(url);
                day = 0;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//script"))
                {
                    HtmlNodeCollection coll = node.ChildNodes;
                    foreach (HtmlNode data in coll)
                    {
                        //Console.WriteLine(data.InnerText.Trim());
                        String input = data.InnerText;
                        String pattern = @"panData\[(\d\d)\] = ""([^""]+)"";";
                        foreach (Match match in Regex.Matches(input, pattern, RegexOptions.IgnoreCase))
                        {
                            //Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                            //Console.WriteLine(match.Groups[0].Value);
                            //Console.WriteLine(match.Groups[1].Value);
                            HtmlDocument doc = new HtmlDocument();
                            String dayData = match.Groups[2].Value;
                            // remove the bold elements to make it easier to parse
                            String dayData1 = Regex.Replace(dayData, "<b>", " ");
                            String dayData2 = Regex.Replace(dayData1, "</b>", " ");
                            //Console.WriteLine(dayData2);
                            doc.LoadHtml(dayData2);
                            PanchangData pData = new PanchangData(Year, month, day + 1);
                            currentIndex = 0;
                            CollectData(doc.DocumentNode, pData, scanPatterns);
                            panchangData[(month - 1)*31 + day] = pData;
                            day++;
                            pData._fieldValues[(int)FieldType.Festival] = GetFestivalData(Year, month, day, timeZone);
                            if (day > 30) break;
                        }
                        if (day > 30) break;
                    }
                    if (day > 30) break;
                }
            }
            ser.WriteObject(fs, yearPanchangData);
            //writer.Close();
            fs.Close();

            FileStream fs1 = new FileStream(fileName, FileMode.Open);
            DataContractSerializer ser1 = new DataContractSerializer(typeof(YearlyPanchangData));
            YearlyPanchangData yearPanchangDataCopy;
            yearPanchangDataCopy = (YearlyPanchangData)ser1.ReadObject(fs1);
            Console.WriteLine("Done");
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

                            GetCalendarDataPerCityAndYear(2012, city._Name, city._UrlToken, timeZone);
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
                        GetCalendarDataPerCityAndYear(2012, city._Name, city._UrlToken, timeZone);
                    }
                }
            }
        }
    }
}
