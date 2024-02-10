using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkTest
{
    [TestClass]
    public partial class NetworkTest
    {
        Network? testNetwork;

        [TestInitialize]
        public void Init()
        {
            var layer_1 = new LayerTopology(3);
            var layer_2 = new LayerTopology(2);
            var layer_3 = new LayerTopology(1);
            testNetwork = new Network(new MockRandom(), [layer_1, layer_2, layer_3]);

            Assert.IsNotNull(testNetwork);
            Assert.AreEqual(2, testNetwork.CountLayers);
        }

        [TestMethod]
        public void PropagateInput()
        {
            var testInput = new List<double> { 1.0, 1.0, 1.0 };

            var result = testNetwork!.Propagate(testInput);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(9, result[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(MismatchedInputSizeException))]
        public void FailToCreateNetwork()
        {
            new Network(new MockRandom(), [new LayerTopology(2)]);
        }

        [TestMethod]
        public void GetWeights()
        {
            var neuron1 = new Neuron(0.1, [0.2, 0.3, 0.4]);
            var neuron2 = new Neuron(0.5, [0.6, 0.7, 0.8]);

            var network = new Network([new Layer([neuron1]), new Layer([neuron2])]);

            List<double> actualWeights = network.GetWeights();
            List<double> expected = [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8];

            Assert.AreEqual(expected.Count, actualWeights.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actualWeights[i]);
            }
        }

        [TestMethod]
        public void FromWeights()
        {
            List<double> weights = [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8];

            List<LayerTopology> layers = [new LayerTopology(3), new LayerTopology(2)];

            var newNetwork = Network.FromWeights(layers, weights);

            var actualWeights = newNetwork.GetWeights();

            Assert.AreEqual(weights.Count, actualWeights.Count);
            for (int i = 0; i < actualWeights.Count; i++)
            {
                Assert.AreEqual(actualWeights[i], weights[i]);
            }
        }

        [TestMethod]
        public void FromWeightsReduceToOneNeuron()
        {
            List<double> weights = [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 0.01, 0.02];

            List<LayerTopology> layers = [new LayerTopology(3), new LayerTopology(2), new LayerTopology(1)];

            var newNetwork = Network.FromWeights(layers, weights);

            var actualWeights = newNetwork.GetWeights();

            Assert.AreEqual(weights.Count, actualWeights.Count);
            for (int i = 0; i < actualWeights.Count; i++)
            {
                Assert.AreEqual(actualWeights[i], weights[i]);
            }
        }

        [TestMethod]
        public void FromWeightsEnhanceToThreeNeurons()
        {
            List<double> weights = [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.1, 0.1, 0.1, 0.2, 0.2, 0.2, 0.3, 0.3, 0.3];

            List<LayerTopology> layers = [new LayerTopology(3), new LayerTopology(2), new LayerTopology(3)];

            var newNetwork = Network.FromWeights(layers, weights);

            var actualWeights = newNetwork.GetWeights();

            Assert.AreEqual(weights.Count, actualWeights.Count);
            for (int i = 0; i < actualWeights.Count; i++)
            {
                Assert.AreEqual(actualWeights[i], weights[i]);
            }
        }
    }
}
