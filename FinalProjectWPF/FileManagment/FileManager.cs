using FinalProjectWPF.Enums;
using FinalProjectWPF.UserManagment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;

namespace FinalProjectWPF.FileManagment
{
    internal class FileManager
    {
        private readonly string usersFilePath = "users.json";
        private readonly string highScoreSnakeFilePath = "highscoreSnake.json";
        private readonly string highScoreCatchTheEggFilePath = "highscoreCatchTheEgg.json";
        private readonly string highScoreDontDropTheMillionFilePath = "highscoreDontDropTheMillion.json";

        public FileManager()
        {
            InitializeFromDataFile();
        }

        public List<User>? GetAllUsers()
        {
            try
            {
                string jsonContent = File.ReadAllText(usersFilePath);
                return JsonSerializer.Deserialize<List<User>>(jsonContent);
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }


        public User CreateNewUser(string name)
        {
            List<User>? users = GetAllUsers() ?? new List<User>();
            int id = users.Count > 0 ? users.Max(u => u.ID) + 1 : 1;
            User player = new User(name, id);
            player.IsLogin = true;
            player.LastLogin = DateTime.Now;
            users.Add(player);
            File.WriteAllText(usersFilePath, JsonSerializer.Serialize(users));
            return player;
        }

        // modified
        public User UpdateUser(int userId, string name)
        {
            List<User> users = GetAllUsers() ?? new List<User>();
            User? user = users.FirstOrDefault(u => u.ID == userId);
            if (name != "")
            {
                if (user != null)
                {
                    user.FullName = name;
                    user.LastLogin = DateTime.Now;
                    File.WriteAllText(usersFilePath, JsonSerializer.Serialize(users));
                }
                
                return user;
            }
            return user;

        }


        public User CheckLastLoginUser()
        {
            List<User> users = GetAllUsers() ?? new List<User>();
            return users.OrderByDescending(u => u.LastLogin).First();
        }
        // modified
        public User LoginUser(int userId)
        {
            List<User> users = GetAllUsers() ?? new List<User>();
            User CurrentUser = users.Where(u => u.ID == userId).First();
            CurrentUser.LastLogin = DateTime.Now;
            CurrentUser.IsLogin = true;
            int index = users.FindIndex(u => u.ID == userId);
            users[index] = CurrentUser;
            File.WriteAllText(usersFilePath, JsonSerializer.Serialize(users));
            return CurrentUser;
        }

        public void LogoutUser(int userId)
        {
            List<User> users = GetAllUsers() ?? new List<User>();
            users.Where(u => u.ID == userId).First().IsLogin = false;
            File.WriteAllText(usersFilePath, JsonSerializer.Serialize(users));
        }

        public ObservableCollection<double> GetUserHighScoresForGame(int userID, GameType game)
        {
            string filePath = GetHighScoreFilePath(game);
            if (!File.Exists(filePath))
            {
                return new ObservableCollection<double>();
            }
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                Dictionary<int, ObservableCollection<double>> usersHighscores = JsonSerializer.Deserialize<Dictionary<int, ObservableCollection<double>>>(jsonContent) ?? new Dictionary<int, ObservableCollection<double>>();
                ObservableCollection<double> filteredUsersHighscores = new ObservableCollection<double>(usersHighscores.TryGetValue(userID, out ObservableCollection<double>? scores) ? scores : new ObservableCollection<double>());

                return filteredUsersHighscores;

            }
            catch (Exception)
            {
                return new ObservableCollection<double>();
            }
        }

        // modify

        public ObservableCollection<(GameType game, double score)> GetUserAllHighScores(int userID)
        {
            ObservableCollection<(GameType game, double score)> allHighScores = new ObservableCollection<(GameType game, double score)>();
            IEnumerable<GameType> gameTypes = Enum.GetValues(typeof(GameType)).Cast<GameType>();

            foreach (GameType game in gameTypes)
            {
                try
                {
                    // Get user's high scores for the specific game
                    ObservableCollection<double> scores = GetUserHighScoresForGame(userID, game);
                    if (scores != null && scores.Count > 0)
                    {
                        // Add the game and the highest score to the collection
                        allHighScores.Add((game, scores.Max()));
                    }
                }
                catch (Exception ex)
                {
                    // Optionally log the exception message or handle it
                    Console.WriteLine($"Error retrieving scores for user {userID} in game {game}: {ex.Message}");
                }
            }

            return allHighScores;
        }


        // modify
        public ObservableCollection<PlayerScore> GetAllPlayersHighScores(GameType game)
        {
            string filePath = GetHighScoreFilePath(game);
            if (!File.Exists(filePath))
            {
                return new ObservableCollection<PlayerScore>();
            }
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                var highScoresData = JsonSerializer.Deserialize<Dictionary<string, List<double>>>(jsonContent);
                ObservableCollection<PlayerScore> finalList = new ObservableCollection<PlayerScore>();

                // Assuming you have a method to get user names from IDs
                var users = GetAllUsers(); // This should return the list of users

                foreach (var entry in highScoresData)
                {
                    string customerId = entry.Key; // This is the customer ID
                    List<double> scores = entry.Value ?? new List<double>(); // Ensure the list is not null

                    var user = users.FirstOrDefault(u => u.ID.ToString() == customerId); // Get the user by ID
                    if (user != null)
                    {
                        // Add scores to the final list, even if it's just one score
                        foreach (var score in scores)
                        {
                            finalList.Add(new PlayerScore { Name = user.FullName, Score = score });
                        }
                    }
                }

                // Sort and return the high scores in descending order
                return new ObservableCollection<PlayerScore>(finalList.OrderByDescending(ps => ps.Score));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while fetching high scores: {ex.Message}");
                return new ObservableCollection<PlayerScore>();
            }
        }

        // modify
        public void AddNewHighScore(int userID, GameType game, double score)
        {
            string filePath = GetHighScoreFilePath(game);
            Dictionary<int, List<double>> usersHighscores;
            if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
            {
                string jsonContent = File.ReadAllText(filePath);
                usersHighscores = JsonSerializer.Deserialize<Dictionary<int, List<double>>>(jsonContent) ?? new Dictionary<int, List<double>>();
            }
            else
            {
                usersHighscores = new Dictionary<int, List<double>>();
            }
            if (!usersHighscores.ContainsKey(userID))
            {
                usersHighscores[userID] = new List<double> { score };
            }
            if (!usersHighscores[userID].Contains(score) && score != 0)
            {
                usersHighscores[userID].Add(score);
            }
            string updatedJson = JsonSerializer.Serialize(usersHighscores, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJson);
        }

        private string GetHighScoreFilePath(GameType game)
        {
            return game switch
            {
                GameType.Snake => highScoreSnakeFilePath,
                GameType.CatchTheEgg => highScoreCatchTheEggFilePath,
                GameType.DontDropTheMillion => highScoreDontDropTheMillionFilePath,
                _ => throw new ArgumentOutOfRangeException(nameof(game), $"No file path defined for game {game}")
            };
        }
        public void InitializeFromDataFile()
        {

            if (!File.Exists(usersFilePath))
            {
                string InitialFilePath = @"..\..\..\FileManagment\DataInitialization.json";
                if (!File.Exists(InitialFilePath))
                {
                    throw new FileNotFoundException("DataInitialization.json file not found.");
                }

                try
                {
                    string jsonContent = File.ReadAllText(InitialFilePath);
                    JsonDocument? data = JsonSerializer.Deserialize<JsonDocument>(jsonContent);
                    if (data != null)
                    {
                        string users = data.RootElement.GetProperty("users").ToString();
                        string highscoreSnake = data.RootElement.GetProperty("highscoreSnake").ToString();
                        string highscoreCatchTheEgg = data.RootElement.GetProperty("highscoreCatchTheEgg").ToString();
                        string highscoreDontDropTheMillion = data.RootElement.GetProperty("highscoreDontDropTheMillion").ToString();

                        File.WriteAllText(usersFilePath, users);
                        File.WriteAllText(highScoreSnakeFilePath, highscoreSnake);
                        File.WriteAllText(highScoreCatchTheEggFilePath, highscoreCatchTheEgg);
                        File.WriteAllText(highScoreDontDropTheMillionFilePath, highscoreDontDropTheMillion);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("DataInitialization.json file was failed to deserialized.");
                }
            }
        }
    }
    public class PlayerScore
    {
        public string Name { get; set; }
        public double Score { get; set; }
    }
}
