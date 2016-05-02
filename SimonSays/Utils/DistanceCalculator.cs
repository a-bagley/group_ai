using System;

namespace SimonSays.Utils
{
    /// <summary>
    /// Calculates the euclidean distance between Kinect skeletal points.
    /// Methods are named in a self-explanatory manner.
    /// </summary>
    class DistanceCalculator
    {
        private SkeletonDataRow mSkeleton;

        public DistanceCalculator(SkeletonDataRow skeletonData)
        {
            mSkeleton = skeletonData;
        }

        /// <summary>
        /// Calculate the euclidean distance between two points using X and Y coordinates.
        /// </summary>
        /// <param name="point1x"></param>
        /// <param name="point1y"></param>
        /// <param name="point2x"></param>
        /// <param name="point2y"></param>
        /// <returns></returns>
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

        public double getWristRightToKneeRight()
        {
            return getDistanceBetweenPoints(mSkeleton.wristRightX, mSkeleton.wristRightY, mSkeleton.kneeRightX, mSkeleton.kneeRightY);
        }
        public double getWristLeftToKneeLeft()
        {
            return getDistanceBetweenPoints(mSkeleton.wristLeftX, mSkeleton.wristLeftY, mSkeleton.kneeLeftX, mSkeleton.kneeLeftY);
        }

        public double getWristRightToKneeLeft()
        {
            return getDistanceBetweenPoints(mSkeleton.wristRightX, mSkeleton.wristRightY, mSkeleton.kneeLeftX, mSkeleton.kneeLeftY);
        }
        public double getWristLeftToKneeRight()
        {
            return getDistanceBetweenPoints(mSkeleton.wristLeftX, mSkeleton.wristLeftY, mSkeleton.kneeRightX, mSkeleton.kneeRightY);
        }

        public double getAnkleRightToAnkleLeft()
        {
            return getDistanceBetweenPoints(mSkeleton.ankleRightX, mSkeleton.ankleRightY, mSkeleton.ankleLeftX, mSkeleton.ankleLeftY);
        }
    }
}
