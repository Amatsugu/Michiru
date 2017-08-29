using Michiru.Calculation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Regression
{
    class LogisticModel
    {
		public ChiruMatrix W { get; set; }
		public double B { get; set; }
		public int Iterations { get; set; }
		public double LearningRate { get; set; }
	}
}
