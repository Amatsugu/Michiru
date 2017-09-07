using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Calculation
{
    public abstract class ActivationFunction
    {
		public static SigmoidActivation Sigmoid
		{
			get
			{
				if (_sigmoid != null)
					return _sigmoid;
				else
					return _sigmoid = new SigmoidActivation();
			}
		}

		public static TanHActivation TanH
		{
			get
			{
				if (_tanH != null)
					return _tanH;
				else
					return _tanH = new TanHActivation();
			}
		}

		public abstract double Activate(double z);
		public abstract double DeActivate(double z);

		public virtual ChiruMatrix Activate(ChiruMatrix z) => z.Map(Activate);

		public virtual ChiruMatrix DeActivate(ChiruMatrix z) => z.Map(DeActivate);

		private static SigmoidActivation _sigmoid;
		private static TanHActivation _tanH;
    }

	public class SigmoidActivation : ActivationFunction
	{
		internal SigmoidActivation()
		{
		}

		public override double Activate(double x) => /*Math.Pow(Math.E, x) / (Math.Pow(Math.E, x) + 1);*/1 / (1 + Math.Pow(Math.E, -x));
		public override double DeActivate(double x) => Math.Pow(Math.E, x) / Math.Pow(Math.Pow(Math.E, x) + 1, 2);

	}

	public class TanHActivation : ActivationFunction
	{
		internal TanHActivation()
		{
		}

		public override double Activate(double x) => Math.Tanh(x);// (1 - Math.Pow(Math.E, -2 * x) / (1 + Math.Pow(Math.E, 2 * x)));
		public override double DeActivate(double x) => 0;//(1 - Math.Pow(Math.Tanh(x), 2));

		public override ChiruMatrix DeActivate(ChiruMatrix z) => 1 - z.Map(x => Math.Pow(x, 2));

	}
}
