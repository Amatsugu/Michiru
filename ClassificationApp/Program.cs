using Michiru.Calculation;
using Michiru.Neural;
using SkiaSharp;
using System;
using System.IO;
using ZeroFormatter;
using Newtonsoft.Json;
using Michiru.Utils;

namespace ClassificationApp
{
	class Program
	{
		public const int SIZE = 1000;

		static void Main(string[] args)
		{
			var trainX = IDXReader.GetDataMatrix(@"Q:\ChiruData\train-images.idx3-ubyte", 28, 60000);
			var trainY = IDXReader.GetLabelMatrix(@"Q:\ChiruData\train-labels.idx1-ubyte", 10, 60000);
			//var testX = IDXReader.GetDataMatrix(@"Q:\ChiruData\t10k-images.idx3-ubyte", 28, 10000);
			//var testY = IDXReader.GetLabelMatrix(@"Q:\ChiruData\t10k-labels.idx1-ubyte", 10, 10000);

			var iterations = 1000;
			//ChiruMath.PARALLEL = true;
			Console.WriteLine("Training");
			var (W1, b1, W2, b2) = NeuralNetwork.Model(trainX, trainY, 16, iterations, 1.2, true);
			/*var parameters = DeepNeuralNetwork.Model(trainX, trainY, new int[] { 32, 16, 10 }, ActivationFunction.Sigmoid, .02, iterations, null, (i, c) =>
			{
				Console.WriteLine($"[{i}]: {c}");
			});*/

			//var parameters = Parameters.FromJSON(File.ReadAllText(@"Q:\ChiruData\params.json"));
			//File.WriteAllText(@"Q:\ChiruData\params.json", parameters.ToJSON());
			var pY = NeuralNetwork.Predict(W1, b1, W2, b2, trainX);
			//var pY = DeepNeuralNetwork.Predict(parameters, trainX, ActivationFunction.Sigmoid);
			Console.WriteLine(pY.Any(x => x == 1));
			Console.WriteLine(pY.ErrorWith(trainY));

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
				var x = X[0, n] = rand.NextDouble();
				var y = X[1, n] = rand.NextDouble();
				if (y >= x*x*x)
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
