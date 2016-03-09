using System.Windows;

namespace SimonSays
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnEasy_Click(object sender, RoutedEventArgs e)
        {            
            var window = new GameWindow(Difficulty.Easy);
            window.Show();
            this.Close();
        }

        private void btnMedium_Click(object sender, RoutedEventArgs e)
        {
            var window = new GameWindow(Difficulty.Medium);
            window.Show();
            this.Close();
        }

        private void btnHard_Click(object sender, RoutedEventArgs e)
        {
            var window = new GameWindow(Difficulty.Hard);
            window.Show();
            this.Close();
        }
    }
}
