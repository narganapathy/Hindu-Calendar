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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Diagnostics;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace HinduCalendarPhone
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }
        public CalendarData Calendar { get {return _calendardata;}}
        public DateTime CurrentDate { get { return _currentDate; } set { _currentDate = value; } }
        public bool FirstLaunch { get { return _firstTimeLaunch; } set { _firstTimeLaunch = value; } }
        
        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            _calendardata = new CalendarData();
            _calendardata.GetCalendarData();
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            if (store.FileExists("PersistedFile.xml"))
            {
                IsolatedStorageFileStream stream = store.OpenFile("PersistedFile.xml", System.IO.FileMode.OpenOrCreate);
                DataContractSerializer ser = new DataContractSerializer(typeof(PersistedData));
                _firstTimeLaunch = false;
                PersistedData data;
                data = ser.ReadObject(stream) as PersistedData;
                _currentDate = new DateTime(data.Year, data.Month, data.Day);
                _calendardata.UpdateCityToken(data.CityToken, data.CityName);
            }
            else
            {
                _firstTimeLaunch = true;
            }
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            PersistedData data = new PersistedData(_currentDate.Year, 
                                                    _currentDate.Month, 
                                                    _currentDate.Day, 
                                                    _calendardata.CityToken, 
                                                    _calendardata.CityName);
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    IsolatedStorageFileStream stream = store.OpenFile("PersistedFile.xml", System.IO.FileMode.Create);
                    DataContractSerializer ser = new DataContractSerializer(typeof(PersistedData));
                    ser.WriteObject(stream, data);
                    stream.Close();
                }
            }
            catch (Exception excep)
            {
                Debug.WriteLine("Isolated Storage exception " + excep.Message);
            }
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;
        private  CalendarData _calendardata;
        private DateTime _currentDate;
        private bool _firstTimeLaunch = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
            _currentDate = DateTime.Today;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}