using SimonSays.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimonSays.NaiveBayes
{
    /// <summary>
    /// Naive Bayes skeletal position classifier.
    /// </summary>
    class NaiveBayesClassifier : Classifier
    {

        /// <summary>
        /// Naive Byaes algorithm engine
        /// </summary>
        private NaiveBayesEngine mNB;

        /// <summary>
        /// Number of attributes that describe the data set
        /// </summary>
        private readonly int NUMBER_OF_ATTRIBUTES = 12;

        /// <summary>
        /// List of gesture names
        /// </summary>
        private List<String> mGestureNameList;

        // Descriptors for length
        private static readonly String VERY_SHORT = "v_short";
        private static readonly String SHORT = "short";
        private static readonly String MEDIUM = "medium";
        private static readonly String LONG = "long";
        private static readonly String VERY_LONG = "v_long";


        public NaiveBayesClassifier()
        {

        }

        /// <summary>
        /// Create Naive Byaes engine and populate with appropriate training data
        /// </summary>
        /// <param name="sdp"></param>
        public void trainAI(RawSkeletalDataPackage sdp)
        {
            mGestureNameList = sdp.getGestureNameList();
            Dictionary<String, List<List<String>>> trainingDataDic = new Dictionary<String, List<List<String>>>(createNaiveBayesData(sdp));
            mNB = new NaiveBayesEngine(NUMBER_OF_ATTRIBUTES, sdp.getTotalGestures(), sdp.getTotalDataRows(), trainingDataDic);
#if DEBUG
            // Test in debug because it takes a noticeable amount of time
            testNB(trainingDataDic);
#endif
        }

        /// <summary>
        /// Get a probabilistic classification guess from skeletal data
        /// </summary>
        /// <param name="skelDataRow"></param>
        /// <returns>Classification guess</returns>
        public Guess makeGuess(SkeletonDataRow skelDataRow)
        {
            double[] values = calculateDistances(skelDataRow);
            String[] lengths = convertDistancesToLengths(values);
            double[] results = new double[mNB.mNumberOfCategories];

            for (int i = 0; i < mNB.mNumberOfCategories; i++)
            {
                // Compute probability of each category (class)
                results[i] = mNB.ClassifyFromDic(lengths, mGestureNameList.ElementAt(i));
            }
            return getBestGuess(results);
        }

        /// <summary>
        /// Find the most probable category based on probability
        /// </summary>
        /// <param name="results"></param>
        /// <returns>Probabilistic Guess</returns>
        private Guess getBestGuess(double[] results)
        {
            double max = -1000.0;
            int max_position = -1;
            for (int i = 0; i < mNB.mNumberOfCategories; i++)
            {
                if (results[i] > max)
                {
                    max_position = i;
                    max = results[i];
                }
            }
            return new Guess(max_position, max);
        }

        /// <summary>
        /// Create appropriate traingin data for the Naive Bayes engine, from skeletal training data.
        /// </summary>
        /// <param name="sdp"></param>
        /// <returns>NB Training data Dictionary</returns>
        private Dictionary<String, List<List<String>>> createNaiveBayesData(RawSkeletalDataPackage sdp)
        {
            Dictionary<String, List<SkeletonDataRow>> skelDataDic = sdp.getSkeletalDataDic();
            Dictionary<String, List<List<String>>> nbTrainingDic = new Dictionary<String, List<List<String>>>();
            foreach (String key in skelDataDic.Keys)
            {
                // For each gesture
                List<List<String>> categoryTrainingData = new List<List<String>>();
                foreach (SkeletonDataRow row in skelDataDic[key])
                {
                    // get distances
                    double[] values = calculateDistances(row);
                    List<String> inputs = new List<String>(convertDistancesToLengths(values));
                    categoryTrainingData.Add(inputs);
                }
                // Add to new training data Dictionary
                nbTrainingDic.Add(key, categoryTrainingData);
            }
            return nbTrainingDic;
        }

        /// <summary>
        /// Calculate useful distances from skeletal coordinates
        /// </summary>
        /// <param name="dRow"></param>
        /// <returns>NB training data row</returns>
        private double[] calculateDistances(SkeletonDataRow dRow)
        {
            DistanceCalculator dCalc = new DistanceCalculator(dRow);
            double[] newTrainingDataRow = new double[NUMBER_OF_ATTRIBUTES];
            newTrainingDataRow[0] = dCalc.getWristLeftToWristRight();
            newTrainingDataRow[1] = dCalc.getWristLeftToHipCentre();
            newTrainingDataRow[2] = dCalc.getWristLeftToShoulderRight();
            newTrainingDataRow[3] = dCalc.getWristRightToHipCentre();
            newTrainingDataRow[4] = dCalc.getWristRightToShoulderLeft();
            newTrainingDataRow[5] = dCalc.getElbowRightToHipCentre();
            newTrainingDataRow[6] = dCalc.getElbowLeftToHipCentre();
            newTrainingDataRow[7] = dCalc.getWristLeftToHipLeft();
            newTrainingDataRow[8] = dCalc.getWristRightToHipRight();
            newTrainingDataRow[9] = dCalc.getWristRightToKneeRight();
            newTrainingDataRow[10] = dCalc.getWristLeftToKneeLeft();
            newTrainingDataRow[11] = dCalc.getAnkleRightToAnkleLeft();

            for (int i = 0; i < newTrainingDataRow.Length; i++)
            {
                newTrainingDataRow[i] = normaliseData(newTrainingDataRow[i]);
            }
            return newTrainingDataRow;
        }

        /// <summary>
        /// Normalise value to between 0 and 1
        /// </summary>
        /// <param name="val"></param>
        /// <returns>Normalised value</returns>
        private double normaliseData(double val)
        {
            return val / 2;
        }

        /// <summary>
        /// Convert an array of numerical distances to
        /// and array of descriptive length Strings.
        /// </summary>
        /// <param name="distances"></param>
        /// <returns></returns>
        private String[] convertDistancesToLengths(double[] distances)
        {
            String[] lengths = new String[distances.Length];

            for (int i = 0; i < distances.Length; i++)
            {
                lengths[i] = classifyLength(distances[i]);
            }
            return lengths;
        }

        /// <summary>
        /// Convert numerical distances to a descriptive length.
        /// </summary>
        /// <param name="distance">
        /// Numerical distance
        /// </param>
        /// <returns></returns>
        private String classifyLength(double distance)
        {
            if (distance < 0.15)
                return VERY_SHORT;
            else if (distance < 0.3)
                return SHORT;
            else if (distance < 0.5)
                return MEDIUM;
            else if (distance < 0.7)
                return LONG;
            else
                return VERY_LONG;
        }

        /// <summary>
        /// Feed Testing data back into Naive Bayes engine and
        /// test classification accuracy
        /// </summary>
        /// <param name="testData"></param>
        private void testNB(Dictionary<String, List<List<String>>> testData)
        {
            int testTotal = 0;
            int totalCorrect = 0;
            int totalIncorrect = 0;
            int keyIndex = 0;
            double[] results = new double[mNB.mNumberOfCategories];
            foreach (String key in testData.Keys)
            {
                foreach (List<String> testSet in testData[key])
                {
                    for (int i = 0; i < mNB.mNumberOfCategories; i++)
                    {
                        string[] testLengths = testSet.Select(j => j.ToString()).ToArray();
                        results[i] = mNB.ClassifyFromDic(testLengths, mGestureNameList.ElementAt(i));
                    }
                    Guess g = getBestGuess(results);
                    if (g.getGuessId() == keyIndex)
                        totalCorrect++;
                    else
                        totalIncorrect++;
                    testTotal++;
                }
                keyIndex++;
            }
            double error = (double)totalIncorrect / (double)testTotal;
            System.Diagnostics.Debug.WriteLine("Correct tests: " + totalCorrect + "\nIncorrect tests: " + totalIncorrect + "\nError: " + error);
        }
    }
}
