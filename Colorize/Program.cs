using System;
using System.IO;
using Michiru.Calculation;
using Michiru.Neural;
using Michiru.Utils;

namespace Colorize
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Write("Loading X... ");
			var trainX = ChiruMatrix.FromJSON(File.ReadAllText("X.json"));
			Console.WriteLine("Done!");
			Console.Write("Loading X... ");
			var trainY = ChiruMatrix.FromJSON(File.ReadAllText("Y.json"));
			Console.WriteLine("Done!");
			var activations = new ActivationFunction[]
			{
				ActivationFunction.Sigmoid,
				ActivationFunction.Sigmoid,
				ActivationFunction.Sigmoid
			};
			ChiruMath.PARALLEL = true;
			var lastCost = double.PositiveInfinity;
			Console.WriteLine("Training!");
			//var parameters = Parameters.FromJSON(File.ReadAllText("p.json"));
			var parameters = DeepNeuralNetwork.Model(trainX, trainY, new int[] { trainY.Height }, activations, 2.2, 15000, null, (i, c) =>
			{
				if (lastCost < c)
					Console.WriteLine($"[{i}] Learning Rate might be too high");
				if (double.IsNaN(c))
					Console.WriteLine($"[{i}] NaN");
				lastCost = c;
				if (i % (10000 * .1) == 0)
					Console.WriteLine($"[{i}] : {c}");
			});
			File.WriteAllText("p.json", parameters.ToJSON());
			Console.WriteLine("Done!");
			Console.Write("Testing... ");
			var pTY = DeepNeuralNetwork.Predict(parameters, trainX, activations, y => y);
			for (int i = 0; i < pTY.Width; i++)
			{
				var file = new FileStream($"test{i}.png", FileMode.Create);
				ImagePreProcessor.Expand(pTY[i], file);
				file.Flush();
				file.Dispose();
			}
			Console.WriteLine("Done!");
			Console.ReadLine();
		}
	}
}
