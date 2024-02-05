using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GeneticModelTest")]

namespace GeneticWorld.Model;

public class Neuron
{
    public double Bias { get; set; }
    public List<double> Weights { get; set; }

    public Neuron(IRandomGenerator randGen, int inputSize)
    {
        Bias = randGen.GetRandomNumberInRange(-1.0, 1.0);

        Weights = new List<double>();
        for (int _ = 0; _ < inputSize; _++)
        {
            Weights.Add(randGen.GetRandomNumberInRange(-1.0, 1.0));
        }
    }

    public Neuron(double bias, List<double> weights)
    {
        Bias = bias;
        Weights = weights;
    }

    internal double Propagate(List<double> inputs)
    {
        if (inputs.Count != Weights.Count)
        {
            throw new MismatchedInputSizeException($"Got {inputs.Count} inputs, but {Weights.Count} were expected");
        }

        var output = Bias + inputs.Zip(Weights, (input, weight) => input * weight).Sum();

        return Math.Max(0, output);
    }
}
