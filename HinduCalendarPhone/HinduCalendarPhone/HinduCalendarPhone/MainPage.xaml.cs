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

namespace HinduCalendarPhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            CreateCalendarView();
        }

        String[] monthNames = {
                                  "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
                               };
        const int numMonths = 12;

        void CreateCalendarView()
        {
            PageTitle.Text = ((HinduCalendarPhone.App)(Application.Current)).Calendar.CityToken;
            for (int i = 0; i < numMonths; i++)
            {
                PivotItem pv = new PivotItem();
                pv.Header = monthNames[i];
                pv.FontSize = 10;
                CalendarPivot.Items.Add(pv);
                MonthView mv = new MonthView(i+1, this);
                pv.Content = mv;
                pv.Tag = (Object)(i+1);
            }
        }
    }
}