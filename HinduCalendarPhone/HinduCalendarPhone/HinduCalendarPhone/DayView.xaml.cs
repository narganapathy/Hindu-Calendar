using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Diagnostics;

namespace HinduCalendarPhone
{
    public partial class DayView : PhoneApplicationPage
    {
        public DayView()
        {
            InitializeComponent();
        }

        DateTime _dt;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string dateString = "";

            if (NavigationContext.QueryString.TryGetValue("date", out dateString))
            {
                Debug.WriteLine("Date to be shown is " + dateString);
                PageTitle.Text = dateString;
                _dt = DateTime.Parse(dateString);
            } 
            else 
            {
                throw new ArgumentException();
            }
            
            ShowDetail();
            App app = Application.Current as App; ;
            app.MainPage.DayViewLoaded();
        }

        public void ShowDetail()
        {
            HinduCalendarPhone.App app = Application.Current as HinduCalendarPhone.App;
            PageTitle.Text = app.Calendar.CityToken;
            PanchangData pdata = app.Calendar.GetPanchangDataForDay(_dt.Year, _dt.Month, _dt.Day);
            DateTextBlock.Text = _dt.ToString("d");
            SunriseTextBlock.Text = pdata._fieldValues[(int)FieldType.Sunrise];
            SunsetTextBlock.Text = pdata._fieldValues[(int)FieldType.Sunset];
            MoonRiseTextBlock.Text = pdata._fieldValues[(int)FieldType.Moonrise];
            TamilYearTextBlock.Text = pdata._fieldValues[(int)FieldType.TamilYear];
            NorthYearTextBlock.Text = pdata._fieldValues[(int)FieldType.NorthYear];
            GujaratYearTextBlock.Text = pdata._fieldValues[(int)FieldType.GujaratYear];
            AyanaTextBlock.Text = pdata._fieldValues[(int)FieldType.Ayana];
            RituTextBlock.Text = pdata._fieldValues[(int)FieldType.Ritu];
            VedicRituTextBlock.Text = pdata._fieldValues[(int)FieldType.VedicRitu];
            TamilMonthTextBlock.Text = pdata._fieldValues[(int)FieldType.TamilMonth];
            // there is a whitespace in front of this string
            SanskritMonthTextBlock.Text = (pdata._fieldValues[(int)FieldType.SanskritMonth]).Trim();
            PakshaTextBlock.Text = (pdata._fieldValues[(int)FieldType.Paksha]).Trim();
            TithiTextBlock.Text = pdata._fieldValues[(int)FieldType.Tithi];
            NakshatraTextBlock.Text = pdata._fieldValues[(int)FieldType.Nakshatra];
            YogaTextBlock.Text = pdata._fieldValues[(int)FieldType.Yoga];
            KaranaTextBlock.Text = pdata._fieldValues[(int)FieldType.Karana];
            SunRasiTextBlock.Text = pdata._fieldValues[(int)FieldType.SunRasi];
            MoonRasiTextBlock.Text = pdata._fieldValues[(int)FieldType.MoonRasi];
            RahuKalamTextBlock.Text = pdata._fieldValues[(int)FieldType.RahuKalam];
            YamaKandamTextBlock.Text = pdata._fieldValues[(int)FieldType.YamaGandam];
            GulikaiTextBlock.Text = pdata._fieldValues[(int)FieldType.Gulikai];
            String festival = pdata._fieldValues[(int)FieldType.Festival];
            if (String.IsNullOrEmpty(festival))
            {
                festival = "No festival";
            }
            FestivalTextBlock.Text = festival;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ApplicationBarChangeCityButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ChangeCity.xaml", UriKind.Relative));
        }
    }
}