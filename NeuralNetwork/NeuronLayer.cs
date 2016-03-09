using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
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

        //
        private double[,] weightChanges;

        //
        private double[] neuronVals;

        //
        private double[] desiredVals;

        //
        private double[] errorVals;

        //
        private double[] biasWeights;

        //
        private double[] biasVals;

        //
        private double learningRate;

        //
        private double momentum;

        //
        private NeuronLayer childLayer;

        //
        private NeuronLayer parentLayer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nNodes"></param>
        /// <param name="nChildNodes"></param>
        /// <param name="nParentNodes"></param>
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

            // Allocate memory
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
        /// 
        /// </summary>
        /// <param name="parentLayer"></param>
        /// <param name="childLayer"></param>
        public void linkNeurons(NeuronLayer parentLayer, NeuronLayer childLayer)
        {
            this.parentLayer = parentLayer;
            this.childLayer = childLayer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getNumberNodes()
        {
            return nNodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getNumberChildNodes()
        {
            return nChildNodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public double getNeuronValue(int node)
        {
            return neuronVals[node];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="val"></param>
        public void setNeuronValue(int node, double val)
        {
            neuronVals[node] = val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public double getDesiredValue(int node)
        {
            return desiredVals[node];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="val"></param>
        public void setDesiredValue(int node, double val)
        {
            desiredVals[node] = val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public double getBiasValue(int node)
        {
            return biasVals[node];
        }

        /// <summary>
        /// 
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

                    //if ((childLayer == null) && linearOutput)
                    //    neuronVals[j] = x;
                   // else
                        neuronVals[j] = 1.0f / (1 + Math.Exp(-x));
                }
            }
        }

        /// <summary>
        /// 
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
        /// 
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

        // Debug, possibly remove later
        /// <summary>
        /// 
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
