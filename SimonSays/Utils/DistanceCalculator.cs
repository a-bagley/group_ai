using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimonSays.Utils
{
    class DistanceCalculator
    {
        private SkeletonDataRow mSkeleton;

        public DistanceCalculator(SkeletonDataRow skeletonData)
        {
            mSkeleton = skeletonData;
        }

        private double getDistanceBetweenPoints(double point1x, double point1y, double point2x, double point2y)
        {
            return Math.Sqrt(Math.Pow((point1x - point2x), 2) + Math.Pow((point1y - point2y), 2));
        }

        public double  getHeadToWristRight()
        {
            return getDistanceBetweenPoints(mSkeleton.headX, mSkeleton.headY, mSkeleton.wristRightX, mSkeleton.wristRightY);
        }

        public double getHeadToWristLeft()
        {
            return getDistanceBetweenPoints(mSkeleton.headX, mSkeleton.headY, mSkeleton.wristLeftX, mSkeleton.wristLeftY);
        }

        public double getElbowRightToHipCentre()
        {
            return getDistanceBetweenPoints(mSkeleton.elbowRightX, mSkeleton.elbowRightY, mSkeleton.hipCenterX, mSkeleton.hipCenterY);
        }

        public double getElbowLeftToHipCentre()
        {
            return getDistanceBetweenPoints(mSkeleton.elbowLeftX, mSkeleton.elbowLeftY, mSkeleton.hipCenterX, mSkeleton.hipCenterY);
        }

        public double getWristRightToShoulderLeft()
        {
            return getDistanceBetweenPoints(mSkeleton.wristRightX, mSkeleton.wristRightY, mSkeleton.shoulderLeftX, mSkeleton.shoulderLeftY);
        }

        public double getWristLeftToShoulderRight()
        {
            return getDistanceBetweenPoints(mSkeleton.wristLeftX, mSkeleton.wristLeftY, mSkeleton.shoulderRightX, mSkeleton.shoulderRightY);
        }

        public double getWristRightToHipCentre()
        {
            return getDistanceBetweenPoints(mSkeleton.wristRightX, mSkeleton.wristRightY, mSkeleton.hipCenterX, mSkeleton.hipCenterY);
        }

        public double getWristLeftToHipCentre()
        {
            return getDistanceBetweenPoints(mSkeleton.wristLeftX, mSkeleton.wristLeftY, mSkeleton.hipCenterX, mSkeleton.hipCenterY);
        }

        public double getWristRightToHipRight()
        {
            return getDistanceBetweenPoints(mSkeleton.wristRightX, mSkeleton.wristRightY, mSkeleton.hipRightX, mSkeleton.hipRightY);
        }

        public double getWristLeftToHipLeft()
        {
            return getDistanceBetweenPoints(mSkeleton.wristLeftX, mSkeleton.wristLeftY, mSkeleton.hipLeftX, mSkeleton.hipLeftY);
        }

        public double getWristLeftToWristRight()
        {
            return getDistanceBetweenPoints(mSkeleton.wristLeftX, mSkeleton.wristLeftY, mSkeleton.wristRightX, mSkeleton.wristRightY);
        }

    }
}
