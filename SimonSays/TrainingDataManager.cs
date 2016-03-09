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

        public double[] calculateDistanceBetweenJoints(int arrSize)
        {

            double[] distances = new double[arrSize];

            foreach (Joint joint in _skeleton.Joints)
            {
                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    switch (joint.JointType) 
                    {

                        case JointType.Head:
                        {
                            distances[0] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.HandRight));
                            distances[1] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.HandLeft));
                            
                            distances[2] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.ElbowRight));
                            distances[3] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.ElbowLeft));
                            
                            break;
                        }
                        
                        case JointType.HipCenter:
                        {
                            distances[4] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.ElbowRight));
                            distances[5] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.ElbowLeft));
                            
                            distances[6] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.HandRight));
                            distances[7] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.HandLeft));
                            
                            distances[8] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.KneeRight));
                            distances[9] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.KneeLeft));

                            distances[10] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.FootRight));
                            distances[11] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.FootLeft));
                            
                            break;
                        }

                        case JointType.ShoulderCenter:
                        {
                            distances[12] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.ElbowRight));
                            distances[13] = getDistanceBetweenTwoJoints(joint, _skeleton.Joints.FirstOrDefault(s => s.JointType == JointType.ElbowLeft));

                            break;
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine(distances);
            return distances;
        }
    }
}
