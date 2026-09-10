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

    public double Propagate(List<double> inputs) => PropagateValues(System.Runtime.InteropServices.CollectionsMarshal.AsSpan(inputs));

    public double PropagateValues(ReadOnlySpan<double> inputs)
    {
        if (inputs.Length != Weights.Count)
        {
            throw new MismatchedInputSizeException($"Got {inputs.Length} inputs, but {Weights.Count} were expected");
        }

        double sum = 0;
        for (var i = 0; i < inputs.Length; i++)
        {
            sum += inputs[i] * Weights[i];
        }

        return Math.Max(0, Bias + sum);
    }
}
