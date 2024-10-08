﻿using DontDropTheMillion;
using FinalProjectWPF.Enums;
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
using System.Windows.Threading;


namespace FinalProjectWPF.Projects.DontDropTheMillion
{
    /// <summary>
    /// Interaction logic for DontDropTheMillion.xaml
    /// </summary>
    public partial class DontDropTheMillion : Page
    {
        private DispatcherTimer _timer = new DispatcherTimer();
        private int _timeRemaining;
        private GameManager _gameManager = new GameManager();
        private QuestionManager _questionManager = new QuestionManager();
        private int _currentQuestionIndex = 1;
        private TriviaQuestion? _currentQuestion;
        private static Random _random = new Random();
        private int CountOfQuestions = 0;
        private DispatcherTimer _betTimer;
        private bool _isBettingUp = false;
        private bool _isBettingDown = false;
        private int _currentAnswerIndex;
        private Dictionary<string, int> _currentBets = new Dictionary<string, int>()
        {
            { "Answer0", 0 },
            { "Answer1", 0 },
            { "Answer2", 0 },
            { "Answer3", 0 }
        };

        public DontDropTheMillion()
        {
            InitializeComponent();
            LoadCatagories();
            SetupTimer();
            SetupBetTimer(); // New method to set up the betting timer
        }
        public void LoadCatagories()
        {
            CountOfQuestions++;
            var levelGrids = LevelGrids.Children.OfType<Grid>().Where(g => g.Tag?.ToString() == "LevelGrid").ToList();
            if (CountOfQuestions - 1 >= 0 && CountOfQuestions - 1 < levelGrids.Count)
            {
                levelGrids[CountOfQuestions - 1].Background = new SolidColorBrush(Colors.Cyan);
            }
            QuestionNum.Content = CountOfQuestions;
            Timer.Visibility = Visibility.Hidden;
            TimerText.Visibility = Visibility.Hidden;
            var (cat1, cat2) = GetTwoRandomCategories();
            Catagory1.Content = cat1.ToString();
            Catagory2.Content = cat2.ToString();

            Catagory1.Visibility = Visibility.Visible;
            Catagory2.Visibility = Visibility.Visible;
        }

        public async Task LoadQuestionForCatagory(TriviaCategory catagory)
        {
            // Hide categories and show question fields
            Timer.Visibility = Visibility.Visible;
            TimerText.Visibility = Visibility.Visible;
            Catagory1.Visibility = Visibility.Hidden;
            Catagory2.Visibility = Visibility.Hidden;
            Catagory1.Content = "";
            Catagory2.Content = "";
            QuestionLabel.Visibility = Visibility.Visible;
            Answer0.Visibility = Visibility.Visible;
            Answer1.Visibility = Visibility.Visible;
            Answer2.Visibility = Visibility.Visible;
            Answer3.Visibility = Visibility.Visible;
            Answer1Bet.Visibility = Visibility.Visible;
            Answer2Bet.Visibility = Visibility.Visible;
            Answer3Bet.Visibility = Visibility.Visible;
            Answer4Bet.Visibility = Visibility.Visible;
            A1Down.Visibility = Visibility.Visible;
            A2Down.Visibility = Visibility.Visible;
            A3Down.Visibility = Visibility.Visible;
            A4Down.Visibility = Visibility.Visible;
            A1Up.Visibility = Visibility.Visible;
            A2Up.Visibility = Visibility.Visible;
            A3Up.Visibility = Visibility.Visible;
            A4Up.Visibility = Visibility.Visible;

            _currentQuestion = await _questionManager.GetQuestionsAsync(GetGameLevel(), catagory);
            QuestionLabel.Text = _currentQuestion.QuestionText;

            Answer0.Text = _currentQuestion.AllAnswers[0];
            Answer1.Text = _currentQuestion.AllAnswers[1];
            Answer2.Text = _currentQuestion.AllAnswers[2];
            Answer3.Text = _currentQuestion.AllAnswers[3];

            StartTimer();
        }

        public void IncreaseBetAmount(string answerKey)
        {
            double availableMoney = _gameManager.TotalMoney - _currentBets.Values.Sum();
            if (availableMoney > 0)
            {
                _currentBets[answerKey] += 20000;
                UpdateUIForBets();
            }
            if (_currentBets.Values.Sum() >= _gameManager.TotalMoney)
            {
                LockIncreaseButtons();
            }
        }

        public void DecreaseBetAmount(string answerKey)
        {
            if (_currentBets[answerKey] > 0)
            {
                _currentBets[answerKey] -= 20000;
                UpdateUIForBets();
            }
            if (_currentBets.Values.Sum() < _gameManager.TotalMoney)
            {
                UnlockIncreaseButtons();
            }
        }

        private async Task HighlightCorrectAnswer()
        {
            await Task.Delay(1000);
            int CorrectIndex = 0;
            for (int i = 0; i <= 3; i++)
            {
                if (_currentQuestion != null && _currentQuestion.AllAnswers[i] == _currentQuestion.CorrectAnswer)
                {
                    CorrectIndex = i;
                }
                else
                {
                    await Task.Delay(500);
                    string imgName = $"A{i}WrongImg";
                    string gridName = $"A{i}Grid";
                    string mainGridName = $"A{i}MainGrid";
                    Grid? mg = mainGrid.Children.OfType<Grid>().Where(r => r.Name == mainGridName).FirstOrDefault();
                    Grid? g = mg.Children.OfType<Grid>().Where(r => r.Name == gridName).FirstOrDefault();
                    Image? m = g.Children.OfType<Image>().Where(im => im.Name == imgName).FirstOrDefault();
                    if (m != null)
                    {
                        m.Visibility = Visibility.Visible;
                    }
                }
            }
            await Task.Delay(500);
            switch (CorrectIndex)
            {
                case 0:
                    A0CorrectImg.Visibility = Visibility.Visible;
                    break;
                case 1:
                    A1CorrectImg.Visibility = Visibility.Visible;
                    break;
                case 2:
                    A2CorrectImg.Visibility = Visibility.Visible;
                    break;
                case 3:
                    A3CorrectImg.Visibility = Visibility.Visible;
                    break;
            }

            await Task.Delay(2000);
            A0WrongImg.Visibility = Visibility.Hidden;
            A1WrongImg.Visibility = Visibility.Hidden;
            A2WrongImg.Visibility = Visibility.Hidden;
            A3WrongImg.Visibility = Visibility.Hidden;
            A0CorrectImg.Visibility = Visibility.Hidden;
            A1CorrectImg.Visibility = Visibility.Hidden;
            A2CorrectImg.Visibility = Visibility.Hidden;
            A3CorrectImg.Visibility = Visibility.Hidden;


        }

        public async Task SubmitAnswers()
        {
            _timer.Stop();
            A1Down.Visibility = Visibility.Hidden;
            A2Down.Visibility = Visibility.Hidden;
            A3Down.Visibility = Visibility.Hidden;
            A4Down.Visibility = Visibility.Hidden;
            A1Up.Visibility = Visibility.Hidden;
            A2Up.Visibility = Visibility.Hidden;
            A3Up.Visibility = Visibility.Hidden;
            A4Up.Visibility = Visibility.Hidden;
            SubmitQuestion.Visibility = Visibility.Hidden;
            UnlockIncreaseButtons();
            SubmitQuestion.IsEnabled = false;
            int remainingMoney = _gameManager.SubmitAnswer(_currentQuestion, _currentBets);

            await HighlightCorrectAnswer();

            _currentBets = _currentBets.ToDictionary(k => k.Key, v => 0);
            UpdateUIForBets();

            if (_gameManager.IsGameOver())
            {
                MessageBox.Show("Game Over! You lost all the money.");
                ResetGameButton.Visibility = Visibility.Visible;
            }
            else if (_currentQuestionIndex >= 10)
            {
                MessageBox.Show("Congratulations! You won the game.");
                ((App)Application.Current).LastGameScore = (remainingMoney, GameType.DontDropTheMillion);
                ResetGameButton.Visibility = Visibility.Visible;
            }
            else
            {
                _currentQuestionIndex++;
                Catagory1.Visibility = Visibility.Visible;
                Catagory2.Visibility = Visibility.Visible;
                QuestionLabel.Visibility = Visibility.Hidden;
                Answer0.Visibility = Visibility.Hidden;
                Answer1.Visibility = Visibility.Hidden;
                Answer2.Visibility = Visibility.Hidden;
                Answer3.Visibility = Visibility.Hidden;
                Answer1Bet.Visibility = Visibility.Hidden;
                Answer2Bet.Visibility = Visibility.Hidden;
                Answer3Bet.Visibility = Visibility.Hidden;
                Answer4Bet.Visibility = Visibility.Hidden;
                Answer1Bet.Content = "";
                Answer2Bet.Content = "";
                Answer3Bet.Content = "";
                Answer4Bet.Content = "";
                UserMoney.Content = _gameManager.TotalMoney.ToString();
                LoadCatagories();
            }
        }


        private void SetupTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += TimerTick;
        }


        private void StartTimer()
        {
            _timeRemaining = 60; // 60 seconds
            Timer.Content = _timeRemaining.ToString();
            Timer.Visibility = Visibility.Visible;
            _timer.Start();
        }


        private async void TimerTick(object sender, EventArgs e)
        {
            _timeRemaining--;

            Timer.Content = _timeRemaining.ToString();
            if (_timeRemaining <= 0)
            {
                _timer.Stop();
                if (UserMoney.Content.ToString() != "0")
                {
                    MessageBox.Show("Time's up! money will be spread radomally between answers");
                    int j = _gameManager.TotalMoney;
                    for (int i = 0; i <= j; i += 20000)
                    {
                        IncreaseBetAmount("Answer" + _random.Next(0, 3));
                    }
                }
                await SubmitAnswers();
            }
        }

        private void SetupBetTimer()
        {
            _betTimer = new DispatcherTimer();
            _betTimer.Interval = TimeSpan.FromMilliseconds(50); // Example interval
            _betTimer.Tick += BetTimer_Tick;
        }

        private void BetTimer_Tick(object sender, EventArgs e)
        {
            if (_isBettingUp)
            {
                IncreaseBetAmount("Answer" + _currentAnswerIndex); // Replace with your actual logic
            }
            else if (_isBettingDown)
            {
                DecreaseBetAmount("Answer" + _currentAnswerIndex); // Replace with your actual logic
            }
        }

        private void UpdateUIForBets()
        {
            Answer1Bet.Content = $"{_currentBets["Answer0"]:C0}";
            Answer2Bet.Content = $"{_currentBets["Answer1"]:C0}";
            Answer3Bet.Content = $"{_currentBets["Answer2"]:C0}";
            Answer4Bet.Content = $"{_currentBets["Answer3"]:C0}";
            UserMoney.Content = _gameManager.TotalMoney - _currentBets.Values.Sum();
        }

        private void LockIncreaseButtons()
        {
            A1Up.IsEnabled = false;
            A2Up.IsEnabled = false;
            A3Up.IsEnabled = false;
            A4Up.IsEnabled = false;
            SubmitQuestion.IsEnabled = true;
            SubmitQuestion.Visibility = Visibility.Visible;
        }

        private void UnlockIncreaseButtons()
        {
            A1Up.IsEnabled = true;
            A2Up.IsEnabled = true;
            A3Up.IsEnabled = true;
            A4Up.IsEnabled = true;
            SubmitQuestion.Visibility = Visibility.Hidden;

        }

        private string GetGameLevel()
        {
            if (_currentQuestionIndex <= 4) return "easy";
            if (_currentQuestionIndex <= 7) return "medium";
            return "hard";
        }

        public static (TriviaCategory, TriviaCategory) GetTwoRandomCategories()
        {
            var categories = Enum.GetValues(typeof(TriviaCategory)).Cast<TriviaCategory>().ToArray();
            var firstCategory = categories[_random.Next(categories.Length)];
            TriviaCategory secondCategory;
            do
            {
                secondCategory = categories[_random.Next(categories.Length)];
            } while (secondCategory == firstCategory);
            return (firstCategory, secondCategory);
        }
        private async void Catagory1_Click(object sender, RoutedEventArgs e)
        {
            await LoadQuestionForCatagory((TriviaCategory)Enum.Parse(typeof(TriviaCategory), Catagory1.Content.ToString()));
        }

        private async void Catagory2_Click(object sender, RoutedEventArgs e)
        {
            await LoadQuestionForCatagory((TriviaCategory)Enum.Parse(typeof(TriviaCategory), Catagory2.Content.ToString()));
        }

        private void HandleBetMouseDown(int answerIndex, bool isBettingUp)
        {
            _currentAnswerIndex = answerIndex;
            _isBettingUp = isBettingUp;
            _isBettingDown = !isBettingUp;
            _betTimer.Start();
        }

        private void HandleBetMouseUp()
        {
            _isBettingUp = false;
            _isBettingDown = false;
            _betTimer.Stop();
        }
        private void A1Up_MouseDown(object sender, RoutedEventArgs e) => HandleBetMouseDown(0, true);
        private void A1Up_MouseUp(object sender, RoutedEventArgs e) => HandleBetMouseUp();
        private void A1Down_MouseDown(object sender, RoutedEventArgs e) => HandleBetMouseDown(0, false);
        private void A1Down_MouseUp(object sender, RoutedEventArgs e) => HandleBetMouseUp();

        private void A2Up_MouseDown(object sender, RoutedEventArgs e) => HandleBetMouseDown(1, true);
        private void A2Up_MouseUp(object sender, RoutedEventArgs e) => HandleBetMouseUp();
        private void A2Down_MouseDown(object sender, RoutedEventArgs e) => HandleBetMouseDown(1, false);
        private void A2Down_MouseUp(object sender, RoutedEventArgs e) => HandleBetMouseUp();

        private void A3Up_MouseDown(object sender, RoutedEventArgs e) => HandleBetMouseDown(2, true);
        private void A3Up_MouseUp(object sender, RoutedEventArgs e) => HandleBetMouseUp();
        private void A3Down_MouseDown(object sender, RoutedEventArgs e) => HandleBetMouseDown(2, false);
        private void A3Down_MouseUp(object sender, RoutedEventArgs e) => HandleBetMouseUp();

        private void A4Up_MouseDown(object sender, RoutedEventArgs e) => HandleBetMouseDown(3, true);
        private void A4Up_MouseUp(object sender, RoutedEventArgs e) => HandleBetMouseUp();
        private void A4Down_MouseDown(object sender, RoutedEventArgs e) => HandleBetMouseDown(3, false);
        private void A4Down_MouseUp(object sender, RoutedEventArgs e) => HandleBetMouseUp();
        private async void SubmitAnswer(object sender, RoutedEventArgs e)
        {
            await SubmitAnswers();
        }
        private void ResetGame(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DontDropTheMillion());
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            NavigationService.Navigate(new DontDropTheMillionPreviewPage());
        }
    }
}
