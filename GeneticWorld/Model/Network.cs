namespace GeneticWorld.Model;

public record struct LayerTopology(int Neurons);

public interface IRandomGenerator
{
    double GetRandomNumberInRange(double minNumber, double maxNumber);
}

public class Network
{

    private readonly List<Layer> Layers;

    public int CountLayers => Layers.Count;

    public Network(IRandomGenerator randomGenerator, List<LayerTopology> layerTopology)
    {
        if (layerTopology.Count <= 1)
        {
            throw new MismatchedInputSizeException("Expected a network with more than one layer");
        }

        Layers = layerTopology.Zip(layerTopology.Skip(1), (Input, Output) => new Layer(randomGenerator, Input.Neurons, Output.Neurons)).ToList();

    }

    public List<double> Propagate(List<double> inputs)
    {
        return [.. Layers.Aggregate(inputs, (nextInput, nextLayer) => nextLayer.Propagate(nextInput))];
    }
}
