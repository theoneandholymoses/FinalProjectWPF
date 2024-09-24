using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectWPF.Projects.CatchTheEgg.Models
{
    internal class Player
    {
        public string Name { get; set; }
        public int HighestScore { get; set; }

        public Player(string name, int score, int highestScore, double hearts)
        {
            Name = name;
            HighestScore = highestScore;
        }
    }
}
