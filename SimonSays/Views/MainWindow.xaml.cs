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
        private AISystemEnum aiTechniqueEnum;

        public MainWindow()
        {
            InitializeComponent();
            this.rbANN.IsChecked = true;
        }

        private void btnEasy_Click(object sender, RoutedEventArgs e)
        {
            var window = new GameWindow(aiTechniqueEnum, DifficultyEnum.Easy);
            window.Show();
            this.Close();
        }

        private void btnMedium_Click(object sender, RoutedEventArgs e)
        {
            var window = new GameWindow(aiTechniqueEnum, DifficultyEnum.Medium);
            window.Show();
            this.Close();
        }

        private void btnHard_Click(object sender, RoutedEventArgs e)
        {
            var window = new GameWindow(aiTechniqueEnum, DifficultyEnum.Hard);
            window.Show();
            this.Close();
        }

        private void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            var window = new TrainWindow();
            window.Show();
            this.Close();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (this.rbANN.IsChecked == true)
            {
                aiTechniqueEnum = AISystemEnum.NN;
            } 
            else if (this.rbNB.IsChecked == true) 
            {
                aiTechniqueEnum = AISystemEnum.NaiveBayes;
            }
        }
    }
}
