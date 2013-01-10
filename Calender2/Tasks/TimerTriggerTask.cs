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
            CalendarDataReader reader = new CalendarDataReader();
            await reader.ReadCalendarYearData("Seattle-WA-USA", 2012);
            UpdateTile(reader);
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
        private void UpdateTile(CalendarDataReader reader)
        {
            DateTime date = DateTime.Today;
            int month = date.Month;
            int day = date.Day;
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            PanchangData pdata =   reader.GetPanchangData(month, day);
             //create the wide template
            ITileWideText01 tileContent = TileContentFactory.CreateTileWideText01();
            tileContent.TextHeading.Text = date.ToString("d");
            // Uncomment the following line to enable debugging
            //tileContent.TextHeading.Text = DateTime.Now.ToString();
            tileContent.TextBody1.Text = pdata._fieldValues[(int)FieldType.SanskritMonth];
            tileContent.TextBody2.Text = pdata._fieldValues[(int)FieldType.TamilMonth];
            tileContent.TextBody3.Text = pdata._fieldValues[(int)FieldType.Festival];

             //create the square template and attach it to the wide template
            ITileSquareText01 squareContent = TileContentFactory.CreateTileSquareText01();
            squareContent.TextHeading.Text = date.ToString("d");
            squareContent.TextBody1.Text = pdata._fieldValues[(int)FieldType.SanskritMonth];
            squareContent.TextBody2.Text = pdata._fieldValues[(int)FieldType.TamilMonth];
            squareContent.TextBody3.Text = pdata._fieldValues[(int)FieldType.Festival];
            tileContent.SquareContent = squareContent;

            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true); 

             //send the notification
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());

             //Send another notification. this gives a nice animation in mogo
            tileContent.TextBody1.Text = pdata._fieldValues[(int)FieldType.Paksha];
            tileContent.TextBody2.Text = pdata._fieldValues[(int)FieldType.Tithi];
            tileContent.TextBody3.Text = pdata._fieldValues[(int)FieldType.Nakshatra];
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification());
        }
    }
}
