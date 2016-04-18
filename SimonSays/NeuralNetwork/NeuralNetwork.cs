using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Number of hidden neurons is roughly equal to the square root of the product of the number of inputs and outputs
// Test 
namespace SimonSays
{
    namespace NeuralNetwork
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
	
	            inputLayer.linkNeurons(null, allLayers[1]);
	            for (int i = 1; i < nHiddenLayers; i++)
		            allLayers[i].linkNeurons(allLayers[i - 1], allLayers[i + 1]);
	            outputLayer.linkNeurons(allLayers[nHiddenLayers], null);
	
	            //Todo move this to init/constructor of NeuronLayer
	            //for (int i = 0; i < nAllLayers; i++)
		        //    allLayers[i].RandomizeWeights();
                ///Done?
	
	            for (int i = 0; i < nAllLayers; i++)
		            System.Diagnostics.Debug.WriteLine(allLayers[i].toString());
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="i"></param>
            /// <param name="value"></param>
            public void setInput(int i, double val)
            {
                if ((i >= 0) && (i < inputLayer.getNumberNodes()))
                {
                    inputLayer.setNeuronValue(i, val);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public double getOutput(int i)
            {
                if ((i >= 0) && (i < outputLayer.getNumberNodes()))
                {
                    return outputLayer.getDesiredValue(i);
                }

                return (double)10000; // to indicate an error
            }

            /// <summary>
            /// 
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
            /// 
            /// </summary>
            public void feedForward()
            {
                for (int i = 0; i < nAllLayers; i++)
                    allLayers[i].CalculateNeuronValues();
            }

            /// <summary>
            /// 
            /// </summary>
            public void backPropagate()
            {
                for (int i = 0; i < nAllLayers; i++)
                    allLayers[i].calculateErrors();

                for (int i = 0; i < nAllLayers; i++)
                    allLayers[i].adjustWeights();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
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

                return id;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
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
            /// 
            /// </summary>
            public void PrintInfo()
            {
                // Test this properly
                System.Diagnostics.Debug.WriteLine("If this is junk, come back soon...");
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
                        System.Diagnostics.Debug.WriteLine(j + ": " + allLayers[i].getBiasValue(j));
                    }
                    System.Diagnostics.Debug.WriteLine("\n");
                }
            }
        }
    }
}
