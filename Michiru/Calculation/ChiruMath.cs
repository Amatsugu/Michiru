using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Michiru.Calculation
{
    public static class ChiruMath
    {
		public static bool PARALLEL = true;
		public static ChiruMatrix AsMatrix(this double[,] d) => new ChiruMatrix(d);

		public static bool Equals(this double[] a, double[] b)
		{
			if (a.Length != b.Length)
				return false;
			for (int i = 0; i < a.Length; i++)
				if (a[i] != b[i])
					return false;
			return true;
		}

		public static bool Equals(this double[,] a, double[,] b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			if (a.GetLength(0) != b.GetLength(0) && a.GetLength(1) != b.GetLength(1))
				return false;
			if(!PARALLEL)
			{
				for (int i = 0; i < h; i++)
					for (int j = 0; j < w; j++)
						if (a[i, j] != b[i, j])
							return false;
			}else
			{
				bool r = true;
				Parallel.For(0, h, (i, ls) =>
				{
					for (int j = 0; j < w; j++)
						if (a[i, j] != b[i, j])
						{
							r = false;
							ls.Stop();
							return;
						}
				});
				return r;
			}
			return true;
		}

		public static double[,] Multiply(double[,] a, double[,] b)
		{
			if (a.GetLength(1) != b.GetLength(0))
				throw new Exception("Cannot Multiply these Matricies");
			int h = a.GetLength(0), w = b.GetLength(1);
			double[,] o = new double[h, w];
			if(!PARALLEL)
			{
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
			}else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						for (int k = 0; k < a.GetLength(1); k++)
						{
							o[i, j] += a[i, k] * b[k, j];
						}
					}
				});
			}
			return o;
		}

		public static double[,] ColMultiply(double[,] a, double[,] b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			if (h != b.GetLength(0) && w != b.GetLength(1))
				throw new Exception("Cannot Multiply these Matricies");
			double[,] o = new double[h, w];
			if(!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] * b[i, j];
					}
				}
			}else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] * b[i, j];
					}
				});
			}
			return o;
		}

		public static double[,] Subtract(double[,] a, double[,] b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			if (h != b.GetLength(0) || w != b.GetLength(1))
				throw new Exception("Cannot subtract these Matricies");
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] - b[i, j];
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] - b[i, j];
					}
				});
			}
			return o;
		}

		public static double[,] Add(double[,] a, double[,] b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			if (h != b.GetLength(0) || w != b.GetLength(1))
				throw new Exception("Cannot Add these Matricies");
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] + b[i, j];
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] + b[i, j];
					}
				});
			}
			return o;
		}

		public static double[,] ColAdd(double[,] a, double[,] b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			if (h != b.GetLength(0))
				throw new Exception("Cannot Add these Matricies");
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] + b[i, 0];
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] + b[i, 0];
					}
				});
			}
			return o;
		}

		public static double[,] Dot(double[,] a, double[,] b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			if (h != b.GetLength(1))
				throw new Exception("Cannot DOT these Matricies");
			double[,] o = new double[1,b.GetLength(1)];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] * b[j, i];
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] * b[j, i];
					}
				});
			}
			return o;
		}

		public static double[,] Transpose(double[,] a)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			double[,] o = new double[w, h];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[j, i] = a[i, j];
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[j, i] = a[i, j];
					}
				});
			}
			return o;
		}

		public static double[,] ScalarMultiply(double[,] a, double b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] * b;
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] * b;
					}
				});
			}
			return o;
		}

		public static double[,] ScalarAdd(double[,] a, double b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] + b;
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] + b;
					}
				});
			}
			return o;
		}

		public static double[,] ScalarSubtract(double[,] a, double b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] - b;
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] - b;
					}
				});
			}
			return o;
		}

		public static double[,] ScalarDivide(double[,] a, double b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] / b;
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = a[i, j] / b;
					}
				});
			}
			return o;
		}
	}
}
