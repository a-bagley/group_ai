using SimonSays.Views;
using System.Windows;

namespace SimonSays
{
    /// <summary>
    /// Starting point of application
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
