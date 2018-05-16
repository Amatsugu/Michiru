using Michiru.Calculation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Neural
{
    public class DeepNeuralNetwork
    {
		/// <summary>
		/// Initailizes the parameters for the network
		/// </summary>
		/// <param name="layerDims">the layers of the network</param>
		/// <returns></returns>
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

		/// <summary>
		/// Performs the linear part of forward propagation
		/// </summary>
		/// <param name="A">Inputs to this layer</param>
		/// <param name="W">Weights for this layer</param>
		/// <param name="b">Biaes for this layer</param>
		/// <returns></returns>
		public static (ChiruMatrix Z, LinearCache) LinearForward(ChiruMatrix A, ChiruMatrix W, ChiruMatrix b)
		{
			var Z = (W * A).ColAdd(b);
			var cache = new LinearCache(A, W, b);
			return (Z, cache);
		}

		/// <summary>
		/// Performs the activations on the linear part of forward propagation
		/// </summary>
		/// <param name="APrev">The activation of the previous layer</param>
		/// <param name="W">Weights for this layer</param>
		/// <param name="b">Biaes for this layer</param>
		/// <param name="activation">Activation fuction for this layer</param>
		/// <returns></returns>
		public static (ChiruMatrix A, ActivationCache activationCache) LinearActivationForward(ChiruMatrix APrev, ChiruMatrix W, ChiruMatrix b, ActivationFunction activation)
		{
			var (Z, linearCache) = LinearForward(APrev, W, b);
			var A = Z.Activate(activation);
			return (A, new ActivationCache(linearCache, Z));
		}

		/// <summary>
		/// Performs forward propagation
		/// </summary>
		/// <param name="X"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static (ChiruMatrix AL, List<ActivationCache> caches) ModelForward(ChiruMatrix X, Parameters parameters, ActivationFunction[] activations)
		{
			List<ActivationCache> caches = new List<ActivationCache>();
			var A = X;
			int L = parameters.W.Length;
			for (int l = 0; l < L-1; l++)
			{
				var APrev = A;
				(ChiruMatrix AA, ActivationCache cache1) = LinearActivationForward(APrev, parameters.W[l], parameters.B[l], activations[l]);
				caches.Add(cache1);
				APrev = A = AA;
			}
			(ChiruMatrix AL, ActivationCache cache2) = LinearActivationForward(A, parameters.W[L-1], parameters.B[L-1], activations[L-1]);
			caches.Add(cache2);
			return (AL, caches);
		}

		/// <summary>
		/// Performs the linear part of back propagation
		/// </summary>
		/// <param name="dZ">The deactivation of the previous layer's output</param>
		/// <param name="cache">The cache for this layer</param>
		/// <returns></returns>
		public static (ChiruMatrix dAPrev, ChiruMatrix dW, ChiruMatrix db) LinearBackward(ChiruMatrix dZ, LinearCache cache)
		{
			int m = cache.A.Width;
			var dW = (dZ * cache.A.T) / m;
			var db = dZ.SumAxis(1) / m;
			var dAPrev = cache.W.T * dZ;
			return (dAPrev, dW, db);
		}

		/// <summary>
		/// Performs back propagation
		/// </summary>
		/// <param name="dA">The activation of the previous layer's output</param>
		/// <param name="cache">Acication cache created for this layer</param>
		/// <param name="activation">Activation function for this layer</param>
		/// <returns></returns>
		public static (ChiruMatrix dAPrev, ChiruMatrix dW, ChiruMatrix db) LinearActivationBackward(ChiruMatrix dA, ActivationCache cache, ActivationFunction activation)
		{
			var dZ = dA.ElementMultiply(cache.Z.DeActivate(activation));
			return LinearBackward(dZ, cache.Linear);
		}

		/// <summary>
		/// Caclulates the gradients required for the network to learn
		/// </summary>
		/// <param name="AL">Output of the final layer during training</param>
		/// <param name="Y">Expected output</param>
		/// <param name="caches">Chaches created during training</param>
		/// <returns></returns>
		public static Gradients ModelBackward(ChiruMatrix AL, ChiruMatrix Y, List<ActivationCache> caches, ActivationFunction[] activations)
		{
			var L = caches.Count;
			var m = AL.Width;
			var grads = new Gradients(L);
			var dAL = -(Y.Divide(AL) - (1 - Y).Divide(1 - AL));

			(grads.dA[L-1], grads.dW[L - 1], grads.db[L - 1]) = LinearActivationBackward(dAL, caches[L - 1], activations[L-1]);

			for (int l = L-2; l >= 0; l--)
			{
				var curCache = caches[l];
				(grads.dA[l], grads.dW[l], grads.db[l]) = LinearActivationBackward(grads.dA[l + 1], curCache, activations[l]);
			}
			return grads;
		}

		/// <summary>
		/// Update the parameters based on the gradients calculated by ModelBackward()
		/// </summary>
		/// <param name="parameters">Parameters to be updated</param>
		/// <param name="grads">Gradients to learn from</param>
		/// <param name="learningRate">Learning Rate</param>
		/// <returns></returns>
		public static Parameters UpdateParameters(Parameters parameters, Gradients grads, double learningRate)
		{
			for(int l = 0; l < parameters.B.Length; l++)
			{
				parameters.W[l] -= learningRate * grads.dW[l];
				parameters.B[l] -= learningRate * grads.db[l];
			}
			return parameters;
		}

		public static Parameters Model(ChiruMatrix X, ChiruMatrix Y, int[] layers, ActivationFunction activationFunction, double learningRate = 0.0075, int iterations = 3000, Parameters parameters = null, Action<int, double> statusReporter = null, Func<bool> cancel = null)
		{
			var activations = new ActivationFunction[layers.Length + 2];
			for (int i = 0; i < activations.Length; i++)
			{
				activations[i] = activationFunction;
			}
			return Model(X, Y, layers, activations, learningRate, iterations, parameters, statusReporter, cancel);
		}

		/// <summary>
		/// Create a Network Model and train it
		/// </summary>
		/// <param name="X">The input data set</param>
		/// <param name="Y">The expected outputs for the input data set</param>
		/// <param name="layers">An array that represents the number and depth of the layers</param>
		/// <param name="learningRate">Learning Rate of the network</param>
		/// <param name="iterations">Number of iterations to train the network</param>
		/// <param name="statusReporter">A callback that will be used to report the training status of the network</param>
		/// <param name="parameters">Parameters for the network, from prior training</param>
		/// <param name="cancel">A callback that will be used to determine if the training should be canceld after the current iteration</param>
		/// <returns>The resulting parameters after training</returns>
		public static Parameters Model(ChiruMatrix X, ChiruMatrix Y, int[] layers, ActivationFunction[] activationFunctions, double learningRate = 0.0075, int iterations = 3000, Parameters parameters = null, Action < int, double> statusReporter = null, Func<bool> cancel = null)
		{
			int percentile = iterations / 10;
			percentile = percentile == 0 ? 1 : percentile;
			if (activationFunctions.Length != layers.Length + 2)
				throw new Exception("Mismatch between number of activtion functions and number of layers");
			var layerDims = new int[layers.Length + 2];
			layerDims[0] = X.Height;
			layerDims[layerDims.Length - 1] = Y.Height;
			for (int i = 1; i < layers.Length + 1; i++)
			{
				layerDims[i] = layers[i - 1];
			}
			if(parameters == null)
				parameters = InitializeParameters(layerDims);
			for (int i = 0; i < iterations; i++)
			{
				var f = ModelForward(X, parameters, activationFunctions);
				var cost = ComputeCost(f.AL, Y);
				statusReporter?.Invoke(i, cost);
				var b = ModelBackward(f.AL, Y, f.caches, activationFunctions);
				parameters = UpdateParameters(parameters, b, learningRate);
				/*if (printCost && (i + 1) % percentile == 0)
				{
					Console.WriteLine($"[{i + 1}/{iterations}] Cost: {cost} \t {(DateTime.Now - startTime).TotalSeconds}s \t+{(DateTime.Now - tic).TotalSeconds}s");
					tic = DateTime.Now;
				}*/
				if (cancel != null && cancel())
					break;
			}
			return parameters;
		}

		/// <summary>
		/// Make predictions based on a network model
		/// </summary>
		/// <param name="parameters">The parameters from a network model</param>
		/// <param name="X">Data set to make predictions based on</param>
		/// <param name="activationFunction">Activation function to use for all layers</param>
		/// <returns>Matrix of the resulting predictions</returns>
		public static ChiruMatrix Predict(Parameters parameters, ChiruMatrix X, ActivationFunction activationFunction)
		{
			var activations = new ActivationFunction[parameters.B.Length + 1];
			for (int i = 0; i < activations.Length; i++)
			{
				activations[i] = activationFunction;
			}
			return ModelForward(X, parameters, activations).AL.Map(x => x > .5 ? 1 : 0);
		}


		/// <summary>
		/// Make predictions based on a network model
		/// </summary>
		/// <param name="parameters">The parameters from a network model</param>
		/// <param name="X">Data set to make predictions based on</param>
		/// <param name="activations">Activation Functions to use for each layer</param>
		/// <returns>Matrix of the resulting predictions</returns>
		public static ChiruMatrix Predict(Parameters parameters, ChiruMatrix X, ActivationFunction[] activations) => ModelForward(X, parameters, activations).AL.Map(x => x > .5 ? 1 : 0);
		public static ChiruMatrix Predict(Parameters parameters, ChiruMatrix X, ActivationFunction[] activations, Func<double, double> mapFunction) => ModelForward(X, parameters, activations).AL.Map(mapFunction);

		public static double ComputeCost(ChiruMatrix AL, ChiruMatrix Y)
		{
			var cost = (AL.Map(x => x == 0 ? 0 : Math.Log(x)).ElementMultiply(Y)) + ((1 - Y).ElementMultiply((1 - AL).Map(x => x == 0 ? 0 : Math.Log(x))));
			return cost.Sum() / -Y.Width;
		}
	}
}
