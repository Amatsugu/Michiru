using System;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Neural
{
    class Synapse
    {
		public double Weight
		{
			get
			{
				return _weight;
			}
			set
			{
				OldWeight = _weight;
				_weight = value;
			}
		}
		private double _weight;
		public double OldWeight { get; private set; }
		public Neuron Input { get; }
		public Neuron Output { get; }
		private static Random RAND = new Random();

		public Synapse(double weight, Neuron inputNeuron, Neuron outputNeuron)
		{
			Weight = weight;
			Input = inputNeuron;
			Output = outputNeuron;
		}

		public double GetOutput() => Input.GetOutput() * Weight;

		public static Synapse[] FromNeurons(Neuron[] neurons)
		{
			Synapse[] syn = new Synapse[neurons.Length];
			for (int i = 0; i < neurons.Length; i++)
			{
				syn[i] = new Synapse(RAND.NextDouble(), neurons[i], neurons[i]);
			}
			return syn;
		}
    }
}
