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

        public double _distanceBetweenHeadAndHandRight { get; set; }

        public double _distanceBetweenHeadAndHandLeft { get; set; }

        public double _distanceBetweenHipCenterAndElbowLeft { get; set; }

        public double _distanceBetweenHipCenterAndElbowRight { get; set; }

        public double _distanceBetweenHipCenterAndKneeLeft { get; set; }

        public double _distanceBetweenHipCenterAndKneeRight { get; set; }

        public double _distanceBetweenHipCenterAndFootLeft { get; set; }

        public double _distanceBetweenHipCenterAndFootRight { get; set; }

        public double _distanceBetweenShoulderCenterAndElbowLeft { get; set; }

        public double _distanceBetweenShoulderCenterAndElbowRight { get; set; }

        public String _gesture { get; set; }

        public TrainingDataRow (String gesture) 
        {
            this._gesture = gesture;
        }

    }
}
