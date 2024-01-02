using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticModelTest
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
    }
}
