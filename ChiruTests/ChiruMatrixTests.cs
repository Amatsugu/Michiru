using Microsoft.VisualStudio.TestTools.UnitTesting;
using Michiru.Calculation;
using System.Diagnostics;


namespace ChiruTests
{
    [TestClass]
    public class ChiruMatrixTests
    {
		static ChiruMatrix a = new ChiruMatrix(new double[,]
			{
				{1, 2, 3 },
				{1, 2, 3 },
				{1, 2, 3 }
			}), b = a;


		[TestMethod]
		[TestCategory("Mathix Operations")]
		public void Dot()
		{
			var r = a / b;
			Assert.IsTrue(ChiruMath.Equals(r, (new double[] { 14, 14, 14 })));
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
			Assert.AreEqual(r, e);
		}

		[TestMethod]
		[TestCategory("Mathix Operations")]
		public void Subtract()
		{
			var r = a - b;
			var e = ChiruMatrix.Zeros(3, 3);
			Assert.AreEqual(r, e);
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
			Assert.AreEqual(r, e);
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
			Assert.AreEqual(r, e);
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
			Assert.AreEqual(r, e);
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
			Assert.AreEqual(r, e);
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
			Assert.AreEqual(r, e);
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
			Assert.AreEqual(r, e);
		}

	}
}
