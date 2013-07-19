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
    public partial class Help : PhoneApplicationPage
    {
        HinduCalendarPhone.App _app;

        public Help()
        {
            InitializeComponent();
            this.toggleSwitch.Checked += new EventHandler<RoutedEventArgs>(toggle_Checked);
            this.toggleSwitch.Unchecked += new EventHandler<RoutedEventArgs>(toggle_Unchecked);
            _app = Application.Current as HinduCalendarPhone.App;
            this.toggleSwitch.IsChecked = _app.Calendar.UseLocation;
        }
 
        void toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            this.toggleSwitch.Content = "Location is off";
            this.toggleSwitch.SwitchForeground = new SolidColorBrush(Colors.Red);
            _app.Calendar.UseLocation = false;
        }
         
        void toggle_Checked(object sender, RoutedEventArgs e)
        {
            this.toggleSwitch.Content = "Location is on";
            this.toggleSwitch.SwitchForeground = new SolidColorBrush(Colors.Green);
            _app.Calendar.UseLocation = true;
            _app.Calendar.GetCalendarData(true);
        }
    }
}