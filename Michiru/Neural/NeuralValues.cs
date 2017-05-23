using Michiru.Calculation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Michiru.Neural
{
    struct NeuralValues
	{
		public double[,] Values { get; }
		public int Length { get; }
		public int Width { get; }


		public NeuralValues(double[,] values)
		{
			Values = values;
			Length = values.GetLength(0);
			Width = values.GetLength(1);
		}

		public double this[int i, int j] => Values[i,j];

		public double[] this[int i]
		{
			get
			{
				double[] o = new double[Values.GetLength(1)];
				for (int j = 0; j < o.Length; j++)
					o[j] = Values[i, j];
				return o;
			}
			set
			{
				for (int j = 0; j < Values.GetLength(1); j++)
					Values[i, j] = value[j];
			}
		}


		public NeuralValues Multiply(NeuralValues b) => new NeuralValues(MatrixMath.Multiply(Values, b.Values));

		public NeuralValues Subtract(NeuralValues b) => new NeuralValues(MatrixMath.Subtract(Values, b.Values));

		public static NeuralValues operator -(NeuralValues a, NeuralValues b) => a.Subtract(b);
		public static NeuralValues operator -(NeuralValues a, double[,] b) => new NeuralValues(MatrixMath.Subtract(a.Values, b));
		public static NeuralValues operator -(double[,] a, NeuralValues b) => new NeuralValues(MatrixMath.Subtract(a, b.Values));

		public static NeuralValues operator *(NeuralValues a, NeuralValues b) => a.Multiply(b);
		public static NeuralValues operator *(NeuralValues a, double[,] b) => new NeuralValues(MatrixMath.Multiply(a.Values, b));
		public static NeuralValues operator *(double[,] a, NeuralValues b) => new NeuralValues(MatrixMath.Multiply(a, b.Values));

		public static NeuralValues operator *(double a, NeuralValues b) => new NeuralValues(MatrixMath.Scalar(a, b.Values));
		public static NeuralValues operator *(NeuralValues a, double b) => new NeuralValues(MatrixMath.Scalar(b, a.Values));

		public static double operator /(NeuralValues a, NeuralValues b) => MatrixMath.Dot(a.Values, b.Values);
		public static double operator /(NeuralValues a, double[,] b) => MatrixMath.Dot(a.Values, b);
		public static double operator /(double[,] a, NeuralValues b) => MatrixMath.Dot(a, b.Values);

		public NeuralValues Activate(ActivationFunction activator) => new NeuralValues(Activation.Activate(Values, activator));

		public NeuralValues Transpose() => new NeuralValues(MatrixMath.Transpose(Values));

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("{\n");
			for (int i = 0; i < Length; i++)
			{
				sb.Append($"\t[{i}]{{{string.Join(", ", this[i])}}}\n");
			}
			sb.Append("}");
			return sb.ToString();
		}

		public bool Equals(NeuralValues obj)
		{
			return Values.Equals(obj.Values);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != typeof(NeuralValues))
				return false;

			return Equals((NeuralValues)obj);
		}

		public override int GetHashCode()
		{
			return Values.GetHashCode();
		}
	}
}
