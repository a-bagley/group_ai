using System;
using System.Windows;

namespace SimonSays
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        System.Windows.Threading.DispatcherTimer _timer = new System.Windows.Threading.DispatcherTimer();
        TimeSpan _time;
        private bool restartTimer;
        private bool gameOver;
        int seconds = 0;
        bool firstTimer = true;
        int lives;
        int score = 0;

        public GameWindow(Difficulty difficulty)
        {
            InitializeComponent();            

            switch (difficulty)
            {
                case Difficulty.Easy:
                    lives = 5;
                    seconds = 6;
                    break;
                case Difficulty.Medium:
                    lives = 3;
                    seconds = 5;
                    break;
                case Difficulty.Hard:
                    lives = 1;
                    seconds = 3;
                    break;
                default:
                    lives = 3;
                    seconds = 5;
                    break;
            }

            lblLives.Content = "Lives: " + lives;
            lblScore.Content = "Score: " + score;
            lblSeconds.Content = seconds;

            StartCountdown();

            if (restartTimer)
            {
                CalculateGestureScore();
            }

        }

        private void CalculateGestureScore()
        {
            MessageBox.Show("Score");

            // Display stars and increment score

            // If score is 0 lose a life

            if (lives == 0)
            {
                var window = new GameOver();
                window.Show();
                this.Close();
            }
        }

        private void StartCountdown()
        {
            _time = TimeSpan.FromSeconds(seconds);
            _timer.Tick += new EventHandler(dispatcherTimer_Tick);
            _timer.Interval = new TimeSpan(0, 0, 1); 
            _timer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            lblSeconds.Content = _time.ToString("ss");

            if (_time == TimeSpan.Zero)
            {
                _timer.Stop();
                restartTimer = false;
                CalculateGestureScore();               
            }
            _time = _time.Add(TimeSpan.FromSeconds(-1));
        }

        private void btnRestartTimer_Click(object sender, RoutedEventArgs e)
        {
            _time = TimeSpan.FromSeconds(seconds);
            _timer.Start();
        }

    }
}
