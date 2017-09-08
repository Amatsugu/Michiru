using Michiru.Calculation;
using System;
using SkiaSharp;
using System.IO;
using Michiru.Utils;
using Michiru.Regression;
using System.Collections.Generic;
using Newtonsoft.Json;
using Michiru.Neural;

namespace Michiru
{
    class Program
    {
		static void Main(string[] args)
		{
			/*var trainX = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Train\X.json"));
			var trainY = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Train\Y.json"));
			var testY = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Test\Y.json"));
			var testX = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Test\X.json"));
			//Standardize
			trainX /= 255;
			testX /= 255;
			*/

			var X = new double[,] 
			{
				{  1.62434536, -0.61175641, -0.52817175},
				{  -1.07296862, 0.86540763, -2.3015387}
			}.AsMatrix();
			var Y = new double[,]
			{
				{ 1.74481176, -0.7612069, 0.3190391 }
			}.AsMatrix();
			var r = NeuralNetwork.Model(X, Y, 4, 10000, true);
			Console.WriteLine($"W1:\n{r.W1}");
			Console.WriteLine($"b1:\n{r.b1}");
			Console.WriteLine($"W2:\n{r.W2}");
			Console.WriteLine($"b2:\n{r.b2}");
			Console.ReadLine();
		}
    }
}