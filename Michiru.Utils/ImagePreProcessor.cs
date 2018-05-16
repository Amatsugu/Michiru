using SkiaSharp;
using System;
using System.IO;
using System.Linq;
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

		public static void Resize(string inDir, string outDir, double scaleFactor, Func<int, string, string> nameFunc = null)
		{
			if (nameFunc == null)
				nameFunc = (i, n) => n;
			var files = Directory.EnumerateFiles(inDir, "*", SearchOption.TopDirectoryOnly);
			foreach(var f in files)
			{
				using (var img = SKBitmap.Decode(f))
				{
					img.Resize(new SKImageInfo
					{
						Height = (int)(img.Height * scaleFactor),
						Width = (int)(img.Width * scaleFactor)
					}, SKBitmapResizeMethod.Hamming);

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

		public static void Expand(ChiruMatrix flatColors, Stream dest, int size, bool greyScale = false)
		{
            var surf = SKSurface.Create(size, size, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            var canvas = surf.Canvas;
			int incrementor = 3;
			if (greyScale)
				incrementor = 1;
			for (int i = 0; i < flatColors.Height; i += incrementor)
			{
				SKColor col;
				if(greyScale)
					col = new SKColor((byte)flatColors[i, 0], (byte)flatColors[i, 0], (byte)flatColors[i, 0]);
				else
					col = new SKColor((byte)flatColors[i, 0], (byte)flatColors[i + 1, 0], (byte)flatColors[i + 2, 0]);
                int y = (i / incrementor) / size;
                int x = (i / incrementor) - (y * size);
                canvas.DrawPoint(x, y, col);
			}
            surf.Snapshot().Encode().SaveTo(dest);
            surf.Dispose();
		}

        public static ChiruMatrix ToGreyscale(ChiruMatrix images)
        {
            var grey = ChiruMatrix.Zeros(images.Height, images.Width);
            for (int i = 0; i < images.Width; i++)
            {
                var image = images[i];
                for (int j = 0; j < image.Height; j+= 3)
                {
                    double a = image[j, 0] + image[j + 1, 0] + image[j + 2, 0];
                    a /= 3;
                    grey[j, i] = a;
                    grey[j+1, i] = a;
                    grey[j+2, i] = a;
                }
            }
            return grey;
        }
	}
}
