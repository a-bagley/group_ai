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

        private TextWriter textWriter;

        private CsvHelper.CsvWriter csvWriter;

        private String trainingDataPath = @"C:\simon_training_data\";

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
            loadTrainingDataFiles();
        }

        private void loadTrainingDataFiles()
        {
            String[] filePaths = Directory.GetFiles(trainingDataPath);
            foreach(String path in filePaths)
            {
                gestureNameList.Add(Path.GetFileName(path));
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
