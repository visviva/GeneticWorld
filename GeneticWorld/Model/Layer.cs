using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GeneticModelTest")]

namespace GeneticWorld.Model
{
    internal class Layer
    {
        private readonly List<Neuron> _neurons;

        public int CountOfNeurons => _neurons.Count;

        public Layer(IRandomGenerator randomGenerator, int inputSize, int outputSize)
        {
            _neurons = Enumerable.Range(0, outputSize).Select(i => new Neuron(randomGenerator, inputSize)).ToList();
        }

        internal List<double> Propagate(List<double> inputs) => _neurons.Select(neuron => neuron.Propagate(inputs)).ToList();
    }
}
