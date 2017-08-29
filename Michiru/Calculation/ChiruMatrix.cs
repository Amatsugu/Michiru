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
		public static ChiruMatrix Zero(int w, int h) => new ChiruMatrix(new double[w, h]);
		//Add
		public ChiruMatrix Add(ChiruMatrix b) => new ChiruMatrix(ChiruMath.Add(Values, b.Values));
		public static ChiruMatrix operator +(ChiruMatrix a, ChiruMatrix b) => a.Add(b);
		public static ChiruMatrix operator +(ChiruMatrix a, double[,] b) => new ChiruMatrix(ChiruMath.Add(a.Values, b));
		public static ChiruMatrix operator +(double[,] a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.Add(a, b.Values));
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
		//Dot
		public double[] Dot(ChiruMatrix b) => ChiruMath.Dot(Values, b.Values);
		public static double[] operator /(ChiruMatrix a, ChiruMatrix b) => ChiruMath.Dot(a.Values, b.Values);
		public static double[] operator /(ChiruMatrix a, double[,] b) => ChiruMath.Dot(a.Values, b);
		public static double[] operator /(double[,] a, ChiruMatrix b) => ChiruMath.Dot(a, b.Values);
		//Scalar
		public static ChiruMatrix operator *(double a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.ScalarMultiply(a, b.Values));
		public static ChiruMatrix operator *(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarMultiply(b, a.Values));
		public static ChiruMatrix operator +(double a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.ScalarAdd(a, b.Values));
		public static ChiruMatrix operator +(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarAdd(b, a.Values));
		public static ChiruMatrix operator -(double a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.ScalarSubtract(a, b.Values));
		public static ChiruMatrix operator -(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarSubtract(b, a.Values));
		public static ChiruMatrix operator /(double a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.ScalarDivide(a, b.Values));
		public static ChiruMatrix operator /(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarDivide(b, a.Values));


		public ChiruMatrix Activate(ActivationFunction activator) => new ChiruMatrix(Activation.Activate(Values, activator));

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
						sb.Append("\t");
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
