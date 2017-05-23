using Michiru.Calculation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Neural
{
    class NeuralResults
    {
		public NeuralValues[] HiddenSums { get; private set; }
		public NeuralValues[] HiddenResults { get; private set; }
		public NeuralValues OutputSum { get; private set; }
		public NeuralValues OutputResult { get; private set; }

		private ActivationFunction _act;
		private int _curLayer;

		public NeuralResults(int hiddenLayers, ActivationFunction activator)
		{
			_act = activator;
			HiddenSums = new NeuralValues[hiddenLayers];
			HiddenResults = new NeuralValues[hiddenLayers];
			_curLayer = hiddenLayers - 1;
		}

		public double[,] AddHiddenSum(double[,] sum)
		{
			if (_curLayer < 0)
				throw new Exception("Hidden Layer out of bounds");
			HiddenSums[_curLayer] = new NeuralValues(sum);
			HiddenResults[_curLayer] = HiddenSums[_curLayer].Activate(_act);
			_curLayer--;
			return sum;
		}

		public NeuralResults SetOutputSum(double[,] sum)
		{
			OutputSum = new NeuralValues(sum);
			OutputResult = OutputSum.Activate(_act);
			return this;
		}
	}
}
