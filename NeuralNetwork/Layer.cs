using System.Runtime.CompilerServices;

namespace Cortex;

public class Layer
{
    private readonly List<Neuron> _neurons;

    public List<double> GetWeights() => _neurons.SelectMany(neuron => new[] { neuron.Bias }.Concat(neuron.Weights)).ToList();

    public int CountOfNeurons => _neurons.Count;

    public Layer(IRandomNumber randomGenerator, int inputSize, int outputSize)
    {
        _neurons = Enumerable.Range(0, outputSize).Select(i => new Neuron(randomGenerator, inputSize)).ToList();
    }

    public Layer(List<Neuron> neurons) => _neurons = neurons;

    public List<double> Propagate(List<double> inputs) => _neurons.Select(neuron => neuron.Propagate(inputs)).ToList();
}
