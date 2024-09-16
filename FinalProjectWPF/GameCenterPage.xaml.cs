using FinalProjectWPF.FileManagment;
using FinalProjectWPF.Projects.Snake;
using FinalProjectWPF.UserManagment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FinalProjectWPF
{
    public partial class GameCenterPage : Page
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        FileManager fm = ((App)Application.Current).fmGlobal;
        int LoggedInUser = ((App)Application.Current).LoggedInUserID;

        public GameCenterPage()
        {
            User CurrentUser;
            DateTime date = new DateTime(2024, 1, 1);
            if (fm.CheckLastLoginUser().LastLogin > date)
            {
                CurrentUser = fm.CheckLastLoginUser();
                LoggedInUser = CurrentUser.ID;
                ((App)Application.Current).LoggedInUserID = LoggedInUser;
            }
            else
            {
                
                Random random = new Random();
                int randomNumber = random.Next(1000, 10000);
                CurrentUser = fm.CreateNewUser("player" + randomNumber.ToString());
            }
            InitializeComponent();
            DataContext = CurrentUser;
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            ClockText.Text = DateTime.Now.ToString("HH:mm:ss");
            ClockDate.Text = DateTime.Now.ToString("d");
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = (sender as Image)!;
            image.Opacity = 0.6;
        }


        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image)!.Opacity = 1;
        }

        //private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (UserOptions.Visibility == Visibility.Visible)
        //    {
        //        UserOptions.Visibility = Visibility.Hidden;
        //    }
        //    else
        //    {
        //        UserOptions.Visibility = Visibility.Visible;
        //    }
        //}

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (UserOptions.Visibility == Visibility.Visible)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }


        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Border).BorderBrush = Brushes.AliceBlue;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).BorderBrush = Brushes.Transparent;
        }

        private void CloseMenu()
        {
            //Subtitle.Visibility = Visibility.Hidden;
            //Subtitle.Width = 0;
            //Subtitle.Height = 0;
            UserOptions.Visibility = Visibility.Hidden;
            UserName.Visibility = Visibility.Visible;
            EditUserInput.Width = 0;
            EditUserInput.Visibility = Visibility.Hidden;
            SaveNewName.Visibility = Visibility.Hidden;
            EditUserButton.Visibility = Visibility.Hidden;
            SaveNewName.Visibility = Visibility.Hidden;
            SaveNewName.Width = 0;
            SaveNewName.Height = 0;
            SwitchUserButton.Visibility = Visibility.Hidden;
            AddUserButton.Visibility = Visibility.Hidden;
            SelectionList.Visibility = Visibility.Hidden;
            SelectionList.Width = 0;
            SelectionList.Height = 0;
            NewUserInput.Visibility = Visibility.Hidden;
            NewUserInput.Width = 0;
            NewUserInput.Height = 0;
            CreateUserButton.Visibility = Visibility.Hidden;
            CreateUserButton.Width = 0;
            CreateUserButton.Height = 0;
        }

        private void OpenMenu()
        {
            UserOptions.Visibility = Visibility.Visible;
            EditUserInput.Visibility = Visibility.Visible;
            SaveNewName.Visibility = Visibility.Visible;
            EditUserButton.Visibility = Visibility.Visible;
            EditUserButton.Width = 50;
            SwitchUserButton.Visibility = Visibility.Visible;
            SwitchUserButton.Width = 50;
            AddUserButton.Visibility = Visibility.Visible;
            AddUserButton.Width = 50;
            SelectionList.Width = 120;
            SelectionList.Height = 20;
        }

        private bool IsClickInUserPanelArea(Point clickPosition)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(AppGrid, clickPosition);

            if (hitResult != null)
            {
                DependencyObject clickedElement = hitResult.VisualHit;

                while (clickedElement != null)
                {
                    if (clickedElement == UserPanel)
                    {
                        return true;
                    }

                    clickedElement = VisualTreeHelper.GetParent(clickedElement);
                }
            }
            return false;
        }

        private void UserViewbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (UserOptions.Visibility == Visibility.Visible)
            {
                Point clickPosition = e.GetPosition(AppGrid);

                if (IsClickInUserPanelArea(clickPosition))
                {
                    return;
                    //TestColor.Background = Brushes.Red;
                }
                else
                {
                    CloseMenu();
                    //TestColor.Background = Brushes.Transparent;
                }
            }
            else return;
        }

        public void SnakeGame_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new SnakeHomePage());
        }


        //private void SwitchUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    EditUserButton.Visibility = Visibility.Hidden;
        //    SwitchUserButton.Visibility = Visibility.Hidden;
        //    UserSelection.Visibility = Visibility.Visible;
        //    DateTime date = new DateTime(2024, 1, 1);

        //    List<User> users = fm.GetAllUsers() ?? new List<User>();
        //    List<User> localUsers = users.Where(u => u.LastLogin > date).ToList();

        //    if(SelectionList.Items.Count == 0) {
        //        foreach (User User in localUsers)
        //        {
        //            ComboBoxItem ItemToSelect = new ComboBoxItem
        //            {
        //                Content = User.FullName
        //            };
        //            SelectionList.Items.Add(ItemToSelect);
        //        }
        //    }
        //}

        private void SwitchUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditUserButton.Visibility = Visibility.Hidden;
            EditUserButton.Width = 0;
            SwitchUserButton.Visibility = Visibility.Hidden;
            SwitchUserButton.Width = 0;
            AddUserButton.Visibility = Visibility.Hidden;
            AddUserButton.Width = 0;
            //Subtitle.Visibility = Visibility.Visible;
            //Subtitle.Width = 100;
            //Subtitle.Height = 30;
            //Subtitle.Content = "Select another user:";
            SelectionList.Visibility = Visibility.Visible;

            DateTime date = new DateTime(2024, 1, 1);

            List<User> users = fm.GetAllUsers() ?? new List<User>();
            List<User> localUsers = users.Where(u => u.LastLogin > date).ToList();

            if (SelectionList.Items.Count == 0)
            {
                foreach (User User in localUsers)
                {
                    ComboBoxItem ItemToSelect = new ComboBoxItem
                    {
                        Content = User.FullName
                    };
                    SelectionList.Items.Add(ItemToSelect);
                }
            }

        }

        //private void CreateNewUser(string name)
        //{
        //    User CurrentUser = fm.CreateNewUser(name);
        //    fm.LoginUser(CurrentUser.ID);
        //    ComboBoxItem c = new ComboBoxItem();
        //    c.Content = CurrentUser.FullName;
        //    SelectionList.Items.Add(c);
        //    ((App)Application.Current).LoggedInUserID = CurrentUser.ID;
        //}

        //private void SelectedUser(object sender, SelectionChangedEventArgs e)
        //{
        //    EditUserButton.Visibility = Visibility.Visible;
        //    SwitchUserButton.Visibility = Visibility.Visible;
        //    UserSelection.Visibility = Visibility.Hidden;
        //    ComboBoxItem selectedItem = (ComboBoxItem)SelectionList.SelectedItem;
        //    string selectedUserName = selectedItem.Content.ToString(); 

        //    User CurrentUser = fm.CheckLastLoginUser();
        //    fm.LogoutUser(CurrentUser.ID);

        //    CurrentUser = fm.GetAllUsers().FirstOrDefault(user => user.FullName == selectedUserName);
        //    fm.LoginUser(CurrentUser.ID);
        //}

        //private void EditName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    EditUserButton.Visibility = Visibility.Hidden;
        //    SwitchUserButton.Visibility = Visibility.Hidden;
        //    UserName.Visibility = Visibility.Hidden;
        //    EditUserInput.Visibility = Visibility.Visible;
        //}

        private void SelectedUser(object sender, SelectionChangedEventArgs e)
        {
            EditUserButton.Visibility = Visibility.Visible;
            SwitchUserButton.Visibility = Visibility.Visible;
            SelectionList.Visibility = Visibility.Hidden;
            ComboBoxItem selectedItem = (ComboBoxItem)SelectionList.SelectedItem;
            string selectedUserName = selectedItem.Content.ToString();

            User CurrentUser = fm.CheckLastLoginUser();
            fm.LogoutUser(CurrentUser.ID);

            CurrentUser = fm.GetAllUsers().FirstOrDefault(user => user.FullName == selectedUserName);
            DataContext = CurrentUser;
            fm.LoginUser(CurrentUser.ID);
            CloseMenu();
        }
        // modified
        private void NewUserPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditUserButton.Visibility = Visibility.Hidden;
            EditUserButton.Width = 0;
            SwitchUserButton.Visibility = Visibility.Hidden;
            SwitchUserButton.Width = 0;
            AddUserButton.Visibility = Visibility.Hidden;
            AddUserButton.Width = 0;
            SelectionList.Width = 0;
            SelectionList.Height = 0;

            NewUserInput.Visibility = Visibility.Visible;
            NewUserInput.Width = 120;
            NewUserInput.Height = 30;
            NewUserInput.Focus();
            CreateUserButton.Visibility = Visibility.Visible;
            CreateUserButton.Width = 50;
            CreateUserButton.Height = 70;
        }
        // modified
        private void CreatUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            User CurrentUser = fm.CheckLastLoginUser();
            fm.LogoutUser(CurrentUser.ID);
            string newUserName = NewUserInput.Text;
            DataContext = fm.LoginUser(CreateNewUser(newUserName).ID);
            CloseMenu();
        }

        // modified
        private User CreateNewUser(string name)
        {
            if (name != "")
            {
                User newUser = fm.CreateNewUser(name);
                fm.LogoutUser(newUser.ID);
                ComboBoxItem c = new ComboBoxItem();
                if (SelectionList.Items.Count != 0)
                {
                    c.Content = newUser.FullName;
                    SelectionList.Items.Add(c);
                }
                ((App)Application.Current).LoggedInUserID = newUser.ID;
                return newUser;
            }
            User CurrentUser = fm.CheckLastLoginUser();
            return CurrentUser;
        }
        // modified
        private void EditName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditUserButton.Visibility = Visibility.Hidden;
            SwitchUserButton.Visibility = Visibility.Hidden;
            UserName.Visibility = Visibility.Hidden;
            AddUserButton.Visibility = Visibility.Hidden;
            EditUserInput.Visibility = Visibility.Visible;
            EditUserInput.Width = 120;
            EditUserInput.Height = 30;
            EditUserInput.Margin = new Thickness(10);
            EditUserInput.Focus();
            SaveNewName.Visibility = Visibility.Visible;
            SaveNewName.Width = 40;
            SaveNewName.Height = 70;
        }
        // modified
        private void EditUserName(object sender, MouseButtonEventArgs e)
        {
            User CurrentUser = fm.CheckLastLoginUser();
            DataContext = fm.UpdateUser(CurrentUser.ID, EditUserInput.Text);
            CloseMenu();
        }
        //private void UpdateUserNameOnScreen()
        //{

        //}

        //private void HighScoresUpdateForAllGames()
        //{

        //}

        //private void CreateUserButtonClicked()
        //{

        //}

        
    }
}
