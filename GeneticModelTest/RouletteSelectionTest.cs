using System.Runtime.CompilerServices;

namespace GeneticModelTest
{
    [TestClass]
    public partial class RouletteWheelSelectionTest
    {
        class MockRandomGenerator : IRandomGenerator
        {
            readonly Random rng = new();
            public double GetRandomNumberInRange(double minNumber, double maxNumber)
            {
                return rng.NextDouble() * (maxNumber - minNumber) + minNumber;
            }
        }

        class FakeIndividual(double fitness) : IIndividual
        {
            public double Fitness { get; set; } = fitness;
            public Chromosome Chromosome { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public IIndividual create(Chromosome chromosome) => throw new NotImplementedException();
        }

        readonly List<FakeIndividual> Population = [new(2), new(1), new(4), new(3)];

        [TestMethod]
        public void SuccessfullySelect()
        {
            var rng = new MockRandomGenerator();
            var selector = new RouletteWheelSelection();
            var histogram = Population.ToDictionary(key => key, _ => 0);

            int cycles = 100000;
            var sumOfWeights = Population.Select(x => x.Fitness).Sum();

            for (int i = 0; i < cycles; i++)
            {
                var selectedIndividual = selector.Select(rng, Population);
                histogram[selectedIndividual] += 1;
            }

            foreach (var individual in Population)
            {
                var expected = individual.Fitness / sumOfWeights;
                var real = (double)histogram[individual] / (double)cycles;
                Assert.AreEqual(expected, real, 0.01);
            }
        }
    }
}
