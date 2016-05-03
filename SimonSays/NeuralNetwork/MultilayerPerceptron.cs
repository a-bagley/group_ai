using SimonSays.Utils;

namespace SimonSays.NeuralNetwork
{
    class MultilayerPerceptron
    {
        /// <summary>
        /// Neural network object
        /// </summary>
        private NeuralNetwork neuralNetwork;

        /// <summary>
        /// List of nodes for each layer
        /// </summary>
        private int[] nodeList;

        /// <summary>
        /// Number of inputs
        /// </summary>
        private int nInputs;

        /// <summary>
        /// number of outputs
        /// </summary>
        private int nOutputs;

        /// <summary>
        /// Learning rate
        /// </summary>
        private double learningRate;

        /// <summary>
        /// Momentum factor
        /// </summary>
        private double momentum;

        /// <summary>
        /// Construct an MLP
        /// </summary>
        /// <param name="nodeList">List of layers and associated neurons</param>
        /// <param name="learningRate"></param>
        /// <param name="momentum"></param>
        public MultilayerPerceptron(int[] nodeList, double learningRate, double momentum)
        {
            this.nodeList = nodeList;
            this.nInputs = nodeList[0];
            this.nOutputs = nodeList[nodeList.Length - 1];
            this.learningRate = learningRate;
            this.momentum = momentum;

            this.neuralNetwork = new NeuralNetwork(nodeList, learningRate, momentum);
        }

        /// <summary>
        /// Train the Multilayer Perceptron with a traing data set
        /// </summary>
        /// <param name="trainingData">
        /// 2D Array of training data sets
        /// </param>
        public void trainMLP(double[,] trainingData)
        {
            int i, j;
            double error = 1;
            int nTrainingSets = trainingData.GetLength(0);
            int setMarker = 0;

            System.Diagnostics.Debug.WriteLine("************Pre training*******************");
            // Ensure training will end by tracking cycles
            int maxCycles = 0;
            while ((error > 0.001) && (maxCycles<50000)) // 0.001 is the optimal error rate
            {
                error = 0;
                maxCycles++;
                for (i = 0; i < nTrainingSets; i++)
                {
                    for (j = 0; j < nInputs; j++)
                    {
                        neuralNetwork.setInput(j, trainingData[i, j]);
                        setMarker = j;
                    }

                    for (j = 0; j < nOutputs; j++)
                    {
                        neuralNetwork.setDesiredOutput(j, trainingData[i, setMarker + 1]); // Get the next piece of data after all the input data
                        setMarker++;
                    }

                    neuralNetwork.feedForward();
                    error += neuralNetwork.calculateError();
                    neuralNetwork.backPropagate();
                }
                error = error / nTrainingSets;
                System.Diagnostics.Debug.WriteLine("Error: " + error);
            }
            System.Diagnostics.Debug.WriteLine("************Post training*******************");
            neuralNetwork.PrintInfo();
            testMLP(trainingData);
        }

        /// <summary>
        /// Generate a classification guess based on inputs
        /// </summary>
        /// <param name="inputData">
        /// The input values to the MLP
        /// </param>
        /// <returns>
        /// The id of the output matched
        /// </returns>
        public Guess GenerateMLPResult(double[] inputData)
        {
            // Provide array of distances for inputs
            for (int i = 0; i < nInputs; i++)
            {
                neuralNetwork.setInput(i, inputData[i]);
            }
            neuralNetwork.feedForward();
            int resultId = neuralNetwork.getMaxOutputID();
            double resultValue = neuralNetwork.getOutput(resultId);
            return new Guess(resultId, resultValue);
        }

        /// <summary>
        /// Test the MLP with the traing data to see how
        /// well it has been learned
        /// </summary>
        /// <param name="trainingData"></param>
        public void testMLP(double[,] trainingData)
        {
            int totalCorrect = 0;
            int totalIncorrect = 0;
            double[] testRow = new double[trainingData.GetLength(1)];
            for (int i = 0; i < trainingData.GetLength(0); i++)
            {
                for (int j = 0; j < trainingData.GetLength(1); j++)
                {
                    testRow[j] = trainingData[i, j];
                }
                Guess testResult = GenerateMLPResult(testRow);
                if (trainingData[i, testResult.getGuessId() + nInputs] == 0.9)
                {
                    totalCorrect++;
                }
                else
                {
                    totalIncorrect++;
                }
            }
            double error = (double)totalIncorrect / (double)trainingData.GetLength(0);
            System.Diagnostics.Debug.WriteLine("Correct tests: " + totalCorrect + "\nIncorrect tests: " + totalIncorrect + "\nError: " + error);
        }

        public double getLearningRate()
        {
            return learningRate;
        }

        public double getMomentum()
        {
            return momentum;
        }
    }
}
