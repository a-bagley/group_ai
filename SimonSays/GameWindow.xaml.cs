﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.IO;
using System.Threading;
using SimonSays.Utils;
using SimonSays.NeuralNetwork;

namespace SimonSays
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

        System.Windows.Threading.DispatcherTimer _timer = new System.Windows.Threading.DispatcherTimer();
        TimeSpan _time;
        private bool restartTimer;
        int mCountdownSeconds = 0;
        int mLives;
        int mScoreTotal = 0;
        int score = 0;

        private String mTargetGesture = "unassigned";

        private TrainingDataManager mTDManager;
        private MLPClassifier mBrain;
        private Boolean mAIReady = false;
        private Boolean mAIBusy = false;
        private int mTickCounter = 0;

        public GameWindow(Difficulty difficulty)
        {
            InitializeComponent();

            switch (difficulty)
            {
                case Difficulty.Easy:
                    mLives = 4;
                    mCountdownSeconds = 15;
                    break;
                case Difficulty.Medium:
                    mLives = 3;
                    mCountdownSeconds = 10;
                    break;
                case Difficulty.Hard:
                    mLives = 2;
                    mCountdownSeconds = 5;
                    break;
                default:
                    mLives = 3;
                    mCountdownSeconds = 10;
                    break;
            }
            updateLivesUI();
            updateScoreUI();
            lblSeconds.Content = mCountdownSeconds;

            // NN stuff
            mTDManager = new TrainingDataManager();
            mTDManager.initForPlaying();
            mBrain = new MLPClassifier(0.1, 0.9); //learning rate 0.2, and momentum 0.9
            Thread aiTrainingThread = new System.Threading.Thread(delegate()
            {
                if (mTDManager.getNumberOfDataRows() > 0)
                {
                    mBrain.trainAI(new RawSkeletalDataPackage(mTDManager.getRawDataDictionary(), mTDManager.getGestureList(), mTDManager.getNumberOfDataRows()));
                    System.Diagnostics.Debug.WriteLine("MLP trained and ready!");
                    mAIReady = true;
                    restartGame();
                    StartCountdown();
                }
                else
                {
                    // Show error on screen
                    System.Diagnostics.Debug.WriteLine("\n**Warning! No training data found, you need training data before you can play\n");
                }
            });
            aiTrainingThread.Start();         
        }

        private int NeuralNetworkCalculation()
        {
            return 0;
        }

        private void updateScoreUI(double score)
        {
            var imageName = "";
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
            //switch (score)
            //{
            //    case 0:
            //        imageName = "fail.png";
            //        lives -= 1;                    
            //        break;
            //    case 1:
            //        imageName = "onestars.png";
            //        break;
            //    case 2:
            //        imageName = "twostars.png";
            //        break;
            //    case 3:
            //        imageName = "threestars.png";
            //        break;
            //    case 4:
            //        imageName = "fourstars.png";
            //        break;
            //    case 5:
            //        imageName = "fivestars.png";
            //        break;
            //    default:
            //        break;
            //}
            mScoreTotal += (int)score;
            imgScore.Source = new BitmapImage(new Uri("pack://application:,,,/Images/" + imageName));
            lblLives.Content = "Lives: " + mLives;
            lblScore.Content = "Score: " + mScoreTotal;
            //if (mLives == 0)
            //{
            //    var window = new GameOver();
            //    window.Show();
            //    this.Close();
            //}
        }

        private void updateScoreUI()
        {
            lblScore.Content = "Score: " + mScoreTotal;
        }

        private void updateLivesUI()
        {
            lblLives.Content = "Lives: " + mLives;
        }

        private void subtractLife()
        {
            mLives--;
            if (mLives == 0)
            {
                var window = new GameOver(score);
                window.Show();
                this.Close();
            }
        }

        private void setSimonSaysCommand(String command)
        {
            lblSimonSaysCommand.Content = "Simon says " + command;
        }

        private void StartCountdown()
        {
            _time = TimeSpan.FromSeconds(mCountdownSeconds);
            _timer.Tick += new EventHandler(dispatcherTimer_Tick);
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
        }

        private void restartCountDown()
        {
            _time = TimeSpan.FromSeconds(mCountdownSeconds);
            _timer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            lblSeconds.Content = _time.ToString("ss");

            if (_time == TimeSpan.Zero)
            {
                _timer.Stop();
                subtractLife();
                updateLivesUI();
                restartCountDown();
            }
            _time = _time.Add(TimeSpan.FromSeconds(-1));
        }

        private void btnRestartGame_Click(object sender, RoutedEventArgs e)
        {
            restartGame();
        }

        private void restartGame()
        {
            mScoreTotal = 0;
            mTargetGesture = mTDManager.getRandomGesture(mTargetGesture);
            Dispatcher.Invoke(new Action(() => setSimonSaysCommand(mTargetGesture)));
            restartCountDown();
        }

        private void testPlayerGesture(Skeleton currentPlayerSkel)
        {
            if (mAIReady)
            {
                Thread aiClassifyThread = new System.Threading.Thread(delegate()
                {
                    Guess aiGuess = mBrain.makeGuess(mTDManager.createSkeletalDataRow(currentPlayerSkel));
                    processGuess(aiGuess);
                });
                aiClassifyThread.Start();
            }
        }

        private void processGuess(Guess aiGuess)
        {
            // Update UI here
            String gestureMatched = mTDManager.getGestureName(aiGuess.getGuessId());
            if (gestureMatched.Equals(mTargetGesture) && aiGuess.getGuessValue() > 0.7)
            {
                //get next target gesture
                // display new target gesture
                mTargetGesture = mTDManager.getRandomGesture(mTargetGesture);
                Dispatcher.Invoke(new Action(() => setSimonSaysCommand(mTargetGesture)));
                Dispatcher.Invoke(new Action(() => updateScoreUI(aiGuess.getGuessValue())));
                Dispatcher.Invoke(new Action(() => restartCountDown()));
                System.Diagnostics.Debug.WriteLine("\n*** Correct gesture\n");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("\n%%% Wrong gesture\n");
                //Dispatcher.Invoke(new Action(() => updateScoreUI(aiGuess.getGuessValue())));
            }
            mAIBusy = false;
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
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
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
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
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
                            if (mTickCounter % 60 == 0)
                            {
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
            _timer.Stop();
            window.Show();
            this.Close();
        }

    }
}
