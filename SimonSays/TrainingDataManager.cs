using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SimonSays
{
    class TrainingDataManager
    {
        private TrainingDataSaver tdSaver;

        private Skeleton mSkeleton;

        private int mNumberOfGestures;

        private List<String> mGestureNameList = new List<String>();

        private List<SkeletonDataRow> mCurrentTrainingDataList = new List<SkeletonDataRow>();

        private Dictionary<String, List<SkeletonDataRow>> mRawDataDictionary;

        private TextWriter mTextWriter;

        private TextReader mTextReader;

        private CsvHelper.CsvWriter mCsvWriter;

        private CsvHelper.CsvReader mCsvReader;

        private readonly String RAW_DATA_PATH = @"C:\simon_training_data\";

        private double[,] mTrainingData;

        private int mNumberOfDataRows = 0;

        public TrainingDataManager()
        {
            tdSaver = new TrainingDataSaver();
        }

        public void initForTraining()
        {
            if (!Directory.Exists(RAW_DATA_PATH))
            {
                System.IO.Directory.CreateDirectory(RAW_DATA_PATH);
            }
            loadTrainingDataInfo();
            // Do you want this here Sam?
            //loadRawDataFiles();
        }

        public void initForPlaying()
        {
            if (!Directory.Exists(RAW_DATA_PATH))
            {
                System.IO.Directory.CreateDirectory(RAW_DATA_PATH);
            }
            loadTrainingDataInfo();
            // Do you want this here Sam?
            loadRawDataFiles();
        }

        public void loadTrainingDataInfo()
        {
            mGestureNameList.Clear();
            String[] filePaths = Directory.GetFiles(RAW_DATA_PATH);
            foreach (String path in filePaths)
            {
                int pos = path.LastIndexOf("/") + 1;
                String name = path.Substring(pos, path.Length - pos);
                pos = name.LastIndexOf(".");
                if (pos > 0)
                    name = name.Substring(0, pos);
                mGestureNameList.Add(Path.GetFileName(name));
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
                mCsvReader = new CsvHelper.CsvReader(mTextReader);
                List<SkeletonDataRow> trainingDataList = mCsvReader.GetRecords<SkeletonDataRow>().ToList();
                mNumberOfDataRows += trainingDataList.Count;
                mRawDataDictionary.Add(file, trainingDataList);
            }
        }


        // nFiles is the number of outputs since each file represents a gesture.
        // nTrainingRows is the total number of rows from each file
        //private void loadTrainingData(int nTrainingRows, int nInputs)
        //{
        //    // Create new 2D Array with the required sizes.
        //    mTrainingData = new double[nTrainingRows,nInputs+mNumberOfGestures];
            
        //    // Set all values as default to 0.1 (basically a cheat way to set the output values I can explain)
        //    for (int i = 0; i < mTrainingData.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < mTrainingData.GetLength(1); j++)
        //        {
        //            mTrainingData[i, j] = 0.1;
        //        }
        //    }

        //    // Index's to keep track of where we are in the trainng data based on the loop through the dictionary.
        //    int columnIndex = 0, rowIndex, fileIndex = 0;

        //    foreach (String key in mRawDataDictionary.Keys)
        //    {
        //        // Loop through eash row associated to the gesture.
        //        foreach (SkeletonDataRow row in mRawDataDictionary[key])
        //        {
        //            // Add method here to return double[] containg the distances we want from
        //            // the training data row values.
        //            double[] values = new double[12];

        //            for (rowIndex = 0; rowIndex < values.Length; rowIndex++)
        //            {
        //                mTrainingData[columnIndex,rowIndex] = values[rowIndex];
        //            }

        //            // Here the output value will be set to 0.9 for all the training data rows it relates to.
        //            mTrainingData[columnIndex, rowIndex + fileIndex] = 0.9;
        //            columnIndex++;
        //        }
        //        // Next file/gesture is about to be looped through so we need to increase the index of the 0.9 value.
        //        fileIndex++;
        //    }
        //}

        public double[,] getTrainingData()
        {
            return mTrainingData;
        }

        public void setSkeletalSource(Skeleton skel)
        {
            this.mSkeleton = skel;
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

        public String getRandomGesture()
        {
            Random rand = new Random();
            return mGestureNameList.ElementAt(rand.Next(mGestureNameList.Count - 1));
        }

        public SkeletonDataRow createSkeletalDataRow(Skeleton skel)
        {
            SkeletonDataRow trainingDataRow = new SkeletonDataRow();

            foreach (Joint joint in skel.Joints)
            {
                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    switch(joint.JointType)
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
