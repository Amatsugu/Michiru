using Michiru.Calculation;using System;using SkiaSharp;using System.IO;using Michiru.Utils;using Michiru.Regression;using System.Collections.Generic;using Newtonsoft.Json;using Michiru.Neural;namespace Michiru{
	class Program
	{		static void Main(string[] args)		{
			/*Console.Write("Loading Data... ");
			var trainX = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Train\X.json"));			var trainY = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Train\Y.json"));			var testY = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Test\Y.json"));			var testX = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Test\X.json"));
			//Standardize
			trainX /= 255;			testX /= 255;
			Console.WriteLine("Done!");
			*/
			var trainX = new double[,]
			{
				{ 0, 1, 1, 0 },
				{ 0, 0, 1, 1}
			}.AsMatrix();
			var trainY = new double[,]
			{
				{ 0, 0, 1, 0 }
			}.AsMatrix();
			var testX = trainX;
			var testY = trainY;
			
			Console.WriteLine("Training Model:");
			(ChiruMatrix W1, ChiruMatrix b1, ChiruMatrix W2, ChiruMatrix b2) = NeuralNetwork.Model(trainX, trainY, 4, 100, true);
			File.WriteAllText(@"D:\ChiruData\NN\W1.json", W1.ToJSON());			File.WriteAllText(@"D:\ChiruData\NN\W2.json", W2.ToJSON());			File.WriteAllText(@"D:\ChiruData\NN\b1.json", b1.ToJSON());			File.WriteAllText(@"D:\ChiruData\NN\b2.json", b2.ToJSON());
			Console.WriteLine("Done");
			/*Console.Write("Loading Model... ");
			var W1 = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\NN\W1.json"));			var b1 = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\NN\b1.json"));			var W2 = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\NN\W2.json"));			var b2 = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\NN\b2.json"));			Console.WriteLine("Done!");*/			Console.Write("Testing Model... ");			var pT = NeuralNetwork.Predict(W1, b1, W2, b2, trainX);			var p = NeuralNetwork.Predict(W1, b1, W2, b2, testX);			Console.WriteLine("Done!");			Console.WriteLine($"Train Accuracy: {(((trainY / pT.T) + ((1 - trainY) / (1 - pT.T))).Sum() / trainY.Width) * 100}%");			Console.WriteLine($"Test Accuracy: {(((testY / p.T) + ((1 - testY) / (1 - p.T))).Sum() / testY.Width) * 100}%");			Console.ReadLine();		}
	}}