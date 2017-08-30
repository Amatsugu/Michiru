using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Michiru.Calculation;
using Newtonsoft.Json;

namespace Michiru.Calculation
{
    public struct ChiruMatrix
	{
		public double[,] Values { get; }
		public int Height { get; }
		public int Width { get; }
		public ChiruMatrix T => Transpose();

		private static Random _RAND = new Random();

		public ChiruMatrix(double[,] values)
		{
			Values = values;
			Height = values.GetLength(0);
			Width = values.GetLength(1);
		}

		public double this[int i, int j]
		{
			get
			{
				return Values[i, j];
			}
			set
			{
				Values[i, j] = value;
			}
		}

		public double[,] this[int j]
		{
			get
			{
				double[,] o = new double[Height,1];
				for (int i = 0; i < Height; i++)
					o[i,0] = Values[i, j];
				return o;
			}
			set
			{
				for (int i = 0; i < Values.GetLength(1); i++)
					Values[i, j] = value[i, 0];
			}
		}


		//Zeros
		public static ChiruMatrix Zero(int size) => Zero(size, size);
		public static ChiruMatrix Zero(int h, int w) => new ChiruMatrix(new double[h, w]);
		//Random
		public static ChiruMatrix Random(int size) => Random(size, size);
		public static ChiruMatrix Random(int h, int w)
		{
			var m = new ChiruMatrix(new double[h, w]);
			for (int i = 0; i <  h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					m[i, j] = _RAND.NextDouble();
				}
			}
			return m;
		}

		//Copy
		public ChiruMatrix Copy() => ((double[,])Values.Clone()).AsMatrix();
		//Add
		public ChiruMatrix Add(ChiruMatrix b) => new ChiruMatrix(ChiruMath.Add(Values, b.Values));
		public static ChiruMatrix operator +(ChiruMatrix a, ChiruMatrix b) => a.Add(b);
		public static ChiruMatrix operator +(ChiruMatrix a, double[,] b) => new ChiruMatrix(ChiruMath.Add(a.Values, b));
		public static ChiruMatrix operator +(double[,] a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.Add(a, b.Values));
		//Add Col
		public ChiruMatrix ColAdd(ChiruMatrix b) => new ChiruMatrix(ChiruMath.ColAdd(Values, b.Values));
		//Subtract
		public ChiruMatrix Subtract(ChiruMatrix b) => new ChiruMatrix(ChiruMath.Subtract(Values, b.Values));
		public static ChiruMatrix operator -(ChiruMatrix a, ChiruMatrix b) => a.Subtract(b);
		public static ChiruMatrix operator -(ChiruMatrix a, double[,] b) => new ChiruMatrix(ChiruMath.Subtract(a.Values, b));
		public static ChiruMatrix operator -(double[,] a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.Subtract(a, b.Values));
		//Multiply
		public ChiruMatrix Multiply(ChiruMatrix b) => new ChiruMatrix(ChiruMath.Multiply(Values, b.Values));
		public static ChiruMatrix operator *(ChiruMatrix a, ChiruMatrix b) => a.Multiply(b);
		public static ChiruMatrix operator *(ChiruMatrix a, double[,] b) => new ChiruMatrix(ChiruMath.Multiply(a.Values, b));
		public static ChiruMatrix operator *(double[,] a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.Multiply(a, b.Values));
		//Multiply Col
		public ChiruMatrix ColMultiply(ChiruMatrix b) => new ChiruMatrix(ChiruMath.ColMultiply(Values, b.Values));
		//Dot
		public ChiruMatrix Dot(ChiruMatrix b) => ChiruMath.Dot(Values, b.Values).AsMatrix();
		public static ChiruMatrix operator /(ChiruMatrix a, ChiruMatrix b) => ChiruMath.Dot(a.Values, b.Values).AsMatrix();
		public static ChiruMatrix operator /(ChiruMatrix a, double[,] b) => ChiruMath.Dot(a.Values, b).AsMatrix();
		public static ChiruMatrix operator /(double[,] a, ChiruMatrix b) => ChiruMath.Dot(a, b.Values).AsMatrix();
		//Scalar
		public static ChiruMatrix operator *(double a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.ScalarMultiply(a, b.Values));
		public static ChiruMatrix operator *(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarMultiply(b, a.Values));
		public static ChiruMatrix operator +(double a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.ScalarAdd(a, b.Values));
		public static ChiruMatrix operator +(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarAdd(b, a.Values));
		public static ChiruMatrix operator -(double a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.ScalarSubtract(-a, b.Values));
		public static ChiruMatrix operator -(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarSubtract(b, a.Values));
		public static ChiruMatrix operator /(double a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.ScalarDivide(a, b.Values));
		public static ChiruMatrix operator /(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarDivide(b, a.Values));

		public ChiruMatrix Map(Func<double, double> f)
		{
			var m = new double[Height, Width];
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					m[i,j] = f.Invoke(Values[i, j]);
				}
			}
			return m.AsMatrix();
		}

		public bool Any(Func<double, bool> condition)
		{
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					if (condition(Values[i, j]))
						return true;
				}
			}
			return false;
		}

		public bool HasNaN() => Any(double.IsNaN);

		public ChiruMatrix Activate(ActivationFunction activator) => activator.Activate(this);
		public ChiruMatrix DeActivate(ActivationFunction activator) => activator.DeActivate(this);

		public ChiruMatrix Transpose() => new ChiruMatrix(ChiruMath.Transpose(Values));

		public double Sum()
		{
			double o = 0;
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width; j++)
				{
					o += Values[i, j];
				}
			}
			return o;
		}

		public ChiruMatrix SumToMatrix(int axis)
		{
			ChiruMatrix sum;
			if(axis == 0)
			{
				sum = Zero(1, Width);
				for (int i = 0; i < Height; i++)
				{
					for (int j = 0; j < Width; j++)
					{
						sum[0, j] += this[i, j];
					}
				}
			}else if(axis == 1)
			{
				sum = Zero(Height, 1);
				for (int i = 0; i < Height; i++)
				{
					for (int j = 0; j < Width; j++)
					{
						sum[i, 0] += this[i, j];
					}
				}
			}else
			{
				throw new Exception("Invalid Axis");
			}

			return sum;
		}


		public double Mean() => Sum() / (Height * Width);

		public ChiruMatrix Abs() => ChiruMath.Abs(Values).AsMatrix();

		public string ToJSON() => JsonConvert.SerializeObject(Values);

		public static ChiruMatrix FromJSON(string JSON) => new ChiruMatrix(JsonConvert.DeserializeObject<double[,]>(JSON));

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < Height; i++)
			{
				sb.Append("[ ");
				for (int j = 0; j < Width; j++)
				{
					sb.Append($"{Values[i, j]}");
					if (j + 1 != Width)
						sb.Append(", \t");
				}
				sb.Append(" ]\n");
			}
			return sb.ToString();
		}

		public bool Equals(ChiruMatrix obj)
		{
			return ChiruMath.Equals(Values, obj.Values);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != typeof(ChiruMatrix))
				return false;

			return Equals((ChiruMatrix)obj);
		}

		public override int GetHashCode()
		{
			return Values.GetHashCode();
		}

	}
}
