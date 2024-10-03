using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DontDropTheMillion
{
    public class TriviaQuestion
    {
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> IncorrectAnswers { get; set; }
        public List<string> AllAnswers { get; set; } = new List<string>();

        public TriviaQuestion(string questionText, string correctAnswer, List<string> incorrectAnswers)
        {
            QuestionText = questionText;
            CorrectAnswer = correctAnswer;
            IncorrectAnswers = incorrectAnswers;
            ShuffleAnswers();
        }
        public void ShuffleAnswers()
        {
            AllAnswers.Add(CorrectAnswer);
            AllAnswers.AddRange(IncorrectAnswers);
            AllAnswers = Shuffle(AllAnswers);
        }

        private List<T> Shuffle<T>(List<T> list)
        {
            var random = new System.Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
            return list;
        }
    }
}
