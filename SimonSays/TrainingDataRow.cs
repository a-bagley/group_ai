using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimonSays
{
    enum Gesture 
    {
        ARM_LEFT,
        ARM_RIGHT,
        LEG_LEFT,
        LEG_RIGHT
    }

    class TrainingDataRow
    {

        public double headX { get; set; }
        public double headY { get; set; }

        public double shoulderCenterX { get; set; }
        public double shoulderCenterY { get; set; }

        public double shoulderRightX { get; set; }
        public double shoulderRightY { get; set; }

        public double shoulderLeftX { get; set; }
        public double shoulderLeftY { get; set; }

        public double elbowRightX { get; set; }
        public double elbowRightY { get; set; }

        public double elbowLeftX { get; set; }
        public double elbowLeftY { get; set; }

        public double wristRightX { get; set; }
        public double wristRightY { get; set; }

        public double wristLeftX { get; set; }
        public double wristLeftY { get; set; }

        public double handRightX { get; set; }
        public double handRightY{ get; set; }

        public double handLeftX { get; set; }
        public double handLeftY { get; set; }

        public double hipCenterX { get; set; }
        public double hipCenterY { get; set; }

        public double hipRightX { get; set; }
        public double hipRightY { get; set; }

        public double hipLeftX { get; set; }
        public double hipLeftY { get; set; }

        public double kneeRightX { get; set; }
        public double kneeRightY { get; set; }

        public double kneeLeftX { get; set; }
        public double kneeLeftY { get; set; }

        public double ankleRightX { get; set; }
        public double ankleRightY { get; set; }

        public double ankleLeftX { get; set; }
        public double ankleLeftY { get; set; }

        public double footRightX { get; set; }
        public double footRightY { get; set; }

        public double footLeftX { get; set; }
        public double footLeftY { get; set; }
    }
}
