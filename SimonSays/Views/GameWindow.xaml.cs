﻿using Microsoft.Kinect;
using SimonSays.NaiveBayes;
using SimonSays.NeuralNetwork;
using SimonSays.Utils;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimonSays.Views
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        #region Skeleton Properties
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;
        #endregion

        /// <summary>
        /// Round countdown timer
        /// </summary>
        System.Windows.Threading.DispatcherTimer mRoundTimer = new System.Windows.Threading.DispatcherTimer();
        TimeSpan mRoundTimeSpan;

        /// <summary>
        // Max round countdown time
        /// </summary>
        int mCountdownSeconds = 0;

        /// <summary>
        /// Number of lives for difficulty
        /// </summary>
        int mLivesReset;

        /// <summary>
        /// Current number of lives left
        /// </summary>
        int mLives;

        /// <summary>
        /// Current score total
        /// </summary>
        int mScoreTotal = 0;

        /// <summary>
        /// Gesture the player must do
        /// </summary>
        private String mTargetGesture = "unassigned";

        /// <summary>
        /// Traing data manager
        /// </summary>
        private TrainingDataManager mTDManager;

        /// <summary>
        /// AI system in use
        /// </summary>
        private AISystemEnum mAIType = AISystemEnum.NN;

        /// <summary>
        /// MLP (ANN) Classifier
        /// </summary>
        private MLPClassifier mBrain;

        /// <summary>
        /// Naive Bayes Classifier
        /// </summary>
        private NaiveBayesClassifier mNaiveBayes;

        /// <summary>
        /// Track if AI is ready for use
        /// </summary>
        private Boolean mAIReady = false;

        /// <summary>
        /// Track if AI is free to start a classification
        /// </summary>
        private Boolean mAIBusy = false;

        /// <summary>
        /// Track Kinect FPS
        /// </summary>
        private int mTickCounter = 0;

        /// <summary>
        /// Random number source
        /// </summary>
        private Random rand = new Random();

        /// <summary>
        /// Is the current gesture being asked by Simon or not
        /// </summary>
        private Boolean mSimonAsked = false;

        /// <summary>
        /// Timer for non-Siomn asked gestures
        /// </summary>
        private System.Timers.Timer mNoneSimonTimer;

        /// <summary>
        /// Period to wait before clearing a non-Simon gesture command
        /// </summary>
        private int mNoneSimonWaitPeriod;

        /// <summary>
        /// Constructs game window for playing the main game
        /// </summary>
        /// <param name="ai"></param>
        /// <param name="difficulty"></param>
        public GameWindow(AISystemEnum ai, DifficultyEnum difficulty)
        {
            // Init Kinect
            InitializeComponent();
            // Disable UI whilst loading AI
            blockUI(true);
            mAIType = ai;

            SetDifficulty(difficulty);
            
            updateLivesUI();
            updateScoreUI();
            lblSeconds.Content = mCountdownSeconds;
            mNoneSimonWaitPeriod = 4500;

            mTDManager = new TrainingDataManager();
            // Load skeletal training data
            mTDManager.initForPlaying();
            // Setup ANN
            if (mAIType == AISystemEnum.NN)
            {
                mBrain = new MLPClassifier(0.1, 0.9); //learning rate 0.1, and momentum 0.9
                // Train in seperate thread
                Thread aiTrainingThread = new System.Threading.Thread(delegate()
                {
                    if (mTDManager.getNumberOfDataRows() > 0)
                    {
                        mBrain.trainAI(new RawSkeletalDataPackage(mTDManager.getRawDataDictionary(), mTDManager.getGestureList(), mTDManager.getNumberOfDataRows()));
                        System.Diagnostics.Debug.WriteLine("MLP trained and ready!");
                        mAIReady = true;
                        restartGame();
                        blockUI(false);
                        StartCountdown();
                        // game starts
                    }
                    else
                    {
                        // Show error on screen
                        System.Diagnostics.Debug.WriteLine("\n**Warning! No training data found, you need training data before you can play\n");
                    }
                });
                aiTrainingThread.IsBackground = true;
                aiTrainingThread.Start();
            }
            // Setup Naive Bayes
            else if (mAIType == AISystemEnum.NaiveBayes)
            {
                if (mTDManager.getNumberOfDataRows() > 0)
                {
                    mNaiveBayes = new NaiveBayesClassifier();
                    mNaiveBayes.trainAI(new RawSkeletalDataPackage(mTDManager.getRawDataDictionary(), mTDManager.getGestureList(), mTDManager.getNumberOfDataRows()));
                    System.Diagnostics.Debug.WriteLine("MLP trained and ready!");
                    mAIReady = true;
                    restartGame();
                    blockUI(false);
                    StartCountdown();
                    // Game starts
                }
                else
                {
                    // Show error on screen
                    System.Diagnostics.Debug.WriteLine("\n**Warning! No training data found, you need training data before you can play\n");
                }
            }
        }

        /// <summary>
        /// Set the difficulty of the game
        /// </summary>
        /// <param name="difficulty">The difficulty level</param>
        private void SetDifficulty(DifficultyEnum difficulty)
        {
            switch (difficulty)
            {
                case DifficultyEnum.Easy:
                    mLivesReset = 4;
                    mLives = 4;
                    mCountdownSeconds = 15;
                    break;
                case DifficultyEnum.Medium:
                    mLivesReset = 3;
                    mLives = 3;
                    mCountdownSeconds = 10;
                    break;
                case DifficultyEnum.Hard:
                    mLivesReset = 2;
                    mLives = 2;
                    mCountdownSeconds = 5;
                    break;
                default:
                    mLivesReset = 3;
                    mLives = 3;
                    mCountdownSeconds = 10;
                    break;
            }
        }

        /// <summary>
        /// Updates the UI relating to the score and lives
        /// </summary>
        /// <param name="score">The players score</param>
        private void updateScoreUI(double score)
        {
            setStarRating(score);
            lblLives.Content = "Lives: " + mLives;
            lblScore.Content = "Score: " + mScoreTotal;
        }

        /// <summary>
        /// Set the amount of stars the player has achieved
        /// </summary>
        /// <param name="score">The players score</param>
        private void setStarRating(double score)
        {
            String imageName = "";
            if (mAIType == AISystemEnum.NN)
            {
                if (score != -1)
                {
                    score = score * 10;
                    if (score > 8.5)
                    {
                        imageName = "fivestars.png";
                        score = 5;
                    }
                    else if (score > 8)
                    {
                        imageName = "fourstars.png";
                        score = 4;
                    }
                    else if (score > 7.5)
                    {
                        imageName = "threestars.png";
                        score = 3;
                    }
                    else if (score > 7)
                    {
                        imageName = "twostars.png";
                        score = 2;
                    }
                    else
                    {
                        imageName = "onestars.png";
                        score = 1;
                    }
                    mScoreTotal += (int)score;
                }
                else
                {
                    imageName = "fail.png";
                    subtractLife();
                }
            }
            // Tick or Cross for NB
            else if (mAIType == AISystemEnum.NaiveBayes)
            {
                if (score != -1)
                {
                    imageName = "correct.png";
                    mScoreTotal += 5;
                }
                else
                {
                    imageName = "fail.png";
                    subtractLife();
                }
            }
            imgScore.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageName));
        }

        /// <summary>
        /// Updates the score on the UI
        /// </summary>
        private void updateScoreUI()
        {
             Dispatcher.BeginInvoke(new Action(() =>lblScore.Content = "Score: " + mScoreTotal));
        }
        
        /// <summary>
        /// Updates the amount of lives on the UI
        /// </summary>
        private void updateLivesUI()
        {
             Dispatcher.BeginInvoke(new Action(() => lblLives.Content = "Lives: " + mLives));
        }

        /// <summary>
        /// Removes a life from the player
        /// </summary>
        private void subtractLife()
        {
            mLives--;
            if (mNoneSimonTimer != null)
            {
                mNoneSimonTimer.Stop();
                mNoneSimonTimer.Dispose();
            }
            if (mLives == 0)
            {
                var window = new GameOver(mScoreTotal);
                mRoundTimer.Stop();
                window.Show();
                this.Close();
            }
        }

        /// <summary>
        /// Responsible for setting the command in the game
        /// </summary>
        /// <param name="command">The gesture</param>
        private void setSimonSaysCommand(String command)
        {
            int isSimon = rand.Next(2);
            if (isSimon == 1)
            {
            lblSimonSaysCommand.Content = "Simon says " + command;
                mSimonAsked = true;
            }
            else if (isSimon == 0)
            {
                lblSimonSaysCommand.Content = command;
                mSimonAsked = false;
                startNoneSimonTimer();
            }
        }

        /// <summary>
        /// Disable UI elements
        /// </summary>
        /// <param name="block">Decides if the UI should be blocked</param>
        private void blockUI(bool block)
        {
            if (block)
            {
                 Dispatcher.BeginInvoke(new Action(() => btnHome.IsEnabled = false));
                 Dispatcher.BeginInvoke(new Action(() => btnRestartGame.IsEnabled = false));
            }
            else
            {
                 Dispatcher.BeginInvoke(new Action(() => btnHome.IsEnabled = true));
                 Dispatcher.BeginInvoke(new Action(() => btnRestartGame.IsEnabled = true));
            }
        }

        /// <summary>
        /// Starts the count down
        /// </summary>
        private void StartCountdown()
        {
            mRoundTimeSpan = TimeSpan.FromSeconds(mCountdownSeconds);
            mRoundTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            mRoundTimer.Interval = new TimeSpan(0, 0, 1);
            mRoundTimer.Start();
        }

        /// <summary>
        /// Restarts the count down
        /// </summary>
        private void restartCountDown()
        {
            mRoundTimeSpan = TimeSpan.FromSeconds(mCountdownSeconds);
            mRoundTimer.Start();
        }

        /// <summary>
        /// Called on each timer tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            lblSeconds.Content = mRoundTimeSpan.ToString("ss");

            if (mRoundTimeSpan == TimeSpan.Zero)
            {
                mRoundTimer.Stop();
                subtractLife();
                updateLivesUI();
                restartCountDown();
                mTargetGesture = mTDManager.getRandomGesture(mTargetGesture);
                Dispatcher.BeginInvoke(new Action(() => setSimonSaysCommand(mTargetGesture)));
            }
            mRoundTimeSpan = mRoundTimeSpan.Add(TimeSpan.FromSeconds(-1));
        }

        /// <summary>
        /// Start the timer for an none simon gesture
        /// </summary>
        private void startNoneSimonTimer()
        {
            System.Threading.Thread.Sleep(1000);
            mNoneSimonTimer = new System.Timers.Timer();
            mNoneSimonTimer.AutoReset = false;
            mNoneSimonTimer.Interval = mNoneSimonWaitPeriod;
            mNoneSimonTimer.Elapsed += onNonSimonTimerExpired;
            mNoneSimonTimer.Start();
        }

        /// <summary>
        /// Clear none simon gesture
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void onNonSimonTimerExpired(Object source, System.Timers.ElapsedEventArgs e)
        {
            mTargetGesture = mTDManager.getRandomGesture(mTargetGesture);
            Dispatcher.BeginInvoke(new Action(() => setSimonSaysCommand(mTargetGesture)));
            Dispatcher.BeginInvoke(new Action(() => restartCountDown()));
            System.Diagnostics.Debug.WriteLine("\n---Player passed Non-Simon Says round\n");
        }

        /// <summary>
        /// Restart the game button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRestartGame_Click(object sender, RoutedEventArgs e)
        {
            restartGame();
        }

        /// <summary>
        /// Restarts the game
        /// </summary>
        private void restartGame()
        {
            mScoreTotal = 0;
            mLives = mLivesReset;
            updateLivesUI();
            updateScoreUI();
            mTargetGesture = mTDManager.getRandomGesture(mTargetGesture);
            Dispatcher.BeginInvoke(new Action(() => setSimonSaysCommand(mTargetGesture)));
            restartCountDown();
        }

        /// <summary>
        /// Tests if the player's current gesture in a background thread
        /// </summary>
        /// <param name="currentPlayerSkel"></param>
        private void testPlayerGesture(Skeleton currentPlayerSkel)
        {
            if (mAIReady)
            {
                if (mAIType == AISystemEnum.NN)
                {
                    Thread aiClassifyThread = new System.Threading.Thread(delegate()
                    {
                        Guess aiGuess = mBrain.makeGuess(mTDManager.createSkeletalDataRow(currentPlayerSkel));
                        processGuess(aiGuess);
                    });
                    aiClassifyThread.IsBackground = true;
                    aiClassifyThread.Start();
                }
                else if (mAIType == AISystemEnum.NaiveBayes)
                {
                    Thread aiClassifyThread = new System.Threading.Thread(delegate()
                    {
                        Guess aiGuess = mNaiveBayes.makeGuess(mTDManager.createSkeletalDataRow(currentPlayerSkel));
                        processGuessNB(aiGuess);
                    });
                    aiClassifyThread.IsBackground = true;
                    aiClassifyThread.Start();
                }
            }
        }

        /// <summary>
        /// Decides if a guess is correct using the NN
        /// </summary>
        /// <param name="aiGuess"></param>
        private void processGuess(Guess aiGuess)
        {
            // Update UI here
            String gestureMatched = mTDManager.getGestureName(aiGuess.getGuessId());
            if (gestureMatched.Equals(mTargetGesture) && aiGuess.getGuessValue() > 0.7)
            {
                // Thread safe UI calls
                Dispatcher.BeginInvoke(new Action(() => updateScoreUI((mSimonAsked) ? aiGuess.getGuessValue() : -1)));
                mTargetGesture = mTDManager.getRandomGesture(mTargetGesture);
                Dispatcher.BeginInvoke(new Action(() => setSimonSaysCommand(mTargetGesture)));
                Dispatcher.BeginInvoke(new Action(() => restartCountDown()));
                System.Diagnostics.Debug.WriteLine("\n*** Correct gesture\n");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("\n%%% Wrong gesture\n");
            }
            mAIBusy = false;
        }

        /// <summary>
        /// Decides if a guess is correct using Naive Bayes
        /// </summary>
        /// <param name="aiGuess"></param>
        private void processGuessNB(Guess aiGuess)
        {
            // Update UI here
            String gestureMatched = mTDManager.getGestureName(aiGuess.getGuessId());
            if (gestureMatched.Equals(mTargetGesture))
            {
                // Thread safe UI calls
                if (mNoneSimonTimer != null)
                    mNoneSimonTimer.Dispose();
                Dispatcher.BeginInvoke(new Action(() => updateScoreUI((mSimonAsked) ? aiGuess.getGuessValue() : -1)));
                mTargetGesture = mTDManager.getRandomGesture(mTargetGesture);
                Dispatcher.BeginInvoke(new Action(() => setSimonSaysCommand(mTargetGesture)));
                Dispatcher.BeginInvoke(new Action(() => restartCountDown()));
                System.Diagnostics.Debug.WriteLine("\n*** Correct gesture\n");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("\n%%% Wrong gesture\n");
            }
            mAIBusy = false;
        }        

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            imgSkeleton.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBar.Visibility = Visibility.Visible;
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
                this.sensor.Dispose();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event.
        /// Calls AI system, sending the player's current skeletal data at set intervals.
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                            // Should run roughly every 1.5s
                            if (mTickCounter % 45 == 0)
                            {
                                // If AI setup and not busy, attempt to classify the player's current gesture
                                if (mAIReady && !mAIBusy)
                                {
                                    mAIBusy = true;
                                    testPlayerGesture(skel);
                                }
                            }
                            mTickCounter++;
                            if (mTickCounter == Int32.MaxValue)
                                mTickCounter = 0;
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }

        //Handles the home button so that we can go back to the start window
        //Also stopping the timer when the button is pressed
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            mRoundTimer.Stop();
            if (mNoneSimonTimer != null)
            {
                mNoneSimonTimer.Stop();
                mNoneSimonTimer.Dispose();
            }
            window.Show();
            this.Close();
        }

    }
}
