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
		private double _lerningRate;
		private ActivationFunction _act;

		public NeuralNet(int inputs, int outputs, int[] hiddenLayers, ActivationFunction activator, double lerningRate)
		{
			_inputLayer = new InputLayer(inputs);
			_act = activator;
			_lerningRate = lerningRate;
			_HiddenLayers = new NeuralLayer[hiddenLayers.Length];
			for (int i = 0; i < hiddenLayers.Length; i++)
			{
				_HiddenLayers[i] = new NeuralLayer(hiddenLayers[i], (i - 1 < 0) ? _inputLayer : _HiddenLayers[i - 1]);
			}
			_outputLayer = new OutputLayer(outputs, _HiddenLayers.Last());
		}

		public NeuralValues GetOutput(NeuralValues inputs)
		{
			return GetResult(inputs).OutputSum;
		}

		public NeuralResults GetResult(NeuralValues inputs)
		{
			_inputLayer.SetInputs(inputs);
			return _outputLayer.GetResult(_HiddenLayers.Length, _act);
		}

		public NeuralNet Train(TrainingData data, ActivationFunction dActivator, int iterations = 10_000_000)
		{
			for (int ir = 0; ir < iterations; ir++)
			{
				for (int ti = 0; ti < data.Count; ti++)
				{
					NeuralValues tIn = new NeuralValues(new double[1, data.Inputs.Width]);
					tIn[0] = data.Inputs[ti];
					NeuralValues tOut = new NeuralValues(new double[1, data.Outputs.Width]);
					tOut[0] = data.Outputs[ti];
					NeuralResults results = GetResult(tIn);
					NeuralValues errorOutputLayer = tOut - results.OutputResult;
					double deltaOutputLayer = results.OutputSum.Activate(dActivator) / errorOutputLayer;
					for (int i = 0; i < results.HiddenSums.Length; i++)
					{
						NeuralValues hiddenOutputChanges = (deltaOutputLayer * results.HiddenSums[i].Transpose()) * _lerningRate;
						double deltaHiddenLayer = (new NeuralValues(_HiddenLayers[i].Weights) * deltaOutputLayer) / results.HiddenSums[i].Activate(dActivator);
						_HiddenLayers[i].Weights = MatrixMath.Add(_HiddenLayers[i].Weights, hiddenOutputChanges.Values);
					}
					NeuralValues inputHiddenChanges = (deltaOutputLayer * tIn.Transpose()) * _lerningRate;
					_inputLayer.Weights = MatrixMath.Add(_HiddenLayers[0].Weights, inputHiddenChanges.Values);
				}
			}
			return this;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Hidden Layers:");
			for (int i = 0; i < _HiddenLayers.Length; i++)
			{
				sb.AppendLine($"\t[{i}] W:{new NeuralValues(_HiddenLayers[i].Weights)}");
			}
			sb.AppendLine("Output Layer:");
			sb.AppendLine($"\t W:{new NeuralValues(_outputLayer.Weights)}");

			return sb.ToString();
		}
	}
}
