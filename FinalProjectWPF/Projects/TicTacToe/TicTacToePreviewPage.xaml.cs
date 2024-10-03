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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalProjectWPF.Projects.TicTacToe
{
    /// <summary>
    /// Interaction logic for TicTacToePreviewPage.xaml
    /// </summary>
    public partial class TicTacToePreviewPage : Page
    {
        public TicTacToePreviewPage()
        {
            InitializeComponent();
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void MoveToProject_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TicTacToe());
        }
    }
}
