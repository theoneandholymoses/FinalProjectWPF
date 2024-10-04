using DontDropTheMillion;
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
        }
        public void LoadCatagories()
        {
            CountOfQuestions++;
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
            QuestionLabel.Content = _currentQuestion.QuestionText;

            Answer0.Content = _currentQuestion.AllAnswers[0];
            Answer1.Content = _currentQuestion.AllAnswers[1];
            Answer2.Content = _currentQuestion.AllAnswers[2];
            Answer3.Content = _currentQuestion.AllAnswers[3];

            StartTimer();
            TheRightAnswer.Content = _currentQuestion.CorrectAnswer;
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
                    string recName = $"A{i}Rec";
                    Rectangle? r = mainGrid.Children.OfType<Rectangle>().Where(r => r.Name == recName).FirstOrDefault();
                    if (r != null)
                    {
                        r.Fill = new SolidColorBrush(Colors.Red);
                    }
                }
            }
            await Task.Delay(500);
            switch (CorrectIndex)
            {
                case 0:
                    A0Rec.Fill = new SolidColorBrush(Colors.LightGreen); 
                    break;
                case 1:
                    A1Rec.Fill = new SolidColorBrush(Colors.LightGreen);
                    break;
                case 2:
                    A2Rec.Fill = new SolidColorBrush(Colors.LightGreen);
                    break;
                case 3:
                    A3Rec.Fill = new SolidColorBrush(Colors.LightGreen);
                    break;
            }

            await Task.Delay(2000); 

            A0Rec.Fill = new SolidColorBrush(Colors.Transparent);
            A1Rec.Fill = new SolidColorBrush(Colors.Transparent);
            A2Rec.Fill = new SolidColorBrush(Colors.Transparent);
            A3Rec.Fill = new SolidColorBrush(Colors.Transparent);

        }

        //    private void EndGame()
        //    {
        //        MessageBox.Show($"Game over! You finished with ${_gameManager.TotalMoney}.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }
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

            }
            else if (_currentQuestionIndex >= 10) 
            {
                MessageBox.Show("Congratulations! You won the game.");

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
            _timeRemaining = 15; // 60 seconds
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
                MessageBox.Show("Time's up! money will be spread radomally between answers");
                if (_gameManager.TotalMoney != 0)
                {
                    int j = _gameManager.TotalMoney;
                    for (int i=0; i <= j; i += 20000)
                    {
                        IncreaseBetAmount("Answer"+_random.Next(0,3));
                    }
                }
                await SubmitAnswers(); 
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
        private void A1Up_Click(object sender, RoutedEventArgs e)
        {
            IncreaseBetAmount("Answer0");
        }
        private void A1Down_Click(object sender, RoutedEventArgs e)
        {
            DecreaseBetAmount("Answer0");
        }
        private void A2Up_Click(object sender, RoutedEventArgs e)
        {
            IncreaseBetAmount("Answer1");
        }
        private void A2Down_Click(object sender, RoutedEventArgs e)
        {
            DecreaseBetAmount("Answer1");
        }
        private void A3Up_Click(object sender, RoutedEventArgs e)
        {
            IncreaseBetAmount("Answer2");
        }
        private void A3Down_Click(object sender, RoutedEventArgs e)
        {
            DecreaseBetAmount("Answer2");
        }
        private void A4Up_Click(object sender, RoutedEventArgs e)
        {
            IncreaseBetAmount("Answer3");
        }
        private void A4Down_Click(object sender, RoutedEventArgs e)
        {
            DecreaseBetAmount("Answer3");
        }
        private async void SubmitAnswer(object sender, RoutedEventArgs e)
        {
            await SubmitAnswers();
        }
    }
}
