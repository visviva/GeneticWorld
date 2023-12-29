using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GeneticModelTest")]

namespace GeneticWorld.Model
{
    internal class Layer
    {
        private readonly List<Neuron> Neurons;

        public int CountOfNeurons => Neurons.Count;

        public Layer(IRandomGenerator randomGenerator, int inputSize, int outputSize)
        {
            Neurons = Enumerable.Range(0, outputSize).Select(i => new Neuron(randomGenerator, inputSize)).ToList();
        }

        internal List<double> Propagate(List<double> inputs) => Neurons.Select(neuron => neuron.Propagate(inputs)).ToList();
    }
}