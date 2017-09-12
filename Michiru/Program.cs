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
			
			Console.Write("Loading Data... ");
			var trainX = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Train\X.json"));
			var trainY = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Train\Y.json"));
			var testY = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Test\Y.json"));
			var testX = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Test\X.json"));
			//Standardize
			trainX /= 255;
			testX /= 255;
			Console.WriteLine("Done!");
			/*
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
			var testY = trainY;*/
			Console.WriteLine("Training Model:");
			var r = DeepNeuralNetwork.Model(trainX, trainY, new int[] { 100, 25, 6 }, .0075, 10, true);
			//File.WriteAllText(@"D:\ChiruData\DNN\b2.json", JsonConvert.SerializeObject(r));
			Console.WriteLine("Done");
			Console.Write("Testing Model... ");
			var pT = DeepNeuralNetwork.Predict(r, trainX);
			var p = DeepNeuralNetwork.Predict(r, testX);
			Console.WriteLine("Done!");
			Console.WriteLine($"Train Accuracy: {pT.ErrorWith(trainY)}%");
			Console.WriteLine($"Test Accuracy: {p.ErrorWith(testY)}%");
			Console.ReadLine();
		}
	}
}
