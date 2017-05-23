using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Neural
{
    struct TrainingData
    {
		public int Count { get; }

		public NeuralValues Inputs { get; set; }
		public NeuralValues Outputs { get; set; }

		public TrainingData(double[,] inputs, double[,] outputs)
		{
			Count = inputs.Length;
			Inputs = new NeuralValues(inputs);
			Outputs = new NeuralValues(outputs);
		}
    }
}
