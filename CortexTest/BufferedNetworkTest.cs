namespace NeuralNetworkTest;

[TestClass]
public class BufferedNetworkTest
{
    [DataTestMethod]
    [DataRow(3)]
    [DataRow(160)] // Exercises pooled scratch storage as well as stack storage.
    public void BufferedPropagationMatchesReferenceAcrossUnequalLayers(int width)
    {
        var random = new Random(42);
        var sizes = new[] { 5, width, 7, 4, 2 };
        var layers = new List<Layer>();
        var reference = new List<List<Neuron>>();
        for (var layer = 1; layer < sizes.Length; layer++)
        {
            var neurons = Enumerable.Range(0, sizes[layer]).Select(_ =>
                new Neuron(random.NextDouble() - 0.5,
                    Enumerable.Range(0, sizes[layer - 1]).Select(_ => random.NextDouble() - 0.5).ToList())).ToList();
            reference.Add(neurons);
            layers.Add(new Layer(neurons));
        }

        var network = new Network(layers);
        var output = new double[2];
        for (var sample = 0; sample < 10; sample++)
        {
            var input = Enumerable.Range(0, sizes[0]).Select(_ => random.NextDouble()).ToArray();
            var expected = input;
            foreach (var neurons in reference)
                expected = neurons.Select(n => Math.Max(0, n.Bias + expected.Zip(n.Weights, (x, w) => x * w).Sum())).ToArray();

            network.PropagateInto(input, output);
            for (var i = 0; i < output.Length; i++)
                Assert.AreEqual(expected[i], output[i], 1e-12);
        }
    }

    [TestMethod]
    public void BufferedPropagationRejectsWrongOutputSize()
    {
        var network = new Network([new Layer([new Neuron(0, [1, 2])])]);
        Assert.ThrowsException<MismatchedInputSizeException>(() => network.PropagateInto(new double[2], new double[2]));
    }

    [TestMethod]
    public void BufferedPropagationRejectsWrongInputSize()
    {
        var network = new Network([new Layer([new Neuron(0, [1, 2])])]);
        Assert.ThrowsException<MismatchedInputSizeException>(() => network.PropagateInto(new double[1], new double[1]));
    }
}
