using Michiru.Calculation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Neural
{
    public class DeepNeuralNetwork
    {
		public static Parameters InitializeParameters(int[] layerDims)
		{
			var parameters = new Parameters(layerDims.Length - 1);
			for (int l = 1; l < layerDims.Length; l++)
			{
				parameters.W[l - 1] = ChiruMatrix.Random(layerDims[l], layerDims[l - 1]);
				parameters.B[l - 1] = ChiruMatrix.Zeros(layerDims[l], 1);
			}
			return parameters;
		}

		public static (ChiruMatrix Z, LinearCache) LinearForward(ChiruMatrix A, ChiruMatrix W, ChiruMatrix b)
		{
			var Z = (W * A).ColAdd(b);
			var cache = new LinearCache(A, W, b);
			return (Z, cache);
		}

		public static (ChiruMatrix A, ActivationCache activationCache) LinearActivationForward(ChiruMatrix APrev, ChiruMatrix W, ChiruMatrix b, ActivationFunction activation)
		{
			var (Z, linearCache) = LinearForward(APrev, W, b);
			var A = Z.Activate(activation);
			return (A, new ActivationCache(linearCache, Z));
		}

		public static (ChiruMatrix AL, List<ActivationCache> caches) ModelForward(ChiruMatrix X, Parameters parameters)
		{
			List<ActivationCache> caches = new List<ActivationCache>();
			var A = X;
			int L = parameters.W.Length;
			for (int l = 0; l < L-1; l++)
			{
				var APrev = A;
				(ChiruMatrix AA, ActivationCache cache1) = LinearActivationForward(APrev, parameters.W[l], parameters.B[l], ActivationFunction.TanH);
				caches.Add(cache1);
				APrev = A = AA;
			}
			(ChiruMatrix AL, ActivationCache cache2) = LinearActivationForward(A, parameters.W[L-1], parameters.B[L-1], ActivationFunction.Sigmoid);
			caches.Add(cache2);
			return (AL, caches);
		}

		public static (ChiruMatrix dAPrev, ChiruMatrix dW, ChiruMatrix db) LinearBackward(ChiruMatrix dZ, LinearCache cache)
		{
			int m = cache.A.Width;
			var dW = (dZ * cache.A.T) / m;
			var db = dZ.SumAxis(1) / m;
			var dAPrev = cache.W.T * dZ;
			return (dAPrev, dW, db);
		}

		public static (ChiruMatrix dAPrev, ChiruMatrix dW, ChiruMatrix db) LinearActivationBackward(ChiruMatrix dA, ActivationCache cache, ActivationFunction activation)
		{
			var dZ = dA.ElementMultiply(cache.Z.DeActivate(activation));
			return LinearBackward(dZ, cache.Linear);
		}

		public static Gradients ModelBackward(ChiruMatrix AL, ChiruMatrix Y, List<ActivationCache> caches)
		{
			var L = caches.Count;
			var m = AL.Width;
			var grads = new Gradients(L);
			var dAL = -(Y.Divide(AL) - (1 - Y).Divide(1 - AL));

			(grads.dA[L-1], grads.dW[L - 1], grads.db[L - 1]) = LinearActivationBackward(dAL, caches[L - 1], ActivationFunction.Sigmoid);

			for (int l = L-2; l >= 0; l--)
			{
				var curCache = caches[l];
				(grads.dA[l], grads.dW[l], grads.db[l]) = LinearActivationBackward(grads.dA[l + 1], curCache, ActivationFunction.TanH);
			}
			return grads;
		}

		public static Parameters UpdateParameters(Parameters parameters, Gradients grads, double learningRate)
		{
			for(int l = 0; l < parameters.B.Length; l++)
			{
				parameters.W[l] -= learningRate * grads.dW[l];
				parameters.B[l] -= learningRate * grads.db[l];
			}
			return parameters;
		}

		public static Parameters Model(ChiruMatrix X, ChiruMatrix Y, int[] layers, double learningRate = 0.0075, int iterations = 3000, bool printCost = false, Action<int, double> statusReporter = null, Parameters parameters = null, Func<bool> cancel = null)
		{
			int percentile = iterations / 10;
			percentile = percentile == 0 ? 1 : percentile;
			var layerDims = new int[layers.Length + 2];
			layerDims[0] = X.Height;
			layerDims[layerDims.Length - 1] = Y.Height;
			for (int i = 1; i < layers.Length + 1; i++)
			{
				layerDims[i] = layers[i - 1];
			}
			if(parameters == null)
				parameters = InitializeParameters(layerDims);
			DateTime tic, startTime = tic = DateTime.Now;
			for (int i = 0; i < iterations; i++)
			{
				var f = ModelForward(X, parameters);
				var cost = ComputeCost(f.AL, Y);
				statusReporter?.Invoke(i, cost);
				var b = ModelBackward(f.AL, Y, f.caches);
				parameters = UpdateParameters(parameters, b, learningRate);
				if (printCost && (i + 1) % percentile == 0)
				{
					Console.WriteLine($"[{i + 1}/{iterations}] Cost: {cost} \t {(DateTime.Now - startTime).TotalSeconds}s \t+{(DateTime.Now - tic).TotalSeconds}s");
					tic = DateTime.Now;
				}
				if (cancel != null && cancel())
					break;
			}
			return parameters;
		}

		public static ChiruMatrix Predict(Parameters parameters, ChiruMatrix X) => ModelForward(X, parameters).AL.Map(x => x > .5 ? 1 : 0);

		public static double ComputeCost(ChiruMatrix AL, ChiruMatrix Y)
		{
			var cost = (AL.Map(x => x == 0 ? 0 : Math.Log(x)).ElementMultiply(Y)) + ((1 - Y).ElementMultiply((1 - AL).Map(x => x == 0 ? 0 : Math.Log(x))));
			return cost.Sum() / -Y.Width;
		}
	}
}
