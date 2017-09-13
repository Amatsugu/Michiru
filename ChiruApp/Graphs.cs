using Michiru.Calculation;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiruApp
{
	class Graphs
	{
		public static void DrawGraph(SKSurface surf, ChiruMatrix data, SKPMColor lineColor)
		{
			double yMax = 0;
			for(int j = 0; j < data.Width; j++)
			{
				if (yMax < data[0, j])
					yMax = data[0, j];
			}
			DrawGraph(surf, data, 0, data.Width, 0, yMax, lineColor);
		}

		public static void DrawGraph(SKSurface surf, ChiruMatrix data, double xMin, double xMax, double yMin, double yMax, SKPMColor lineColor, double padding = 10)
		{
			var canvas = surf.Canvas;
			var snap = surf.Snapshot();
			int h = snap.Height, w = snap.Width;
			canvas.DrawRect(new SKRect(0, 0, w, h), new SKPaint { Color = new SKColor(255, 255, 255) });
			var colors = new SKColorTable(h * w);
			int pX, pY;
			int lpX = 0, lpY = 0;
			var paint = new SKPaint
			{
				Color = new SKColor(255, 0, 100),
				IsAntialias = true,
				StrokeWidth = 2,
			};
			var textPaint = new SKPaint
			{
				Color = new SKColor(0, 0, 0),
				IsAntialias = true,
				TextAlign = SKTextAlign.Right,
				TextSize = 40
			};
			for (int j = 0; j < data.Width; j++)
			{
				pX = (int)(padding + ((j / xMax) * (w - (2 * padding))));
				pY = (int)(padding + ((data[0,j] / yMax) * (h - (2 * padding))));
				canvas.DrawCircle(pX, h - pY, 5, paint);
				if (j > 0)
					canvas.DrawLine(lpX, h - lpY, pX, h - pY, paint);
				if (j == data.Width - 1)
					canvas.DrawText($"Cost: {data[0, j]}", pX, h - pY - 10, textPaint);
				lpX = pX;
				lpY = pY;
			}
			canvas.Flush();
		}
	}
}
