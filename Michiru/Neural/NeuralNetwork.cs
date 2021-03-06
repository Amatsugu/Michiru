using Michiru.Calculation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Neural
{
    public class NeuralNetwork
    {
		private static readonly ActivationFunction activationFunction = ActivationFunction.TanH;

		public static (ChiruMatrix W1, ChiruMatrix b1, ChiruMatrix W2, ChiruMatrix b2) InitalizeParameters(int nX, int nH, int nY)
		{
			var W1 = ChiruMatrix.Random(nH, nX);
			var b1 = ChiruMatrix.Zeros(nH, 1);
			var W2 = ChiruMatrix.Random(nY, nH);
			var b2 = ChiruMatrix.Zeros(nY, 1);
			return (W1, b1, W2, b2);
		}

		public static (ChiruMatrix Z1, ChiruMatrix A1, ChiruMatrix Z2, ChiruMatrix A2) ForwardPropagation(ChiruMatrix X, ChiruMatrix W1, ChiruMatrix b1, ChiruMatrix W2, ChiruMatrix b2)
		{
			var Z1 = (W1 * X).ColAdd(b1);
			var A1 = Z1.Activate(activationFunction);
			var Z2 = (W2 * A1).ColAdd(b2);
			var A2 = Z2.Activate(ActivationFunction.Sigmoid);

			return (Z1, A1, Z2, A2);
		}

		public static (ChiruMatrix dW1, ChiruMatrix db1, ChiruMatrix dW2, ChiruMatrix db2) BackPropagation(ChiruMatrix X, ChiruMatrix Y, ChiruMatrix W1, ChiruMatrix b1, ChiruMatrix W2, ChiruMatrix b2, ChiruMatrix Z1, ChiruMatrix A1, ChiruMatrix Z2, ChiruMatrix A2)
		{
			var m = Y.Width;
			var dZ2 = A2 - Y;
			var dW2 = (dZ2 * A1.T) / m;
			var db2 = dZ2.SumAxis(MatrixAxis.Horizontal) / m;
			var dZ1 = (W2.T * dZ2).ElementMultiply(1 - A1.Map(x => Math.Pow(x, 2)));
			var dW1 = (dZ1 * X.T) / m;
			var db1 = dZ1.SumAxis(MatrixAxis.Horizontal) / m;

			return (dW1, db1, dW2, db2);
		}

		public static (ChiruMatrix W1, ChiruMatrix b1, ChiruMatrix W2, ChiruMatrix b2) UpdateParameters(ChiruMatrix W1, ChiruMatrix b1, ChiruMatrix W2, ChiruMatrix b2, ChiruMatrix dW1, ChiruMatrix db1, ChiruMatrix dW2, ChiruMatrix db2, double learningRate = 1.2)
		{
			W1 = W1 - (learningRate * dW1);
			b1 = b1 - (learningRate * db1);
			W2 = W2 - (learningRate * dW2);
			b2 = b2 - (learningRate * db2);
			return (W1, b1, W2, b2);
		}

		public static (ChiruMatrix W1, ChiruMatrix b1, ChiruMatrix W2, ChiruMatrix b2) Model(ChiruMatrix X, ChiruMatrix Y, int hiddenLayerSize, int iterations = 10000, double learningRate = 1.2, bool printCost = false)
		{
			(int nX, int nH, int nY) = LayerSizes(X, Y, hiddenLayerSize);
			var parameters = InitalizeParameters(nX, nH, nY);
			DateTime tic, startTime = tic = DateTime.Now;
			int percentile = iterations / 10;
			percentile = percentile == 0 ? 1 : percentile;
			for (int i = 0; i < iterations; i++)
			{
				var forward = ForwardPropagation(X, parameters.W1, parameters.b1, parameters.W2, parameters.b2);
				var cost = ComputeCost(forward.A2, Y);
				var back = BackPropagation(X, Y, parameters.W1, parameters.b1, parameters.W2, parameters.b2, forward.Z1, forward.A1, forward.Z2, forward.A2);

				parameters = UpdateParameters(parameters.W1, parameters.b1, parameters.W2, parameters.b2, back.dW1, back.db1, back.dW2, back.db2, learningRate);

				if (printCost && (i+1) % percentile == 0)
				{
					Console.WriteLine($"[{i+1}/{iterations}] Cost: {cost} \t {(DateTime.Now - startTime).TotalSeconds}s \t+{(DateTime.Now - tic).TotalSeconds}s");
					tic = DateTime.Now;
				}
			}
			return parameters;
		}

		public static ChiruMatrix Predict(ChiruMatrix W1, ChiruMatrix b1, ChiruMatrix W2, ChiruMatrix b2, ChiruMatrix X)
		{
			var r = ForwardPropagation(X, W1, b1, W2, b2);
			return r.A2.Map(x => x > .5 ? 1 : 0);
		}

		public static double ComputeCost(ChiruMatrix A2, ChiruMatrix Y)
		{
			var cost = (A2.Map(x => x == 0 ? 0 : Math.Log(x)).ElementMultiply(Y)) + ((1 - Y).ElementMultiply((1 - A2).Map(x => x == 0 ? 0 : Math.Log(x))));
			return cost.Sum() / -Y.Width;
		}

		private static (int nX, int nH, int nY) LayerSizes(ChiruMatrix X, ChiruMatrix Y, int hiddenLayerSize = 4) => (X.Height, hiddenLayerSize, Y.Height);
	}
}
