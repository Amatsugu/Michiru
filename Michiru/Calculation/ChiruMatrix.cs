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
		public int Height => Values.GetLength(0);
		[JsonIgnore]
		public int Width => Values.GetLength(1);
		[JsonIgnore]
		public ChiruMatrix T => Transpose();
        [JsonIgnore]
        public ChiruMatrix Shape => new double[,] { { Height, Width }}.AsMatrix();

		[JsonIgnore]
		private static Random _RAND = new Random();
		

		public ChiruMatrix(double[,] values) => Values = values;

		public ref double this[int i, int j]
		{
			get => ref Values[i, j];
			//set => Values[i, j] = value;
		}

		public ChiruMatrix this[int j]
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
		public static ChiruMatrix Zeros(int h, int w) => new ChiruMatrix(new double[h, w]);
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
		public ChiruMatrix Copy() => ((double[,])Values.Clone()).AsMatrix();
		//Add
		private ChiruMatrix Add(ChiruMatrix b) => new ChiruMatrix(ChiruMath.Add(Values, b.Values));
		public static ChiruMatrix operator +(ChiruMatrix a, ChiruMatrix b) => ChiruMath.Add(a.Values, b.Values).AsMatrix();
		public static ChiruMatrix operator +(ChiruMatrix a, double[,] b) => new ChiruMatrix(ChiruMath.Add(a.Values, b));
		public static ChiruMatrix operator +(double[,] a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.Add(a, b.Values));
		//Add Col
		public ChiruMatrix ColAdd(ChiruMatrix b) => new ChiruMatrix(ChiruMath.ColAdd(Values, b.Values));
		//Subtract
		private ChiruMatrix Subtract(ChiruMatrix b) => new ChiruMatrix(ChiruMath.Subtract(Values, b.Values));
		public static ChiruMatrix operator -(ChiruMatrix a, ChiruMatrix b) => ChiruMath.Subtract(a.Values, b.Values).AsMatrix();
		public static ChiruMatrix operator -(ChiruMatrix a, double[,] b) => new ChiruMatrix(ChiruMath.Subtract(a.Values, b));
		public static ChiruMatrix operator -(double[,] a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.Subtract(a, b.Values));
		//Multiply
		private ChiruMatrix Multiply(ChiruMatrix b) => new ChiruMatrix(ChiruMath.Multiply(Values, b.Values));
		public static ChiruMatrix operator *(ChiruMatrix a, ChiruMatrix b) => ChiruMath.Multiply(a.Values, b.Values).AsMatrix();
		public static ChiruMatrix operator *(ChiruMatrix a, double[,] b) => new ChiruMatrix(ChiruMath.Multiply(a.Values, b));
		public static ChiruMatrix operator *(double[,] a, ChiruMatrix b) => new ChiruMatrix(ChiruMath.Multiply(a, b.Values));
		//Multiply Col
		/// <summary>
		/// Multiplies the given column matrix with each column of this matrix elementwise
		/// </summary>
		/// <param name="b">Column Matrix</param>
		/// <returns></returns>
		public ChiruMatrix ElementMultiply(ChiruMatrix b) => new ChiruMatrix(ChiruMath.ElementMultiply(Values, b.Values));
		//Dot
		private ChiruMatrix Dot(ChiruMatrix b) => ChiruMath.Dot(Values, b.Values).AsMatrix();
		public static ChiruMatrix operator /(ChiruMatrix a, ChiruMatrix b) => ChiruMath.Dot(a.Values, b.Values).AsMatrix();
		public static ChiruMatrix operator /(ChiruMatrix a, double[,] b) => ChiruMath.Dot(a.Values, b).AsMatrix();
		public static ChiruMatrix operator /(double[,] a, ChiruMatrix b) => ChiruMath.Dot(a, b.Values).AsMatrix();
		//Scalar
		public static ChiruMatrix operator *(double a, ChiruMatrix b) => b * a;
		public static ChiruMatrix operator *(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarMultiply(a.Values, b));
		public static ChiruMatrix operator +(double a, ChiruMatrix b) => b + a;
		public static ChiruMatrix operator +(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarAdd(a.Values, b));
		public static ChiruMatrix operator -(ChiruMatrix a) => a * -1;
		public static ChiruMatrix operator -(double a, ChiruMatrix b) => -b + a;
		public static ChiruMatrix operator -(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarSubtract(a.Values, b));
		public static ChiruMatrix operator /(ChiruMatrix a, double b) => new ChiruMatrix(ChiruMath.ScalarDivide(a.Values, b));
		public static ChiruMatrix operator /(double a, ChiruMatrix b) => (1 / a) * b;

		public ChiruMatrix Divide(ChiruMatrix b) => ChiruMath.ElementDivide(Values, b.Values).AsMatrix();

		/// <summary>
		/// Aplies the given function to every element of this matrix, the result is placed in a new matrix
		/// </summary>
		/// <param name="f">Function to apply</param>
		/// <returns>A new matrix containing the values after the function has been applied</returns>
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

		/// <summary>
		/// Tests each element in the matrix with the given condition, returns true if at least one element meets the condition
		/// </summary>
		/// <param name="condition">Condition to be tested</param>
		/// <returns></returns>
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

		public bool IsEmpty() => Values == null || Height == 0 || Width == 0;


		/// <summary>
		/// Checks if any element in the matrix is NaN
		/// </summary>
		/// <returns></returns>
		public bool HasNaN() => Any(double.IsNaN);

		public double ErrorWith(ChiruMatrix actual) => (Math.Abs(((actual - this).ElementDivide(actual)).Map(x => double.IsNaN(x) || double.IsInfinity(x) ? 0 : x).Sum() / Width)) * 100;

		private ChiruMatrix ElementDivide(ChiruMatrix b) => ChiruMath.ElementDivide(Values, b.Values).AsMatrix();

		public ChiruMatrix Activate(ActivationFunction activator) => activator.Activate(this);
		public ChiruMatrix DeActivate(ActivationFunction activator) => activator.DeActivate(this);

		private ChiruMatrix Transpose() => new ChiruMatrix(ChiruMath.Transpose(Values));

		/// <summary>
		/// Calculate the sum of the entire matrix
		/// </summary>
		/// <returns>sum as a double</returns>
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

		/// <summary>
		/// Calculate the sum along a given axis
		/// </summary>
		/// <param name="axis">The axis to sum</param>
		/// <returns>The sum as a matrix</returns>
		public ChiruMatrix SumAxis(int axis)
		{
			ChiruMatrix sum;
			if(axis == 0)
			{
				sum = Zeros(1, Width);
				for (int i = 0; i < Height; i++)
				{
					for (int j = 0; j < Width; j++)
					{
						sum[0, j] += this[i, j];
					}
				}
			}else if(axis == 1)
			{
				sum = Zeros(Height, 1);
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

		/// <summary>
		/// Calulares the mean values of each element in the matrix
		/// </summary>
		/// <returns></returns>
		public double Mean() => Sum() / (Height * Width);

		/// <summary>
		/// Returns a new matrix containing the absolute value of the elements of this matrix
		/// </summary>
		/// <returns></returns>
		public ChiruMatrix Abs() => Map(Math.Abs);

		public ChiruMatrix AppendColumns(ChiruMatrix matrix)
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
		public string ToJSON() => JsonConvert.SerializeObject(Values);

		/// <summary>
		/// Deserializes a matrix from a JSON string
		/// </summary>
		/// <param name="JSON">Matris as a JSON string</param>
		/// <returns></returns>
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

		public bool Equals(ChiruMatrix obj) => ChiruMath.Equals(Values, obj.Values);

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != typeof(ChiruMatrix))
				return false;

			return Equals((ChiruMatrix)obj);
		}

		public override int GetHashCode() => Values.GetHashCode();
	}
}
