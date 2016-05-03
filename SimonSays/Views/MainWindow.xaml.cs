using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using SimonSays.Utils;
using System;
using System.Linq;
using System.Windows;

namespace SimonSays.Views
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// AI system to use
        /// </summary>
        private AISystemEnum aiSystem;

        /// <summary>
        /// Kinect sensor object
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Kinect speech recognition engine
        /// </summary>
        private SpeechRecognitionEngine speechRecognizer;

        /// <summary>
        /// Construct Main menu window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.rbANN.IsChecked = true;
        }

        /// <summary>
        /// Easy button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEasy_Click(object sender, RoutedEventArgs e)
        {
            EasyClickFunctionality();
        }

        /// <summary>
        /// Easy click 
        /// </summary>
        private void EasyClickFunctionality() 
        {
            killKinect();
            var window = new GameWindow(aiSystem, DifficultyEnum.Easy);
            window.Show();
            this.Close();
        }

        /// <summary>
        /// Medium button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMedium_Click(object sender, RoutedEventArgs e)
        {
            MediumClickFunctionality();
        }

        /// <summary>
        /// Medium click
        /// </summary>
        private void MediumClickFunctionality()
        
        {
            killKinect();
            var window = new GameWindow(aiSystem, DifficultyEnum.Medium);
            window.Show();
            this.Close();
        }

        /// <summary>
        /// Hard button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHard_Click(object sender, RoutedEventArgs e)
        {
            HardClickFunctionality();
        }

        /// <summary>
        /// Hard click
        /// </summary>
        private void HardClickFunctionality()
        {
            killKinect();
            var window = new GameWindow(aiSystem, DifficultyEnum.Hard);
            window.Show();
            this.Close();
        }

        /// <summary>
        /// Training button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            TrainClickFunctionality();
        }

        /// <summary>
        /// Train click
        /// </summary>
        private void TrainClickFunctionality()
        {
            killKinect();
            var window = new TrainWindow();
            window.Show();
            this.Close();
        }

        /// <summary>
        /// Toggle AI system to use
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (this.rbANN.IsChecked == true)
            {
                aiSystem = AISystemEnum.NN;
            } 
            else if (this.rbNB.IsChecked == true) 
            {
                aiSystem = AISystemEnum.NaiveBayes;
            }
        }

        /// <summary>
        /// Get the speech recognizer
        /// </summary>
        /// <returns></returns>
        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        private KinectSensor InitializeKinect()
        {
            sensor = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);

            if (sensor != null)
            {
                speechRecognizer = CreateSpeechRecognizer();
                // Start the sensor
                sensor.Start();
                // Start audio stream
                StartStreamingKinectAudio();
            }

            return sensor;
        }

        //Start streaming audio
        private void StartStreamingKinectAudio()
        {
            //set sensor audio source to variable
            var audioSource = sensor.AudioSource;
            // set the beam angle to the direction the audio is pointing
            audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            // start the audiosource
            var kinectStream = audioSource.Start();
            // configure incoming audio stream
            speechRecognizer.SetInputToAudioStream(kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            // make sure the recognizer does not stop after finsihing
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            // reduce the background and ambient noise for better accuracy
            sensor.AudioSource.EchoCancellationMode = EchoCancellationMode.None;
            sensor.AudioSource.AutomaticGainControlEnabled = false;
        }

        /// <summary>
        /// Responsible for recognising the speech
        /// </summary>
        /// <returns></returns>
        private SpeechRecognitionEngine CreateSpeechRecognizer()
        {
            RecognizerInfo ri = GetKinectRecognizer();
            SpeechRecognitionEngine sre;
            sre = new SpeechRecognitionEngine(ri.Id);

            // Add the words that we want the game to recognise
            var grammar = new Choices();
            grammar.Add("SIMON EASY");
            grammar.Add("SIMON MEDIUM");
            grammar.Add("SIMON HARD");
            grammar.Add("SIMON TRAIN");

            //set culture - language, coutry/region
            var gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(grammar);

            // set up the grammar builder

            var g = new Grammar(gb);
            sre.LoadGrammar(g);

            // set events for recogizing and rejecting speech
            sre.SpeechRecognized += SreSpeechRecognised;
            sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;
            return sre;

        }

        private void RejectSpeech(RecognitionResult result)
        {
            System.Diagnostics.Debug.WriteLine("Sorry Simon didn't quite understand that");
        }

        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            RejectSpeech(e.Result);
        }

        // Speech is recognised
        private void SreSpeechRecognised(object sender, SpeechRecognizedEventArgs e)
        {
            //Change this value to adjust the accuracy the higer the value the better the accuracy
            // the more accurate it will have to be, lower it if it is not recognizing you
            if (e.Result.Confidence < .99)
            {
                RejectSpeech(e.Result);
            }

            //set here what you want the game to do when it recognises speech
            switch (e.Result.Text)
            {
                case "SIMON EASY":
                    System.Diagnostics.Debug.WriteLine("easy button pressed");
                    EasyClickFunctionality();
                    break;
                case "SIMON MEDIUM":
                    System.Diagnostics.Debug.WriteLine("medium button pressed");
                    MediumClickFunctionality();
                    break;
                case "SIMON HARD":
                    System.Diagnostics.Debug.WriteLine("hard button pressed");
                    HardClickFunctionality();
                    break;
                case "SIMON TRAIN":
                    System.Diagnostics.Debug.WriteLine("home button pressed");
                    TrainClickFunctionality();
                    break;
            }

        }

        private void Speech_Checked(object sender, RoutedEventArgs e)
        {
            if (this.speech_CheckBox.IsChecked == true)
            {
                InitializeKinect();
            }
            else if (this.speech_CheckBox.IsChecked == false)
            {
                killKinect();
            }
        }

        /// <summary>
        /// Kill the kinect system
        /// </summary>
        private void killKinect()
        {
            if (this.sensor != null)
            {
                sensor.Stop();
                sensor.Dispose();
            }
        }
    }
}
