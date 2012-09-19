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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string dateString = "";
            HinduCalendarPhone.App app = Application.Current as HinduCalendarPhone.App;

            if (NavigationContext.QueryString.TryGetValue("date", out dateString))
            {
                Debug.WriteLine("Date to be shown is " + dateString);
                PageTitle.Text = dateString;
                app.CurrentDate = DateTime.Parse(dateString);
            } 
            ShowDetail();
            if (app.FirstLaunch)
            {
                MessageBox.Show("Please set your city using the settings menu");
                app.FirstLaunch = false;
            }
        }

        public void ShowDetail()
        {
            HinduCalendarPhone.App app = Application.Current as HinduCalendarPhone.App;
            EventDate.Value = app.CurrentDate;
            PageTitle.Text = app.Calendar.CityName;
            PanchangData pdata = app.Calendar.GetPanchangDataForDay(app.CurrentDate.Year, app.CurrentDate.Month, app.CurrentDate.Day);
            DateTextBlock.Text = app.CurrentDate.ToString("d");
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


        private void ApplicationBarChangeCityButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ChangeCity.xaml", UriKind.Relative));
        }

        private void ApplicationBarHelpButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        private void EventDate_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            HinduCalendarPhone.App app = Application.Current as HinduCalendarPhone.App;
            if (e.NewDateTime.HasValue)
            {
                DateTime dt = e.NewDateTime.Value;
                if (dt.Year == 2012)
                {
                    app.CurrentDate = e.NewDateTime.Value;
                }
                else
                {
                    MessageBox.Show("Year should be equal to 2012");
                    EventDate.Value = DateTime.Today;
                }
            } 
        }
    }
}