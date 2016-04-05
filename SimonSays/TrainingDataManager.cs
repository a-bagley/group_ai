using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace SimonSays
{
    class TrainingDataManager
    {
        private Skeleton _skeleton;

        public TrainingDataManager(Skeleton skeleton)
        {
            this._skeleton = skeleton;
        }

        private double getDistanceBetweenTwoJoints(Joint joint1, Joint joint2)
        {
            return Math.Sqrt(Math.Pow((joint1.Position.X - joint2.Position.X), 2) + Math.Pow((joint1.Position.Y - joint2.Position.Y), 2));
        }

        public TrainingDataRow saveSkeletalPoints()
        {
            TrainingDataRow trainingDataRow = new TrainingDataRow();

            foreach (Joint joint in _skeleton.Joints)
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

                        //Todo Add the rest to match the TrainingDataRow class
                    }
                }
            }
            return trainingDataRow;
        }

        public TrainingDataRow calculateDistanceBetweenJoints()
        {
            TrainingDataRow trainingDataRow = new TrainingDataRow();

            foreach (Joint joint in _skeleton.Joints)
            {
                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    switch (joint.JointType) 
                    {

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
    }
}
