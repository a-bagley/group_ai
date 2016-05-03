using System;
using System.Collections.Generic;
using System.Linq;

namespace SimonSays.NaiveBayes
{
    /// <summary>
    /// Naive Bayes style algorithm
    /// </summary>
    class NaiveBayesEngine
    {
        /// <summary>
        /// Traing data Dictionary used for probabilistic calclations
        /// </summary>
        private Dictionary<String, List<List<String>>> mTrainingDataDic;

        /// <summary>
        /// Number of attributes used for classification
        /// </summary>
        public int mNumberOfAttributes { get; set; }

        /// <summary>
        /// Number of classifications
        /// </summary>
        public int mNumberOfCategories { get; set; }

        /// <summary>
        /// Total number of traing data sets
        /// </summary>
        public int mTotalTrainingData { get; set; }

        /// <summary>
        /// pseudocount  to prevent 0 value probabilities
        /// </summary>
        private double m = 2.0;

        /// <summary>
        /// Prior estimate of probability
        /// </summary>
        private double p = 0.5;

        /// <summary>
        /// Construct a Naive Byaes classification engine
        /// </summary>
        /// <param name="nAttributes"></param>
        /// <param name="nCategories"></param>
        /// <param name="totalDataRows"></param>
        /// <param name="trainingDataDic"></param>
        public NaiveBayesEngine(int nAttributes, int nCategories, int totalDataRows, Dictionary<String, List<List<String>>> trainingDataDic)
        {
            mNumberOfAttributes = nAttributes;
            mNumberOfCategories = nCategories;
            mTotalTrainingData = totalDataRows;
            mTrainingDataDic = new Dictionary<String, List<List<String>>>(trainingDataDic);
        }

        /// <summary>
        /// Classify a test subject based on probabilistic calculation.
        /// </summary>
        /// <param name="test">The test set parameters</param>
        /// <param name="category">The class to test against</param>
        /// <returns>Probability of test subject being of the supplied class</returns>
        public double ClassifyFromDic(String[] test, String category)
        {
            int[] count = new int[mNumberOfAttributes];
            for (int i = 0; i < mNumberOfAttributes; i++)
            {
                count[i] = 0;
            }

            double p_category = 0.0;
            int num_category = mTrainingDataDic[category].Count;
            List<List<String>> categoryTrainingData = mTrainingDataDic[category];
            p_category = (double)num_category / (double)mTotalTrainingData;

            // iterate through attributes, checking all data sets
            int j = 0;
            for (int e = 0; e < mNumberOfAttributes; e++)
            {
                foreach (List<String> dataSet in categoryTrainingData)
                {
                    if (test[e].Equals(dataSet.ElementAt(e)))
                    {
                        count[e]++;
                    }
                }
                // Build up compound probability
                p_category *= ((double)count[e] + m * p) / ((double)num_category + m);
            }
            System.Diagnostics.Debug.WriteLine("probability of " + category + " = " + p_category);
            return p_category;
        }
    }
}
