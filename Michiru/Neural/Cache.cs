using Michiru.Calculation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Neural
{
    public struct LinearCache
    {
		public ChiruMatrix A { get; set; }
		public ChiruMatrix W { get; set; }
		public ChiruMatrix B { get; set; }

		public LinearCache(ChiruMatrix A, ChiruMatrix W, ChiruMatrix b)
		{
			this.A = A;
			this.W = W;
			this.B = b;
		}
	}


	public struct ActivationCache
	{
		public LinearCache Linear { get; set; }
		public ChiruMatrix Z { get; set; }

		public ActivationCache(LinearCache linearCache, ChiruMatrix activationCache)
		{
			Linear = linearCache;
			Z = activationCache;
		}
	}
}
