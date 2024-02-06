using System.Reflection.Metadata;
using GeneticWorld.Model;

namespace EvolutionSim.Core;

public class Brain
{
    public Network Network { get; set; }

    public Brain(IRandomGenerator rng, Eye eye)
    {
        Network = new(rng, GetLayerTopologies(eye));
    }

    public Brain(Network network)
    {
        Network = network;
    }

    public static List<LayerTopology> GetLayerTopologies(Eye eye)
    {
        List<LayerTopology> topology = [
            new LayerTopology(eye.Cells),
            new LayerTopology(Parameters.Neurons),
            new LayerTopology(2)
            ];

        return topology;
    }

    internal static Brain FromChromosome(Chromosome chromosome, Eye eye) => new(Network.FromWeights(GetLayerTopologies(eye), chromosome.Genes));

    public Chromosome Chromosome => new(Network.GetWeights());

}
