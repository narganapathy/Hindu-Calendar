using Calender2.Data;
using NotificationsExtensions.TileContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Windows.Storage;
using System.Runtime.Serialization;
using Windows.ApplicationModel.Background;
using CalendarData;

// The Item Detail Page ite template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace Calender2
{
    [DataContract(Name = "PrivateEvent", Namespace = "http://www.jyotishcalendar.com")]
    [KnownType(typeof(PrivateEvent))]
    public class PrivateEvent
    {
        [DataMember(Name = "Date")]
        public String _date;
        [DataMember(Name = "EventName")]
        public String _eventText;
        public PrivateEvent(String date, String text)
        {
            _date = date;
            _eventText = text;
        }
    }

    [DataContract(Name = "PrivateEvents", Namespace = "http://www.jyotishcalendar.com")]
    [KnownType(typeof(PrivateEvents))]
    public class PrivateEvents
    {
        [DataMember(Name = "PrivateEventList")]
        public List<PrivateEvent> _privateEventList;
        public PrivateEvents()
        {
            _privateEventList = new List<PrivateEvent>();
        }

        public bool Contains(DateTime date, String eventText)
        {
            foreach (PrivateEvent evt in _privateEventList)
            {
                if (evt._date.Equals(date.ToString("d")) && evt._eventText.Equals(eventText))
                {
                    return true;
                }
            }
            return false;
        }

        public List<PrivateEvent> GetEventsForDate(DateTime date)
        {
            List<PrivateEvent> privateEventList = new List<PrivateEvent>();
            foreach (PrivateEvent evt in _privateEventList)
            {
                if (evt._date.Equals(date.ToString("d"))) 
                {
                    privateEventList.Add(evt);
                }
            }
            return privateEventList;
        }

        public PrivateEvent GetFirstEventForDate(DateTime date)
        {
            foreach (PrivateEvent evt in _privateEventList)
            {
                if (evt._date.Equals(date.ToString("d"))) 
                {
                    return evt;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage : Calender2.Common.LayoutAwarePage
    {
        String[] _dayStrings = { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };

        // Used to build settings UI
        Rect _windowBounds;
        double _settingsWidth = 346;
        Popup _settingsPopup;
        DateItem _currentHighlightedDateItem;
        List<ListBoxItem> _personalEventList;
        ListBoxItem _currentEventItem;
        PrivateEvents _privateEvents;

        public ItemDetailPage()
        {
            // Disable the previous and next App Bar buttons initially
            this.DefaultViewModel["CanFlipNext"] = false;
            this.DefaultViewModel["CanFlipPrevious"] = false;
            this.InitializeComponent();
            _windowBounds = Window.Current.Bounds;
            Window.Current.SizeChanged += OnWindowSizeChanged;
            SettingsPane.GetForCurrentView().CommandsRequested += SettingsPane_CommandsRequested;
            cityTitle.PointerReleased +=cityTitle_PointerReleased;
            _personalEventList = new List<ListBoxItem>();

            
            // This is necessary to ensure that we set the right month.
            // flipview_selectionchagned gets invoked before the page is loaded. This results in a null selected item. 
            // however we now call this event handler from the loaded handler for the page so that 
            // flip view is initialized
            Loaded += async (s, e) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        this.FlipView_SelectionChanged(this.flipView, null);
                    });
            };
        }

        private void cityTitle_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CitySelection), this);
        }

        // the following set of routines are used to create the settings page.
        void OnWindowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            _windowBounds = Window.Current.Bounds;
        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }
        }

        void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;
        }

        private void SettingsPane_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand cmd = new SettingsCommand("sample", "About", (x) =>
                {
                    _settingsPopup = new Popup();
                    _settingsPopup.Closed += OnPopupClosed;
                    Window.Current.Activated += OnWindowActivated;
                    _settingsPopup.IsLightDismissEnabled = true;
                    _settingsPopup.Width = _settingsWidth;
                    _settingsPopup.Height = _windowBounds.Height;

                    AboutSettings mypane = new AboutSettings();
                    mypane.Width = _settingsWidth;                    
                    mypane.Height = _windowBounds.Height;

                    _settingsPopup.Child = mypane;
                    _settingsPopup.SetValue(Canvas.LeftProperty, _windowBounds.Width - _settingsWidth);
                    _settingsPopup.SetValue(Canvas.TopProperty, 0);
                    _settingsPopup.IsOpen = true;
                });

            args.Request.ApplicationCommands.Add(cmd);
        }


        // The following set of routines are used for managing the monthview grid
        async void flipView_Loaded(object sender, RoutedEventArgs e)
        {
            FlipView flipView = sender as FlipView;
            SampleDataItem item = this.flipView.Items[this.flipView.SelectedIndex] as SampleDataItem;
            FlipViewItem selectedFlipViewItem = (FlipViewItem)this.flipView.ItemContainerGenerator.ContainerFromIndex(flipView.SelectedIndex);
            if (selectedFlipViewItem != null)
            {
                Grid monthView = (Grid)FindNamedElement(selectedFlipViewItem, "monthView");
                BuildCalendar(monthView, ((flipView.SelectedIndex) % 12) + 1, item);
            }
            else
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
                {
                    this.flipView_Loaded(this.flipView, null);
                });
            }
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
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            var currentItem = SampleDataSource.GetItem((String)navigationParameter);
            this.DefaultViewModel["Group"] = currentItem.Group;
            this.DefaultViewModel["Items"] = currentItem.Group.Items;

            this.pageTitle.Text = "Hindu Calendar - " + currentItem.Title + " " + currentItem.Year.ToString();
            this.cityTitle.Text = currentItem.Group.city._Name;
            this.flipView.SelectedItem = currentItem;

            StorageFile privateEventFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync("PrivateEvents.txt", CreationCollisionOption.OpenIfExists);
            Windows.Storage.FileProperties.BasicProperties prop = await privateEventFile.GetBasicPropertiesAsync();
            if (prop.Size != 0)
            {
                Stream stream = await privateEventFile.OpenStreamForReadAsync();
                DataContractSerializer ser = new DataContractSerializer(typeof(PrivateEvents));
                _privateEvents = (PrivateEvents)ser.ReadObject(stream);
                // If we have a zero size file or no events, we can get this too
                stream.Dispose();
            }

            if (_privateEvents == null)
            {
                    _privateEvents = new PrivateEvents();
            }
            await SampleDataSource.GetClosestCity();
            CancelTimerTrigger();
            UpdateTitle();
            ScheduleTiles(currentItem);
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected async override void SaveState(Dictionary<String, Object> pageState)
        {
            var item = (SampleDataItem)this.flipView.SelectedItem;
            pageState["SelectedItem"] = item.UniqueId;
            StorageFile privateEventFile = await ApplicationData.Current.RoamingFolder.CreateFileAsync("PrivateEvents.txt", CreationCollisionOption.ReplaceExisting);
            Stream stream = await privateEventFile.OpenStreamForWriteAsync();
            stream.Position = 0;
            DataContractSerializer ser = new DataContractSerializer(typeof(PrivateEvents));
            ser.WriteObject(stream, _privateEvents);
            stream.Dispose();
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
            try
            {
                this.DefaultViewModel["CanFlipNext"] = this.flipView.Items != null && this.flipView.SelectedIndex < (this.flipView.Items.Count - 1);
                this.DefaultViewModel["CanFlipPrevious"] = this.flipView.SelectedIndex > 0;
                SampleDataItem item = this.flipView.Items[this.flipView.SelectedIndex] as SampleDataItem;
                this.pageTitle.Text = "Hindu Calendar - " + item.Title + " " + item.Year.ToString();
                this.cityTitle.Text = item.Group.city._Name;
                FlipViewItem selectedFlipViewItem = (FlipViewItem)this.flipView.ItemContainerGenerator.ContainerFromIndex(flipView.SelectedIndex);
                if (selectedFlipViewItem != null)
                {
                    if (_currentHighlightedDateItem != null)
                    {
                        _currentHighlightedDateItem.HighlightBorder(false);
                    }
                    _currentHighlightedDateItem = null;
                    Grid monthView = (Grid)FindNamedElement(selectedFlipViewItem, "monthView");
                    int month = ((flipView.SelectedIndex) % 12) + 1;
                    BuildCalendar(monthView, month, item);
                    Debug.Assert(_currentHighlightedDateItem != null);
                    DayViewGridStoryboard.Begin();
                }
            }
            catch (ArgumentException exc)
            {
                Debug.WriteLine("Exception is " + exc.Message);
                Debug.Assert(false);
            }
        }

        const int numRows = 6;
        const int numCols = 7;
        private void BuildCalendar(Grid monthView, int month, SampleDataItem item) 
        {
            int row, col;
            DateItem[,] dateItems;

            if (monthView.Tag == null)
            {
                dateItems = new DateItem[numRows, numCols];

                for (row = 0; row < numRows; row++)
                {
                    for (col = 0; col < numCols; col++)
                    {
                        DateItem dateItem = new DateItem();
                        dateItem.SetValue(Grid.RowProperty, row);
                        dateItem.SetValue(Grid.ColumnProperty, col);
                        monthView.Children.Add(dateItem);
                        dateItems[row, col] = dateItem;
                        if (row == 0)
                        {
                            DayOfWeek day = (DayOfWeek)col;
                            dateItem.SetDay(_dayStrings[(int)day]);
                        }
                        else
                        {
                            dateItem.SetDay(" ");
                            dateItem.PointerReleased += dateItem_PointerReleased;
                        }
                    }
                }
                monthView.Tag = dateItems;
            }

            // collapse them all to be opened later
            dateItems = (DateItem[,])monthView.Tag;
            for (row = 1; row < numRows; row++)
            {
                for (col = 0; col < numCols; col++)
                {
                    dateItems[row, col].Visibility = Visibility.Collapsed;
                }
            }

            String previousTamilMonth, tamilMonth;
            String previousSanskritMonth, sanskritMonth;
            String previousPaksha, paksha;
            bool fullMoonDayFound = false;
            bool newMoonDayFound = false;
            DateItem currentDateItem = null;

            row = 1;
            previousTamilMonth = null;
            previousSanskritMonth = null;

            previousPaksha = "";
            tamilMonthTitle.Text = "";
            sanskritMonthTitle.Text = "";
            for (int day = 1; day <= 31 ; day++)
            {
                DateTime dateTime;
                try
                {
                    dateTime = new DateTime(item.Year, month, day);
                    col = (int)dateTime.DayOfWeek;
                    
                    String festival, nakshatra;
                    bool isNewMoonDay, isFullMoonDay;
                    bool highlight;

                    item.GetDateData(month, day, out isNewMoonDay, out isFullMoonDay, out festival, out paksha, out nakshatra, out tamilMonth);
                    PanchangData pdata = item.GetPanchangData(month, day);
                    sanskritMonth = pdata._fieldValues[(int)FieldType.SanskritMonth];

                    if (isNewMoonDay)
                    {
                        newMoonDayFound = true;
                    }

                    if (isFullMoonDay)
                    {
                        fullMoonDayFound = true;
                    }

                    // Sometimes the tithi changes in the middle of the day and is not captured. Lets fix it here
                    if ((previousPaksha.Contains("Shukla") == true) && (paksha.Contains("Krishna") == true) && (fullMoonDayFound == false))
                    {
                        // Set the previous item to full moon day
                        currentDateItem.SetDay(currentDateItem.GetDay(), false, true, null, null, "KeepExisting");
                    }

                    // Sometimes the tithi changes in the middle of the day and is not captured. Lets fix it here
                    if ((previousPaksha.Contains("Krishna") == true) && (paksha.Contains("Shukla") == true) && (newMoonDayFound == false))
                    {
                        // Set the previous item to new moon day
                        currentDateItem.SetDay(currentDateItem.GetDay(), true, false, null, null, "KeepExisting");
                    }

                    previousPaksha = paksha;

                    currentDateItem = dateItems[row, col];
                    PrivateEvent evt = null;
                    if (_privateEvents != null)
                    {
                        evt = _privateEvents.GetFirstEventForDate(dateTime);
                    }

                    if (evt != null)
                    {
                        currentDateItem.SetDay(day,
                            isNewMoonDay, isFullMoonDay, evt._eventText, null, nakshatra);
                    }
                    else
                    {
                        currentDateItem.SetDay(day,
                            isNewMoonDay, isFullMoonDay, festival, null, nakshatra);
                    }
                        
                    currentDateItem.Visibility = Visibility.Visible;
                    if (String.Equals(previousTamilMonth, tamilMonth, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        tamilMonthTitle.Text += (previousTamilMonth == null) ? tamilMonth : ("-" + tamilMonth);
                        previousTamilMonth = tamilMonth;
                    }

                    if (String.Equals(previousSanskritMonth, sanskritMonth, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        sanskritMonthTitle.Text += ((previousSanskritMonth == null) ? sanskritMonth : ("-" + sanskritMonth.Trim()));
                        previousSanskritMonth = sanskritMonth;
                    }

                    highlight = false;
                    // If its the curent month then highlight the current day
                    if (month == DateTime.Today.Month) 
                    {
                        if (day == DateTime.Today.Day)
                        {
                            // Highlight today
                            highlight = true;
                        }
                    }
                    else if (day == 1)
                    {
                        // Highlight the first day of some other month
                            highlight = true;
                    }

                    if (highlight)
                    { 
                        currentDateItem.HighlightBorder(true);
                        ShowDetail(month, day, item);
                        _currentHighlightedDateItem = currentDateItem;
                    }

                    if (col == (numCols-1))
                    {
                        row++;
                        if (row == numRows)
                        {
                            row = 1; // Reset it back to the first row. Provides a foldable calender
                        }
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
        }

        // Update tile for today
        private void UpdateTile( SampleDataItem item, DateTime dueTime, DateTime expiryTime)
        {
            DateTime date = dueTime;
            int month = date.Month;
            int day = date.Day;
            String festival;
            var notifier = TileUpdateManager.CreateTileUpdaterForApplication();

            Debug.WriteLine("Update tile {0} {1}", dueTime, expiryTime);
            festival = item.GetFestival(month, day);
            PanchangData pdata = item.GetPanchangData(month, day);
            // create the wide template
            ITileWideText01 tileContent = TileContentFactory.CreateTileWideText01();
            tileContent.TextHeading.Text = date.ToString("d");
            tileContent.TextBody1.Text = pdata._fieldValues[(int)FieldType.SanskritMonth];
            tileContent.TextBody2.Text = pdata._fieldValues[(int)FieldType.TamilMonth];
            tileContent.TextBody3.Text = festival;

            // create the square template and attach it to the wide template
            ITileSquareText01 squareContent = TileContentFactory.CreateTileSquareText01();
            squareContent.TextHeading.Text = date.ToString("d");
            squareContent.TextBody1.Text = pdata._fieldValues[(int)FieldType.SanskritMonth];
            squareContent.TextBody2.Text = pdata._fieldValues[(int)FieldType.TamilMonth];
            squareContent.TextBody3.Text = festival;
            tileContent.SquareContent = squareContent;

            // send the notification
            ScheduledTileNotification futureTile = new ScheduledTileNotification(tileContent.GetXml(), dueTime);
            futureTile.ExpirationTime = expiryTime;
            notifier.AddToSchedule(futureTile);


            // Send another notification. this gives a nice animation in mogo
            tileContent.TextBody1.Text = pdata._fieldValues[(int)FieldType.Paksha];
            tileContent.TextBody2.Text = pdata._fieldValues[(int)FieldType.Tithi];
            tileContent.TextBody3.Text = pdata._fieldValues[(int)FieldType.Nakshatra];
            futureTile = new ScheduledTileNotification(tileContent.GetXml(), dueTime);
            futureTile.ExpirationTime = expiryTime;
            notifier.AddToSchedule(futureTile);
            Debug.WriteLine("Count of scheduled notifications {0}", notifier.GetScheduledTileNotifications().Count);
        }
        
        private void dateItem_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            DateItem dateItem = sender as DateItem;

            // If its a mouse click look for left mouse button click
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                Windows.UI.Input.PointerPoint currentPoint = e.GetCurrentPoint(dateItem);
                Windows.UI.Input.PointerUpdateKind kind = currentPoint.Properties.PointerUpdateKind;
                if (kind != Windows.UI.Input.PointerUpdateKind.LeftButtonReleased)
                {
                    return;
                }
            }

            // If the user clicks on the empty squares skip
            if (dateItem.GetDay() > 0)
            {
                try
                {
                    SampleDataItem item = this.flipView.Items[this.flipView.SelectedIndex] as SampleDataItem;
                    int month = ((flipView.SelectedIndex) % 12) + 1;
                    ShowDetail(month, dateItem.GetDay(), item);
                    dateItem.HighlightBorder(true);

                    DateItem oldItem = _currentHighlightedDateItem;
                    if (oldItem != null)
                    {
                        oldItem.HighlightBorder(false);
                    }

                    _currentHighlightedDateItem = dateItem;
                }
                catch (ArgumentOutOfRangeException err)
                {
                    Debug.WriteLine("Argument out of range" + err);
                }
            }
        }

        public void ShowDetail(int currentMonth, int day, SampleDataItem item)
        {
            PanchangData pdata = item.GetPanchangData(currentMonth, day);
            DateTime dateTime = new DateTime(item.Year, currentMonth, day);
            DateTextBlock.Text = dateTime.ToString("d");
            SunriseTextBlock.Text= pdata._fieldValues[(int)FieldType.Sunrise];
            SunsetTextBlock.Text = pdata._fieldValues[(int)FieldType.Sunset];
            MoonRiseTextBlock.Text = pdata._fieldValues[(int)FieldType.Moonrise];
            TamilYearTextBlock.Text = (pdata._fieldValues[(int)FieldType.TamilYear] == null) ? "None" : pdata._fieldValues[(int)FieldType.TamilYear];
            NorthYearTextBlock.Text = pdata._fieldValues[(int)FieldType.NorthYear];
            GujaratYearTextBlock.Text = pdata._fieldValues[(int)FieldType.GujaratYear];
            AyanaTextBlock.Text = pdata._fieldValues[(int)FieldType.Ayana];
            RituTextBlock.Text = pdata._fieldValues[(int)FieldType.Ritu];
            VedicRituTextBlock.Text = pdata._fieldValues[(int)FieldType.VedicRitu];
            TamilMonthTextBlock.Text = pdata._fieldValues[(int)FieldType.TamilMonth];
            // there is a whitespace in front of this string
            SanskritMonthTextBlock.Text = (pdata._fieldValues[(int)FieldType.SanskritMonth]).Trim();
            PakshaTextBlock.Text = (pdata._fieldValues[(int)FieldType.Paksha]).Trim();
            TithiTextBlock.Text = pdata._fieldValues[(int)FieldType.Tithi];
            NakshatraTextBlock.Text = pdata._fieldValues[(int)FieldType.Nakshatra];
            YogaTextBlock.Text = pdata._fieldValues[(int)FieldType.Yoga];
            KaranaTextBlock.Text = pdata._fieldValues[(int)FieldType.Karana];
            SunRasiTextBlock.Text = pdata._fieldValues[(int)FieldType.SunRasi];
            MoonRasiTextBlock.Text = pdata._fieldValues[(int)FieldType.MoonRasi];
            RahuKalamTextBlock.Text = pdata._fieldValues[(int)FieldType.RahuKalam];
            YamaKandamTextBlock.Text = pdata._fieldValues[(int)FieldType.YamaGandam];
            GulikaiTextBlock.Text = pdata._fieldValues[(int)FieldType.Gulikai];
            String festival = item.GetFestival(currentMonth, day);
            if (String.IsNullOrEmpty(festival))
            {
                festival = "No data";
            }
            FestivalTextBlock.Text = festival;
            PersonalEventListScroller.Visibility = Visibility.Collapsed;
            Separator.BorderThickness = new Thickness(0, 0, 0, 0);
            PersonalEventList.Items.Clear();
            if (_privateEvents != null)
            {
                List<PrivateEvent> privateEventList = _privateEvents.GetEventsForDate(dateTime);
                foreach (PrivateEvent evt in privateEventList)
                {
                    AddPrivateEvent(dateTime, evt._eventText, false, evt);
                }
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

        public void UpdateTitle()
        {
            var item = (SampleDataItem)this.flipView.SelectedItem;
            this.cityTitle.Text = item.Group.city._Name;
            // update the data for the new city
            FlipView_SelectionChanged(null, null);
        }

        
        private void PersonalEventClick(object sender, RoutedEventArgs e)
        {
            int day = _currentHighlightedDateItem.GetDay();
            int month = (flipView.SelectedIndex % 12) + 1;
            int year = ((SampleDataItem)flipView.SelectedItem).Year;
            DateTime date = new DateTime(year, month, day);
            AddPrivateEvent(date, PeTextBox.Text, true, null);
            PeTextBox.Text = String.Empty;
        }

        private async void AddPrivateEvent(DateTime date, String eventText, bool newEvent, PrivateEvent pEvent)
        {

            if (newEvent && _privateEvents.Contains(date, eventText))
            {
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("Event  name already exists");
                await md.ShowAsync();
                return;
            }

            App app = (App)Application.Current;
            ListBoxItem item = new ListBoxItem();
            TextBlock textBlock = new TextBlock();
            textBlock.Text = eventText;
            item.Content = textBlock;
            PersonalEventList.Items.Add(item);
            PersonalEventListScroller.Visibility = Visibility.Visible;
            Separator.BorderThickness = new Thickness(0, 5, 0, 0);
            _personalEventList.Add(item);
            if (newEvent)
            {
                Debug.Assert(pEvent == null);
                pEvent = new PrivateEvent(date.ToString("d"), eventText);
                _privateEvents._privateEventList.Add(pEvent);
                _currentHighlightedDateItem.SetPrivateEvent(pEvent._eventText);
            }
            item.Tag = pEvent;
        }

        private void RemoveEventClick(object sender, RoutedEventArgs e)
        {
            if (_currentEventItem != null)
            {
                _privateEvents._privateEventList.Remove(_currentEventItem.Tag as PrivateEvent);
                _personalEventList.Remove(_currentEventItem);
                PersonalEventList.Items.Remove(_currentEventItem);
                _currentEventItem = null;
                RemoveEventButton.IsEnabled = false;
                _currentHighlightedDateItem.SetPrivateEvent(String.Empty);
            }
        }

        private void PersonalEventList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            _currentEventItem = lb.SelectedItem as ListBoxItem;
            RemoveEventButton.IsEnabled = true;
        }

        public void CancelTimerTrigger()
        {
            // If there is a pending timer from an older install unregister it
            if (BackgroundTaskRegistration.AllTasks.Count > 0)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == "TimeTriggeredTask")
                    {
                        task.Value.Unregister(true);
                    }
                }
                return;
            }
        }

        public void ScheduleTiles(SampleDataItem currentItem)
        {
            var notifier = Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication();
            var scheduled = notifier.GetScheduledTileNotifications();

            if (scheduled.Count > 0)
            {
                for (int i = 0; i < scheduled.Count; i++)
                {
                    Debug.WriteLine("Notification due time {0} delivery time {1}", scheduled[i].DeliveryTime, scheduled[i].ExpirationTime);
                }
                return;
            }

            notifier.Clear();
            notifier.EnableNotificationQueue(true); 
            // Schedule for the rest of the year
            DateTime today = DateTime.Today;
            int currentYear = currentItem.Year;
            for (int i = 0; i < 365; i++)
            {
                if (i == 0)
                {
                    // to get an immediate update add a tile 3 minutes from now
                    UpdateTile(currentItem, DateTime.Now.AddMinutes(1), DateTime.Today.AddDays(1));
                }
                else
                {
                    DateTime dueDate = today.AddDays(i);
                    if (dueDate.Year == currentYear)
                    {
                        UpdateTile(currentItem, DateTime.Today.AddDays(i), DateTime.Today.AddDays(i+1));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            Debug.WriteLine("Count of scheduled notifications {0}", notifier.GetScheduledTileNotifications().Count);
        }
    }
}