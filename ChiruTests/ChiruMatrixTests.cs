using Microsoft.VisualStudio.TestTools.UnitTesting;
using Michiru.Calculation;
using System.Diagnostics;


namespace ChiruTests
{
    [TestClass]
    public class ChiruMatrixTests
    {
		private static ChiruMatrix a = new ChiruMatrix(new double[,]
		{
			{1, 2, 3 },
			{1, 2, 3 },
			{1, 2, 3 }
		});
		private static readonly ChiruMatrix b = a;

		
		[TestMethod]
		[TestCategory("Mathix Operations")]
		public void Dot()
		{
			var r = a / b;
			Assert.AreEqual(new double[,] { { 6, 12, 18 } }.AsMatrix(), r);
		}

		[TestMethod]
		[TestCategory("Mathix Operations")]
		public void Multiply()
		{
			var r = a * b;
			var e = new ChiruMatrix(new double[,]
			{
				{6, 12, 18 },
				{6, 12, 18 },
				{6, 12, 18 }
			});
			Assert.AreEqual(e, r);
		}

		[TestMethod]
		public void ErrorWith()
		{
			var e = new double[,] { { 10, 10, 0 }, { 10, 10, 10 } }.AsMatrix().ErrorWith(new double[,] { { 10, 10, 0 }, { 10, 10, 10 } }.AsMatrix());
			Assert.AreEqual(e, 0);
		}

		[TestMethod]
		[TestCategory("Mathix Operations")]
		public void Subtract()
		{
			var r = a - b;
			var e = ChiruMatrix.Zeros(3, 3);
			Assert.AreEqual(e, r);
		}

		[TestMethod]
		[TestCategory("Mathix Operations")]
		public void Add()
		{
			var r = a + b;
			var e = new ChiruMatrix(new double[,]
			{
				{2, 4, 6 },
				{2, 4, 6 },
				{2, 4, 6 }
			});
			Assert.AreEqual(e, r);
		}

		[TestMethod]
		[TestCategory("Mathix Operations")]
		public void SumAxisHorizontal()
		{
			var r = a.SumAxis(MatrixAxis.Horizontal);

			var e = new double[,]
			{
				{6},
				{6},
				{6}
			}.AsMatrix();
			Assert.IsTrue(r.Equals(e));
		}

		[TestMethod]
		[TestCategory("Mathix Operations")]
		public void SumAxisVertical()
		{
			var r = a.SumAxis(MatrixAxis.Vertical);

			var e = new double[,]
			{
				{3, 6, 9},
			}.AsMatrix();
			Assert.IsTrue(r.Equals(e));
		}

		[TestMethod]
		[TestCategory("Scalar Operations")]
		public void ScalarMultiply()
		{
			var r = a * 2;
			var e = new ChiruMatrix(new double[,]
			{
				{2, 4, 6 },
				{2, 4, 6 },
				{2, 4, 6 }
			});
			Assert.AreEqual(e, r);
		}

		[TestMethod]
		[TestCategory("Scalar Operations")]
		public void ScalarAdd()
		{
			var r = a + 2;
			var e = new ChiruMatrix(new double[,]
			{
				{3, 4, 5 },
				{3, 4, 5 },
				{3, 4, 5 }
			});
			Assert.AreEqual(e, r);
		}

		[TestMethod]
		[TestCategory("Scalar Operations")]
		public void ScalarSubtract()
		{
			var r = a - 2;
			var e = new ChiruMatrix(new double[,]
			{
				{-1, 0, 1 },
				{-1, 0, 1 },
				{-1, 0, 1 }
			});
			Assert.AreEqual(e, r);
		}

		[TestMethod]
		[TestCategory("Scalar Operations")]
		public void ScalarDivide()
		{
			var r = a / 2;
			var e = new ChiruMatrix(new double[,]
			{
				{.5, 1, 1.5 },
				{.5, 1, 1.5 },
				{.5, 1, 1.5 }
			});
			Assert.AreEqual(e, r);
		}

		[TestMethod]
		[TestCategory("Mathix Operations")]
		public void Transpose()
		{
			var r = a.T;
			var e = new ChiruMatrix(new double[,]
			{
				{1, 1, 1 },
				{2, 2, 2 },
				{3, 3, 3 }
			});
			Assert.AreEqual(e, r);
		}

	}
}
