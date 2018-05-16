using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Michiru.Calculation;

namespace Michiru.Utils
{
    class IDXReader
    {

		public static ChiruMatrix GetDataMatrix(string path, int size, int count)
		{
			var file = new FileStream(path, FileMode.Open);
			var reader = new BinaryReader(file, Encoding.BigEndianUnicode);
			int magic = reader.ReadInt32();
			int numData = reader.ReadInt32();
			int numRows = reader.ReadInt32();
			int numCols = reader.ReadInt32();
			var data = ChiruMatrix.Zeros(size * size, count);

			for (int j = 0; j < count; j++)
			{
				for (int i = 0; i < size * size; i++)
				{
					data[i, j] = reader.ReadByte();
				}
			}
			reader.Close();
			reader.Dispose();
			file.Close();
			file.Dispose();
			return data;
		}

		public static ChiruMatrix GetLabelMatrix(string path, int size, int count, Action<ChiruMatrix, double> formatter = null)
		{
			var file = new FileStream(path, FileMode.Open);
			var reader = new BinaryReader(file, Encoding.BigEndianUnicode);
			int magic = reader.ReadInt32();
			int numData = reader.ReadInt32();
			var data = ChiruMatrix.Zeros(size, count);

			for (int j = 0; j < count; j++)
			{
				var d = reader.ReadByte();
				if (formatter == null)
					data[d, j] = 1;
				else
					formatter(data, d);
			}
			reader.Close();
			reader.Dispose();
			file.Close();
			file.Dispose();
			return data;
		}
	}
}
