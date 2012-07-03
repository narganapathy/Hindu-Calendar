using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Calender2.Data;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace Calender2
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for
    /// the currently selected item.
    /// </summary>
    public sealed partial class CitySelector : Calender2.Common.LayoutAwarePage
    {

        public CitySelector()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The
        /// Parameter property provides the group to be displayed.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
            // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]
            this.DefaultViewModel["Groups"] = e.Parameter;
            this.DefaultViewModel["CityOrStates"] = null;
            this.DefaultViewModel["Cities"] = null;
            // Select the first item automatically unless logical page navigation is
            // being used (see the logical page navigation #region below.)
            //if (!this.UsingLogicalPageNavigation()) this.itemsViewSource.View.MoveCurrentToFirst();
        }

        #region Logical page navigation

        // Visual state management typically reflects the four application view states directly
        // (full screen landscape and portrait plus snapped and filled views.)  The split page is
        // designed so that the snapped and portrait view states each have two distinct sub-states:
        // either the item list or the details are displayed, but not both at the same time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed, or null
        /// for the current view state.  This parameter is optional with null as the default
        /// value.</param>
        /// <returns>True when the view state in question is portrait or snapped, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
        {
            if (viewState == null) viewState = ApplicationView.Value;
            return viewState == ApplicationViewState.FullScreenPortrait ||
                viewState == ApplicationViewState.Snapped;
        }

        /// <summary>
        /// Invoked when an item within the list is selected.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is Snapped)
        /// displaying the selected item.</param>
        /// <param name="e">Event data that describes how the selection was changed.</param>
        void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Invalidate the view state when logical page navigation is in effect, as a change
            // in selection may cause a corresponding change in the current logical page.  When
            // an item is selected this has the effect of changing from displaying the item list
            // to showing the selected item's details.  When the selection is cleared this has the
            // opposite effect.
            if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();
        }

        /// <summary>
        /// Invoked when the page's back button is pressed.
        /// </summary>
        /// <param name="sender">The back button instance.</param>
        /// <param name="e">Event data that describes how the back button was clicked.</param>
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation() && continentListView.SelectedItem != null)
            {
                // When logical page navigation is in effect and there's a selected item that
                // item's details are currently displayed.  Clearing the selection will return to
                // the item list.  From the user's point of view this is a logical backward
                // navigation.
                this.continentListView.SelectedItem = null;
            }
            else
            {
                // When logical page navigation is not in effect, or when there is no selected
                // item, use the default back button behavior.
                base.GoBack(sender, e);
            }
        }

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// view state.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed.</param>
        /// <returns>The name of the desired visual state.  This is the same as the name of the
        /// view state except when there is a selected item in portrait and snapped views where
        /// this additional logical page is represented by adding a suffix of _Detail.</returns>
        protected override string DetermineVisualState(ApplicationViewState viewState)
        {
            // Update the back button's enabled state when the view state changes
            var logicalPageBack = this.UsingLogicalPageNavigation(viewState) && this.continentListView.SelectedItem != null;
            var physicalPageBack = this.Frame != null && this.Frame.CanGoBack;
            this.DefaultViewModel["CanGoBack"] = logicalPageBack || physicalPageBack;

            // Start with the default visual state name, and add a suffix when logical page
            // navigation is in effect and we need to display details instead of the list
            var defaultStateName = base.DetermineVisualState(viewState);
            return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
        }

        #endregion



        private void ContinentListView_Loaded(object sender, RoutedEventArgs e)
        {
            ListView view = sender as ListView;
            view.IsItemClickEnabled = true;
            Debug.WriteLine("COntinent listview loaded");
        }

        private void cityOrStateListView_Loaded_1(object sender, RoutedEventArgs e)
        {
            ListView view = sender as ListView;
            view.IsItemClickEnabled = true;
            Debug.WriteLine("City or state listview loaded");
        }

        private void cityListView_Loaded_1(object sender, RoutedEventArgs e)
        {
            ListView view = sender as ListView;
            view.IsItemClickEnabled = true;
            Debug.WriteLine("City listview loaded");
        }

        private async void cityListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            //if (e.AddedItems.Count == 0)
            //{
            //    Debug.WriteLine("city Count is zero");
            //    return;
            //}

            //if (cityListViewFirstTime == false)
            //{
            //    cityListViewFirstTime = true;
            //    return;
            //}

            CityItem cityItem = (e.ClickedItem) as CityItem;
            Debug.WriteLine("City clicked " + cityItem.Title + cityItem._city._UrlToken);
            City city = cityItem._city;
            await SampleDataSource.DataSource.ChangeCity(city._UrlToken);
            this.Frame.GoBack();
        }

        private async void cityOrStateListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            //if (cityOrStateListViewFirstTime == false)
            //{
            //    cityOrStateListViewFirstTime = true;
            //    return;
            //}
            CityOrStateItem cityOrStateItem = (e.ClickedItem) as CityOrStateItem;
            StateOrCity stateOrCity = cityOrStateItem._stateOrCity;
            if (stateOrCity is State)
            {
                Debug.WriteLine("is a state");
                Debug.WriteLine("State clicked " + cityOrStateItem.Title);
                this.DefaultViewModel["Cities"] = cityOrStateItem.Items;
            }
            else
            {
                City city = stateOrCity as City;
                Debug.WriteLine("not a state: City clicked" + cityOrStateItem.Title);
                this.DefaultViewModel["Cities"] = null;
                await SampleDataSource.DataSource.ChangeCity(city._UrlToken);
                this.Frame.GoBack();
            }
        }

        private void continentListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            SubcontinentGroup group = (e.ClickedItem) as SubcontinentGroup;
            Debug.WriteLine("Continent clicked " + group.Title);
            this.DefaultViewModel["CityOrStates"] = group.Items;
        }
    }
}
