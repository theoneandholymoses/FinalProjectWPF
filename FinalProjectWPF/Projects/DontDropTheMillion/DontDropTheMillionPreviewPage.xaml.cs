using FinalProjectWPF.Enums;
using FinalProjectWPF.FileManagment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace FinalProjectWPF.Projects.DontDropTheMillion
{
    /// <summary>
    /// Interaction logic for DontDropTheMillionPreviewPage.xaml
    /// </summary>
    public partial class DontDropTheMillionPreviewPage : Page
    {
        FileManager fm;
        int LoggedInUser;
        ObservableCollection<double> GameScoreData;
        GameType DontDropTheMillion;
        public DontDropTheMillionPreviewPage()
        {
            InitializeComponent();
            fm = (FileManager)((App)Application.Current).fmGlobal;
            LoggedInUser = ((App)Application.Current).LoggedInUserID;

            GameScoreData = fm.GetUserHighScoresForGame(LoggedInUser, DontDropTheMillion);
            ScoreBoard.ItemsSource = GameScoreData;
        }

        private void BackgroundMedia_MediaEnded(object sender, RoutedEventArgs e)
        {
            GifBackground.Position = TimeSpan.Zero;
            GifBackground.Play();
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void OpenApp_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DontDropTheMillion());
        }
        private void Button_ClickGameInfo(object sender, RoutedEventArgs e)
        {
            PopUpWindow.IsOpen = true;
        }
    }
}