using Michiru.Calculation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ZeroFormatter;

namespace Michiru.Neural
{
	[ZeroFormattable]
    public class Parameters
    {
		[Index(0)]
		public virtual ChiruMatrix[] W { get; set; }
		[Index(1)]
		public virtual ChiruMatrix[] B { get; set; }

		public Parameters()
		{

		}

		public Parameters(int layers)
		{
			W = new ChiruMatrix[layers];
			B = new ChiruMatrix[layers];
		}

		public string ToJSON() => JsonConvert.SerializeObject(this);
		public static Parameters FromJSON(string json) => JsonConvert.DeserializeObject<Parameters>(json);
	}
}
