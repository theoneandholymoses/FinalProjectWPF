using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinalProjectWPF.Projects.CatchTheEgg.Models
{
    internal class Egg
    {
        private string _imgSrc;
        private Rectangle _ItemShape;

        public int Position { get; set; }
        public bool IsMissed = false;
        public string ImgSrc
        {
            get
            { return _imgSrc; }
            set
            {
                if (_imgSrc != value)
                {
                    _imgSrc = value;
                    UpdateEggShapeImage();
                    OnPropertyChanged(nameof(ImgSrc));
                }
            }
        }
        public Rectangle ItemShape
        {
            get
            {
                return _ItemShape;
            }
            set
            {
                if (value != _ItemShape)
                {
                    _ItemShape = value;
                    OnPropertyChanged(nameof(ItemShape));
                }
            }
        }

        public Egg(int position, string imgSrc)
        {
            Position = position;
            ImgSrc = imgSrc;
            ItemShape = new Rectangle
            {
                Width = 30,
                Height = 45,
                Fill = new ImageBrush(new BitmapImage(new Uri(imgSrc, UriKind.Relative)))
            };
        }

        private void UpdateEggShapeImage()
        {
            if (ItemShape != null && !string.IsNullOrEmpty(ImgSrc))
            {
                ItemShape.Fill = new ImageBrush(new BitmapImage(new Uri(ImgSrc, UriKind.Relative)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
