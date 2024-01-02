using System;

namespace GeneticModelTest
{
    [TestClass]
    public partial class NeuronTest
    {
        [TestMethod]
        public void SuccessfullyCreateNeuron()
        {
            var neuron = new Neuron(new MockRandom(), 3);
            Assert.AreEqual(1.0, neuron.Bias, double.Epsilon);

            var correct_weights = new List<double> { 1.0, 1.0, 1.0 };

            var result = correct_weights.Zip(neuron.Weights, (expected, real) => Utility.Equals(expected, real, double.Epsilon)).ToList();

            Assert.IsFalse(result.Contains(false));
        }

        [TestMethod]
        public void SuccessfullyPropagateNeuron()
        {
            var neuron = new Neuron(new MockRandom(), 3);
            var result = neuron.Propagate([1, 1, 1]);
            Assert.AreEqual(4.0, result, double.Epsilon);
        }

        [TestMethod]
        [ExpectedException(typeof(MismatchedInputSizeException))]
        public void FailPropagateNeuron()
        {
            var neuron = new Neuron(new MockRandom(), 2);
            neuron.Propagate([1, 1, 1]);
        }
    }
}
