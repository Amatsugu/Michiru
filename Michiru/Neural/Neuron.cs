using Michiru.Calculation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Michiru.Neural
{
    class Neuron
    {
		private ActivationFunction _act;
		public Synapse[] Synapses { get; }

		protected Neuron()
		{
			Synapses = null;
			_act = null;
		}

		public Neuron(Synapse[] inputs, ActivationFunction activator)
		{
			Synapses = inputs;
			_act = activator;
		}

		public virtual double GetOutput() =>_act(Synapses.Select((n) => n.GetOutput()).Sum());

		public virtual Neuron BackPropagate(double deltaOutputSum)
		{
			foreach(Synapse n in Synapses)
			{
				if(n.Input.GetType() == typeof(InputNeuron))
				{
					continue;
				}else
				{
					n.Weight += deltaOutputSum * GetOutput();
					n.Input.BackPropagate(deltaOutputSum);
				}
			}
			return this;
		}
    }

	class InputNeuron : Neuron
	{
		private double _input;

		public InputNeuron(double input)
		{
			_input = input;
		}

		public override double GetOutput() => _input;
	}
}
