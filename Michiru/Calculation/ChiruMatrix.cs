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
		[JsonIgnore]
		public readonly int Height => Values.GetLength(0);
		[JsonIgnore]
		public readonly int Width => Values.GetLength(1);
		[JsonIgnore]
		public readonly ChiruMatrix T => Transpose();
        [JsonIgnore]
        public readonly ChiruMatrix Shape => new double[,] { { Height, Width }}.AsMatrix();

		[JsonIgnore]
		private static Random _RAND = new();
		

		public ChiruMatrix(double[,] values) => Values = values;

		public readonly ref double this[int i, int j]
		{
			get => ref Values[i, j];
			//set => Values[i, j] = value;
		}

		public readonly ChiruMatrix this[int j]
		{
			get
			{
				double[,] o = new double[Height,1];
				for (int i = 0; i < Height; i++)
					o[i, 0] = Values[i, j];
				return o.AsMatrix();
			}
			set
			{
				for (int i = 0; i < Values.GetLength(1); i++)
					Values[i, j] = value[i, 0];
			}
		}


		//Zeros
		/// <summary>
		/// Generates a new square matrix with initial values of zero
		/// </summary>
		/// <param name="size">Size of the matrix</param>
		/// <returns></returns>
		public static ChiruMatrix Zeros(int size) => Zeros(size, size);
		/// <summary>
		/// Generates a new matrix with inital values of zero
		/// </summary>
		/// <param name="h">Height</param>
		/// <param name="w">Width</param>
		/// <returns></returns>
		public static ChiruMatrix Zeros(int h, int w) => new(new double[h, w]);
		//Random
		/// <summary>
		/// Generates a new square matrix with random initial values
		/// </summary>
		/// <param name="size">Size of the matrix</param>
		/// <returns></returns>
		public static ChiruMatrix Random(int size) => Random(size, size);
		/// <summary>
		/// Generates a new matrix with random initial values
		/// </summary>
		/// <param name="h">Height</param>
		/// <param name="w">Width</param>
		/// <returns></returns>
		public static ChiruMatrix Random(int h, int w)
		{
			var m = new double[h, w];
			for (int i = 0; i <  h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					m[i, j] = _RAND.NextDouble();
				}
			}
			return m.AsMatrix();
		}

		//Copy
		/// <summary>
		/// Creates a copy of this matrix
		/// </summary>
		/// <returns></returns>
		public readonly ChiruMatrix Copy() => ((double[,])Values.Clone()).AsMatrix();
		//Add
		private readonly ChiruMatrix Add(ChiruMatrix b) => new(ChiruMath.Add(Values, b.Values));
		public static ChiruMatrix operator +(ChiruMatrix a, ChiruMatrix b) => ChiruMath.Add(a.Values, b.Values).AsMatrix();
		public static ChiruMatrix operator +(ChiruMatrix a, double[,] b) => new(ChiruMath.Add(a.Values, b));
		public static ChiruMatrix operator +(double[,] a, ChiruMatrix b) => new(ChiruMath.Add(a, b.Values));
		//Add Col
		public readonly ChiruMatrix ColAdd(ChiruMatrix b) => new(ChiruMath.ColAdd(Values, b.Values));
		//Subtract
		private readonly ChiruMatrix Subtract(ChiruMatrix b) => new(ChiruMath.Subtract(Values, b.Values));
		public static ChiruMatrix operator -(ChiruMatrix a, ChiruMatrix b) => ChiruMath.Subtract(a.Values, b.Values).AsMatrix();
		public static ChiruMatrix operator -(ChiruMatrix a, double[,] b) => new(ChiruMath.Subtract(a.Values, b));
		public static ChiruMatrix operator -(double[,] a, ChiruMatrix b) => new(ChiruMath.Subtract(a, b.Values));
		//Multiply
		private readonly ChiruMatrix Multiply(ChiruMatrix b) => new(ChiruMath.Multiply(Values, b.Values));
		public static ChiruMatrix operator *(ChiruMatrix a, ChiruMatrix b) => ChiruMath.Multiply(a.Values, b.Values).AsMatrix();
		public static ChiruMatrix operator *(ChiruMatrix a, double[,] b) => new(ChiruMath.Multiply(a.Values, b));
		public static ChiruMatrix operator *(double[,] a, ChiruMatrix b) => new(ChiruMath.Multiply(a, b.Values));
		//Multiply Col
		/// <summary>
		/// Multiplies the given column matrix with each column of this matrix elementwise
		/// </summary>
		/// <param name="b">Column Matrix</param>
		/// <returns></returns>
		public readonly ChiruMatrix ElementMultiply(ChiruMatrix b) => new(ChiruMath.ElementMultiply(Values, b.Values));
		//Dot
		private readonly ChiruMatrix Dot(ChiruMatrix b) => ChiruMath.Dot(Values, b.Values).AsMatrix();
		public static ChiruMatrix operator /(ChiruMatrix a, ChiruMatrix b) => ChiruMath.Dot(a.Values, b.Values).AsMatrix();
		public static ChiruMatrix operator /(ChiruMatrix a, double[,] b) => ChiruMath.Dot(a.Values, b).AsMatrix();
		public static ChiruMatrix operator /(double[,] a, ChiruMatrix b) => ChiruMath.Dot(a, b.Values).AsMatrix();
		//Scalar
		public static ChiruMatrix operator *(double a, ChiruMatrix b) => b * a;
		public static ChiruMatrix operator *(ChiruMatrix a, double b) => new(ChiruMath.ScalarMultiply(a.Values, b));
		public static ChiruMatrix operator +(double a, ChiruMatrix b) => b + a;
		public static ChiruMatrix operator +(ChiruMatrix a, double b) => new(ChiruMath.ScalarAdd(a.Values, b));
		public static ChiruMatrix operator -(ChiruMatrix a) => a * -1;
		public static ChiruMatrix operator -(double a, ChiruMatrix b) => -b + a;
		public static ChiruMatrix operator -(ChiruMatrix a, double b) => new(ChiruMath.ScalarSubtract(a.Values, b));
		public static ChiruMatrix operator /(ChiruMatrix a, double b) => new(ChiruMath.ScalarDivide(a.Values, b));
		public static ChiruMatrix operator /(double a, ChiruMatrix b) => (1 / a) * b;

		public readonly ChiruMatrix Divide(ChiruMatrix b) => ChiruMath.ElementDivide(Values, b.Values).AsMatrix();

		/// <summary>
		/// Aplies the given function to every element of this matrix, the result is placed in a new matrix
		/// </summary>
		/// <param name="f">Function to apply</param>
		/// <returns>A new matrix containing the values after the function has been applied</returns>
		public readonly ChiruMatrix Map(Func<double, double> f)
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

		/// <summary>
		/// Tests each element in the matrix with the given condition, returns true if at least one element meets the condition
		/// </summary>
		/// <param name="condition">Condition to be tested</param>
		/// <returns></returns>
		public readonly bool Any(Func<double, bool> condition)
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

		public readonly bool IsEmpty() => Values == null || Height == 0 || Width == 0;


		/// <summary>
		/// Checks if any element in the matrix is NaN
		/// </summary>
		/// <returns></returns>
		public bool HasNaN() => Any(double.IsNaN);

		public readonly double ErrorWith(ChiruMatrix actual) => (Math.Abs(((actual - this).ElementDivide(actual)).Map(x => double.IsNaN(x) || double.IsInfinity(x) ? 0 : x).Sum() / Width)) * 100;

		private readonly ChiruMatrix ElementDivide(ChiruMatrix b) => ChiruMath.ElementDivide(Values, b.Values).AsMatrix();

		public readonly ChiruMatrix Activate(ActivationFunction activator) => activator.Activate(this);
		public readonly ChiruMatrix DeActivate(ActivationFunction activator) => activator.DeActivate(this);

		private readonly ChiruMatrix Transpose() => new(ChiruMath.Transpose(Values));

		/// <summary>
		/// Calculate the sum of the entire matrix
		/// </summary>
		/// <returns>sum as a double</returns>
		public readonly double Sum() => ChiruMath.Sum(Values);

		/// <summary>
		/// Calculate the sum along a given axis
		/// </summary>
		/// <param name="axis">The axis to sum</param>
		/// <returns>The sum as a matrix</returns>
		public readonly ChiruMatrix SumAxis(MatrixAxis axis) => ChiruMath.SumAxis(Values, axis).AsMatrix();

		/// <summary>
		/// Calulares the mean values of each element in the matrix
		/// </summary>
		/// <returns></returns>
		public readonly double Mean() => Sum() / (Height * Width);

		/// <summary>
		/// Returns a new matrix containing the absolute value of the elements of this matrix
		/// </summary>
		/// <returns></returns>
		public readonly ChiruMatrix Abs() => Map(Math.Abs);

		public readonly ChiruMatrix AppendColumns(ChiruMatrix matrix)
		{
			if (matrix.Height != Height)
				throw new Exception("Matricies are not of the same height");
			var m = new double[Height, Width + matrix.Width];
			for (int i = 0; i < Height; i++)
			{
				for (int j = 0; j < Width + matrix.Width; j++)
				{
					if(j >= Width)
					{
						m[i, j] = matrix[i, j - Width];
					}else
					{
						m[i, j] = this[i, j];
					}
				}
			}
			return m.AsMatrix();
		}

		/// <summary>
		/// Serializes the matrix to a JSON string
		/// </summary>
		/// <returns>JSON string</returns>
		public readonly string ToJSON() => JsonConvert.SerializeObject(Values);

		/// <summary>
		/// Deserializes a matrix from a JSON string
		/// </summary>
		/// <param name="JSON">Matris as a JSON string</param>
		/// <returns></returns>
		public static ChiruMatrix FromJSON(string JSON) => new(JsonConvert.DeserializeObject<double[,]>(JSON));

		public override string ToString()
		{
			StringBuilder sb = new();
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

		public readonly bool Equals(ChiruMatrix obj) => ChiruMath.Equals(Values, obj.Values);

		public override readonly bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != typeof(ChiruMatrix))
				return false;

			return Equals((ChiruMatrix)obj);
		}

		public override readonly int GetHashCode() => Values.GetHashCode();
	}
}
