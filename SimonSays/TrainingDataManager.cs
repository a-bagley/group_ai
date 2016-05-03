using Microsoft.Kinect;
using SimonSays.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace SimonSays
{
    class TrainingDataManager
    {
        /// <summary>
        /// Number of gestures in use
        /// </summary>
        private int mNumberOfGestures;

        /// <summary>
        /// LIst of gesture names
        /// </summary>
        private List<String> mGestureNameList = new List<String>();

        /// <summary>
        /// Current skeletal data list
        /// </summary>
        private List<SkeletonDataRow> mCurrentSkeletalDataList = new List<SkeletonDataRow>();

        /// <summary>
        /// Skeletal data dictionary
        /// </summary>
        private Dictionary<String, List<SkeletonDataRow>> mRawDataDictionary;

        /// <summary>
        /// Text Writer for writing to files
        /// </summary>
        private TextWriter mTextWriter;

        /// <summary>
        /// Text Reader for reading files
        /// </summary>
        private TextReader mTextReader;

        /// <summary>
        /// Used to write to CSV files
        /// </summary>
        private CsvHelper.CsvWriter mCsvWriter;

        /// <summary>
        /// Used to read CSV files
        /// </summary>
        private CsvHelper.CsvReader mCsvReader;

        /// <summary>
        /// Data save location
        /// </summary>
        private readonly String RAW_DATA_PATH = @"C:\simon_training_data\";

        /// <summary>
        /// Total number of training data rows
        /// </summary>
        private int mNumberOfDataRows = 0;

        public TrainingDataManager()
        {

        }

        /// <summary>
        /// Prepare for training
        /// </summary>
        public void initForTraining()
        {
            if (!Directory.Exists(RAW_DATA_PATH))
            {
                System.IO.Directory.CreateDirectory(RAW_DATA_PATH);
            }
            loadTrainingDataInfo();
        }

        /// <summary>
        /// Prepare for playing
        /// </summary>
        public void initForPlaying()
        {
            if (!Directory.Exists(RAW_DATA_PATH))
            {
                System.IO.Directory.CreateDirectory(RAW_DATA_PATH);
            }
            loadTrainingDataInfo();
            loadRawDataFiles();
        }

        /// <summary>
        /// Load existing gesture names from disk
        /// </summary>
        public void loadTrainingDataInfo()
        {
            mGestureNameList.Clear();
            String[] filePaths = Directory.GetFiles(RAW_DATA_PATH);
            foreach (String path in filePaths)
            {
                mGestureNameList.Add(getFileNameFromPath(path));
            }
            mNumberOfGestures = mGestureNameList.Count;
        }

        /// <summary>
        /// Load skeletal training data fiels from disk
        /// </summary>
        private void loadRawDataFiles()
        {
            String[] files = Directory.GetFiles(RAW_DATA_PATH);
            mNumberOfDataRows = 0;
            mRawDataDictionary = new Dictionary<String, List<SkeletonDataRow>>();

            foreach (String file in files)
            {
                mTextReader = File.OpenText(file);
                String name = getFileNameFromPath(file);
                mCsvReader = new CsvHelper.CsvReader(mTextReader);
                List<SkeletonDataRow> trainingDataList = mCsvReader.GetRecords<SkeletonDataRow>().ToList();
                mNumberOfDataRows += trainingDataList.Count;
                mRawDataDictionary.Add(name, trainingDataList);
            }
            if (mTextReader != null)
            {
                mTextReader.Close();
            }
            else
            {
                // Show warning
                MessageBox.Show("No trainig data detected, please use the training window to create some.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Extract name from file name
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private String getFileNameFromPath(String filePath)
        {
            String name = Path.GetFileName(filePath);
            int pos = name.LastIndexOf(".");
            if (pos > 0)
                name = name.Substring(0, pos);
            return name;
        }

        /// <summary>
        /// Setup CSV writing for new skeletal training data
        /// </summary>
        /// <param name="gestureName"></param>
        public void startAddNewTrainingSet(String gestureName)
        {
            String gestureFileName = RAW_DATA_PATH + gestureName + ".csv";
            mTextWriter = File.CreateText(gestureFileName);
            mCsvWriter = new CsvHelper.CsvWriter(mTextWriter);
            mCurrentSkeletalDataList = new List<SkeletonDataRow>();
        }

        /// <summary>
        /// Finish saving new skeletal training data
        /// </summary>
        public void finishAddNewTrainingSet()
        {
            mCsvWriter.WriteRecords(mCurrentSkeletalDataList);
            mTextWriter.Close();
        }

        /// <summary>
        /// Add skeletal training data row to current CSV file
        /// </summary>
        /// <param name="skel"></param>
        public void saveRawSkeletalSet(Skeleton skel)
        {
            SkeletonDataRow row = createSkeletalDataRow(skel);
            mCurrentSkeletalDataList.Add(row);
        }

        /// <summary>
        /// Get the list of gestures
        /// </summary>
        /// <returns></returns>
        public List<String> getGestureList()
        {
            return mGestureNameList;
        }

        public String getGestureName(int gestureNumber)
        {
            return mGestureNameList.ElementAt(gestureNumber);
        }

        public Dictionary<String, List<SkeletonDataRow>> getRawDataDictionary()
        {
            return mRawDataDictionary;
        }

        public int getNumberOfDataRows()
        {
            return mNumberOfDataRows;
        }

        public void appendGestureNameToList(String gesture)
        {
            mGestureNameList.Add(gesture);
        }

        /// <summary>
        /// Get a random gesture.
        /// Fake random to ensure the same gesture does
        /// not appear back-to-back
        /// </summary>
        /// <param name="lastGesture"></param>
        /// <returns></returns>
        public String getRandomGesture(String lastGesture)
        {
            Random rand = new Random();
            String nextGesture = mGestureNameList.ElementAt(rand.Next(mGestureNameList.Count - 1));
            while (lastGesture.Equals(nextGesture))
            {
                nextGesture = mGestureNameList.ElementAt(rand.Next(mGestureNameList.Count - 1)); // Exclude standing
            }
            return nextGesture;
        }

        /// <summary>
        /// Convert Kinect skeleton object to custom
        /// SkeletonDataRown object
        /// </summary>
        /// <param name="skel"></param>
        /// <returns>Current player position in X and Y coordinates</returns>
        public SkeletonDataRow createSkeletalDataRow(Skeleton skel)
        {
            SkeletonDataRow trainingDataRow = new SkeletonDataRow();

            foreach (Joint joint in skel.Joints)
            {
                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    switch (joint.JointType)
                    {
                        case JointType.Head:
                            {
                                trainingDataRow.headX = joint.Position.X;
                                trainingDataRow.headY = joint.Position.Y;
                                break;
                            }

                        case JointType.ShoulderCenter:
                            {
                                trainingDataRow.shoulderCenterX = joint.Position.X;
                                trainingDataRow.shoulderCenterY = joint.Position.Y;
                                break;
                            }

                        case JointType.ShoulderRight:
                            {
                                trainingDataRow.shoulderRightX = joint.Position.X;
                                trainingDataRow.shoulderRightY = joint.Position.Y;
                                break;
                            }

                        case JointType.ShoulderLeft:
                            {
                                trainingDataRow.shoulderLeftX = joint.Position.X;
                                trainingDataRow.shoulderLeftY = joint.Position.Y;
                                break;
                            }

                        case JointType.ElbowRight:
                            {
                                trainingDataRow.elbowRightX = joint.Position.X;
                                trainingDataRow.elbowRightY = joint.Position.Y;
                                break;
                            }

                        case JointType.ElbowLeft:
                            {
                                trainingDataRow.elbowLeftX = joint.Position.X;
                                trainingDataRow.elbowLeftY = joint.Position.Y;
                                break;
                            }

                        case JointType.WristRight:
                            {
                                trainingDataRow.wristRightX = joint.Position.X;
                                trainingDataRow.wristRightY = joint.Position.Y;
                                break;
                            }

                        case JointType.WristLeft:
                            {
                                trainingDataRow.wristLeftX = joint.Position.X;
                                trainingDataRow.wristLeftY = joint.Position.Y;
                                break;
                            }

                        case JointType.HandRight:
                            {
                                trainingDataRow.handRightX = joint.Position.X;
                                trainingDataRow.handRightY = joint.Position.Y;
                                break;
                            }

                        case JointType.HandLeft:
                            {
                                trainingDataRow.handLeftX = joint.Position.X;
                                trainingDataRow.handLeftY = joint.Position.Y;
                                break;
                            }

                        case JointType.HipCenter:
                            {
                                trainingDataRow.hipCenterX = joint.Position.X;
                                trainingDataRow.hipCenterY = joint.Position.Y;
                                break;
                            }

                        case JointType.HipRight:
                            {
                                trainingDataRow.hipRightX = joint.Position.X;
                                trainingDataRow.hipRightY = joint.Position.Y;
                                break;
                            }

                        case JointType.HipLeft:
                            {
                                trainingDataRow.hipLeftX = joint.Position.X;
                                trainingDataRow.hipLeftY = joint.Position.Y;
                                break;
                            }

                        case JointType.KneeRight:
                            {
                                trainingDataRow.kneeRightX = joint.Position.X;
                                trainingDataRow.kneeRightY = joint.Position.Y;
                                break;
                            }

                        case JointType.KneeLeft:
                            {
                                trainingDataRow.kneeLeftX = joint.Position.X;
                                trainingDataRow.kneeLeftY = joint.Position.Y;
                                break;
                            }

                        case JointType.AnkleRight:
                            {
                                trainingDataRow.ankleRightX = joint.Position.X;
                                trainingDataRow.ankleRightY = joint.Position.Y;
                                break;
                            }

                        case JointType.AnkleLeft:
                            {
                                trainingDataRow.ankleLeftX = joint.Position.X;
                                trainingDataRow.ankleLeftY = joint.Position.Y;
                                break;
                            }

                        case JointType.FootRight:
                            {
                                trainingDataRow.footRightX = joint.Position.X;
                                trainingDataRow.footRightY = joint.Position.Y;
                                break;
                            }

                        case JointType.FootLeft:
                            {
                                trainingDataRow.footLeftX = joint.Position.X;
                                trainingDataRow.footLeftY = joint.Position.Y;
                                break;
                            }
                    }
                }
            }
            return trainingDataRow;
        }
    }
}
