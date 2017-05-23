using Michiru.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Michiru.Neural
{
    class NeuralLayer
    {
		public int Size { get; protected set; }
		public double[,] Weights { get; set; }
		public NeuralLayer PrevLayer { get; protected set; }
		public static Random RAND = new Random();

		protected NeuralLayer()
		{
		}

		public NeuralLayer(int neuronCount, NeuralLayer prevLayer)
		{
			Size = neuronCount;
			Weights = new double[prevLayer.Size, neuronCount];
			for (int i = 0; i < prevLayer.Size; i++)
			{
				for (int j = 0; j < neuronCount; j++)
				{
					Weights[i, j] = RAND.NextDouble();
				}
			}
			PrevLayer = prevLayer;
		}

		public virtual double[,] GetOutput(NeuralResults r) => MatrixMath.Multiply((PrevLayer.GetType() == typeof(InputLayer)) ? PrevLayer.GetOutput(r) : r.AddHiddenSum(PrevLayer.GetOutput(r)), Weights);
	}

	class InputLayer : NeuralLayer
	{

		private double[,] _inputs;

		public InputLayer(int inputCount, int inputWidh)
		{
			Size = inputWidh;
			_inputs = new double[inputCount, inputWidh];
		}

		public InputLayer SetInputs(NeuralValues inputs)
		{
			for (int i = 0; i < _inputs.GetLength(0); i++)
			{
				for (int j = 0; j < _inputs.GetLength(1); j++)
				{
					_inputs[i, j] = inputs[i, j];
				}
			}
			return this;
		}

		public override double[,] GetOutput(NeuralResults r)
		{
			return _inputs;
		}


	}

	class OutputLayer : NeuralLayer
	{
		public OutputLayer(int outputs, NeuralLayer prevLayer) : base(outputs, prevLayer)
		{

		}

		public virtual NeuralResults GetResult(int hiddenLayers, ActivationFunction activator)
		{
			NeuralResults r = new NeuralResults(hiddenLayers, activator);
			r.SetOutputSum(GetOutput(r));
			return r;
		}
	}
}
