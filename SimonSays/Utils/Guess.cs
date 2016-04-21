using System;
using System.Collections.Generic;
using System.Text;

namespace SimonSays.Utils
{
    /// <summary>
    /// Used to show the results of AI systems.
    /// </summary>
    class Guess
    {
        private int mGuessId;
        private double mGuessValue;

        public Guess()
        {

        }

        public Guess(int id, double value)
        {
            this.mGuessId = id;
            this.mGuessValue = value;
        }

        public int getGuessId()
        {
            return mGuessId;
        }

        public double getGuessValue() 
        {
            return mGuessValue;
        }
    }
}
