using System.Windows;

namespace SimonSays.Views
{
    /// <summary>
    /// Interaction logic for GameOver.xaml
    /// </summary>
    public partial class GameOver : Window
    {
        public GameOver(int score)
        {
            InitializeComponent();
            lblScoreValue.Content = score;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication();
        }

        private void btnPlayAgain_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            ExitApplication();
        }

        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
