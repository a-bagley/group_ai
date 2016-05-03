using SimonSays.Utils;
using System;
using System.Collections.Generic;

namespace SimonSays.NeuralNetwork
{
    class MLPClassifier : Classifier
    {
        private MultilayerPerceptron mMLP;
        
        /// <summary>
        /// Learning rate value
        /// </summary>
        private double mLearningRate;

        /// <summary>
        /// Momentum value
        /// </summary>
        private double mMomentum;

        /// <summary>
        /// Number of desired inputs to MLP.
        /// This should match the numebr of Skeletal distances used.
        /// </summary>
        private readonly int NUMBER_OF_INPUTS = 12;

        public MLPClassifier(double learningRate, double momentum)
        {
            mLearningRate = learningRate;
            mMomentum = momentum;
        }

        /// <summary>
        /// Train the ANN classifier
        /// </summary>
        /// <param name="sdp"></param>
        public void trainAI(RawSkeletalDataPackage sdp)
        {
            // 2 hidden layers is optimal
            int n = 2;
            // add input and output
            n += 2;
            int[] nodeSet = new int[n];
            nodeSet[0] = NUMBER_OF_INPUTS;
            for (int i = 1; i < nodeSet.Length - 1; i++)
            {
                nodeSet[i] = NUMBER_OF_INPUTS;
            }
            nodeSet[nodeSet.Length - 1] = sdp.getTotalGestures();
            mMLP = new MultilayerPerceptron(nodeSet, mLearningRate, mMomentum);          
            mMLP.trainMLP(createMLPTrainingData(sdp));
        }

        /// <summary>
        /// Get the classification guess off the ANN
        /// from a set of skeletal inputs
        /// </summary>
        /// <param name="skelDataRow"></param>
        /// <returns>Classification Guess</returns>
        public Guess makeGuess(SkeletonDataRow skelDataRow)
        {
            double[] values = calculateDistances(skelDataRow);
            return mMLP.GenerateMLPResult(values);
        }

        /// <summary>
        /// Create ANN training data from skeletal coordinate data
        /// </summary>
        /// <param name="sdp"></param>
        /// <returns>2D traing data array</returns>
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

            // For gesture
            foreach (String key in skelDataDic.Keys)
            {
                // For data row of gesture
                foreach (SkeletonDataRow row in skelDataDic[key])
                {
                    // Get input disatnces for this data row
                    double[] values = calculateDistances(row);

                    // Copy singular training data to row in full 2D traing data array
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

        /// <summary>
        /// Convert skeletal data to 12 useful distances
        /// </summary>
        /// <param name="dRow"></param>
        /// <returns>Array of body-part distances</returns>
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

    }
}
