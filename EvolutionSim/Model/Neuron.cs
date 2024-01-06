using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GeneticModelTest")]

namespace GeneticWorld.Model;

internal class Neuron
{
    public double Bias;
    public List<double> Weights;

    public Neuron(IRandomGenerator randGen, int inputSize)
    {
        Bias = randGen.GetRandomNumberInRange(-1.0, 1.0);

        Weights = new List<double>();
        for (int _ = 0; _ < inputSize; _++)
        {
            Weights.Add(randGen.GetRandomNumberInRange(-1.0, 1.0));
        }
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
