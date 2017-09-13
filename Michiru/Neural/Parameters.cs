using Michiru.Calculation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Neural
{
    public class Parameters
    {
		public ChiruMatrix[] W { get; set; }
		public ChiruMatrix[] B { get; set; }

		public Parameters(int layers)
		{
			W = new ChiruMatrix[layers];
			B = new ChiruMatrix[layers];
		}

		public string ToJSON() => JsonConvert.SerializeObject(this);
		public static Parameters FromJSON(string json) => JsonConvert.DeserializeObject<Parameters>(json);
	}
}
