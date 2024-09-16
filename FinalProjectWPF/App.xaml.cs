using FinalProjectWPF.FileManagment;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FinalProjectWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public int LoggedInUserID { get; set; }
        public dynamic fmGlobal = new FileManager();
    }

}
