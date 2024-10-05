using FinalProjectWPF.Projects.MyCalendar.UserControls;
using FinalProjectWPF.Projects.MyCalender.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalProjectWPF.Projects.MyCalendar
{
    /// <summary>
    /// Interaction logic for MyCalendar.xaml
    /// </summary>
    public partial class MyCalendar : Page
    {
        private DateTime _currentDay = DateTime.Today;
        private TaskFileManager taskFileManager = new TaskFileManager();
        private ObservableCollection<MyTask>? tasks;
        public MyCalendar()
        {
            InitializeComponent();
            MainCalendar.DisplayDate = _currentDay;
            MainCalendar.SelectedDate = _currentDay;
            InitializeTasksForDate(_currentDay);
            UpdateYearMonth(_currentDay);


        }

        private async Task InitializeTasksForDate(DateTime SelectedDate)
        {
            //    // Sample JSON parsing
            ObservableCollection<MyTask> tasks = taskFileManager.GetAllTasks();
            TaskTable.Children.Clear(); // Clear any existing items
            int count = 0;
            foreach (MyTask task in tasks)
            {
                if (task.TaskStartTime.Date == SelectedDate)
                {
                    count++;
                    DateTime taskStartTime = task.TaskStartTime;
                    DateTime taskEndTime = task.TaskEndTime;
                    string taskTime = taskStartTime.ToString() + " " + taskEndTime.ToString();
                    var taskControl = new Item
                    {
                        Title = task.Title,
                        Time = taskTime,
                        Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FBC9B1")) // Fixed to always use this color

                    };
                    taskControl.EditButtonClicked += (s, e) =>
                    {
                        // This is where you can handle the event and get the task details
                        OpenEditPopup(task);
                    };


                    TaskTable.Children.Add(taskControl);
                }
            }
            if (_currentDay.Day < 10) 
            {
                SelectedDayNumber.Text = $"0{_currentDay.Day.ToString()}";
            }
            else
            {
                SelectedDayNumber.Text = _currentDay.Day.ToString();
            }
            SelectedMonthText.Text = _currentDay.ToString("MMMM"); // Full month name
            SelectedMonthSide.Text = _currentDay.ToString("MMMM"); // Full month name
            SelectedDayText.Text = _currentDay.ToString("dddd");
            TeskCount.Text = $"{count.ToString()} Tasks";
        }

        private void MainCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainCalendar.SelectedDate.HasValue)
            {
                DateTime selectedDate = MainCalendar.SelectedDate.Value;
                _currentDay = selectedDate;
                InitializeTasksForDate(selectedDate); 
            }
        }
        private void PrevDayButton_Click(object sender, RoutedEventArgs e)
        {
            _currentDay = _currentDay.AddDays(-1);
            MainCalendar.DisplayDate = _currentDay;
            MainCalendar.SelectedDate = _currentDay;
            InitializeTasksForDate(_currentDay);
            UpdateYearMonth(_currentDay);
        }
        private void NextDayButton_Click(object sender, RoutedEventArgs e)
        {
            _currentDay = _currentDay.AddDays(1);
            MainCalendar.DisplayDate = _currentDay;
            MainCalendar.SelectedDate = _currentDay;
            InitializeTasksForDate(_currentDay);
            UpdateYearMonth(_currentDay);
        }

        private void UpdateYearMonth(DateTime day)
        {
            foreach (RadioButton r in YearBar.Children.OfType<RadioButton>())
            {
                if (day.Year.ToString() == r.Content.ToString())
                {
                    r.IsChecked = true;
                }
            }
            foreach (RadioButton r in MonthBar.Children.OfType<RadioButton>())
            {
                if (day.Month.ToString() == r.Content.ToString())
                {
                    r.IsChecked = true;
                }
            }
        }

        private void MoveToMonth(object sender, RoutedEventArgs e)
        {
            RadioButton r = sender as RadioButton;
            string name = r.Name;
            string monthNumberString;
            if (name.Length > 8) // For MyMonth10, MyMonth11, MyMonth12
            {
                monthNumberString = name.Substring(name.Length - 2);
            }
            else // For MyMonth1 to MyMonth9
            {
                monthNumberString = name.Substring(name.Length - 1);
            }
            if (!int.TryParse(monthNumberString, out int monthNumber))
            {
                MessageBox.Show("Invalid month number.");
                return; // Exit if parsing fails
            }

            DateTime d = _currentDay;

            // Validate the day based on the month and adjust if necessary
            int day = d.Day;
            int daysInMonth = DateTime.DaysInMonth(d.Year, monthNumber);
            if (day > daysInMonth)
            {
                day = daysInMonth; // Set day to the last valid day of the month
            }

            // Create a new DateTime object for the current month and day
            _currentDay = new DateTime(d.Year, monthNumber, day);

            // Update the calendar display
            MainCalendar.DisplayDate = _currentDay;
            MainCalendar.SelectedDate = _currentDay;

            // Initialize tasks for the new date
            InitializeTasksForDate(_currentDay);
        }

        private void MoveToYear(object sender, RoutedEventArgs e)
        {
            DateTime d = _currentDay;
            int month = d.Month;
            int day = d.Day;

            // Determine if the sender is a RadioButton or a regular Button
            if (sender is RadioButton r)
            {
                // Extract the year number from the RadioButton's content
                string yearNumberString = r.Content.ToString();

                // Parse the year number and handle potential errors
                if (!int.TryParse(yearNumberString, out int yearnum))
                {
                    MessageBox.Show("Invalid year number.");
                    return; // Exit if parsing fails
                }

                // Set the new year to the parsed year number
                _currentDay = new DateTime(yearnum, month, day);
            }
            else if (sender is Button button) // Check if the sender is a Button (Next or Previous Year)
            {
                int yearChange = button.Name == "PreviousYearButton" ? -1 : 1; // Determine if we're moving to previous or next year
                int newYear = d.Year + yearChange;

                // Validate the day based on the new year
                int daysInMonth = DateTime.DaysInMonth(newYear, month);
                if (day > daysInMonth)
                {
                    day = daysInMonth; // Set day to the last valid day of the month
                }

                // Set the new date to the current month and day in the new year
                _currentDay = new DateTime(newYear, month, day);

            }
            else
            {
                return; // Exit if the sender is neither a RadioButton nor a Button
            }

            // Update the calendar display
            MainCalendar.DisplayDate = _currentDay;
            MainCalendar.SelectedDate = _currentDay;

            // Initialize tasks for the new date
            InitializeTasksForDate(_currentDay);

            // Update year buttons based on the new current day
            int yearNumber = _currentDay.Year;
            MyYear1.Content = (yearNumber - 2).ToString();
            MyYear2.Content = (yearNumber - 1).ToString();
            MyYear3.Content = yearNumber.ToString();
            MyYear4.Content = (yearNumber + 1).ToString();
            MyYear5.Content = (yearNumber + 2).ToString();

            // Optional: Update the month display
            UpdateYearMonth(_currentDay);
        }
        private void BackToToday_Click(object sender, RoutedEventArgs e)
        {
            _currentDay = DateTime.Today;
            UpdateYearMonth(_currentDay);
            InitializeTasksForDate(_currentDay);
        }

        private void OpenEditPopup(MyTask task)
        {
            TaskIDBox.Text = task.Id.ToString();
            TaskNameBox.Text = task.Title;
            TaskDescriptionBox.Text = task.Description;
            TaskLocationBox.Text = task.Location;
            TaskDatePicker.SelectedDate = task.TaskStartTime.Date;
            string startHour = task.TaskStartTime.ToString("HH:mm");
            foreach (ComboBoxItem item in TaskStartTimeComboBox.Items)
            {
                if (item.Tag.ToString() == startHour)
                {
                    TaskStartTimeComboBox.SelectedItem = item;
                    break;
                }
            }

            // Set the selected end time
            string endHour = task.TaskEndTime.ToString("HH:mm");
            foreach (ComboBoxItem item in TaskEndTimeComboBox.Items)
            {
                if (item.Tag.ToString() == endHour)
                {
                    TaskEndTimeComboBox.SelectedItem = item;
                    break;
                }
            }
            TaskPopup.IsOpen = true;
        }
        private void CreateTaskButton_Click(object sender, RoutedEventArgs e)
        {
            TaskNameBox.Text = "";
            TaskIDBox.Text = "";
            TaskDescriptionBox.Text = "";
            TaskLocationBox.Text = "";
            TaskDatePicker.SelectedDate = _currentDay;
            TaskStartTimeComboBox.SelectedItem = null;
            TaskEndTimeComboBox.SelectedItem = null;
            AllDayCheckBox.IsChecked = false;
            TaskPopup.IsOpen = true;
        }

        private void SubmitTask_Click(object sender, RoutedEventArgs e)
        {
            DateTime taskStartTime;
            DateTime taskEndTime;
            // Validate the inputs as needed (e.g., checking for empty fields)
            if (string.IsNullOrWhiteSpace(TaskNameBox.Text) ||
                string.IsNullOrWhiteSpace(TaskDescriptionBox.Text) ||
                string.IsNullOrWhiteSpace(TaskLocationBox.Text) ||
                TaskDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please fill all the fields correctly.");
                return;
            }
            if (!AllDayCheckBox.IsChecked == true && (TaskStartTimeComboBox.SelectedItem == null || TaskEndTimeComboBox.SelectedItem == null))
            {
                MessageBox.Show("Please fill all the fields correctly.");
                return;
            }
            if (!AllDayCheckBox.IsChecked == true)
            {
                taskStartTime = TaskDatePicker.SelectedDate.Value.Date.Add(TimeSpan.Parse(((ComboBoxItem)TaskStartTimeComboBox.SelectedItem).Content.ToString()));
                taskEndTime = TaskDatePicker.SelectedDate.Value.Date.Add(TimeSpan.Parse(((ComboBoxItem)TaskEndTimeComboBox.SelectedItem).Content.ToString()));
                if (taskEndTime <= taskStartTime)
                {
                    MessageBox.Show("End time must be greater than start time.");
                    return;
                }
            }
            else
            {
                taskStartTime = new DateTime(TaskDatePicker.SelectedDate.Value.Year, TaskDatePicker.SelectedDate.Value.Month, TaskDatePicker.SelectedDate.Value.Day, 0, 0, 0);
                taskEndTime = new DateTime(TaskDatePicker.SelectedDate.Value.Year, TaskDatePicker.SelectedDate.Value.Month, TaskDatePicker.SelectedDate.Value.Day, 23, 59, 0);
            }

           

            if (string.IsNullOrWhiteSpace(TaskIDBox.Text))
            {
                int newId = taskFileManager.GetNextTaskID();
                MyTask newTask = new MyTask(newId, TaskNameBox.Text, TaskDescriptionBox.Text, TaskLocationBox.Text, taskStartTime, taskEndTime);
                taskFileManager.CreateTask(newTask); 
            }
            else
            {
                MyTask newTask = new MyTask(int.Parse(TaskIDBox.Text), TaskNameBox.Text, TaskDescriptionBox.Text, TaskLocationBox.Text, taskStartTime, taskEndTime);
                taskFileManager.UpdateTask(newTask);
            }
            TaskPopup.IsOpen = false;
            InitializeTasksForDate(_currentDay);
        }


        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TaskIDBox.Text, out int taskId))
            {
                if (MessageBox.Show("Are you sure you want to delete this task?", "Confirm Deletion", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    taskFileManager.DeleteTask(taskId);
                    TaskPopup.IsOpen = false;
                    InitializeTasksForDate(_currentDay);
                }
            }
            else
            {
            }
        }

    }
}
