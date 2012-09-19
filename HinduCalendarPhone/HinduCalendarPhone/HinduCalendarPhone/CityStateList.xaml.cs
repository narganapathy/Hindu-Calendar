using System;
using System.Diagnostics;
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
using System.Windows.Navigation;
using Calender2.Data;

namespace HinduCalendarPhone
{
    public partial class CityStateList : UserControl
    {
        ChangeCity _changeCityPage;
        StateOrCity[] _stateOrCityList;
        int _subContinentIndex;

        public CityStateList(StateOrCity[] stateOrCityList, ChangeCity changeCityPage, int subContinentIndex)
        {
            InitializeComponent();
            _changeCityPage = changeCityPage;
            _stateOrCityList = stateOrCityList;
            _subContinentIndex = subContinentIndex;
            CreateCityStateList(stateOrCityList);
        }

        void CreateCityStateList(StateOrCity[] stateOrCityList)
        {
            for (int i = 0; i < stateOrCityList.Length; i++)
            {
                StateOrCity stateOrCity = stateOrCityList[i];
                ListBoxItem item = new ListBoxItem();
                TextBlock textBlock = new TextBlock();
                item.Content = textBlock;
                textBlock.Text = stateOrCity._Name;
                textBlock.Style = Application.Current.Resources["CityStateListText"] as Style;
                CityStateListBox.Items.Add(item);
                item.Tag = i;
            }
        }

        private void CityStateListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = CityStateListBox.SelectedIndex;

            // ignore invalid index
            if (index == -1)
            {
                return;
            }

            ListBoxItem item = CityStateListBox.SelectedItem as ListBoxItem;
            City city = null;
            if (_stateOrCityList[index] is State)
            {
                State state = _stateOrCityList[index] as State;
                _changeCityPage.NavigationService.Navigate(new Uri("/StatePage.xaml?StateIndex=" + index + 
                            "&ContinentIndex=" + _subContinentIndex, UriKind.Relative));
            }
            else
            {
                city = _stateOrCityList[index] as City;
                String token = city._UrlToken;
                App app = Application.Current as App;
                app.Calendar.UpdateCityToken(token, city._Name);
                _changeCityPage.NavigationService.GoBack();
            }
        }
    }
}
