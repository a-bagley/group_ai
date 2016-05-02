namespace SimonSays.Utils
{
    /// <summary>
    /// Used to show the classification results of AI systems
    /// </summary>
    class Guess
    {
        /// <summary>
        /// The output ID of the highest confidence AI output.
        /// Maps directy to the list of gesture names
        /// </summary>
        private int mGuessId;

        /// <summary>
        /// The value of the highest confidence classification guess.
        /// </summary>
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
