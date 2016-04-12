using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class MultilayerPerceptron
    {
        /// <summary>
        /// Neural network
        /// </summary>
        private NeuralNetwork neuralNetwork;

        private int[] nodeList;

        private int nInputs;

        private int nOutputs;

        private double learningRate;

        private double momentum;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeList"></param>
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
            int	i, j;
	        double error = 1;
            int nTrainingSets = trainingData.GetLength(1);
            int setMarker = 0;
     
	        System.Diagnostics.Debug.WriteLine("************Pre training*******************");
            neuralNetwork.PrintInfo();

	        while((error > 0.05) ) //&& (c<50000))
	        {
		        error = 0;
		        //c++;
                for (i = 0; i < nTrainingSets; i++)
		        {
                    for (j = 0; j < nInputs; j++ )
                    {
                        neuralNetwork.setInput(j, trainingData[i, j]);
                        setMarker = j;
                    }

                    for (j = 0; j < nOutputs; j++ )
                    {
                        neuralNetwork.setDesiredOutput(j, trainingData[i, setMarker + 1]); // Get the next piece of data after all the input data
                        setMarker++;
                    }
                    
                    neuralNetwork.feedForward();
                    error += neuralNetwork.calculateError();
                    neuralNetwork.backPropagate();
		        }
                error = error / nTrainingSets;
	        }
            System.Diagnostics.Debug.WriteLine("************Post training*******************");
            neuralNetwork.PrintInfo();
        }

        //Todo finish implementing the Multilyaer Perceptron.

        //Todo decide on how training data will be pulled together from multiple files.

        //Todo Add another layer (sigh) for pulling traing data together.

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

        public void testMLP()
        {

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
