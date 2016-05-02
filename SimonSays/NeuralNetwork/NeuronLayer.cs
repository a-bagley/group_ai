using System;

namespace SimonSays.NeuralNetwork
{
    public class NeuronLayer
    {
        /// <summary>
        /// The number of nodes in this neuron layer
        /// </summary>
        private int nNodes;

        /// <summary>
        /// The number of nodes in the child neuron layer
        /// </summary>
        private int nChildNodes;

        /// <summary>
        /// The number of nodes in the parent neuron layer
        /// </summary>
        private int nParentNodes;

        /// <summary>
        /// The weight values for the nodes of this
        /// </summary>
        private double[,] weights;

        /// <summary>
        /// Changes in to weights
        /// </summary>
        private double[,] weightChanges;

        /// <summary>
        /// The neuron values of the layer
        /// </summary>
        private double[] neuronVals;

        /// <summary>
        /// Target neuron values
        /// </summary>
        private double[] desiredVals;

        /// <summary>
        /// Error values of neurons
        /// </summary>
        private double[] errorVals;

        /// <summary>
        /// Bias wieghts for the layer
        /// </summary>
        private double[] biasWeights;

        /// <summary>
        /// Bias values used
        /// </summary>
        private double[] biasVals;

        /// <summary>
        /// Learning rate of the layer
        /// </summary>
        private double learningRate;

        /// <summary>
        /// Momentum factor used
        /// </summary>
        private double momentum;

        /// <summary>
        /// The connected child layer
        /// </summary>
        private NeuronLayer childLayer;

        /// <summary>
        /// The connected parent layer
        /// </summary>
        private NeuronLayer parentLayer;

        /// <summary>
        /// Construct a Neuron layer
        /// </summary>
        /// <param name="nNodes">Number of Nodes in the layer</param>
        /// <param name="nChildNodes">Number of connected child nodes</param>
        /// <param name="nParentNodes">Number of connected parent nodes</param>
        /// <param name="learningRate"></param>
        /// <param name="momentum"></param>
        public NeuronLayer(int nNodes, int nChildNodes, int nParentNodes, double learningRate, double momentum)
        {
            this.childLayer = null;
            this.parentLayer = null;
            this.nNodes = nNodes;
            this.nChildNodes = nChildNodes;
            this.nParentNodes = nParentNodes;
            this.learningRate = learningRate;
            this.momentum = momentum;

            int i, j;

            // Allocate array memory
            neuronVals = new double[nNodes];
            desiredVals = new double[nNodes];
            errorVals = new double[nNodes];

            weights = new double[nNodes, nChildNodes];
            weightChanges = new double[nNodes, nChildNodes];

            biasVals = new double[nChildNodes];
            biasWeights = new double[nChildNodes];

            // Randomise node values to start
            Random rand = new Random();
            for (i = 0; i < nNodes; i++)
            {
                for (j = 0; j < nChildNodes; j++)
                {
                    weights[i, j] = rand.NextDouble();
                    weightChanges[i, j] = 0;
                }
            }
            for (j = 0; j < nChildNodes; j++)
            {
                biasVals[j] = -1;
                biasWeights[j] = rand.NextDouble();
            }
        }

        /// <summary>
        /// Link the layer to its child and parent
        /// </summary>
        /// <param name="parentLayer"></param>
        /// <param name="childLayer"></param>
        public void linkNeurons(NeuronLayer parentLayer, NeuronLayer childLayer)
        {
            this.parentLayer = parentLayer;
            this.childLayer = childLayer;
        }

        public int getNumberNodes()
        {
            return nNodes;
        }

        public int getNumberChildNodes()
        {
            return nChildNodes;
        }

        /// <summary>
        /// Get neuron value at selected node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public double getNeuronValue(int node)
        {
            return neuronVals[node];
        }

        /// <summary>
        /// Set the value of desired node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="val"></param>
        public void setNeuronValue(int node, double val)
        {
            neuronVals[node] = val;
        }

        public double getDesiredValue(int node)
        {
            return desiredVals[node];
        }

        public void setDesiredValue(int node, double val)
        {
            desiredVals[node] = val;
        }

        public double getBiasWeight(int node)
        {
            return biasWeights[node];
        }

        /// <summary>
        /// Use Sigmoid function to calculate neuron values
        /// </summary>
        public void CalculateNeuronValues()
        {
            int i, j;
            double x;

            if (parentLayer != null)
            {
                for (j = 0; j < nNodes; j++)
                {
                    x = 0;
                    for (i = 0; i < nParentNodes; i++)
                    {
                        x += parentLayer.neuronVals[i] * parentLayer.weights[i,j];
                    }
                    x += parentLayer.biasVals[j] * parentLayer.biasWeights[j];
                    neuronVals[j] = 1.0f / (1.0f + (double)Math.Exp(-x));
                }
            }
        }

        /// <summary>
        /// Calculate the errors from desired values
        /// </summary>
        public void calculateErrors()
        {
            int i, j;
            double total;
            if (childLayer == null) // output layer
            {
                for (i = 0; i < nNodes; i++)
                {
                    errorVals[i] = (desiredVals[i] - neuronVals[i]) * neuronVals[i] * (1.0f - neuronVals[i]);
                }
            }
            else if (parentLayer == null)
            { // input layer
                for (i = 0; i < nNodes; i++)
                {
                    errorVals[i] = 0.0f;
                }
            }
            else
            { // hidden layer
                for (i = 0; i < nNodes; i++)
                {
                    total = 0;
                    for (j = 0; j < nChildNodes; j++)
                    {
                        total += childLayer.errorVals[j] * weights[i,j];
                    }
                    errorVals[i] = total * neuronVals[i] * (1.0f - neuronVals[i]);
                }
            }
        }

        /// <summary>
        /// Adjust the weights appropriately
        /// </summary>
        public void adjustWeights()
        {
            int i, j;
            double dw;

            if (childLayer != null)
            {
                for (i = 0; i < nNodes; i++)
                {
                    for (j = 0; j < nChildNodes; j++)
                    {
                        dw = learningRate * childLayer.errorVals[j] * neuronVals[i];
                        weights[i,j] += dw + momentum * weightChanges[i,j];
                        weightChanges[i,j] = dw;
                    }
                }

                for (j = 0; j < nChildNodes; j++)
                {
                    biasWeights[j] += learningRate * childLayer.errorVals[j] * biasVals[j];
                }
            }
        }

        /// <summary>
        /// Print neuron layer info
        /// </summary>
        /// <returns></returns>
        public String toString()
        {
	        String layerInfo = "This layer has " + nNodes
			        + " nodes\n " + nChildNodes + " child nodes\n"
			        + nParentNodes + " parent nodes";
	        return layerInfo;
        }
        
    }
}
