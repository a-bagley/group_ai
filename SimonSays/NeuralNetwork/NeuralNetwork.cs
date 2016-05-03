using System;

namespace SimonSays.NeuralNetwork
{
    class NeuralNetwork
    {
        /// <summary>
        /// Input layer of the neural network
        /// </summary>
        private NeuronLayer inputLayer;

        /// <summary>
        /// Output layer of the neural network
        /// </summary>
        private NeuronLayer outputLayer;

        /// <summary>
        /// Array containing all neural network layers
        /// </summary>
        private NeuronLayer[] allLayers;

        /// <summary>
        /// Number of hidden layers in the neural network
        /// </summary>
        private int nHiddenLayers;

        /// <summary>
        /// Total number of layers in the neural network
        /// </summary>
        private int nAllLayers;


        /// <summary>
        /// Neural Network constructor
        /// </summary>
        /// <param name="nodeList">
        /// Array of nodes
        /// </param>
        /// <param name="fLearningRate">
        /// Neural network learning rate
        /// </param>
        /// <param name="fMomentum">
        /// Neural network momentum factor
        /// </param>
        public NeuralNetwork(int[] nodeList, double fLearningRate, double fMomentum)
        {
            //Todo Consider removing input and output params in favour of a singular array of nodes
            this.nAllLayers = nodeList.Length;
            this.nHiddenLayers = nAllLayers - 2;
	        this.allLayers = new NeuronLayer[nAllLayers];
	
            // Create ANN layers with different numbers of nodes
	        for (int i = 0; i < nAllLayers; i++) 
	        {
		        if (i == 0)
                    allLayers[i] = inputLayer = new NeuronLayer(nodeList[i], nodeList[i + 1], 0, fLearningRate, fMomentum);
		        else if (i < nHiddenLayers)
                    allLayers[i] = new NeuronLayer(nodeList[i], nodeList[i + 1], allLayers[i - 1].getNumberNodes(), fLearningRate, fMomentum);
		        else if(i < nHiddenLayers + 1)
                    allLayers[i] = new NeuronLayer(nodeList[i], nodeList[i + 1], allLayers[i - 1].getNumberNodes(), fLearningRate, fMomentum);
		        else
                    allLayers[i] = outputLayer = new NeuronLayer(nodeList[i], 0, allLayers[i - 1].getNumberNodes(), fLearningRate, fMomentum);
	        }
            // Link neuron layers together
	        inputLayer.linkNeurons(null, allLayers[1]);
	        for (int i = 1; i < nAllLayers - 1; i++)
		        allLayers[i].linkNeurons(allLayers[i - 1], allLayers[i + 1]);
	        outputLayer.linkNeurons(allLayers[nHiddenLayers], null);
            // Display ANN structure
	        for (int i = 0; i < nAllLayers; i++)
		        System.Diagnostics.Debug.WriteLine(allLayers[i].toString());
        }

        /// <summary>
        /// Set the input value of an inout node
        /// </summary>
        /// <param name="i">node to set</param>
        /// <param name="value">value to set</param>
        public void setInput(int i, double val)
        {
            // Ensure desired node is accessible
            if ((i >= 0) && (i < inputLayer.getNumberNodes()))
            {
                inputLayer.setNeuronValue(i, val);
            }
        }

        /// <summary>
        /// Get the value of a specific output node
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double getOutput(int i)
        {
            // Ensure desired node is accessible
            if ((i >= 0) && (i < outputLayer.getNumberNodes()))
            {
                return outputLayer.getNeuronValue(i);
            }

            return (double)10000; // to indicate an error
        }

        /// <summary>
        /// Set the desired value of a specific output node
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        public void setDesiredOutput(int i, double val)
        {
            if ((i >= 0) && (i < outputLayer.getNumberNodes()))
            {
                outputLayer.setDesiredValue(i, val);
            }
        }

        /// <summary>
        /// Process inputs through the ANN
        /// </summary>
        public void feedForward()
        {
            for (int i = 0; i < nAllLayers; i++)
            {
                allLayers[i].CalculateNeuronValues();
            }
        }

        /// <summary>
        /// Teach the ANN using back propogation algorithm
        /// </summary>
        public void backPropagate()
        {
            for (int i = nAllLayers - 1; i > 0; i--)
                allLayers[i].calculateErrors();

            for (int i = nHiddenLayers; i >= 0; i--)
                allLayers[i].adjustWeights();
        }

        /// <summary>
        /// Get the ID of the highest node output (confidence)
        /// </summary>
        /// <returns>The guessed classification node ID</returns>
        public int getMaxOutputID()
        {
            int i, id;
            double maxval;

            maxval = outputLayer.getNeuronValue(0);
            id = 0;

            for (i = 1; i < outputLayer.getNumberNodes(); i++)
            {
                if (outputLayer.getNeuronValue(i) > maxval)
                {
                    maxval = outputLayer.getNeuronValue(i);
                    id = i;
                }
            }
            System.Diagnostics.Debug.WriteLine("Output: " + id + " guessed");
            return id;
        }

        /// <summary>
        /// Calculate the errors from the desired values
        /// </summary>
        /// <returns>Error rate</returns>
        public double calculateError()
        {
            int i;
            double error = 0;

            for (i = 0; i < outputLayer.getNumberNodes(); i++)
            {
                error += Math.Pow(outputLayer.getNeuronValue(i) - outputLayer.getDesiredValue(i), 2);
            }

            error = error / outputLayer.getNumberNodes();

            return error;
        }

        /// <summary>
        /// Print information about the ANN
        /// </summary>
        public void PrintInfo()
        {
            int i, j;

            for (i = 0; i < nAllLayers; i++)
            {
                System.Diagnostics.Debug.WriteLine("--------------------------------------------------------");
                System.Diagnostics.Debug.WriteLine("Layer: " + i);
                System.Diagnostics.Debug.WriteLine("--------------------------------------------------------");
                System.Diagnostics.Debug.WriteLine("Node Values:");
                for (j = 0; j < allLayers[i].getNumberNodes(); j++)
                {
                    System.Diagnostics.Debug.WriteLine(j + ": " + allLayers[i].getNeuronValue(j));
                }
                System.Diagnostics.Debug.WriteLine("\n");
                System.Diagnostics.Debug.WriteLine("Bias Weights:");
                for (j = 0; j < allLayers[i].getNumberChildNodes(); j++)
                {
                    System.Diagnostics.Debug.WriteLine(j + ": " + allLayers[i].getBiasWeight(j));
                }
                System.Diagnostics.Debug.WriteLine("\n");
            }
        }
    }
}
