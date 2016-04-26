using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimonSays.NaiveBayes
{
    /// <summary>
    /// Naive Bayes style algorithm
    /// </summary>
    class NaiveBayesEngine
    {

        private Dictionary<String, List<List<String>>> mTrainingDataDic;

        public int mNumberOfAttributes { get; set; }

        public int mNumberOfCategories { get; set; }

        public int mTotalTrainingData { get; set; }

        private double m = 2.0;

        private double p = 0.5;

        public NaiveBayesEngine(int nAttributes, int nCategories, int totalDataRows, Dictionary<String, List<List<String>>> trainingDataDic)
        {
            mNumberOfAttributes = nAttributes;
            mNumberOfCategories = nCategories;
            mTotalTrainingData = totalDataRows;
            mTrainingDataDic = new Dictionary<String, List<List<String>>>(trainingDataDic);
        }

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
                        count[e]++; //[j]
                    }
                }
                p_category *= ((double)count[e] + m * p) / ((double)num_category + m); //[j]
            }
            System.Diagnostics.Debug.WriteLine("probability of " + category + " = " + p_category);//((double)count[j] + m * p) / ((double)num_category + m));
            return p_category;
        }
    }
}
