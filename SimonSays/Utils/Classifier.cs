namespace SimonSays.Utils
{
    /// <summary>
    /// CLassifier interface that AI systems must implement for modularity
    /// </summary>
    interface Classifier
    {
        void trainAI(RawSkeletalDataPackage skeletalDataPackage);
        Guess makeGuess(SkeletonDataRow skelDataRow);

    }
}
