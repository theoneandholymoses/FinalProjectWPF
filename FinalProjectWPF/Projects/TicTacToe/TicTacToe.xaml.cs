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

namespace FinalProjectWPF.Projects.TicTacToe
{
    /// <summary>
    /// Interaction logic for TicTacToe.xaml
    /// </summary>
    public partial class TicTacToe : Page
    {
        private int _player = 0;
        private char[] _boardArr = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private int _Pscore = 0;
        private int _P2score = 0;
        private int _Cscore = 0;
        private bool _gameOver = false;
        private Button[] _buttons;
        private int _gameLevel = 1;
        int[,] winConditions = new int[,]
        {
            {1, 2, 3},
            {4, 5, 6},
            {7, 8, 9},
            {1, 4, 7},
            {2, 5, 8},
            {3, 6, 9},
            {1, 5, 9},
            {3, 5, 7}
        };
        public TicTacToe()
        {
            InitializeComponent();
            _buttons = new Button[] { button1, button2, button3, button4, button5, button6, button7, button8, button9 };
            if (_player == 1)
            {
                if (_gameLevel == 2)
                {
                    ComputerPlayHard();
                }
                else
                {
                    ComputerPlayEasy();
                }
            }
        }

        private async Task ComputerPlayEasy()
        {
            await Task.Delay(1000);
            Random rand = new Random();
            int computerMove;
            do
            {
                computerMove = rand.Next(1, 10); // Random number between 1 and 9
            }
            while (_boardArr[computerMove] == 'X' || _boardArr[computerMove] == 'O');

            _buttons[computerMove - 1].Content = _player == 0 ? 'O' : 'X';
            _boardArr[computerMove] = _player == 0 ? 'O' : 'X';

            if (CheckWin() == 1)
            {
                WinnerDisplay.Content = "Computer win";
                _gameOver = true;
            }
            if (isBoarderFull() && CheckWin() == 0)
            {
                WinnerDisplay.Content = "it's a tie";
            };
        }

        private async Task ComputerPlayHard()
        {
            await Task.Delay(1000);
            int computerMove = -1;

            // Determine the computer's mark based on the current player
            char computerMark = _player == 0 ? 'O' : 'X';
            char playerMark = _player == 0 ? 'X' : 'O';

            // Check for a winning move first
            computerMove = GetBestMove(computerMark); // Computer's mark
            if (computerMove == -1)
            {
                // Check if we need to block the player from winning
                computerMove = GetBestMove(playerMark); // Player's mark
            }

            // If no winning or blocking move, pick a random move
            if (computerMove == -1)
            {
                Random rand = new Random();
                do
                {
                    computerMove = rand.Next(1, 10); // Random number between 1 and 9
                }
                while (_boardArr[computerMove] == 'X' || _boardArr[computerMove] == 'O');
            }

            // Make the move
            _buttons[computerMove - 1].Content = computerMark;
            _boardArr[computerMove] = computerMark;

            if (CheckWin() == 1)
            {
                WinnerDisplay.Content = "Computer wins!";
                _gameOver = true;
            }
            else if (isBoarderFull())
            {
                WinnerDisplay.Content = "It's a tie!";
            }
        }

        // Helper function to find the best move for either winning or blocking
        private int GetBestMove(char playerMark)
        {
            for (int i = 0; i < winConditions.GetLength(0); i++)
            {
                int pos1 = winConditions[i, 0];
                int pos2 = winConditions[i, 1];
                int pos3 = winConditions[i, 2];

                // Check if two spots are filled by the player and one is empty
                if (_boardArr[pos1] == playerMark && _boardArr[pos2] == playerMark && _boardArr[pos3] != 'X' && _boardArr[pos3] != 'O')
                {
                    return pos3;
                }
                if (_boardArr[pos1] == playerMark && _boardArr[pos3] == playerMark && _boardArr[pos2] != 'X' && _boardArr[pos2] != 'O')
                {
                    return pos2;
                }
                if (_boardArr[pos2] == playerMark && _boardArr[pos3] == playerMark && _boardArr[pos1] != 'X' && _boardArr[pos1] != 'O')
                {
                    return pos1;
                }
            }
            return -1; // No winning or blocking move found
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (_gameLevel > 0)
            {
                if (clickedButton.Content == null && !_gameOver)
                {
                    if (!_gameOver)
                    {
                        if (_player == 0)
                        {
                            clickedButton.Content = 'X';
                        }
                        else
                        {
                            clickedButton.Content = 'O';
                        }
                        int index = int.Parse(clickedButton.Name.Last().ToString());
                        _boardArr[index] = (char)clickedButton.Content;
                    }

                    if (CheckWin() == 1 && !_gameOver)
                    {
                        WinnerDisplay.Content = "Player win";
                        _gameOver = true;

                    }
                    else if (isBoarderFull() && CheckWin() == 0)
                    {
                        WinnerDisplay.Content = "it's a tie";
                    }
                    else
                    {
                        if (_gameLevel == 2)
                        {
                            await ComputerPlayHard();
                        }
                        else
                        {
                            await ComputerPlayEasy();
                        }
                    };
                }
            }
            else
            {
                if (clickedButton.Content == null)
                {
                    if (!_gameOver)
                    {
                        if (_player == 0)
                        {
                            clickedButton.Content = 'X';
                            UpdateTurnDisplay();
                        }
                        else
                        {
                            clickedButton.Content = 'O';
                            UpdateTurnDisplay();

                        }
                        int index = int.Parse(clickedButton.Name.Last().ToString());
                        _boardArr[index] = (char)clickedButton.Content;
                    }

                    if (CheckWin() == 1 && !_gameOver)
                    {
                        if (_player == 0)
                        {
                            WinnerDisplay.Content = "Player X win";
                            _Pscore++;
                            Pscore.Content = _Pscore;
                            _gameOver = true;
                            UpdateTurnDisplay();

                        }
                        else
                        {
                            WinnerDisplay.Content = "Player O win";
                            _P2score++;
                            P2score.Content = _P2score;
                            _gameOver = true;
                            UpdateTurnDisplay();

                        }

                    };
                    if (isBoarderFull() && CheckWin() == 0)
                    {
                        WinnerDisplay.Content = "it's a tie";
                        UpdateTurnDisplay();
                    };
                }
            }
        }
        private void UpdateTurnDisplay()
        {
            if (_player == 1)
            {
                _player = 0;
                CurrentPdisplay.Content = "Player O turn";
            }
            else
            {
                _player = 1;
                CurrentPdisplay.Content = "Player X turn";
            }
        }

        private bool isBoarderFull()
        {

            for (int i = 1; i < _boardArr.Length; i++)
            {
                if (_boardArr[i] != 'X' && _boardArr[i] != 'O')
                {
                    return false;
                }
            }
            return true;
        }

        private int CheckWin()
        {
            char tempPlayer = _player == 0 ? 'X' : 'O';

            for (int i = 0; i < winConditions.GetLength(0); i++)
            {
                if (_boardArr[winConditions[i, 0]] == _boardArr[winConditions[i, 1]] && _boardArr[winConditions[i, 1]] == _boardArr[winConditions[i, 2]])
                {
                    return 1;
                }
            }

            return 0;
        }



        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            WinnerDisplay.Content = null;
            for (int slotToReset = 1; slotToReset < _boardArr.Length; slotToReset++)
            {
                _boardArr[slotToReset] = (char)slotToReset;
            }

            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].Content = null;
            }
            _gameOver = false;
            _Pscore = 0;
            Pscore.Content = "0";
            _Cscore = 0;
            Cscore.Content = "0";
            _P2score = 0;
            P2score.Content = "0";

        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (StartP.IsChecked == true)
            {
                _player = 0;
            }
            else
            {
                _player = 1;
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Gamelevel.SelectedIndex == 0 && StartP != null)
            {
                _gameLevel = 1;
                StartP.Visibility = Visibility.Visible;
                StartP2.Visibility = Visibility.Visible;
                StartButton.Visibility = Visibility.Visible;
                Cscore.Visibility = Visibility.Visible;
                P2score.Visibility = Visibility.Hidden;
                SecondPlayer.Content = "Computer score";
            }
            else if (Gamelevel.SelectedIndex == 1)
            {
                _gameLevel = 2;
                StartP.Visibility = Visibility.Visible;
                StartP2.Visibility = Visibility.Visible;
                StartButton.Visibility = Visibility.Visible;
                Cscore.Visibility = Visibility.Visible;
                P2score.Visibility = Visibility.Hidden;
                SecondPlayer.Content = "Computer score";
            }
            else if (StartP != null)
            {
                _gameLevel = 0;
                _player = 0;
                StartP.Visibility = Visibility.Hidden;
                StartP2.Visibility = Visibility.Hidden;
                StartButton.Visibility = Visibility.Hidden;
                Cscore.Visibility = Visibility.Hidden;
                P2score.Visibility = Visibility.Visible;
                SecondPlayer.Content = "Player 2 score";
            }
        }



        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_player == 1)
            {
                if (_gameLevel == 2)
                {
                    await ComputerPlayHard();
                }
                else
                {
                    await ComputerPlayEasy();
                }
            }
        }
    }
}