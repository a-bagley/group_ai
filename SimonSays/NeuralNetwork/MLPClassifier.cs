using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimonSays.Utils;

namespace SimonSays.NeuralNetwork
{
    class MLPClassifier : Classifier
    {
        private MultilayerPerceptron mMLP;
        
        private double mLearningRate;

        private double mMomentum;

        /// <summary>
        /// Number of desired inputs to MLP.
        /// </summary>
        private readonly int NUMBER_OF_INPUTS = 9;

        public MLPClassifier(double learningRate, double momentum)
        {
            mLearningRate = learningRate;
            mMomentum = momentum;
            //mMLP = new MultilayerPerceptron(nodeList, learningRate, momentum);
        }

        public void trainAI(RawSkeletalDataPackage sdp)
        {
            // 9 is the magic number!
            //square root of the product of the number of inputs and outputs
            ////int n = (int)Math.Sqrt(9 * sdp.getTotalGestures());
            int n = 1;
            // add input and output
            n += 2;
            int[] nodeSet = new int[n];
            nodeSet[0] = NUMBER_OF_INPUTS;
            for (int i = 1; i < nodeSet.Length - 1; i++)
            {
                nodeSet[i] = NUMBER_OF_INPUTS; //3
            }
            nodeSet[nodeSet.Length - 1] = sdp.getTotalGestures();
            mMLP = new MultilayerPerceptron(nodeSet, mLearningRate, mMomentum);          
            mMLP.trainMLP(createMLPTrainingData(sdp));
        }

        public Guess makeGuess(SkeletonDataRow skelDataRow)
        {
            double[] values = calculateDistances(skelDataRow);
            return mMLP.GenerateMLPResult(values);
        }

        private double[,] createMLPTrainingData(RawSkeletalDataPackage sdp)
        {
            double[,] tData = new double[sdp.getTotalDataRows(), NUMBER_OF_INPUTS + sdp.getTotalGestures()];
            // populate training data array with 0.1
            for (int i = 0; i < tData.GetLength(0); i++)
            {
                for (int j = 0; j < tData.GetLength(1); j++)
                {
                    tData[i, j] = 0.1;
                }
            }

            Dictionary<String, List<SkeletonDataRow>> skelDataDic = sdp.getSkeletalDataDic();
            int columnIndex = 0, rowIndex, fileIndex = 0;

            foreach (String key in skelDataDic.Keys)
            {
                foreach (SkeletonDataRow row in skelDataDic[key])
                {
                    // Add method here to return double[] containg the distances we want from
                    // the training data row values.
                    double[] values = calculateDistances(row);

                    for (rowIndex = 0; rowIndex < values.Length; rowIndex++)
                    {
                        tData[columnIndex,rowIndex] = values[rowIndex];
                    }

                    // Here the output value will be set to 0.9 for all the training data rows it relates to.
                    tData[columnIndex, rowIndex + fileIndex] = 0.9;
                    columnIndex++;
                }
                fileIndex++;
            }
            return tData;
        }

        private double[] calculateDistances(SkeletonDataRow dRow)
        {
            DistanceCalculator dCalc = new DistanceCalculator(dRow);
            double[] newTrainingDataRow = new double[NUMBER_OF_INPUTS];
            newTrainingDataRow[0] = dCalc.getWristLeftToWristRight();
            newTrainingDataRow[1] = dCalc.getWristLeftToHipCentre();
            newTrainingDataRow[2] = dCalc.getWristLeftToShoulderRight();
            newTrainingDataRow[3] = dCalc.getWristRightToHipCentre();
            newTrainingDataRow[4] = dCalc.getWristRightToShoulderLeft();
            newTrainingDataRow[5] = dCalc.getElbowRightToHipCentre();
            newTrainingDataRow[6] = dCalc.getElbowLeftToHipCentre();
            newTrainingDataRow[7] = dCalc.getWristLeftToHipLeft();
            newTrainingDataRow[8] = dCalc.getWristRightToHipRight();
            return newTrainingDataRow;
        }

    }
}
