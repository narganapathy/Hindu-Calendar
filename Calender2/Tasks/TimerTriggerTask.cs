using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
//using Calender2.Data;
using NotificationsExtensions.TileContent;
using System.Diagnostics;
using Windows.UI.Notifications;
using CalendarData;

namespace Tasks
{
    public sealed class TimerTriggerTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral = null;
        IBackgroundTaskInstance _taskInstance = null;

        //
        // The Run method is the entry point of a background task.
        //
        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            //
            // Associate a cancellation handler with the background task.
            //
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            //
            // Get the deferral object from the task instance, and take a reference to the taskInstance;
            //
            _deferral = taskInstance.GetDeferral();

            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            // delete all previous notifications
            var notifier = Windows.UI.Notifications.TileUpdateManager.CreateTileUpdaterForApplication();
            var scheduled = notifier.GetScheduledTileNotifications();

            for (int i = 0, len = scheduled.Count; i < len; i++) 
            {
                    notifier.RemoveFromSchedule(scheduled[i]);
            }

            CalendarDataReader reader = new CalendarDataReader();
            String cityToken = Windows.Storage.ApplicationData.Current.LocalSettings.Values["CityName"] as String;
            DateTime today = DateTime.Today;
            await reader.ReadCalendarYearData(cityToken, today.Year);
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            for (int i = 0; i < 31; i++)
            {
                if (i == 0)
                {
                    // to get an immediate update add a tile 3 minutes from now
                    UpdateTile(reader, DateTime.Now.AddMinutes(3));
                }
                else
                {
                    UpdateTile(reader, today.AddDays(i));
                }
            }
            _taskInstance = taskInstance;
            _deferral.Complete();
        }

        //
        // Handles background task cancellation.
        //
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //
            // Indicate that the background task is canceled.
            //

        }

         //Update tile for today
        private void UpdateTile(CalendarDataReader reader, DateTime dueTime)
        {
            int month = dueTime.Month;
            int day = dueTime.Day;
            PanchangData pdata =   reader.GetPanchangData(month, day);
             //create the wide template
            ITileWideText01 tileContent = TileContentFactory.CreateTileWideText01();
            tileContent.TextHeading.Text = dueTime.ToString("d");
            // Uncomment the following line to enable debugging
            tileContent.TextBody1.Text = pdata._fieldValues[(int)FieldType.SanskritMonth];
            tileContent.TextBody2.Text = pdata._fieldValues[(int)FieldType.TamilMonth];
            tileContent.TextBody3.Text = pdata._fieldValues[(int)FieldType.Festival];

             //create the square template and attach it to the wide template
            ITileSquareText01 squareContent = TileContentFactory.CreateTileSquareText01();
            squareContent.TextHeading.Text = dueTime.ToString("d");
            squareContent.TextBody1.Text = pdata._fieldValues[(int)FieldType.SanskritMonth];
            squareContent.TextBody2.Text = pdata._fieldValues[(int)FieldType.TamilMonth];
            squareContent.TextBody3.Text = pdata._fieldValues[(int)FieldType.Festival];
            tileContent.SquareContent = squareContent;


            if (dueTime > DateTime.Now)
            {
                ScheduledTileNotification futureTile = new ScheduledTileNotification(tileContent.GetXml(), dueTime);
                TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(futureTile);
            }
            else
            {
                //send the notification
                //TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());
            }

             //Send another notification. this gives a nice animation in mogo
            tileContent.TextBody1.Text = pdata._fieldValues[(int)FieldType.Paksha];
            tileContent.TextBody2.Text = pdata._fieldValues[(int)FieldType.Tithi];
            tileContent.TextBody3.Text = pdata._fieldValues[(int)FieldType.Nakshatra];

            if (dueTime > DateTime.Now)
            {
                ScheduledTileNotification futureTile = new ScheduledTileNotification(tileContent.GetXml(), dueTime);
                TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(futureTile);
            }
            else
            {
                //send the notification
                //TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());
            }
        }
    }
}