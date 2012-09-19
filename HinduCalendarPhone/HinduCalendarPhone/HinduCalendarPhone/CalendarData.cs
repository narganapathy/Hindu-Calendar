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
using System.IO;

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
        }

        String _cityToken = "Seattle-WA-USA";
        String _cityName = "Seattle, WA";
        int _year = 2012;
        YearlyPanchangData _calendarYearData;
        
        public void GetCalendarData()
        {
            String fileName = String.Format("Assets\\IndianCalendar-{0}-{1}.dat", _cityToken, _year);
            var streamResourceInfo = App.GetResourceStream(new Uri(fileName, UriKind.Relative));
            using (var stream = streamResourceInfo.Stream)
            {
                using (var streamReader = new StreamReader(stream))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(YearlyPanchangData));
                    _calendarYearData = (YearlyPanchangData)ser.ReadObject(stream);

                }
            }
        }

        public void UpdateCityToken(String token, String name)
        {
            _cityToken = token;
            _cityName = name;
            GetCalendarData();
            App app = Application.Current as App;
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
             return _calendarYearData._panchangData[(month - 1) * 31 + day - 1];
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
