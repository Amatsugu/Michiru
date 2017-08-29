using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Michiru.Calculation;

namespace Michiru.Utils
{
    public static class ImagePreProcessor
    {
		public static void Crop(string inDir, string outDir)
		{
			var files = Directory.EnumerateFiles(inDir, "*", SearchOption.TopDirectoryOnly);
			int c = 0;
			foreach (var f in files)
			{
				using (var image = SKImage.FromBitmap(SKBitmap.Decode(f)))
				{
					int h = image.Height, w = image.Width;
					SKRectI crop;
					if (h > w)
					{
						crop = SKRectI.Create(0, (h - w) / 2, w, w);
					}
					else
					{
						crop = SKRectI.Create((w - h) / 2, 0, h, h);
					}
					using (var subset = image.Subset(crop))
					{

						using (var cropped = subset.Encode().AsStream())
						{
							string num = c++.ToString();
							while (num.Length != 3)
								num = $"0{num}";
							using (var file = File.Create($@"{outDir}\{num}_ZR.png"))
							{
								cropped.Seek(0, SeekOrigin.Begin);
								cropped.CopyTo(file);
								file.Flush();
							}
						}
					}
				}
			}
		}

		public static (ChiruMatrix x, ChiruMatrix y) Flatten(string dir, string zeroPattern)
		{
			double[,] x = null, y;
			var files = Directory.EnumerateFiles(dir, "*.png", SearchOption.AllDirectories).ToList();
			int m = files.Count();
			y = new double[1, m];
			Parallel.For(0, m, mIndex =>
			{
				var f = files[mIndex];
				if (!f.Contains(zeroPattern))
					y[0, mIndex] = 1;
				var bitmap = SKBitmap.Decode(f);
				if (x == null)
					x = new double[bitmap.Height * bitmap.Width * 3, m];
				Parallel.For(0, bitmap.Pixels.Length, i =>
				{
					int ci = i * 3;
					x[ci, mIndex] = bitmap.Pixels[i].Red;
					x[ci + 1, mIndex] = bitmap.Pixels[i].Green;
					x[ci + 2, mIndex] = bitmap.Pixels[i].Blue;
				});
				bitmap.Dispose();
			});
			return (x.AsMatrix(), y.AsMatrix());
		}

		public static void Expand(double[] flatColors, Stream dest)
		{
			var colors = new SKColor[flatColors.Length / 3];
			int c = 0;
			for (int i = 0; i < flatColors.Length; i += 3)
			{
				colors[c++] = new SKColor((byte)flatColors[i], (byte)flatColors[i + 1], (byte)flatColors[i + 2]);
			}
			var ctable = new SKColorTable(colors);
			var img = new SKBitmap(new SKImageInfo(100, 100), ctable);
			var image = SKImage.FromBitmap(img);
			image.Encode().SaveTo(dest);
		}
    }
}
