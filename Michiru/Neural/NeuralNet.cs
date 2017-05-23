using Michiru.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Michiru.Neural
{
    class NeuralNet
    {
		private InputLayer _inputLayer;
		private OutputLayer _outputLayer;
		private NeuralLayer[] _HiddenLayers;

		public NeuralNet(int inputs, int outputs, int[] hiddenLayers, ActivationFunction activator)
		{
			_inputLayer = new InputLayer(inputs);
			_HiddenLayers = new NeuralLayer[hiddenLayers.Length];
			for (int i = 0; i < hiddenLayers.Length; i++)
			{
				_HiddenLayers[i] = new NeuralLayer(hiddenLayers[i], activator, (i - 1 < 0) ? _inputLayer : _HiddenLayers[i - 1]);
			}
			_outputLayer = new OutputLayer(outputs, activator, _HiddenLayers.Last());
		}

		public NeuralValues GetOutput(NeuralValues inputs)
		{
			_inputLayer.SetInputs(inputs);
			return _outputLayer.GetOutput();
		}

		public NeuralNet Train(TrainingData data, ActivationFunction dActivator, int iterations = 1000_000)
		{
			for (int it = 0; it < iterations; it++)
			{

				for (int i = 0; i < data.Count; i++)
				{
					NeuralValues outputSum = GetOutput(data.Inputs[i]);
					double[] deltaOutputSum = new double[outputSum.Count];
					for (int j = 0; j < outputSum.Count; j++)
					{
						deltaOutputSum[j] = dActivator(outputSum[j]) * (data.Outputs[i][j] - outputSum[j]);
					}
					_outputLayer.BackPropagate(deltaOutputSum);
					double[] hiddenSum = _HiddenLayers.Last().GetOutput().Values.Select(v => dActivator(v)).ToArray(); //Scale this up
					double[] hiddenToOuterW = _outputLayer.GetNeurons().First().Synapses.Select(s => s.OldWeight).ToArray();
					double[] deltaHW = hiddenSum.Zip(hiddenToOuterW, (hs, hw) => deltaOutputSum[0] * hs * hw).ToArray();
					double[] dw = new double[deltaHW.Length * data.Inputs[i].Count];
					int k = 0;
					foreach (double iv in data.Inputs[i].Values)
					{
						dw[k] = deltaHW[k % deltaHW.Length] * iv;
						k++;
					}
				}
			}
			return this;
		}
	}
}
