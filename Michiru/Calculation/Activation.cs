using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Calculation
{
	public delegate double ActivationFunction(double x);
    class Activation
    {

		public static double S(double x) => 1 / (1 + Math.Pow(Math.E, -x));

		public static double DS(double x) => Math.Pow(Math.E, x)/Math.Pow(Math.Pow(Math.E, x) + 1, 2);

		public static double H(double x) => (1 - Math.Pow(Math.E, -2 * x) / (1 + Math.Pow(Math.E, 2 * x)));

		public static double DH(double x) => (Math.Pow(Math.E, -2 * x) * ((4 * Math.Pow(Math.E, 2 * x)) - (2 * Math.Pow(Math.E, 4 * x)) + 2));
    }
}
