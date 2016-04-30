namespace SimonSays.Utils
{
    interface Classifier
    {
        void trainAI(RawSkeletalDataPackage skeletalDataPackage);
        Guess makeGuess(SkeletonDataRow skelDataRow);

    }
}
