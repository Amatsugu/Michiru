using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Michiru.Calculation
{
	public enum MatrixAxis
	{
		Vertical = 0,
		Horizontal = 1,
	}
	public static class ChiruMath
    {
		public static bool PARALLEL = true;
		public static ChiruMatrix AsMatrix(this double[,] d) => new ChiruMatrix(d);

		/// <summary>
		/// Compares two matrices for equality
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>result of comparison</returns>
		public static bool Equals(this double[,] a, double[,] b)
		{
			int h = a.GetLength(0), w = a.GetLength(1);
			if (h != b.GetLength(0) && w != b.GetLength(1))
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


		/// <summary>
		/// Matrix multiply
		/// </summary>
		/// <param name="left">Left matrix</param>
		/// <param name="right">Right matrix</param>
		/// <returns>Matrix product</returns>
		public static double[,] Multiply(double[,] left, double[,] right)
		{
#if DEBUG
			if (left.GetLength(1) != right.GetLength(0))
				throw new Exception("Cannot Multiply these Matricies");
#endif
			int h = left.GetLength(0), w = right.GetLength(1), w1 = left.GetLength(1);
			double[,] o = new double[h, w];
			if(!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						for (int k = 0; k < w1; k++)
						{
							o[i, j] += left[i, k] * right[k, j];
						}
					}
				}
			}else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						for (int k = 0; k < left.GetLength(1); k++)
						{
							o[i, j] += left[i, k] * right[k, j];
						}
					}
				});
			}
			return o;
		}

		/// <summary>
		/// Multiplies a matrix per element with another matrix
		/// </summary>
		/// <param name="left">Left matrix</param>
		/// <param name="right">Right matrix</param>
		/// <returns>Product matrix</returns>
		public static double[,] ElementMultiply(double[,] left, double[,] right)
		{
			int h = left.GetLength(0), w = right.GetLength(1);
#if DEBUG
			if (h != right.GetLength(0) && w != right.GetLength(1))
				throw new Exception("Cannot Multiply these Matricies");
#endif
			double[,] o = new double[h, w];
			if(!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] * right[i, j];
					}
				}
			}else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] * right[i, j];
					}
				});
			}
			return o;
		}

		/// <summary>
		/// Divides a matrix per element with another matrix, left / right
		/// </summary>
		/// <param name="left">Left matrix</param>
		/// <param name="right">Right matrix</param>
		/// <returns>Quotient matrix</returns>
		public static double[,] ElementDivide(double[,] left, double[,] right)
		{
			int h = left.GetLength(0), w = right.GetLength(1);
#if DEBUG
			if (h != right.GetLength(0) && w != right.GetLength(1))
				throw new Exception("Cannot Divivde these Matricies");
#endif
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] / right[i, j];
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] / right[i, j];
					}
				});
			}
			return o;
		}

		/// <summary>
		/// Subtracts right from left
		/// </summary>
		/// <param name="left">Left matrix</param>
		/// <param name="right">Right matrix</param>
		/// <returns>Difference</returns>
		public static double[,] Subtract(double[,] left, double[,] right)
		{
			int h = left.GetLength(0), w = left.GetLength(1);
#if DEBUG
			if (h != right.GetLength(0) || w != right.GetLength(1))
				throw new Exception("Cannot subtract these Matricies");
#endif
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] - right[i, j];
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] - right[i, j];
					}
				});
			}
			return o;
		}

		/// <summary>
		/// Adds two matrices together
		/// </summary>
		/// <param name="left">Left matrix</param>
		/// <param name="right">Right matrix</param>
		/// <returns>Sum</returns>
		public static double[,] Add(double[,] left, double[,] right)
		{
			int h = left.GetLength(0), w = left.GetLength(1);
#if DEBUG
			if (h != right.GetLength(0) || w != right.GetLength(1))
				throw new Exception("Cannot Add these Matricies");
#endif
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] + right[i, j];
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] + right[i, j];
					}
				});
			}
			return o;
		}

		/// <summary>
		/// Add a single column vector to every column of a matrix
		/// </summary>
		/// <param name="matrix">The matrix</param>
		/// <param name="colVector">The column vector</param>
		/// <returns>The sum after adding the column vector</returns>
		public static double[,] ColAdd(double[,] matrix, double[,] colVector)
		{
			int h = matrix.GetLength(0), w = matrix.GetLength(1);
#if DEBUG
			if (h != colVector.GetLength(0))
				throw new Exception("Cannot Add these Matricies");
#endif
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = matrix[i, j] + colVector[i, 0];
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = matrix[i, j] + colVector[i, 0];
					}
				});
			}
			return o;
		}

		/// <summary>
		/// Computes the dot prodcut between two matrices
		/// </summary>
		/// <param name="left">Left matrix</param>
		/// <param name="right">Right matrix</param>
		/// <returns></returns>
		public static double[,] Dot(double[,] left, double[,] right)
		{
			int h = left.GetLength(0), w = left.GetLength(1);
#if DEBUG
			if (left.GetLength(1) != right.GetLength(0))
				throw new Exception("Cannot DOT these Matricies");
#endif
			double[,] o = new double[1,right.GetLength(1)];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[0, i] += left[i, j] * right[j, i];
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[0, i] += left[i, j] * right[j, i];
					}
				});
			}
			return o;
		}

		/// <summary>
		/// Calculates the transpose of a matrix
		/// </summary>
		/// <param name="a">Matrix to transpose</param>
		/// <returns>Transposed Matrix</returns>
		public static double[,] Transpose(this double[,] a)
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

		/// <summary>
		/// Multiplies a matrix by a scalar
		/// </summary>
		/// <param name="left">The matrix</param>
		/// <param name="right">The scalar</param>
		/// <returns>The matrix product</returns>
		public static double[,] ScalarMultiply(double[,] left, double right)
		{
			int h = left.GetLength(0), w = left.GetLength(1);
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] * right;
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] * right;
					}
				});
			}
			return o;
		}

		/// <summary>
		/// Adds a scalar to a matrix
		/// </summary>
		/// <param name="left">The matrix</param>
		/// <param name="right">The scalar</param>
		/// <returns>The matrix sum</returns>
		public static double[,] ScalarAdd(double[,] left, double right)
		{
			int h = left.GetLength(0), w = left.GetLength(1);
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] + right;
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] + right;
					}
				});
			}
			return o;
		}

		/// <summary>
		/// Subtract a scalar from a matrix
		/// </summary>
		/// <param name="left">The matrix</param>
		/// <param name="right">The scalar</param>
		/// <returns>The matrix difference</returns>
		public static double[,] ScalarSubtract(double[,] left, double right)
		{
			int h = left.GetLength(0), w = left.GetLength(1);
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] - right;
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] - right;
					}
				});
			}
			return o;
		}

		/// <summary>
		/// Divide a matrix by a scalar
		/// </summary>
		/// <param name="left">The matrix</param>
		/// <param name="right">The scalar</param>
		/// <returns>The matrix qoutient</returns>
		public static double[,] ScalarDivide(double[,] left, double right)
		{
			int h = left.GetLength(0), w = left.GetLength(1);
			double[,] o = new double[h, w];
			if (!PARALLEL)
			{
				for (int i = 0; i < h; i++)
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] / right;
					}
				}
			}
			else
			{
				Parallel.For(0, h, i =>
				{
					for (int j = 0; j < w; j++)
					{
						o[i, j] = left[i, j] / right;
					}
				});
			}
			return o;
		}

		/// <summary>
		/// Sum all the elements of the matrix
		/// </summary>
		/// <param name="input">Matrix to sum</param>
		/// <returns>The sum</returns>
		public static double Sum(this double[,] input)
		{
			double sum = 0;
			var c = 0.0;
			for (int i = 0; i < input.GetLength(0); i++)
			{
				for (int j = 0; j < input.GetLength(1); j++)
				{
					var t = sum + input[i, j];
					if (Math.Abs(sum) >= Math.Abs(input[i, j]))
						c += (sum - t) + input[i, j];
					else
						c += (input[i, j] - t) + sum;
					sum = t;
				}
			}
			return sum + c;
		}
		
		/// <summary>
		/// Create a sumation of all of the elements of a matrix along a given axis
		/// </summary>
		/// <param name="input">Matrix to sum</param>
		/// <param name="axis">Axis to sum along</param>
		/// <returns>A 1-dimensional matrix of the sum</returns>
		public static double[,] SumAxis(this double[,] input, MatrixAxis axis)
		{
			double[,] o;
			int h = input.GetLength(0), w = input.GetLength(1);
			if (axis == MatrixAxis.Horizontal)
			{
				o = new double[h, 1];
				if (PARALLEL)
					Parallel.For(0, h, i => o[i, 0] = PairwiseSum(input, i, axis));
				else
					for (int i = 0; i < h; i++)
						o[i, 0] = PairwiseSum(input, i, axis);
			}else
			{
				o = new double[1, w];
				if (PARALLEL)
					Parallel.For(0, h, j => o[0, j] = PairwiseSum(input, j, axis));
				else
					for (int j = 0; j < w; j++)
						o[0, j] = PairwiseSum(input, j, axis);
			}
			return o;
		}

		const int N = 1000;

		/// <summary>
		/// Compute the sum along a specific axis on a specific row/column
		/// </summary>
		/// <param name="input">Input matrix</param>
		/// <param name="j">Row/Colum</param>
		/// <param name="axis">Sum Axis</param>
		/// <param name="x">Matrix subset horizontal offset</param>
		/// <param name="y">Matrix subset vertical offset</param>
		/// <param name="h">Matrix subset height</param>
		/// <param name="w">Matrix subset width</param>
		/// <returns></returns>
		private static double PairwiseSum(double[,] input, int j = 0, MatrixAxis axis = MatrixAxis.Horizontal, int x = 0, int y = 0, int h = -1, int w = -1)
		{
			if (h == -1 || w == -1)
			{
				h = input.GetLength(0);
				w = input.GetLength(1);
			}
			var l = (axis == MatrixAxis.Horizontal) ? w : h;
			if (l <= N)
			{
				double sum;
				if (axis == MatrixAxis.Horizontal)
				{
					sum = input[j, x];
					for (int i = x + 1; i < w; i++)
						sum += input[j, i];
				}else
				{
					sum = input[y, j];
					for (int i = y + 1; i < h; i++)
						sum += input[i, j];
				}
				return sum;
			}else
			{
				if (axis == MatrixAxis.Horizontal)
				{
					w = w / 2;
					return PairwiseSum(input, j, axis, x, y, h, w) + PairwiseSum(input, j, axis, x, y + w, h, w);
				}else
				{
					h = h / 2;
					return PairwiseSum(input, j, axis, x, y, h, w) + PairwiseSum(input, j, axis, x + h, y, h, w);
				}
			}
		}

		/// <summary>
		/// Gives the specified submatrix of a given matrix
		/// </summary>
		/// <param name="input">Matrix to take the submatrix from</param>
		/// <param name="i">Vertical offset of the submatrix</param>
		/// <param name="j">Horizontal offset of the submatrix</param>
		/// <param name="h">Height of the submatrix</param>
		/// <param name="w">Width of the submatrix</param>
		/// <returns>The submatrix</returns>
		public static double[,] SubMatrix(this double[,] input, int i, int j, int h, int w)
		{
			double[,] o = new double[h, w];
			for (int I = i; I < h; I++)
			{
				for (int J = j; J < w; J++)
				{
					o[I - i, J - j] = input[I, J];
				}
			}
			return o;
		}
	}
}
