using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Calender2.Data;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Calender2
{
    public sealed partial class DateItem : UserControl
    {
        int _dayOfMonth;

        public DateItem()
        {
            this.InitializeComponent();
            _dayOfMonth = 0;
        }

        
        public void SetDay(int dayVal, bool newMoonDay, bool fullMoonDay, String festivalString, String tithiString, String nakshatraString)
        {
            _dayOfMonth = dayVal;
            day.Text = dayVal.ToString();

            if (newMoonDay)
            {
                amavasya.Visibility = Visibility.Visible;
            }
            else
            {
                amavasya.Visibility = Visibility.Collapsed ;
            }
            if (fullMoonDay)
            {
                pournami.Visibility = Visibility.Visible;
            }
            else
            {
                pournami.Visibility = Visibility.Collapsed;
            }
            Calender2.App app = (Calender2.App)Application.Current;
            
            if (String.IsNullOrEmpty(festivalString) == false)
            {
                festival.Text = festivalString;
                festival.Visibility = Visibility.Visible;
                SolidColorBrush brush = (SolidColorBrush)app.Resources["DateItemFestivalBackgroundColor"];
                mainStackPanel.Background = brush;
            }
            else
            {
                festival.Visibility = Visibility.Collapsed;
                SolidColorBrush brush = (SolidColorBrush)app.Resources["DayItemBackGroundColor"];
                mainStackPanel.Background = brush;
            }

            if (String.IsNullOrEmpty(tithiString) == false)
            {
                //tithi.Text = tithiString;
                //tithi.Visibility = Visibility.Visible;
            }
            else
            {
                //tithi.Visibility = Visibility.Collapsed;
            }

            if (String.IsNullOrEmpty(nakshatraString) == false)
            {
                nakshatra.Text = nakshatraString;
                nakshatra.Visibility = Visibility.Visible;
            }
            else
            {
                nakshatra.Visibility = Visibility.Collapsed;
            }
        }

        public void SetPrivateEvent(String text)
        {
            if (text == String.Empty)
            {
                festival.Text = text;
                festival.Visibility = Visibility.Collapsed;
            }
            else
            {
                festival.Text = text;
                festival.Visibility = Visibility.Visible;
            }
        }

        public void SetDay(String text)
        {
            day.Text = text;
            day.FontSize = 30;
        }

        public void HighlightBorder(bool highlight)
        {
            Calender2.App app = (Calender2.App)Application.Current;
            SolidColorBrush brush;
            if (highlight)
            {
                brush = (SolidColorBrush)app.Resources["DateItemBorderHighlightColor"];
            }
            else
            {
                brush = (SolidColorBrush)app.Resources["DateItemBorderColor"];
            }
            dayBorder.BorderBrush = brush;
        }

        public int GetDay()
        {
            return _dayOfMonth;
        }
    }
}
