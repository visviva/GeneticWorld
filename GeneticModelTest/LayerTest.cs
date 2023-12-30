
namespace GeneticModelTest
{
    [TestClass]
    public partial class LayerTest
    {
        Layer? testLayer;

        [TestInitialize]
        public void Init()
        {
            testLayer = new Layer(new MockRandom(), 4, 3);
        }

        [TestMethod]
        public void SuccessfullyCreateLayer()
        {
            Assert.IsNotNull(testLayer);
            Assert.AreEqual(3, testLayer.CountOfNeurons);
        }

        [TestMethod]
        public void SuccessfullyPropagateLayer()
        {
            Assert.IsNotNull(testLayer);
            var result = testLayer.Propagate([1, 1, 1, 1]);
            Assert.AreEqual(3, result.Count);

            foreach (var entry in result)
            {
                Assert.AreEqual(5, entry);
            }
        }
    }
}
