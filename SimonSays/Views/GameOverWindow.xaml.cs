using System.Windows;

namespace SimonSays.Views
{
    /// <summary>
    /// Interaction logic for GameOver.xaml
    /// </summary>
    public partial class GameOver : Window
    {
        /// <summary>
        /// Construct Game Over window
        /// </summary>
        /// <param name="score"></param>
        public GameOver(int score)
        {
            InitializeComponent();
            lblScoreValue.Content = score;
        }

        /// <summary>
        /// The exit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            ExitApplication();
        }

        /// <summary>
        /// Play again button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlayAgain_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            ExitApplication();
        }

        /// <summary>
        /// Exit the application
        /// </summary>
        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
