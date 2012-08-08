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

namespace HinduCalendarPhone
{
    public partial class DateItem : UserControl
    {
        int _dayOfMonth;

        public DateItem()
        {
            InitializeComponent();
        }

        public void SetDay(int day, String Text)
        {
            _dayOfMonth = day;
            DayTextBlock.Text = Text;
        }

        public int GetDay()
        {
            return _dayOfMonth;
        }

        public void Highlight(bool hl)
        {
            App app = (HinduCalendarPhone.App)Application.Current;
            SolidColorBrush brush;
            if (hl)
            {
                brush = (SolidColorBrush)app.Resources["PhoneAccentBrush"];
            }
            else
            {
                brush = (SolidColorBrush)app.Resources["PhoneForegroundBrush"];
            }
            DayTextBlock.Foreground = brush;
        }
    }
}
