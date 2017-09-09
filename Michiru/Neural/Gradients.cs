using Michiru.Calculation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Neural
{
	public class Gradients
	{
		public ChiruMatrix[] dA { get; set; }
		public ChiruMatrix[] dW { get; set; }
		public ChiruMatrix[] db { get; set; }

		public Gradients(int layers)
		{
			dA = new ChiruMatrix[layers];
			dW = new ChiruMatrix[layers];
			db = new ChiruMatrix[layers];
		}
	}
}
