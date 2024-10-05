using System;
using System.Collections.Generic;
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

namespace FinalProjectWPF.Projects.MyCalendar.UserControls
{
    /// <summary>
    /// Interaction logic for Item.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        public delegate void EditButtonClickedHandler(object sender, EventArgs e);
        public event EditButtonClickedHandler EditButtonClicked;

        public Item()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.
            Register("Title", typeof(string), typeof(Item));

        public string Time
        {
            get { return (string)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty = DependencyProperty.
            Register("Time", typeof(string), typeof(Item));

        public SolidColorBrush Color
        {
            get { return (SolidColorBrush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.
            Register("Color", typeof(SolidColorBrush), typeof(Item));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.
            Register("Icon", typeof(ImageSource), typeof(Item));
        public ImageSource DurationIcon
        {
            get { return (ImageSource)GetValue(DurationIconProperty); }
            set { SetValue(DurationIconProperty, value); }
        }

        public static readonly DependencyProperty DurationIconProperty = DependencyProperty.
            Register("DurationIcon", typeof(ImageSource), typeof(Item));

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
    //--------------


    //private void AllDayCheckBox_Checked(object sender, RoutedEventArgs e)
    //{
    //    TaskStartTimeComboBox.IsEnabled = false;
    //    TaskEndTimeComboBox.IsEnabled = false;
    //}

    //private void AllDayCheckBox_Unchecked(object sender, RoutedEventArgs e)
    //{
    //    //TaskStartTimeComboBox.IsEnabled = true;
    //    //TaskEndTimeComboBox.IsEnabled = true;
    //}

    //private void CreateTaskButton_Click(object sender, RoutedEventArgs e)
    //{
    //    TaskNameBox.Text = "";
    //    TaskIDBox.Text = "";
    //    TaskDescriptionBox.Text = "";
    //    TaskLocationBox.Text = "";
    //    AllDayCheckBox.IsChecked = false;
    //    TaskPopup.IsOpen = true;
    //}

    //private void DeleteButton_Click(object sender, RoutedEventArgs e)
    //{
    //    int? taskID = int.Parse(TaskIDBox.Text);
    //    taskFileManager.DeleteTask((int)taskID);
    //    TaskPopup.IsOpen = false;
    //    TaskIDBox.Text = null;
    //    InitializeWeekGrid();
    //}
    //private void SubmitButton_Click(object sender, RoutedEventArgs e)
    //{
    //    // Get task details from the textboxes
    //    int? taskID = null;
    //    if (TaskIDBox.Text != "")
    //    {
    //        taskID = int.Parse(TaskIDBox.Text);
    //    }
    //    string taskName = TaskNameBox.Text;
    //    string taskDescription = TaskDescriptionBox.Text;
    //    string taskLocation = TaskLocationBox.Text;

    //    // Get the selected date from the DatePicker
    //    DateTime taskDate = TaskDatePicker.SelectedDate ?? DateTime.Now;

    //    // Get start and end time from the ComboBoxes
    //    string startTimeString = (TaskStartTimeComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();
    //    string endTimeString = (TaskEndTimeComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString();

    //    DateTime taskStartTime = taskDate.Date.Add(TimeSpan.Parse(startTimeString ?? "00:00"));
    //    DateTime taskEndTime = taskDate.Date.Add(TimeSpan.Parse(endTimeString ?? "00:00"));

    //    // Check if "All Day" is checked
    //    if (AllDayCheckBox.IsChecked == true)
    //    {
    //        taskStartTime = taskDate.Date; // Start at 00:00
    //        taskEndTime = taskDate.Date.AddDays(1).AddTicks(-1); // End at 23:59:59.9999999
    //    }
}