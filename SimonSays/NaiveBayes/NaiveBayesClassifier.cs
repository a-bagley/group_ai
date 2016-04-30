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

        private NaiveBayesEngine mNB;

        private readonly int NUMBER_OF_ATTRIBUTES = 12;

        private List<String> mGestureNameList;

        private static readonly String VERY_SHORT  = "v_short";
        private static readonly String SHORT       = "short";
        private static readonly String MEDIUM      = "medium";
        private static readonly String LONG        = "long";
        private static readonly String VERY_LONG   = "v_long";


        public NaiveBayesClassifier()
        {

        }

        public void trainAI(RawSkeletalDataPackage sdp)
        {
            mGestureNameList = sdp.getGestureNameList();
            Dictionary<String, List<List<String>>> trainingDataDic = new Dictionary<String, List<List<String>>>(createNaiveBayesData(sdp));
            mNB = new NaiveBayesEngine(NUMBER_OF_ATTRIBUTES, sdp.getTotalGestures(), sdp.getTotalDataRows(), trainingDataDic);
        }

        public Guess makeGuess(SkeletonDataRow skelDataRow)
        {
            double[] values = calculateDistances(skelDataRow);
            String[] lengths = convertDistancesToLengths(values);
            double[] results = new double[mNB.mNumberOfCategories];

            for (int i = 0; i < mNB.mNumberOfCategories; i++)
            {
                results[i] = mNB.ClassifyFromDic(lengths, mGestureNameList.ElementAt(i));
            }

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

        private Dictionary<String, List<List<String>>> createNaiveBayesData(RawSkeletalDataPackage sdp)
        {
            // hack
            //int i = 0;
            Dictionary<String, List<SkeletonDataRow>> skelDataDic = sdp.getSkeletalDataDic();
            Dictionary<String, List<List<String>>> nbTrainingDic = new Dictionary<String, List<List<String>>>();
            foreach (String key in skelDataDic.Keys)
            {
                List<List<String>> categoryTrainingData = new List<List<String>>();
                foreach (SkeletonDataRow row in skelDataDic[key])
                {
                    double[] values = calculateDistances(row);
                    List<String> inputs = new List<String>(convertDistancesToLengths(values));
                    categoryTrainingData.Add(inputs);
                    //i++;
                    //if (i == 50)
                        //break;
                }
                nbTrainingDic.Add(key, categoryTrainingData);
            }
            return nbTrainingDic;
        }

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

        private double normaliseData(double val)
        {
            return val / 2;
        }

        /// <summary>
        /// Convert an array of numerical distances to
        /// and array of size strings.
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
        /// Convert numerical distances to named sizes.
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

    }
}
