using FinalProjectWPF.Projects.Snake;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FinalProjectWPF
{
    /// <summary>
    /// Interaction logic for GameCenterPage.xaml
    /// </summary>
    public partial class GameCenterPage : Page
    {
        public GameCenterPage()
        {
            InitializeComponent();

            InitializeComponent();
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            ClockText.Text = DateTime.Now.ToString("HH:mm:ss");
            ClockDate.Text = DateTime.Now.ToString("d");
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image image = (sender as Image)!;
            image.Opacity = 0.6;
        }


        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Image)!.Opacity = 1;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Name: User\nEmail: user@gmail.com\nPhone: 0500000000", "👤Contact Information");
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as Border).BorderBrush = Brushes.AliceBlue;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Border).BorderBrush = Brushes.Transparent;
        }


        public void SnakeGame_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new SnakeHomePage());
        }
    }
}
