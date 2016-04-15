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

        private Skeleton skeleton;

        private int numberOfGestures;

        private List<String> gestureNameList = new List<String>();

        private List<TrainingDataRow> trainingDataList = new List<TrainingDataRow>();

        private Dictionary<String, List<TrainingDataRow>> trainingDataDictionary;

        private TextWriter textWriter;

        private TextReader textReader;

        private CsvHelper.CsvWriter csvWriter;

        private CsvHelper.CsvReader csvReader;

        private String trainingDataPath = @"C:\simon_training_data\";

        private double[,] trainingData;

        public TrainingDataManager()
        {
            tdSaver = new TrainingDataSaver();
        }

        public void init()
        {
            if (!Directory.Exists(trainingDataPath))
            {
                System.IO.Directory.CreateDirectory(trainingDataPath);
            }
            loadTrainingDataInfo();
            // Do you want this here Sam?
            loadTrainingDataFiles();
        }

        private void loadTrainingDataFiles()
        {
            //loadTrainingDataInfo();
            String[] files = Directory.GetFiles(trainingDataPath);

            trainingDataDictionary = new Dictionary<String, List<TrainingDataRow>>();

            foreach (String file in files)
            {
                textReader = File.OpenText(file);
                csvReader = new CsvHelper.CsvReader(textReader);
                List<TrainingDataRow> trainingDataList = csvReader.GetRecords<TrainingDataRow>().ToList();
                trainingDataDictionary.Add(file, trainingDataList);
            }
        }


        //This is great Sam but we've skipped a step.
        //We need to load in the CSV files and calculate the key distances
        //then we can feed them into your logic for building the actual training
        //data for the nerual network

        // nFiles is the number of outputs since each file represents a gesture.
        // nTrainingRows is the total number of rows from each file
        private void loadTrainingData(int nTrainingRows, int nInputs)
        {
            // Create new 2D Array with the required sizes.
            trainingData = new double[nTrainingRows,nInputs+numberOfGestures];
            
            // Set all values as default to 0.1 (basically a cheat way to set the output values I can explain)
            for (int i = 0; i < trainingData.GetLength(0); i++)
            {
                for (int j = 0; j < trainingData.GetLength(1); j++)
                {
                    trainingData[i, j] = 0.1;
                }
            }

            // Index's to keep track of where we are in the trainng data based on the loop through the dictionary.
            int columnIndex = 0, rowIndex, fileIndex = 0;

            foreach (String key in trainingDataDictionary.Keys)
            {
                // Loop through eash row associated to the gesture.
                foreach (TrainingDataRow row in trainingDataDictionary[key])
                {
                    // Add method here to return double[] containg the distances we want from
                    // the training data row values.
                    double[] values = new double[12];

                    for (rowIndex = 0; rowIndex < values.Length; rowIndex++)
                    {
                        trainingData[columnIndex,rowIndex] = values[rowIndex];
                    }

                    // Here the output value will be set to 0.9 for all the training data rows it relates to.
                    trainingData[columnIndex, rowIndex + fileIndex] = 0.9;
                    columnIndex++;
                }
                // Next file/gesture is about to be looped through so we need to increase the index of the 0.9 value.
                fileIndex++;
            }
        }

        public double[,] getTrainingData()
        {
            return trainingData;
        }

        public void loadTrainingDataInfo()
        {
            String[] filePaths = Directory.GetFiles(trainingDataPath);
            foreach(String path in filePaths)
            {
                int pos = path.LastIndexOf("/") + 1;
                String name = path.Substring(pos, path.Length - pos);
                pos = name.LastIndexOf(".");
                if (pos > 0)
                    name = name.Substring(0, pos);
                gestureNameList.Add(Path.GetFileName(name));
            }
            numberOfGestures = gestureNameList.Count;
        }

        public void setSkeletalSource(Skeleton skel)
        {
            this.skeleton = skel;
        }

        public void startAddNewTrainingSet(String gestureName)
        {
            String gestureFileName = trainingDataPath + gestureName + ".csv";
            textWriter = File.CreateText(gestureFileName);
            csvWriter = new CsvHelper.CsvWriter(textWriter);
        }

        public void finishAddNewTrainingSet()
        {
            csvWriter.WriteRecords(trainingDataList);
            textWriter.Close();
        }

        public void saveTrainingSet()
        {
            TrainingDataRow row = saveSkeletalPoints();
            trainingDataList.Add(row);
        }

        public List<String> getGestureList()
        {
            return gestureNameList;
        }

        public void appendGestureNameToList(String gesture)
        {
            gestureNameList.Add(gesture);
        }

        private TrainingDataRow saveSkeletalPoints()
        {
            TrainingDataRow trainingDataRow = new TrainingDataRow();

            foreach (Joint joint in skeleton.Joints)
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

        public TrainingDataRow calculateDistanceBetweenJoints()
        {
            TrainingDataRow trainingDataRow = new TrainingDataRow();

            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    switch (joint.JointType) 
                    {
                        //Todo: use some of this code for calculating useful distances from the raw training data

                        /////// The following code can be salvaged for disatnce calculations once training data is loaded

                        //case JointType.Head:
                        //{
                        //    trainingDataRow._distanceBetweenHeadAndHandRight = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.HandRight));
                        //    trainingDataRow._distanceBetweenHeadAndHandLeft = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.HandLeft));
                            
                        //    break;
                        //}
                        
                        //case JointType.HipCenter:
                        //{
                        //    trainingDataRow._distanceBetweenHipCenterAndElbowRight = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.ElbowRight));
                        //    trainingDataRow._distanceBetweenHipCenterAndElbowLeft = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.ElbowLeft));

                        //    trainingDataRow._distanceBetweenHipCenterAndKneeRight = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.KneeRight));
                        //    trainingDataRow._distanceBetweenHipCenterAndKneeLeft = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.KneeLeft));

                        //    trainingDataRow._distanceBetweenHipCenterAndFootRight = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.FootRight));
                        //    trainingDataRow._distanceBetweenHipCenterAndFootLeft = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.FootLeft));
                            
                        //    break;
                        //}

                        //case JointType.ShoulderCenter:
                        //{
                        //    trainingDataRow._distanceBetweenShoulderCenterAndElbowRight = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.ElbowRight));
                        //    trainingDataRow._distanceBetweenShoulderCenterAndElbowLeft = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.ElbowLeft));

                        //    break;
                        //}
                    }
                }
            }

            return trainingDataRow;
        }

        private double getDistanceBetweenTwoJoints(Joint joint1, Joint joint2)
        {
            return Math.Sqrt(Math.Pow((joint1.Position.X - joint2.Position.X), 2) + Math.Pow((joint1.Position.Y - joint2.Position.Y), 2));
        }
    }
}
