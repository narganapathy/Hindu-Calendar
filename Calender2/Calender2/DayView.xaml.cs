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
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Syndication;
using Calender2.Data;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace Calender2
{
    public class DayViewParameters
    {
        public DayViewParameters(int month, int day, SampleDataItem item)
        {
            _month = month;
            _day = day;
            _item = item;
        }
        public int _month;
        public int _day;
        public SampleDataItem _item;
    }

    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class DayView : Calender2.Common.LayoutAwarePage
    {
        MonthDataSource _monthDataSource;
        int             _currentMonth;
        SampleDataItem _sampleItemDataForMonth;
        
        static String[] _monthStrings = { "Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public DayView()
        {
            // Disable the previous and next App Bar buttons initially
            this.DefaultViewModel["CanFlipNext"] = false;
            this.DefaultViewModel["CanFlipPrevious"] = false;

            this.InitializeComponent();
            _monthDataSource = new MonthDataSource();
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
            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            var pData = (DayViewParameters)navigationParameter;
            _currentMonth = pData._month;
            _sampleItemDataForMonth = pData._item;
            this.DefaultViewModel["Group"] = _monthDataSource.ItemGroups[0];
            this.DefaultViewModel["Items"] = _monthDataSource.ItemGroups[0].Items;
            this.pageTitle.Text = _monthStrings[_currentMonth - 1] + " " + pData._day + " " + _sampleItemDataForMonth.Year + " For " + _sampleItemDataForMonth.Group.city._Name;
            this.flipView.SelectedItem = _monthDataSource.ItemGroups[0].Items[pData._day - 1];
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var item = (SampleDataItem)this.flipView.SelectedItem;
            pageState["SelectedItem"] = item.UniqueId;
        }
        
        void flipView_Loaded(object sender, RoutedEventArgs e)
        {
            FlipView flipView = sender as FlipView;
            SampleDataItem item = this.flipView.Items[this.flipView.SelectedIndex] as SampleDataItem;
            FlipViewItem selectedFlipViewItem = (FlipViewItem)this.flipView.ItemContainerGenerator.ContainerFromIndex(flipView.SelectedIndex);
            if (selectedFlipViewItem != null)
            {
                DisplayFeed(flipView.SelectedIndex + 1, _sampleItemDataForMonth, selectedFlipViewItem);
            }
        }

        public DependencyObject FindNamedElement(DependencyObject element, string name)
        {
            string elementName = (string)element.GetValue(FrameworkElement.NameProperty);
            if (elementName == name)
                return element;

            int childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                var resultFromChild = FindNamedElement(VisualTreeHelper.GetChild(element, i), name);
                if (resultFromChild != null)
                    return resultFromChild;
            }

            return null;
        }
        #region Support for the Previous and Next App Bar Buttons

        /// <summary>
        /// Invoked when the item currently displayed changes.
        /// </summary>
        /// <param name="sender">The FlipView instance for which the current item has changed.</param>
        /// <param name="e">Event data that describes how the current item was changed.</param>
        void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Enable the previous and next buttons as appropriate
            this.DefaultViewModel["CanFlipNext"] = this.flipView.Items != null && this.flipView.SelectedIndex < (this.flipView.Items.Count - 1);
            this.DefaultViewModel["CanFlipPrevious"] = this.flipView.SelectedIndex > 0;
            this.pageTitle.Text = _monthStrings[_currentMonth - 1] + " " + (flipView.SelectedIndex + 1) + " " + _sampleItemDataForMonth.Year + " For " + _sampleItemDataForMonth.Group.city._Name;
            FlipViewItem selectedFlipViewItem = (FlipViewItem)flipView.ItemContainerGenerator.ContainerFromIndex(flipView.SelectedIndex);
            if (selectedFlipViewItem != null)
            {
                DisplayFeed(flipView.SelectedIndex + 1,  _sampleItemDataForMonth, selectedFlipViewItem);
            }
        }

        /// <summary>
        /// Invoked when the Previous button on the App Bar is clicked.
        /// </summary>
        /// <param name="sender">The App Bar Button instance.</param>
        /// <param name="e">Event data that describes how the button was clicked.</param>
        void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            this.flipView.SelectedIndex -= 1;
        }

        /// <summary>
        /// Invoked when the Next button on the App Bar is clicked.
        /// </summary>
        /// <param name="sender">The App Bar Button instance.</param>
        /// <param name="e">Event data that describes how the button was clicked.</param>
        void NextButton_Click(object sender, RoutedEventArgs e)
        {
            this.flipView.SelectedIndex += 1;
        }

        #endregion

        private void DisplayFeed(int day, SampleDataItem item, FlipViewItem selectedFlipViewItem)
        {
            TextBlock t;
            PanchangData pdata = item.GetPanchangData(_currentMonth, day);
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "SunriseTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.Sunrise];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "SunsetTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.Sunset];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "MoonRiseTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.Moonrise];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "TamilYearTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.TamilYear];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "NorthYearTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.NorthYear];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "GujaratYearTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.GujaratYear];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "AyanaTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.Ayana];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "RituTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.Ritu];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "VedicRituTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.VedicRitu];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "TamilMonthTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.TamilMonth];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "SanskritMonthTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.SanskritMonth];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "PakshaTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.Paksha];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "TithiTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.Tithi];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "NakshatraTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.Nakshatra];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "YogaTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.Yoga];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "KaranaTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.Karana];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "SunRasiTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.SunRasi];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "MoonRasiTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.MoonRasi];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "RahuKalamTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.RahuKalam];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "YamaKandamTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.YamaGandam];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "GulikaiTextBlock");
            t.Text = pdata._fieldValues[(int)FieldType.Gulikai];
            t = (TextBlock)FindNamedElement(selectedFlipViewItem, "FestivalTextBlock");
            String festival = pdata._fieldValues[(int)FieldType.Festival];
            if (String.IsNullOrEmpty(festival))
            {
                festival = "No festival";
            }
            t.Text = festival;
        }
    }
}
