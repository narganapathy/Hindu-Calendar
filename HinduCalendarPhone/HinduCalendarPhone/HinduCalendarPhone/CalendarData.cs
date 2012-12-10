using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Device.Location;
using System.IO;
using System.Collections.Generic;
using Calender2.Data;

namespace HinduCalendarPhone
{
    enum FieldType
    {
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
    public class PanchangData
    {
        [DataMember(Name = "Year")]
        public int Year;
        [DataMember(Name = "Month")]
        public int Month;
        [DataMember(Name = "Day")]
        public int Day;
        [DataMember(Name = "FieldValues")]
        public String[] _fieldValues;
        public PanchangData(int year, int month, int day)
        {
            _fieldValues = new String[(int)FieldType.MaxFields];
            Year = year;
            Day = day;
            Month = month;
        }
    };

    [DataContract(Name = "YearlyPanchangData", Namespace = "http://www.jyotishcalendar.com")]
    [KnownType(typeof(PanchangData))]
    public class YearlyPanchangData
    {
        [DataMember(Name = "PanchangDataArray")]
        public PanchangData[] _panchangData;
    }

    public class CalendarData
    {
        public CalendarData()
        {
            _calendarYearData = new Dictionary<int, YearlyPanchangData>();
        }

        String _cityToken = "CapeTown-SouthAfrica";
        String _cityName = "CapeTown, South Africa";
        Dictionary<int, YearlyPanchangData> _calendarYearData;
        
        public void GetCalendarDataForYear(int year)
        {
            String fileName = String.Format("Assets\\IndianCalendar-{0}-{1}.dat", _cityToken, year);
            var streamResourceInfo = App.GetResourceStream(new Uri(fileName, UriKind.Relative));
            using (var stream = streamResourceInfo.Stream)
            {
                using (var streamReader = new StreamReader(stream))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(YearlyPanchangData));
                    _calendarYearData.Remove(year);
                    _calendarYearData.Add(year, (YearlyPanchangData)ser.ReadObject(stream));
                }
            }
        }

        public void GetCalendarData(bool getLocation)
        {
            if (getLocation == true)
            {
                //MessageBoxResult result = MessageBox.Show("Please allow Hindu World Calendar to use your location to customize the calendar for your city",
                //                "Hindu World Calendar",
                //                MessageBoxButton.OKCancel);
                //if (result == MessageBoxResult.OK)
                {
                    GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
                    watcher.Start();
                    if (watcher.Permission == GeoPositionPermission.Granted)
                    {
                        System.Diagnostics.Debug.WriteLine("Location services status" + watcher.Status.ToString());
                        GeoCoordinate coord = watcher.Position.Location;
                        _cityToken = Calender2.Data.CityData.FindClosestCity(coord.Latitude, coord.Longitude);
                        City city = Calender2.Data.CityData.GetCityInformation(_cityToken);
                        UpdateCityToken(_cityToken, city._Name);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Location services not initilaized or permission not granted");
                    }
                }
            }
            GetCalendarDataForYear(2012);
            GetCalendarDataForYear(2013);
        }

        public void UpdateCityToken(String token, String name)
        {
            _cityToken = token;
            _cityName = name;
        }

        public void UpdateCityTokenAndGetData(String token, String name)
        {
            _cityToken = token;
            _cityName = name;
            GetCalendarData(false);
        }

        public String CityToken
        {
            get { return _cityToken; }
        }

        public String CityName
        {
            get { return _cityName; }
        }

        public PanchangData GetPanchangDataForDay(int year, int month, int day)
        {
             return _calendarYearData[year]._panchangData[(month - 1) * 31 + day - 1];
        }

    }

    [DataContract(Name = "PersistedData", Namespace = "http://www.jyotishcalendar.com")]
    public class PersistedData
    {
        [DataMember(Name = "Year")]
        public int Year;
        [DataMember(Name = "Month")]
        public int Month;
        [DataMember(Name = "Day")]
        public int Day;
        [DataMember(Name = "CityToken")]
        public string CityToken;
        [DataMember(Name = "CityName")]
        public string CityName;
        public PersistedData(int year, int month, int day, String cityToken, String cityName)
        {
            Year = year;
            Day = day;
            Month = month;
            CityToken = cityToken;
            CityName = cityName;
        }
    };
}
