using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace DontDropTheMillion
{

    public class QuestionManager
    {
        private static readonly HttpClient _client = new HttpClient();

        public async Task<TriviaQuestion> GetQuestionsAsync(string difficulty, TriviaCategory category)
        {
            string categoryValue = ((int)category).ToString(); 
            string url = $"https://opentdb.com/api.php?amount=1&difficulty={difficulty}&category={categoryValue}&type=multiple";
            try
            {
                var response = await _client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                var triviaResponse = JsonSerializer.Deserialize<TriviaApiResponse>(content);

                if (triviaResponse.ResponseCode != 0)
                {
                    throw new Exception($"Trivia API error: Response code {triviaResponse.ResponseCode}");
                }

                if (triviaResponse == null || triviaResponse.Results == null || triviaResponse.Results.Count == 0)
                {
                    throw new Exception("No results found in the trivia API response.");
                }

                var result = triviaResponse.Results[0];

                var decodedQuestion = WebUtility.HtmlDecode(result.Question);
                var decodedCorrectAnswer = WebUtility.HtmlDecode(result.CorrectAnswer);
                var decodedIncorrectAnswers = result.IncorrectAnswers.Select(WebUtility.HtmlDecode).ToList();
                var question = new TriviaQuestion(decodedQuestion, decodedCorrectAnswer, decodedIncorrectAnswers);

                return question;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine($"Error fetching trivia question: {ex.Message}");
                return null;
            }
        }


        public class TriviaApiResponse
        {
            [JsonPropertyName("response_code")]
            public int ResponseCode { get; set; }

            [JsonPropertyName("results")]
            public List<TriviaQuestionResponse> Results { get; set; }
        }

        public class TriviaQuestionResponse
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("difficulty")]
            public string Difficulty { get; set; }

            [JsonPropertyName("category")]
            public string Category { get; set; }

            [JsonPropertyName("question")]
            public string Question { get; set; }

            [JsonPropertyName("correct_answer")]
            public string CorrectAnswer { get; set; }

            [JsonPropertyName("incorrect_answers")]
            public List<string> IncorrectAnswers { get; set; }
        }
    }
    public enum TriviaCategory
    {
        Any = 0,
        General = 9,
        Books = 10,
        Films = 11,
        Music = 12,
        Musicals = 13,
        Television = 14,
        VideoGames = 15,
        BoardGames = 16,
        Nature = 17,
        Computers = 18,
        Mathematics = 19,
        Mythology = 20,
        Sports = 21,
        Geography = 22,
        History = 23,
        Politics = 24,
        Art = 25,
        Celebrities = 26,
        Animals = 27,
        Vehicles = 28,
        EntertainmentComics = 29,
        ScienceGadgets = 30,
        AnimeManga = 31,
        CartoonAnimations = 32
    }
}
