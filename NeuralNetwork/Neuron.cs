using System.Runtime.CompilerServices;

namespace Cortex;
public class Neuron
{
    public double Bias { get; set; }
    public List<double> Weights { get; set; }

    public Neuron(IRandomNumber randGen, int inputSize)
    {
        Bias = randGen.GetRand_minus1_1();

        Weights = new List<double>();
        for (int _ = 0; _ < inputSize; _++)
        {
            Weights.Add(randGen.GetRand_minus1_1());
        }
    }

    public Neuron(double bias, List<double> weights)
    {
        Bias = bias;
        Weights = weights;
    }

    public double Propagate(List<double> inputs)
    {
        if (inputs.Count != Weights.Count)
        {
            throw new MismatchedInputSizeException($"Got {inputs.Count} inputs, but {Weights.Count} were expected");
        }

        var output = Bias + inputs.Zip(Weights, (input, weight) => input * weight).Sum();

        return Math.Max(0, output);
    }
}
