namespace Cortex;

public record struct LayerTopology(int Neurons);

public class Network
{
    private readonly List<Layer> Layers;
    public int CountLayers => Layers.Count;

    public List<double> GetWeights() => Layers.SelectMany(layer => layer.GetWeights()).ToList();

    public Network(IRandomNumber randomGenerator, List<LayerTopology> layerTopology)
    {
        if (layerTopology.Count <= 1)
        {
            throw new MismatchedInputSizeException("Expected a network with more than one layer");
        }

        Layers = layerTopology.Zip(layerTopology.Skip(1), (Input, Output) => new Layer(randomGenerator, Input.Neurons, Output.Neurons)).ToList();

    }

    public Network(List<Layer> layers) => Layers = layers;


    public List<double> Propagate(List<double> inputs)
    {
        return [.. Layers.Aggregate(inputs, (nextInput, nextLayer) => nextLayer.Propagate(nextInput))];
    }

    // Scratch storage belongs to this call; results remain valid after the next propagation.
    public void PropagateInto(scoped ReadOnlySpan<double> inputs, Span<double> outputs)
    {
        if (Layers.Count == 0)
        {
            if (inputs.Length != outputs.Length)
                throw new MismatchedInputSizeException("Input and output sizes must match for an empty network");
            inputs.CopyTo(outputs);
            return;
        }

        var width = 0;
        for (var i = 0; i < Layers.Count - 1; i++)
            width = Math.Max(width, Layers[i].CountOfNeurons);

        double[]? rented = null;
        Span<double> scratch = width <= 128
            ? stackalloc double[width * 2]
            : (rented = System.Buffers.ArrayPool<double>.Shared.Rent(checked(width * 2)));
        try
        {
            for (var i = 0; i < Layers.Count; i++)
            {
                var destination = i == Layers.Count - 1
                    ? outputs
                    : scratch.Slice((i % 2) * width, Layers[i].CountOfNeurons);
                Layers[i].PropagateInto(inputs, destination);
                inputs = destination;
            }
        }
        finally
        {
            if (rented != null)
                System.Buffers.ArrayPool<double>.Shared.Return(rented);
        }
    }

    public static Network FromWeights(IReadOnlyList<LayerTopology> topology, IReadOnlyList<double> weights)
    {
        var queue = new Queue<double>(weights);

        var layers = topology.Zip(topology.Skip(1), (WeightsPerNeuron, NumberOfNeurons) =>
        {
            List<Neuron> neurons = [];
            for (int i = 0; i < NumberOfNeurons.Neurons; i++)
            {
                var bias = queue.Dequeue();
                List<double> weights = [];
                for (int j = 0; j < WeightsPerNeuron.Neurons; j++)
                {
                    weights.Add(queue.Dequeue());
                }
                var newNeuron = new Neuron(bias, weights);
                neurons.Add(newNeuron);
            }
            return new Layer(neurons);
        }).ToList();

        return new Network(layers);
    }
}
