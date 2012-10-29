using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Syndication;
using Windows.Storage;
using Windows.Storage.Streams;


// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace Calender2.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : Calender2.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }
    }
    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }

        public void GetDateData(
                int month, 
                int day, 
                out bool isNewMoonDay, 
                out bool isFullMoonDay, 
                out String festival, 
                out String paksha, 
                out String nakshatra,
                out String tamilMonth)
        {
            YearlyPanchangData data = _group.PanchangDataForYear;
            isNewMoonDay = false;
            isFullMoonDay = false;
           
            Debug.Assert(data._panchangData[(month - 1) * 31 + day - 1].Day == day);
            Debug.Assert(data._panchangData[(month - 1) * 31 + day - 1].Month == month);
            if (data._panchangData[(month - 1) * 31 + day - 1]._fieldValues[(int)FieldType.Tithi] == "Amavasya")
            {
                isNewMoonDay =  true;
            }
            if (data._panchangData[(month - 1) * 31 + day - 1]._fieldValues[(int)FieldType.Tithi] == "Purnima")
            {
                isFullMoonDay = true;
            }
            festival = data._panchangData[(month - 1) * 31 + day - 1]._fieldValues[(int)FieldType.Festival];
            paksha = data._panchangData[(month-1)*31 + day -1]._fieldValues[(int)FieldType.Paksha];
            nakshatra = data._panchangData[(month - 1) * 31 + day - 1]._fieldValues[(int)FieldType.Nakshatra];
            tamilMonth = data._panchangData[(month - 1) * 31 + day - 1]._fieldValues[(int)FieldType.TamilMonth];
        }

        public bool IsNewMoonDay(int month, int day)
        {
            YearlyPanchangData data = _group.PanchangDataForYear;
            Debug.Assert(data._panchangData[(month - 1) * 31 + day - 1].Day == day);
            Debug.Assert(data._panchangData[(month - 1) * 31 + day - 1].Month == month);
            if (data._panchangData[(month-1)*31 + day -1]._fieldValues[(int)FieldType.Tithi] == "Amavasya")
            {
                return true;
            }
            return false;
        }
        
        public bool IsFullMoonDay(int month, int day)
        {
            YearlyPanchangData data = _group.PanchangDataForYear;
            if (data._panchangData[(month-1)*31 + day -1]._fieldValues[(int)FieldType.Tithi] == "Purnima")
            {
                return true;
            }
            return false;
        }

        public String GetFestival(int month, int day)
        {
            YearlyPanchangData data = _group.PanchangDataForYear;
            String festival = data._panchangData[(month - 1) * 31 + day - 1]._fieldValues[(int)FieldType.Festival];
            return festival;
        }

        public PanchangData GetPanchangData(int month, int day)
        {
            YearlyPanchangData data = _group.PanchangDataForYear;
            return data._panchangData[(month - 1) * 31 + day - 1];
        }
        
        public int Year
        {
            get { return _group.Year; }
        }

        public int Month
        {
            get { return int.Parse(Content); }
        }

    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }
        private YearlyPanchangData _yearlyPanchangData;
        private int _year;
        private City _city;

        public YearlyPanchangData PanchangDataForYear
        {
            get {return _yearlyPanchangData;}
            set {_yearlyPanchangData = value;}
        }
        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }
        public City  city
        {
            get { return _city; }
            set { _city = value; }
        }
    }

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

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class SampleDataSource
    {
        private ObservableCollection<SampleDataGroup> _itemGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> ItemGroups
        {
            get { return this._itemGroups; }
        }
        static String[] _monthStrings = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        YearlyPanchangData _calendarYearData;
        SampleDataGroup _group;
        String _cityToken = "Zurich-Switzerland";
        int _year = 2013;
        public static SampleDataSource _sampleDataSource;

        public static async Task InitializeCalendarData()
        {
            _sampleDataSource = new SampleDataSource();
            await _sampleDataSource.GetCalendarYearData();
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataSource DataSource
        {
            get { return _sampleDataSource; }
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            if (uniqueId.Equals("AllGroups"))
            {
                DateTime date = DateTime.Now;
                return _sampleDataSource.ItemGroups[0].Items[date.Month-1];
                //return _sampleDataSource.ItemGroups[0].Items[0];
            }

            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._itemGroups; }
        }

        private City GetCityInformation(String cityToken)
        {
            SubContinent[] subContinents = CityData.GetCityData();
            foreach (SubContinent subContinent in subContinents)
            {
                StateOrCity[] stateOrCityList = subContinent._stateOrCityList;
                for (int i = 0; i < stateOrCityList.Length; i++)
                {
                    StateOrCity stateOrCity = stateOrCityList[i];
                    bool isState = stateOrCity is State;

                    if (isState)
                    {
                        State state = stateOrCity as State;
                        foreach (City city in state._cities)
                        {
                            if (city._UrlToken == cityToken)
                            {
                                return city;
                            }
                        }
                    }
                    else
                    {
                        City city = stateOrCity as City;
                        if (city._UrlToken == cityToken)
                        {
                            return city;
                        }
                    }
                }
            }
            return null;
        }


        // Find and load the calendar data into memory.
        public async Task GetCalendarYearData()
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

                String fileName = String.Format("IndianCalendar-{0}-{1}.dat", _cityToken, _year);

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
                
                _group.PanchangDataForYear = _calendarYearData;
            }
            catch (Exception e)
            {
                Debug.WriteLine("GetCalenderYearData: " + e.Message);
            }
        }

        public static async Task ChangeCity(String cityToken)
        {
            _sampleDataSource._cityToken = cityToken;
            await _sampleDataSource.GetCalendarYearData();
            _sampleDataSource._group.city = _sampleDataSource.GetCityInformation(cityToken);
        }

        public SampleDataSource()
        {
            var group = new SampleDataGroup("Group-1",
                    "Group Title: 1",
                    "Group Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group.PanchangDataForYear = _calendarYearData;
            group.Year = _year;
            
            group.city = GetCityInformation(_cityToken); ;
            for (int month = 0; month < 12; month++)
            {
                var item = new SampleDataItem(_monthStrings[month],
                    _monthStrings[month],
                    _monthStrings[month],
                    "Assets/DarkGray.png",
                    "Group Description: {0} " + _monthStrings[month], 
                    (month+1).ToString(),
                    group);
                group.Items.Add(item);
            }
            this.ItemGroups.Add(group);
            _group = group;
        }
    }

    public sealed class MonthDataSource
    {
        private ObservableCollection<SampleDataGroup> _itemGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> ItemGroups
        {
            get { return this._itemGroups; }
        }

        public IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return this.AllGroups;
        }

        public SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = this.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = this.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._itemGroups; }
        }
         
        public MonthDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n",
                        "Item description");

            var group = new SampleDataGroup("Group-1",
                    "Group Title: 1",
                    "Group Subtitle: 1",
                    "Assets/DarkGray.png",
                    "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            for (int day = 0; day< 31; day++)
            {
                var item = new SampleDataItem(day.ToString(),
                    day.ToString(),
                    day.ToString(),
                    "Assets/DarkGray.png",
                    "Group Description:",
                    ITEM_CONTENT,
                    group);
                group.Items.Add(item);
            }
            this.ItemGroups.Add(group);
            
        }
    }
}
