using Michiru.Calculation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Michiru.Regression
{
    public static class LogisticRegression
	{

		public static (ChiruMatrix dw, double db, double cost) Propagate(ChiruMatrix w, double b, ChiruMatrix X, ChiruMatrix Y)
		{
			var m = Y.Width;
			var a = ((w.Transpose() * X) + b).Activate(Activation.S);
			var loss = ChiruMatrix.Zero(a.Height, a.Width);
			for (int i = 0; i < a.Height; i++)
			{
				for (int j = 0; j < a.Width; j++)
				{
					loss[i, j] = Loss(a[i, j], Y[i, j]);
				}
			}
			var cost = loss.Sum() / - m;

			var dw = (X * (a - Y).Transpose())/m;
			var db = (a - Y).Sum() / m;

			return (dw, db, cost);
		}

		public static (ChiruMatrix w, double b, List<double> costs) Optimize(ChiruMatrix w, double b, ChiruMatrix X, ChiruMatrix Y, double iterations, double learningRate = 0.009, bool printCost = false, Action<double> costOut = null)
		{
			List<double> costs = new List<double>();
			var tic = DateTime.Now;
			for (int i = 0; i < iterations; i++)
			{
				(ChiruMatrix dw, double db, double curCost) = Propagate(w, b, X, Y);
				w = w - (learningRate * dw);
				b = b - (learningRate * db);
				costOut?.Invoke(curCost);
				
				if (i % 100 == 0)
				{
					costs.Add(curCost);
					if (printCost)
					{
						Console.WriteLine($"[{i}] Cost:{curCost} \t{(DateTime.Now - tic).TotalSeconds}s");
						tic = DateTime.Now;
					}
				}
			}
			return (w, b, costs);
		}

		public static ChiruMatrix Predict(ChiruMatrix w, double b, ChiruMatrix X)
		{
			var m = X.Width;
			var predictionY = ChiruMatrix.Zero(1, m);
			var a = ((w.Transpose() * X) + b).Activate(Activation.S);
			for (int i = 0; i < m; i++)
			{
				a[0, i] = (a[0, i] > .5) ? 1 : 0;
			}
			return a;
		}

		public static (ChiruMatrix w, double b, double trainAccuracy, double testAccuracy, ChiruMatrix testPY, ChiruMatrix trainPY, List<double> costs) Model(ChiruMatrix trainX, ChiruMatrix trainY, ChiruMatrix testX, ChiruMatrix testY, int iterations = 2000, double learningRate = .5, bool printCost = false, Action<double> costOut = null)
		{
			var w = ChiruMatrix.Zero(trainX.Height, 1);
			double b = 0;
			var result = Optimize(w, b, trainX, trainY, iterations, learningRate, printCost, costOut);
			w = result.w;
			b = result.b;

			var testPredictY = Predict(w, b, testX);
			var trainPredictY = Predict(w, b, trainX);

			var testPredictError = (100 - ((testPredictY - testY).Abs().Mean()) * 100);
			var trainPredictError = (100 - ((trainPredictY - trainY).Abs().Mean()) * 100);

			return (w, b, trainPredictError, testPredictError, testPredictY, trainPredictY, result.costs);
		}

		static double Loss(double a, double y) => y * Math.Log(a) + (1 - y) * Math.Log(1 - a);
    }
}
