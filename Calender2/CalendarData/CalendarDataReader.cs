using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace CalendarData
{
    public enum FieldType
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

    public class CalendarDataReader
    {
        YearlyPanchangData _calendarYearData;
        // Find and load the calendar data into memory.
        public async Task ReadCalendarYearData(String cityToken, int year)
        {
            try
            {
                StorageFolder folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var folderList = await folder.GetFoldersAsync();

                StorageFolder assetFolder = null;
                foreach (StorageFolder fo in folderList)
                {
                    if (fo.Name == "Assets")
                    {
                        assetFolder = fo;
                        break;
                    }
                }

                if (assetFolder == null)
                {
                    throw new InvalidOperationException();
                }

                String fileName = String.Format("IndianCalendar-{0}-{1}.dat", cityToken, year);

                // Debug code
                //var fileList = await assetFolder.GetFilesAsync();
                //foreach (StorageFile fi in fileList)
                //{
                //    Debug.WriteLine("File found is {0}", fi.Name);
                //}

                StorageFile file = await assetFolder.GetFileAsync(fileName);

                IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read);

                Stream stream = readStream.AsStreamForRead();

                DataContractSerializer ser = new DataContractSerializer(typeof(YearlyPanchangData));
                _calendarYearData = (YearlyPanchangData)ser.ReadObject(stream);
                Debug.Assert(_calendarYearData != null);
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetCalenderYearData: " + e.Message);
            }
        }

        public YearlyPanchangData CalendarYearData
        {
            get
            {
                return _calendarYearData;
            }
        }

        public PanchangData GetPanchangData(int month, int day)
        {
            return _calendarYearData._panchangData[(month - 1) * 31 + day - 1];
        }
    }
}
