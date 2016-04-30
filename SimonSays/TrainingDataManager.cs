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
        private int mNumberOfGestures;

        private List<String> mGestureNameList = new List<String>();

        private List<SkeletonDataRow> mCurrentTrainingDataList = new List<SkeletonDataRow>();

        private Dictionary<String, List<SkeletonDataRow>> mRawDataDictionary;

        private TextWriter mTextWriter;

        private TextReader mTextReader;

        private CsvHelper.CsvWriter mCsvWriter;

        private CsvHelper.CsvReader mCsvReader;

        private readonly String RAW_DATA_PATH = @"C:\simon_training_data\";

        private int mNumberOfDataRows = 0;

        public TrainingDataManager()
        {

        }

        public void initForTraining()
        {
            if (!Directory.Exists(RAW_DATA_PATH))
            {
                System.IO.Directory.CreateDirectory(RAW_DATA_PATH);
            }
            loadTrainingDataInfo();
        }

        public void initForPlaying()
        {
            if (!Directory.Exists(RAW_DATA_PATH))
            {
                System.IO.Directory.CreateDirectory(RAW_DATA_PATH);
            }
            loadTrainingDataInfo();
            loadRawDataFiles();
        }

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
                MessageBox.Show("No CSV Files detected");
            }
        }

        private String getFileNameFromPath(String filePath)
        {
            String name = Path.GetFileName(filePath);
            int pos = name.LastIndexOf(".");
            if (pos > 0)
                name = name.Substring(0, pos);
            return name;
        }

        public void startAddNewTrainingSet(String gestureName)
        {
            String gestureFileName = RAW_DATA_PATH + gestureName + ".csv";
            mTextWriter = File.CreateText(gestureFileName);
            mCsvWriter = new CsvHelper.CsvWriter(mTextWriter);
            mCurrentTrainingDataList = new List<SkeletonDataRow>();
        }

        public void finishAddNewTrainingSet()
        {
            mCsvWriter.WriteRecords(mCurrentTrainingDataList);
            mTextWriter.Close();
            //mCurrentTrainingDataList.Clear();
        }

        public void saveRawSkeletalSet(Skeleton skel)
        {
            SkeletonDataRow row = createSkeletalDataRow(skel);
            mCurrentTrainingDataList.Add(row);
        }

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
