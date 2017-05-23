using Michiru.Calculation;
using Michiru.Neural;
using System;

namespace Michiru
{
    class Program
    {
		static void Main(string[] args)
		{
			TrainingData trainD = new TrainingData(new double[4,2]
			{
				{ 0, 0 },
				{ 1, 0 },
				{ 0, 1 },
				{ 1, 1 }
			}, new double[4,1]
			{
				{ 0 },
				{ 1 },
				{ 1 },
				{ 0 }
			});

			

			DateTime start = DateTime.Now;
			NeuralNet net = new NeuralNet(new int[] { 4, 2 }, 1, new int[] { 3 }, Activation.S, .7);
			Console.WriteLine(net);
			Console.Write("Training... ");
			net.Train(trainD, Activation.DS, (int)20e6);
			Console.WriteLine($"Done in {(DateTime.Now - start).TotalSeconds}s");
			Console.WriteLine(net.GetOutput(trainD.Inputs));
			Console.WriteLine(net);
			Console.ReadLine();
        }
    }
}