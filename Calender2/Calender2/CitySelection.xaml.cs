using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Calender2.Data;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Calender2
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class CitySelection : Calender2.Common.LayoutAwarePage
    {
        ItemDetailPage itemDetailPage;

        public CitySelection()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            SubContinent[] subContinents = Calender2.Data.CityData.GetCityData();
            for (int i = 0; i < subContinents.Length; i++)
            {
                ListBoxItem item = new ListBoxItem();
                TextBlock textBlock = new TextBlock();
                item.Content = textBlock;
                textBlock.Text = subContinents[i]._Name;
                SubContinentList.Items.Add(item);
                SubContinentList.Visibility = Visibility.Visible;
            }
            this.pageTitle.Text = "Change City";
            itemDetailPage = navigationParameter as ItemDetailPage;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void SubContinentList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            int index = SubContinentList.SelectedIndex;

            if (index == -1)
            {
                return;
            }

            SubContinent[] subContinents = Calender2.Data.CityData.GetCityData();
            SubContinent subContinent = subContinents[index];
            ListBox listBoxToUse;
            ScrollViewer scrollerToUse;

            if (subContinent._stateOrCityList[0] is State)
            {
                listBoxToUse = StateList;
                scrollerToUse = StateScroller;
            }
            else
            {
                // Hide state list
                StateScroller.Visibility = Visibility.Collapsed;
                listBoxToUse = CityList;
                scrollerToUse = CityScroller;
            }
            listBoxToUse.Items.Clear();
            listBoxToUse.Tag = subContinent;

            foreach (StateOrCity stateOrCity in subContinent._stateOrCityList)
            {
                ListBoxItem item = new ListBoxItem();
                TextBlock textBlock = new TextBlock();
                item.Content = textBlock;
                textBlock.Text = stateOrCity._Name;
                listBoxToUse.Items.Add(item);
            }
            scrollerToUse.Visibility = Visibility.Visible;
        }

        private void StateList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            int index = StateList.SelectedIndex;

            // ignore invalid index (caused by items.clear() below)
            if (index == -1)
            {
                return;
            }

            ListBox listBox = sender as ListBox;
            SubContinent subContinent = listBox.Tag as SubContinent;
            StateOrCity stateOrCity = subContinent._stateOrCityList[index];
            State state = stateOrCity as State;

            // StateList.Items.Clear();
            CityList.Items.Clear();
            CityList.Tag = state;
            foreach (City city in state._cities)
            {
                ListBoxItem item = new ListBoxItem();
                TextBlock textBlock = new TextBlock();
                item.Content = textBlock;
                textBlock.Text = city._Name;
                CityList.Items.Add(item);
                CityScroller.Visibility = Visibility.Visible;
            }
        }

        private async void CityList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            int index = CityList.SelectedIndex;
            if (index == -1)
            {
                return;
            }
            ListBox listBox = sender as ListBox;
            City city = null;
            if (listBox.Tag is State)
            {
                State state = listBox.Tag as State;
                city = state._cities[index];
            }
            if (listBox.Tag is SubContinent)
            {
                SubContinent subContinent = listBox.Tag as SubContinent;
                city = subContinent._stateOrCityList[index] as City;
            }
            await SampleDataSource.ChangeCity(city._UrlToken);
            // Use the navigation frame to return to the previous page
            itemDetailPage.UpdateTitle();
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }
    }
}
