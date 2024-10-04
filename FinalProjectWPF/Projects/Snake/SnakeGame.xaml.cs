using FinalProjectWPF.Enums;
using FinalProjectWPF.Projects.Snake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FinalProjectWPF.Projects.Snake
{
    /// <summary>
    /// Interaction logic for SnackGame.xaml
    /// </summary>
    public partial class SnakeGame : Page
    {
        private Game game;
        private DispatcherTimer gameTimer;
        private const int GridSize = 20;
        public SnakeGame()
        {
            InitializeComponent();
            InitializeGame();
            GameGrid.KeyDown += MainWindow_KeyDown;
        }


        private void InitializeGame()
        {
            game = new Game(GridSize, GridSize);
            InitializeGameGrid();

            gameTimer = new DispatcherTimer();
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = TimeSpan.FromMilliseconds(150);
            gameTimer.Start();
           
        }



        private void InitializeGameGrid()
        {
            GameGrid.Rows = GridSize;
            GameGrid.Columns = GridSize;

            GameGrid.Children.Clear();
            for (int i = 0; i < GridSize * GridSize; i++)
            {
                GameGrid.Children.Add(new GridCell());
            }
        }

        // modify
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            game.Update();
            UpdateUI();
            if (game.GameOver)
            {
                gameTimer.Stop();
                ((App)Application.Current).LastGameScore = (int.Parse(ScoreText.Content.ToString()), GameType.Snake);
                GameOverOverlay.Visibility = Visibility.Visible;
            }
        }



        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    game.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    game.ChangeDirection(Direction.Down);
                    break;
                case Key.Left:
                    game.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    game.ChangeDirection(Direction.Right);
                    break;
            }
        }

        private void UpdateUI()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    int index = row * GridSize + col;
                    ((GridCell)GameGrid.Children[index]).SetState(game.GetCellState(col, row), game.snake.direction);
                }
            }
            ScoreText.Content = game.Score.ToString();
            GameGrid.Focus();
        }





        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            game.ResetGame();
            GameOverOverlay.Visibility = Visibility.Collapsed;
            gameTimer.Start();
            UpdateUI();
        }
    

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
