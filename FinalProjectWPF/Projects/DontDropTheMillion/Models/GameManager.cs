using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DontDropTheMillion
{
    public class GameManager
    {
        public int TotalMoney { get;  set; } = 1000000;

        public GameManager() { } 

        public int SubmitAnswer(TriviaQuestion question ,Dictionary<string, int> bets)
        {
            var correctAnswer = question.CorrectAnswer;
            int Correctindex = 0;
            for (int i = 0; i <= 3; i++)
            {
                if (question.AllAnswers[i] == correctAnswer)
                {
                    Correctindex = i;
                }
            }
            int lostMoney = bets.Where(bet => bet.Key != $"Answer{Correctindex}").Sum(bet => bet.Value);
            TotalMoney -= lostMoney;
            return TotalMoney;
        }

        public bool IsGameOver()
        {
            return TotalMoney <= 0;
        }


    }

}
