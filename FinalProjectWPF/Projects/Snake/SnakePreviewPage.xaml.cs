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
using static System.Formats.Asn1.AsnWriter;

namespace FinalProjectWPF.Projects.Snake
{
    /// <summary>
    /// Interaction logic for SnackHomePage.xaml
    /// </summary>
    public partial class SnakePreviewPage : Page
    {
        FileManager fm;
        int LoggedInUser;
        public ObservableCollection<PlayerScore> GameScoreData { get; set; } = new ObservableCollection<PlayerScore>();
        GameType Snake;
        public SnakePreviewPage()
        {
            InitializeComponent();
            fm = (FileManager)((App)Application.Current).fmGlobal;
            LoggedInUser = ((App)Application.Current).LoggedInUserID;

            // Call the modified method
            GameScoreData = fm.GetAllPlayersHighScores(GameType.Snake);

            ScoreBoard.ItemsSource = GameScoreData;
        }
        private void BackgroundMedia_MediaEnded(object sender, RoutedEventArgs e)
        {
            GifBackground.Position = TimeSpan.Zero;
            GifBackground.Play();
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GameCenterPage());
        }

        private void OpenApp_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SnakeGame());
        }
        private void Button_ClickGameInfo(object sender, RoutedEventArgs e)
        {
            PopUpWindow.IsOpen = true;
        }
    }
}
