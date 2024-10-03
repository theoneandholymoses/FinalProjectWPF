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
        private DispatcherTimer _timer;
        private int _timeRemaining;
        private GameManager _gameManager = new GameManager();
        private QuestionManager _questionManager = new QuestionManager();
        private int _currentQuestionIndex = 1;
        private TriviaQuestion _currentQuestion;
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
        // Load two random categories
        public void LoadCatagories()
        {
            CountOfQuestions++;
            QuestionNum.Content = CountOfQuestions;
            Timer.Visibility = Visibility.Hidden;
            TimerText.Visibility = Visibility.Hidden;
            // Get random categories and display them
            var (cat1, cat2) = GetTwoRandomCategories();
            Catagory1.Content = cat1.ToString();
            Catagory2.Content = cat2.ToString();

            // Set categories visible
            Catagory1.Visibility = Visibility.Visible;
            Catagory2.Visibility = Visibility.Visible;
        }

        // Loads the question for a selected category
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

            // Get question from the API
            _currentQuestion = await _questionManager.GetQuestionsAsync(GetGameLevel(), catagory);
            QuestionLabel.Content = _currentQuestion.QuestionText;
            // Display answers in the UI
            Answer0.Content = _currentQuestion.AllAnswers[0];
            Answer1.Content = _currentQuestion.AllAnswers[1];
            Answer2.Content = _currentQuestion.AllAnswers[2];
            Answer3.Content = _currentQuestion.AllAnswers[3];

            // Start the timer
            StartTimer();
            TheRightAnswer.Content = _currentQuestion.CorrectAnswer;
        }

        // Handle increasing the bet amount
        public void IncreaseBetAmount(string answerKey)
        {
            // Ensure total money is still available to bet
            double availableMoney = _gameManager.TotalMoney - _currentBets.Values.Sum();
            if (availableMoney > 0)
            {
                _currentBets[answerKey] += 50000; // Increase bet by 10k (example)
                UpdateUIForBets();
            }
            // Check if all money is placed, and lock increase buttons
            if (_currentBets.Values.Sum() >= _gameManager.TotalMoney)
            {
                LockIncreaseButtons();
            }
        }

        public void DecreaseBetAmount(string answerKey)
        {
            if (_currentBets[answerKey] > 0)
            {
                _currentBets[answerKey] -= 50000; // Decrease bet by 10k (example)
                UpdateUIForBets();
            }
            // Unlock increase buttons if any money can still be bet
            if (_currentBets.Values.Sum() < _gameManager.TotalMoney)
            {
                UnlockIncreaseButtons();
            }
        }

        private async Task HighlightCorrectAnswer()
        {
            Task.Delay(1000);
            int CorrectIndex = 0;
            for (int i = 0; i <= 3; i++)
            {
                if (_currentQuestion.AllAnswers[i] == _currentQuestion.CorrectAnswer)
                {
                    CorrectIndex = i;
                    break; // Stop once the correct answer is found
                }
            }

            // Highlight the correct answer
            switch (CorrectIndex)
            {
                case 0:
                    A0Rec.Fill = new SolidColorBrush(Colors.LightGreen); // Set fill to green
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

            await Task.Delay(3000); // Wait for 3 seconds

            // Remove the fill (reset to default)
            switch (CorrectIndex)
            {
                case 0:
                    A0Rec.Fill = new SolidColorBrush(Colors.Transparent); // Reset to default color
                    break;
                case 1:
                    A1Rec.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case 2:
                    A2Rec.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
                case 3:
                    A3Rec.Fill = new SolidColorBrush(Colors.Transparent);
                    break;
            }

        }

        //    private void EndGame()
        //    {
        //        MessageBox.Show($"Game over! You finished with ${_gameManager.TotalMoney}.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
        //    }


        // Handle decreasing the bet amount


        // Submits the current bets and calculates the remaining money
        public async Task SubmitAnswers()
        {
            _timer.Stop();
            int remainingMoney = _gameManager.SubmitAnswer(_currentQuestion, _currentBets);
            // Update UI with new money
            UserMoney.Content = _gameManager.TotalMoney.ToString();
            await HighlightCorrectAnswer();
            // Reset bet fields for next round
            _currentBets = _currentBets.ToDictionary(k => k.Key, v => 0);
            UpdateUIForBets();
            // Check if game is over
            if (_gameManager.IsGameOver())
            {
                MessageBox.Show("Game Over! You lost all the money.");
                // Reset or end the game
            }
            else if (_currentQuestionIndex >= 10) // Assuming 10 questions to win
            {
                MessageBox.Show("Congratulations! You won the game.");
                // Reset or end the game
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
                LoadCatagories();
            }
        }

        // Timer setup
        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += TimerTick;
        }

        // Start the timer for answering
        private void StartTimer()
        {
            _timeRemaining = 60; // 60 seconds
            Timer.Content = _timeRemaining.ToString();
            Timer.Visibility = Visibility.Visible;
            _timer.Start();
        }

        // Timer tick event
        private void TimerTick(object sender, EventArgs e)
        {
            _timeRemaining--;

            Timer.Content = _timeRemaining.ToString();
            if (_timeRemaining <= 0)
            {
                _timer.Stop();
                MessageBox.Show("Time's up! You lost the round.");
                SubmitAnswers(); // Automatically submit with the current bets
            }
        }

        // Update the UI for bets
        private void UpdateUIForBets()
        {
            Answer1Bet.Content = $"{_currentBets["Answer0"]:C}";
            Answer2Bet.Content = $"{_currentBets["Answer1"]:C}";
            Answer3Bet.Content = $"{_currentBets["Answer2"]:C}";
            Answer4Bet.Content = $"{_currentBets["Answer3"]:C}";
            UserMoney.Content = _gameManager.TotalMoney - _currentBets.Values.Sum();
        }

        // Lock increase buttons once all money is placed
        private void LockIncreaseButtons()
        {
            A1Up.IsEnabled = false;
            A2Up.IsEnabled = false;
            A3Up.IsEnabled = false;
            A4Up.IsEnabled = false;
            SubmitQuestion.Visibility = Visibility.Visible;
        }

        // Unlock increase buttons when money can still be placed
        private void UnlockIncreaseButtons()
        {
            A1Up.IsEnabled = true;
            A2Up.IsEnabled = true;
            A3Up.IsEnabled = true;
            A4Up.IsEnabled = true;
            SubmitQuestion.Visibility = Visibility.Hidden;

        }

        // Helper method to get the game difficulty level
        private string GetGameLevel()
        {
            // Assume a difficulty based on question index
            if (_currentQuestionIndex <= 3) return "easy";
            if (_currentQuestionIndex <= 6) return "medium";
            return "hard";
        }

        // Get two random categories
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
        private void Catagory1_Click(object sender, RoutedEventArgs e)
        {
            // Load the question for the selected category (Catagory1)
            LoadQuestionForCatagory((TriviaCategory)Enum.Parse(typeof(TriviaCategory), Catagory1.Content.ToString()));
        }

        // Event handler for Category 2 button
        private void Catagory2_Click(object sender, RoutedEventArgs e)
        {
            // Load the question for the selected category (Catagory2)
            LoadQuestionForCatagory((TriviaCategory)Enum.Parse(typeof(TriviaCategory), Catagory2.Content.ToString()));
        }
        private void A1Up_Click(object sender, RoutedEventArgs e)
        {
            IncreaseBetAmount("Answer0");
        }

        // For decreasing the bet on Answer 1
        private void A1Down_Click(object sender, RoutedEventArgs e)
        {
            DecreaseBetAmount("Answer0");
        }

        // For increasing the bet on Answer 2
        private void A2Up_Click(object sender, RoutedEventArgs e)
        {
            IncreaseBetAmount("Answer1");
        }

        // For decreasing the bet on Answer 2
        private void A2Down_Click(object sender, RoutedEventArgs e)
        {
            DecreaseBetAmount("Answer1");
        }

        // For increasing the bet on Answer 3
        private void A3Up_Click(object sender, RoutedEventArgs e)
        {
            IncreaseBetAmount("Answer2");
        }

        // For decreasing the bet on Answer 3
        private void A3Down_Click(object sender, RoutedEventArgs e)
        {
            DecreaseBetAmount("Answer2");
        }

        // For increasing the bet on Answer 4
        private void A4Up_Click(object sender, RoutedEventArgs e)
        {
            IncreaseBetAmount("Answer3");
        }

        // For decreasing the bet on Answer 4
        private void A4Down_Click(object sender, RoutedEventArgs e)
        {
            DecreaseBetAmount("Answer3");
        }

        private void SubmitAnswer(object sender, RoutedEventArgs e)
        {
            SubmitAnswers();
        }
    }
}
