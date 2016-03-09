using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class NeuralNetwork
    {
        /// <summary>
        /// 
        /// </summary>
        private NeuronLayer inputLayer;

        /// <summary>
        /// 
        /// </summary>
        private NeuronLayer outputLayer;

        /// <summary>
        /// 
        /// </summary>
        private NeuronLayer[] allLayers;

        /// <summary>
        /// 
        /// </summary>
        private int nHiddenLayers;

        /// <summary>
        /// 
        /// </summary>
        private int nAllLayers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nInputNodes"></param>
        /// <param name="nHiddenNodes"></param>
        /// <param name="nOutputNodes"></param>
        /// <param name="fLearningRate"></param>
        /// <param name="fMomentum"></param>
        public NeuralNetwork(int nInputNodes, int[] nHiddenNodes, int nOutputNodes, double fLearningRate, double fMomentum)
        {
            //Todo Consider removing input and output params in favour of a singular array of nodes
            this.nHiddenLayers = nHiddenNodes.Length;
	        this.nAllLayers = nHiddenLayers + 2;
	        this.allLayers = new NeuronLayer[nAllLayers];
	
	        for (int i = 0; i < nAllLayers; i++) 
	        {
		        if (i == 0)
			        allLayers[i] = inputLayer = new NeuronLayer(nInputNodes, nHiddenNodes[i], 0, fLearningRate, fMomentum);
		        else if (i < nHiddenLayers)
			        allLayers[i] = new NeuronLayer(nHiddenNodes[i - 1], nHiddenNodes[i], allLayers[i - 1].getNumberNodes(), fLearningRate, fMomentum);
		        else if(i < nHiddenLayers + 1)
			        allLayers[i] = new NeuronLayer(nHiddenNodes[i - 1], nOutputNodes, allLayers[i - 1].getNumberNodes(), fLearningRate, fMomentum);
		        else 
			        allLayers[i] = outputLayer = new NeuronLayer(nOutputNodes, 0, allLayers[i - 1].getNumberNodes(), fLearningRate, fMomentum);
	        }
	
	        inputLayer.linkNeurons(null, allLayers[1]);
	        for (int i = 1; i < nHiddenLayers; i++)
		        allLayers[i].linkNeurons(allLayers[i - 1], allLayers[i + 1]);
	        outputLayer.linkNeurons(allLayers[nHiddenLayers], null);
	
	        //Todo move this to init/constructor of NeuronLayer
	        //for (int i = 0; i < nAllLayers; i++)
		    //    allLayers[i].RandomizeWeights();
	
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

    }
}
