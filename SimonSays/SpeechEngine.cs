using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.IO;
using System.Threading;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Windows;

public sealed class SpeechEngine
{
    private static readonly SpeechEngine instance;
    public KinectSensor CurrentSensor;
    public SpeechRecognitionEngine speechRecognizer;
    private static Window _currentWindow;

    public static SpeechEngine getInstance
    {
        get 
        {
            return instance;
        }
    }


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

    private SpeechEngine()
    {
        //InitializeComponent();
        InitializeKinect();
    }

    private KinectSensor InitializeKinect()
    {
        CurrentSensor = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected);
        speechRecognizer = CreateSpeechRecognizer();
        // Start the sensor
        CurrentSensor.Start();
        // Start audio stream
        Start();
        return CurrentSensor;
    }

    //Start streaming audio
    private void Start()
    {
        //set sensor audio source to variable
        var audioSource = CurrentSensor.AudioSource;
        // set the beam angle to the direction the audio is pointing
        audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
        // start the audiosource
        var kinectStream = audioSource.Start();
        // configure incoming audio stream
        speechRecognizer.SetInputToAudioStream(kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
        // make sure the recognizer does not stop after finsihing
        speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
        // reduce the background and ambient noise for better accuracy
        CurrentSensor.AudioSource.EchoCancellationMode = EchoCancellationMode.None;
        CurrentSensor.AudioSource.AutomaticGainControlEnabled = false;
    }

    private SpeechRecognitionEngine CreateSpeechRecognizer()
    {
        RecognizerInfo ri = GetKinectRecognizer();
        SpeechRecognitionEngine sre;
        sre = new SpeechRecognitionEngine(ri.Id);

        // Add the words that we want the game to recognise
        var grammar = new Choices();
        grammar.Add("EASY");
        grammar.Add("MEDIUM");
        grammar.Add("HARD");
        grammar.Add("TRAIN");
        grammar.Add("HOME");
        grammar.Add("BACK");
        grammar.Add("RESTART");
        grammar.Add("SIMON SAYS");

        //set culture - language, coutry/region
        var gb = new GrammarBuilder { Culture = ri.Culture };
        gb.Append(grammar);

        // set up the grammar builder

        var g = new Grammar(gb);
        sre.LoadGrammar(g);

        // set events for recogizing, hypothesising and rejecting speech
        sre.SpeechRecognized += SreSpeechRecognised;
        //sre.SpeechHypothesized += SreSpeechHypothesized;
        sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;
        return sre;

    }

    // if speech is rejected
    private void RejectSpeech(RecognitionResult result)
    {
        System.Diagnostics.Debug.WriteLine("Sorry Simon didn't quite understand that");
    }

    private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
    {
        RejectSpeech(e.Result);
    }

    //hypothesized result
    //private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
    //{
    //    System.Diagnostics.Debug.WriteLine("Hypothesized: " + e.Result.Text + " " + e.Result.Confidence);
    //}

    // Speech is recognised
    private void SreSpeechRecognised(object sender, SpeechRecognizedEventArgs e)
    {
        //Change this value to adjust the accuracy the higer the value the better the accuracy
        // the more accurate it will have to be, lower it if it is not recognizing you
        if (e.Result.Confidence < .7)
        {
            RejectSpeech(e.Result);
        }

        //set here what you want the game to do when it recognises speech
        switch (e.Result.Text)
        {
            case "EASY":
                System.Diagnostics.Debug.WriteLine("easy button pressed");
               
                break;
            case "MEDIUM":
                System.Diagnostics.Debug.WriteLine("medium button pressed");
                break;
            case "HARD":
                System.Diagnostics.Debug.WriteLine("hard button pressed");
                break;
            case "HOME":
                System.Diagnostics.Debug.WriteLine("home button pressed");
                break;
            case "TRAIN":
                System.Diagnostics.Debug.WriteLine("train button pressed");
                break;
            case "RESTART":
                System.Diagnostics.Debug.WriteLine("restart button pressed");
                break;
            case "SIMON SAYS":
                System.Diagnostics.Debug.WriteLine("Simon Says has been said");
                break;
        }

    }

    


}
