using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Michiru.Neural
{
    struct NeuralValues
	{
		public double[] Values { get; }
		public int Count { get; }

		public NeuralValues(double[] values)
		{
			Values = values;
			Count = values.Length;
		}

		public NeuralValues(IEnumerable<double> values) : this(values.ToArray())
		{

		}

		public double this[int key]
		{
			get
			{
				return Values[key];
			}
			set
			{
				Values[key] = value;
			}
		}

		public override string ToString()
		{
			return $"[{string.Join(", ", Values)}]";
		}

		public bool Equals(NeuralValues obj)
		{
			return Values.Equals(obj.Values);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != typeof(NeuralValues))
				return false;

			return Equals((NeuralValues)obj);
		}
	}
}
