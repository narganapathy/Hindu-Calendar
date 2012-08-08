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
using System.Diagnostics;
using Microsoft.Phone.Controls;

namespace HinduCalendarPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        bool _dayViewNotLoaded = true;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            CreateCalendarView();
            App app = Application.Current as App;
            app.MainPage = this;
        }

        public void DayViewLoaded()
        {
            _dayViewNotLoaded = false;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_dayViewNotLoaded)
            {
                NavigationService.Navigate(new Uri("/DayView.xaml?date=" + DateTime.Today.ToShortDateString(), UriKind.Relative));
                Debug.WriteLine("Start of app");
            }
        }

        String[] monthNames = {
                                  "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
                               };
        const int numMonths = 12;

        void CreateCalendarView()
        {
            this.CityName.Text = ((HinduCalendarPhone.App)(Application.Current)).Calendar.CityToken;
            for (int i = 0; i < numMonths; i++)
            {
                PivotItem pv = new PivotItem();
                pv.Header = new PivotItemHeader(monthNames[i]);
                pv.FontSize = 10;

                CalendarPivot.Items.Add(pv);
                MonthView mv = new MonthView(i+1, this);
                pv.Content = mv;
                pv.Tag = (Object)(i+1);
            }
            CalendarPivot.SelectedIndex = DateTime.Today.Month - 1;
            CalendarPivot.SelectionChanged += new SelectionChangedEventHandler(CalendarPivot_SelectionChanged);
            // fake call this event to set the first month's names
            CalendarPivot_SelectionChanged(this, null);
        }

        // Get focus and update month texts for the correct month
        void CalendarPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pv = (PivotItem)CalendarPivot.SelectedItem;
            MonthView mv = pv.Content as MonthView;
            mv.UpdateMonthText();
        }

        private void ApplicationBarChangeCityButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ChangeCity.xaml", UriKind.Relative));
        }
    }
}