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
using Calender2.Data;

namespace HinduCalendarPhone
{
    public partial class ChangeCity : PhoneApplicationPage
    {
        public ChangeCity()
        {
            InitializeComponent();
            CreateCityMenu();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        void CreateCityMenu()
        {
            SubContinent[] subContinents = CityData.GetCityData();
            for (int i = 0; i < subContinents.Length; i++)
            {
                PivotItem pv = new PivotItem();
                pv.Header = subContinents[i]._Name;
                pv.FontSize = 10;
                ChangeCityPivot.Items.Add(pv);
                CityStateList list = new CityStateList(subContinents[i]._stateOrCityList, this, i);
                pv.Content = list;
                pv.Tag = subContinents[i];
            }
        }
    }
}