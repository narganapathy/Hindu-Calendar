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
using System.Diagnostics;

namespace HinduCalendarPhone
{
    public partial class MonthView : UserControl
    {
        MainPage _mainPage;
        int _month;

        public MonthView(int month, MainPage mainPage)
        {
            InitializeComponent();
            _mainPage = mainPage;
            _month = month;
            BuildCalendar(this.monthView, month);
        }
        
        const int _numRows = 6;
        const int _numCols = 7;
        const int _year = 2012;
        String[] _dayStrings = { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };

        void dateItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DateItem dateItem = sender as DateItem;
            DateTime dateTime = new DateTime(_year, _month, dateItem.GetDay());
            _mainPage.NavigationService.Navigate(new Uri("/DayView.xaml?date=" + dateTime.ToShortDateString(), UriKind.Relative));

            Debug.WriteLine("Item selected");
        }
        
        private void BuildCalendar(Grid monthView, int month)
        {
            int row, col;
            DateItem[,] dateItems;

            if (monthView.Tag == null)
            {
                dateItems = new DateItem[_numRows, _numCols];

                for (row = 0; row < _numRows; row++)
                {
                    for (col = 0; col < _numCols; col++)
                    {
                        DateItem dateItem = new DateItem();
                        dateItem.SetValue(Grid.RowProperty, row);
                        dateItem.SetValue(Grid.ColumnProperty, col);
                        monthView.Children.Add(dateItem);
                        dateItems[row, col] = dateItem;
                        if (row == 0)
                        {
                            DayOfWeek day = (DayOfWeek)col;
                            dateItem.SetDay(-1, _dayStrings[(int)day]);
                        }
                        else
                        {
                            dateItem.SetDay(-1, " ");
                        }
                    }
                }
                monthView.Tag = dateItems;
            }

            // collapse them all to be opened later
            dateItems = (DateItem[,])monthView.Tag;
            for (row = 1; row < _numRows; row++)
            {
                for (col = 0; col < _numCols; col++)
                {
                    dateItems[row, col].Visibility = Visibility.Collapsed;
                }
            }

            DateItem currentDateItem = null;

            row = 1;

            for (int day = 1; day <= 31; day++)
            {
                DateTime dateTime;
                try
                {
                    dateTime = new DateTime(_year, month, day);
                    col = (int)dateTime.DayOfWeek;

                    
                    currentDateItem = dateItems[row, col];
                    currentDateItem.SetDay(day, day.ToString());
                    currentDateItem.MouseLeftButtonDown += new MouseButtonEventHandler(dateItem_MouseLeftButtonDown);

                    currentDateItem.Visibility = Visibility.Visible;
                    
                    bool highlight = false;
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
                        currentDateItem.Highlight(true);
                        //ShowDetail(month, day, item);
                    }

                    if (col == (_numCols - 1))
                    {
                        row++;
                        if (row == _numRows)
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
    }
}
