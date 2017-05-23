using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Calculation
{
    class MatrixMath
    {
		public static double[,] Multiply(double[,] a, double[,] b)
		{
			if (a.GetLength(1) != b.GetLength(0))
				throw new Exception("Cannot Multiply these Matricies");
			int h = a.GetLength(0), w = b.GetLength(1);
			double[,] o = new double[h, w];
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					for (int k = 0; k < a.GetLength(1); k++)
					{
						o[i, j] += a[i, k] * b[k, j];
					}
				}
			}
			return o;
		}

		public static double[,] Subtract(double[,] a, double[,] b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			if (h != b.GetLength(0) || w != b.GetLength(1))
				throw new Exception("Cannot subtract these Matricies");
			double[,] o = new double[h, w];
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					o[i, j] = a[i, j] - b[i, j];
				}
			}
			return o;
		}

		public static double Dot(double[,] a, double[,] b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			//if (h != b.GetLength(1))
			//	throw new Exception("Cannot DOT these Matricies");
			double o = 0;
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					o += a[i, j] * b[i, j];
				}
			}
			return o;
		}

		public static double[,] Transpose(double[,] a)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			double[,] o = new double[w, h];
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					o[j, i] = a[i, j];
				}
			}
			return o;
		}

		public static double[,] Scalar(double a, double[,] b)
		{
			int h = b.GetLength(0), w = b.GetLength(1);
			double[,] o = new double[h, w];
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					o[i, j] = b[i,j] * a;
				}
			}
			return o;
		}

		internal static double[,] Add(double[,] a, double[,] b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			//if (h != b.GetLength(0) || w != b.GetLength(1))
			//	throw new Exception("Cannot Add these Matricies");
			double[,] o = new double[h,w];
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					o[i,j] = a[i, j] + b[i, j];
				}
			}
			return o;
		}
	}
}
