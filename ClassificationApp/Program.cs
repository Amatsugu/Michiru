using Michiru.Calculation;
using Michiru.Neural;
using SkiaSharp;
using System;
using System.IO;
using ZeroFormatter;
using Newtonsoft.Json;

namespace ClassificationApp
{
	class Program
	{
		public static int SIZE = 1000;

		static void Main(string[] args)
		{
			(ChiruMatrix trainX, ChiruMatrix trainY) = GenerateData(2000);
			SaveAsImage(trainX, trainY, "train.png");
			var activations = new ActivationFunction[]
			{
				ActivationFunction.Sigmoid,
				ActivationFunction.Sigmoid,
				ActivationFunction.Sigmoid,
				ActivationFunction.Sigmoid,
				ActivationFunction.Sigmoid
			};
			ChiruMath.PARALLEL = false;
			var lastCost = double.PositiveInfinity;
			//var parameters = Parameters.FromJSON(File.ReadAllText("p.json"));
			var iterations = 5000;
			var parameters = DeepNeuralNetwork.Model(trainX, trainY, new int[] { 4, 4, 2 }, activations, 0.2, iterations, null, (i, c) =>
			{
				if (lastCost < c)
					Console.WriteLine($"[{i}] Learning Rate might be too high");
				if (double.IsNaN(c))
					Console.WriteLine($"[{i}] NaN");
				lastCost = c;
				if (i % (iterations * .1) == 0)
					Console.WriteLine($"[{i}] : {c}");
			});
			Console.WriteLine("Done!");
			File.WriteAllText("netwok.json", parameters.ToJSON());
			var pY = DeepNeuralNetwork.Predict(parameters, trainX, activations, y => y);
			SaveAsImage(trainX, pY, "trainPredict.png");
			(ChiruMatrix testX, ChiruMatrix testY) = GenerateData(50000);
			var pTY = DeepNeuralNetwork.Predict(parameters, testX, activations, y => y);
			SaveAsImage(testX, pTY, "testPredict.png");
			SaveAsImage(testX, testY, "test.png");

			Console.ReadLine();
		}

		static void SaveAsImage(ChiruMatrix X, ChiruMatrix Y, string name)
		{
			SKBitmap bitmap = new SKBitmap(SIZE, SIZE);
			SKCanvas canvas = new SKCanvas(bitmap);
			DrawData(X, Y, canvas);
			Save(bitmap, name);
		}

		static void Save(SKBitmap bitmap, string name)
		{
			using (FileStream f = new FileStream(name, FileMode.Create))
			{
				SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100).SaveTo(f);
				f.Flush();
			}
		}

		public static Random rand = new Random();
		static (ChiruMatrix X, ChiruMatrix Y) GenerateData(int m)
		{
			double[,] X = new double[2, m], Y = new double[1, m];
			for (int n = 0; n < m; n++)
			{
				X[0, n] = rand.NextDouble();
				X[1, n] = rand.NextDouble();
				if (X[0, n] + X[1, n] > 1)
				{
					Y[0, n] = 1;

				}
				else
				{
					Y[0, n] = 0;
				}
			}
			return (X.AsMatrix(), Y.AsMatrix());
		}

		static void DrawData(ChiruMatrix X, ChiruMatrix Y, SKCanvas canvas)
		{
			canvas.Clear(new SKColor(255, 255, 255));
			SKPaint paintA = new SKPaint
			{
				Color = new SKColor(255, 0, 0)
			}, paintB = new SKPaint
			{
				Color = new SKColor(0, 0, 255)
			};
			for(int i = 0; i < X.Width; i++)
			{
				double x = X[0,i] * SIZE, y = X[1,i] * SIZE;
				if(Y[0,i] >= .5)
				{
					paintA.Color = new SKColor((byte)(255 * (1-((Y[0, i] - .5) * 2))), (byte)(255 * ((Y[0, i] - .5) * 2)), 0);
					canvas.DrawCircle((int)x, (int)y, 3, paintA);
				}
				else
				{
					paintB.Color = new SKColor((byte)(255 * ((Y[0, i]) * 2)), 0, (byte)(255 * (1 - ((Y[0, i]) * 2))));
					canvas.DrawCircle((int)x, (int)y, 3, paintB);
				}
			}
			canvas.Flush();
			canvas.Save();
		}
	}
}
