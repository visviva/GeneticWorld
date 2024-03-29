﻿using System.Runtime.CompilerServices;

namespace GeneticAlgorithmTest
{
    [TestClass]
    public partial class RouletteWheelSelectionTest
    {
        class MockRandomGenerator : IRandomHelper
        {
            readonly Random rng = new();

            public double GetRandom_0_1() => rng.NextDouble();
            public double GetRandomSign() => throw new NotImplementedException();
        }

        class FakeIndividual(double fitness) : IIndividual
        {
            public double Fitness { get; set; } = fitness;
            public Chromosome Chromosome { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public IIndividual create(Chromosome chromosome) => throw new NotImplementedException();
        }

        readonly List<IIndividual> Population = [new FakeIndividual(2), new FakeIndividual(1), new FakeIndividual(4), new FakeIndividual(3)];

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
