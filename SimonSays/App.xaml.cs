using SimonSays.Views;
using System.Windows;

namespace SimonSays
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow startWindow = new MainWindow();
            startWindow.Show();
        }
    }
}
