using Michiru.Calculation;
using Michiru.Neural;
using SkiaSharp;
using System;
using System.IO;

namespace ClassificationApp
{
	class Program
	{
		public static int SIZE = 1000;

		static void Main(string[] args)
		{
			(ChiruMatrix trainX, ChiruMatrix trainY) = GenerateData(10000);
			SKBitmap bitmap = new SKBitmap(SIZE, SIZE);
			SKCanvas canvas = new SKCanvas(bitmap);
			DrawData(trainX, trainY, canvas);
			Save(bitmap, "train.png");
			var activations = new ActivationFunction[]
			{
				ActivationFunction.Sigmoid,
				ActivationFunction.Sigmoid,
				ActivationFunction.Sigmoid,
				ActivationFunction.Sigmoid
			};
			ChiruMath.PARALLEL = false;
			var parameters = DeepNeuralNetwork.Model(trainX, trainY, new int[] { 5, 2 }, activations, 0.002, 50000, null, (i, c) =>
			{
				if(i % (10000 * .1) == 0)
					Console.WriteLine($"[{i}] : {c}");
			});
			var pY = DeepNeuralNetwork.Predict(parameters, trainX, activations);
			bitmap = new SKBitmap(SIZE, SIZE);
			canvas = new SKCanvas(bitmap);
			DrawData(trainX, pY, canvas);
			Save(bitmap, "predict.png");
			Console.ReadLine();
		}

		static void Save(SKBitmap bitmap, string name)
		{
			using (FileStream f = new FileStream(name, FileMode.Create))
			{
				SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100).SaveTo(f);
				f.Flush();
			}
		}

		static (ChiruMatrix X, ChiruMatrix Y) GenerateData(int m)
		{
			Random rand = new Random(2);
			double[,] X = new double[2, m], Y = new double[1, m];
			for (int n = 0; n < 1000; n++)
			{
				X[0, n] = rand.NextDouble();
				X[1, n] = rand.NextDouble();
				if (X[0, n] >= 0.5)
				{
					if (X[1, n] >= 0.5)
						Y[0, n] = 1;
					else
						Y[0, n] = 0;
				}
				else
				{
					if (X[1, n] >= 0.5)
						Y[0, n] = 0;
					else
						Y[0, n] = 1;
				}
			}

			return (X.AsMatrix(), Y.AsMatrix());
		}

		static void DrawData(ChiruMatrix X, ChiruMatrix Y, SKCanvas canvas)
		{
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
				if(Y[0,i] == 1.0)
					canvas.DrawCircle((int)x, (int)y, 3, paintA);
				else
					canvas.DrawCircle((int)x, (int)y, 3, paintB);
			}
			canvas.Flush();
			canvas.Save();
		}
	}
}
