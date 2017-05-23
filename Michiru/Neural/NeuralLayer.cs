using Michiru.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Michiru.Neural
{
    class NeuralLayer
    {
		public int Size { get; }
		protected Neuron[] _neurons;

		protected NeuralLayer()
		{
		}

		public NeuralLayer(int neuronCount, ActivationFunction activator, NeuralLayer prevLayer)
		{
			Size = neuronCount;
			_neurons = new Neuron[neuronCount];
			for (int i = 0; i < neuronCount; i++)
				_neurons[i] = new Neuron(Synapse.FromNeurons(prevLayer._neurons), activator);
		}

		public virtual NeuralLayer BackPropagate(double[] deltaOutputSum)
		{
			for (int i = 0; i < deltaOutputSum.Length; i++)
			{
				_neurons[i].BackPropagate(deltaOutputSum[i]);
			}
			return this;
		}

		public NeuralValues GetOutput() => new NeuralValues((from Neuron n in _neurons select n.GetOutput()));

	}

	class InputLayer : NeuralLayer
	{

		public InputLayer(int inputCount)
		{
			_neurons = new InputNeuron[inputCount];
			for (int i = 0; i < inputCount; i++)
			{
				_neurons[i] = new InputNeuron(0);
			}
		}

		public InputLayer SetInputs(NeuralValues inputs)
		{
			for (int i = 0; i < _neurons.Length; i++)
			{
				_neurons[i] = new InputNeuron(inputs[i]);
			}
			return this;
		}
	}

	class OutputLayer : NeuralLayer
	{
		public OutputLayer(int outputs, ActivationFunction activator, NeuralLayer prevLayer) : base(outputs, activator, prevLayer)
		{

		}

		public Neuron[] GetNeurons() => _neurons;
	}
}
