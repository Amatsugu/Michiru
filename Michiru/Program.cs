using Michiru.Calculation;
using System;
using SkiaSharp;
using System.IO;
using Michiru.Utils;
using Michiru.Regression;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Michiru
{
    class Program
    {
		static void Main(string[] args)
		{
			var trainX = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Train\X.json"));
			var trainY = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Train\Y.json"));
			var testY = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Test\Y.json"));
			var testX = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\Test\X.json"));

			/*(ChiruMatrix trainX, ChiruMatrix trainY) = ImagePreProcessor.Flatten(@"D:\ChiruData\Train", "Non-ZR");
			(ChiruMatrix testX, ChiruMatrix testY) = ImagePreProcessor.Flatten(@"D:\ChiruData\Test", "Non-ZR");

			File.WriteAllText(@"D:\ChiruData\Train\X.json", trainX.ToJSON());
			File.WriteAllText(@"D:\ChiruData\Train\Y.json", trainY.ToJSON());
			File.WriteAllText(@"D:\ChiruData\Test\X.json", testX.ToJSON());
			File.WriteAllText(@"D:\ChiruData\Test\Y.json", testY.ToJSON());*/


			//Standardize
			trainX /= 255;
			testX /= 255;


			var result = LogisticRegression.Model(trainX, trainY, testX, testY, 3000, .0001, true);

			File.WriteAllText(@"D:\ChiruData\modelW.json", result.w.ToJSON());
			File.WriteAllText(@"D:\ChiruData\modelB.json", result.b.ToString());

			Console.WriteLine($"Train Accuracy: {result.trainAccuracy}%");
			Console.WriteLine($"Test Accuracy: {result.testAccuracy}%");

			Console.WriteLine(result.testPY);

			/*var w = ChiruMatrix.FromJSON(File.ReadAllText(@"D:\ChiruData\modelW.json"));
			var b = double.Parse(File.ReadAllText(@"D:\ChiruData\modelB.json"));
			var trainPredictY = LogisticRegression.Predict(w, b, trainX);
			var testPredictY = LogisticRegression.Predict(w, b, testX);


			Console.WriteLine($"Train Accuracy: {100 - ((trainPredictY - trainY).Abs().Mean() * 100)}%");
			Console.WriteLine($"Test Accuracy: {100 - ((testPredictY - testY).Abs().Mean() * 100)}%");*/



			Console.ReadLine();
		}
    }
}