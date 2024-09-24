using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectWPF.Projects.CatchTheEgg.Models
{
     internal class Basket : INotifyPropertyChanged
    {
        private double _size;
        private double _position; 

        public double Position { 
            get => _position;
            set 
            { 
                _position = value;
                OnPropertyChanged(nameof(Position));
            } 
        }
        public double Size {
            get => _size;
            set
            {
                _size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public Basket(double position, double size)
        {
            Position = position;
            Size = size;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
