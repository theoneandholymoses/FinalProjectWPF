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

namespace FinalProjectWPF.Projects.CatchTheEgg
{
    /// <summary>
    /// Interaction logic for CatchTheEggPreviewPage.xaml
    /// </summary>
    public partial class CatchTheEggPreviewPage : Page
    {
        FileManager fm;
        int LoggedInUser;
        public ObservableCollection<PlayerScore> GameScoreData { get; set; } = new ObservableCollection<PlayerScore>();
        GameType CatchTheEgg;


        public CatchTheEggPreviewPage()
        {
            InitializeComponent();

            fm = (FileManager)((App)Application.Current).fmGlobal;
            LoggedInUser = ((App)Application.Current).LoggedInUserID;

            GameScoreData = fm.GetAllPlayersHighScores(GameType.CatchTheEgg);
            ScoreBoard.ItemsSource = GameScoreData;
        }

        private void BackgroundMedia_MediaEnded(object sender, RoutedEventArgs e)
        {
            Gif1Background.Position = TimeSpan.Zero;
            Gif1Background.Play();
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GameCenterPage());
        }

        private void OpenApp_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CatchTheEggGame());
        }

        private void Button_ClickGameInfo(object sender, RoutedEventArgs e)
        {
            PopUpWindow.IsOpen = true;
        }
    }
}