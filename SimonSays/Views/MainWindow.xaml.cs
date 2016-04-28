using System.Windows;
using SimonSays.Utils;
using SimonSays.Views;

namespace SimonSays.Views
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
            var window = new GameWindow(AISystemEnum.NN, DifficultyEnum.Easy);
            window.Show();
            this.Close();
        }

        private void btnMedium_Click(object sender, RoutedEventArgs e)
        {
            var window = new GameWindow(AISystemEnum.NN, DifficultyEnum.Medium);
            window.Show();
            this.Close();
        }

        private void btnHard_Click(object sender, RoutedEventArgs e)
        {
            var window = new GameWindow(AISystemEnum.NN, DifficultyEnum.Hard);
            window.Show();
            this.Close();
        }

        private void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            var window = new TrainWindow();
            window.Show();
            this.Close();
        }
    }
}
