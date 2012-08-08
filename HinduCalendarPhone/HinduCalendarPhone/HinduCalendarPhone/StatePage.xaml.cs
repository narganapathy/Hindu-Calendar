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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Calender2.Data;
using System.Diagnostics;

namespace HinduCalendarPhone
{
    public partial class StatePage : PhoneApplicationPage
    {
        int _stateIndex, _contIndex;
        State _state;
        public StatePage()
        {
            InitializeComponent();
        }
        
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            String stateIndexString, contIndexString;
            if (NavigationContext.QueryString.TryGetValue("StateIndex", out stateIndexString))
            {
                _stateIndex = int.Parse(stateIndexString);
            }
            else
            {
                throw new ArgumentException();
            }
            if (NavigationContext.QueryString.TryGetValue("ContinentIndex", out contIndexString))
            {
                _contIndex = int.Parse(contIndexString);
            }
            else
            {
                throw new ArgumentException();
            }

            _state = Calender2.Data.CityData.GetCityData()[_contIndex]._stateOrCityList[_stateIndex] as State;
            PageTitle.Text = _state._Name;
            BuildCityList();
        }

        void BuildCityList()
        {
            for (int i = 0; i < _state._cities.Length; i++)
            {
                City city = _state._cities[i];
                ListBoxItem item = new ListBoxItem();
                TextBlock textBlock = new TextBlock();
                item.Content = textBlock;
                textBlock.Text = city._Name;
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
            String token = _state._cities[index]._UrlToken;
            String name  = _state._cities[index]._Name;
            App app = Application.Current as App;
            app.Calendar.UpdateCityToken(token, name);
            this.NavigationService.RemoveBackEntry();
            this.NavigationService.GoBack();
        }
    }
}