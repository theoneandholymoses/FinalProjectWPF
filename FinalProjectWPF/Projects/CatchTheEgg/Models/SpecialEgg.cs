using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectWPF.Projects.CatchTheEgg.Models
{
    internal class SpecialEgg : Egg
    {
        public string Skill { get; set; }

        public SpecialEgg(int position, string imgSrc, string skill) : base(position, imgSrc)
        {
            Skill = skill;
            ItemShape.Name = "special";
        }
    }
}
