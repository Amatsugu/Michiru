using Michiru.Calculation;
using Michiru.Neural;
using System;

namespace Michiru
{
    class Program
    {
		static void Main(string[] args)
		{
			TrainingData trainD = new TrainingData(new double[][]
			{
				new double[]{ 0, 0 },
				new double[]{ 1, 0 },
				new double[]{ 0, 1 },
				new double[]{ 1, 1 }
			}, new double[][]
			{
				new double[] { 0 },
				new double[] { 1 },
				new double[] { 1 },
				new double[] { 0 }
			});

			NeuralValues input = new NeuralValues(new double[] { 1, 1 });
			int[] hiddenLayers = new int[] { 3 };
			Console.Write("Training... ");
			DateTime start = DateTime.Now;
			NeuralNet net = new NeuralNet(2, 1, hiddenLayers, Activation.S);
			net.Train(trainD, Activation.DS);
			Console.WriteLine($"Done in {(DateTime.Now - start).TotalSeconds}s");
			for(int i = 0; i < trainD.Inputs.Length; i++)
				Console.WriteLine($"{trainD.Inputs[i]}{net.GetOutput(trainD.Inputs[i])}");
			Console.ReadLine();
        }
    }
}