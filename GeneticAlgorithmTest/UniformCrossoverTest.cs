﻿
namespace GeneticAlgorithmTest
{
    [TestClass]
    public partial class UniformCrossoverTest
    {

        public class ToggleRandom : IRandomHelper
        {
            private bool _toggle = false;

            public double GetToggledValue()
            {
                _toggle = !_toggle;
                return _toggle ? 0.8 : 0.2;
            }

            public double GetRandom_0_1() => GetToggledValue();
            public double GetRandomSign() => GetRandom_0_1() >= 0.5 ? 1.0 : -1.0;
        }

        readonly UniformCrossover _crossover = new(new ToggleRandom());

        Chromosome? _parentA;
        Chromosome? _parentB;

        [TestInitialize]
        public void Init()
        {
            var genesA = new List<double>();
            var genesB = new List<double>();

            for (int i = 1; i <= 100; i++)
            {
                genesA.Add(i);
                genesB.Add(-i);
            }

            _parentA = new Chromosome(genesA);
            _parentB = new Chromosome(genesB);
        }

        [TestMethod]
        public void SuccessfullyCrossover()
        {
            var child = _crossover.Crossover(_parentA!, _parentB!);

            int fromA = child.Genes.Count(g => _parentA!.Genes.Contains(g) && g > 0);
            int fromB = child.Genes.Count(g => _parentB!.Genes.Contains(g) && g < 0);

            Assert.AreEqual(50, fromA);
            Assert.AreEqual(50, fromB);
        }
    }
}
