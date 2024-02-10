namespace GeneticAlgorithmTest;

[TestClass]
public partial class GaussianMutationTest
{

    class RealRandom : IRandomHelper
    {
        readonly Random _random = new Random();

        public double GetRandom_0_1() => _random.NextDouble() * 1.0;
        public double GetRandomSign() => GetRandom_0_1() >= 0.5 ? 1.0 : -1.0;
    }

    class FakeIndividual : IIndividual
    {
        public double Fitness => throw new NotImplementedException();

        public Chromosome Chromosome { get; set; } = new([1.0, 2.0, 3.0, 4.0, 5.0]);

        public IIndividual create(Chromosome chromosome)
        {
            var newIndividual = new FakeIndividual
            {
                Chromosome = chromosome
            };
            return newIndividual;
        }
    }

    readonly RealRandom _random = new RealRandom();

    [TestMethod]
    public void DoesNotChangeOriginalChromosome_ChanceZero_CoeffZero()
    {
        GaussianMutation _gaussian = new(_random, 0, 0);
        var mutant = _gaussian.Mutate(new FakeIndividual());
        CollectionAssert.AreEqual(new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0 }, mutant.Chromosome.Genes.ToArray());
    }

    [TestMethod]
    public void DoesNotChangeOriginalChromosome_ChanceZero_CoeffNonZero()
    {
        GaussianMutation _gaussian = new(_random, 0, 0.5);
        var mutant = _gaussian.Mutate(new FakeIndividual());
        CollectionAssert.AreEqual(new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0 }, mutant.Chromosome.Genes.ToArray());
    }

    [TestMethod]
    public void DoesNotChangeOriginalChromosome_ChanceNonZero_CoeffZero()
    {
        GaussianMutation _gaussian = new(_random, 0.5, 0);
        var mutant = _gaussian.Mutate(new FakeIndividual());
        CollectionAssert.AreEqual(new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0 }, mutant.Chromosome.Genes.ToArray());
    }

    [TestMethod]
    public void DoesNotChangeOriginalChromosome_ChanceNonZero_CoeffNonZero()
    {
        GaussianMutation _gaussian = new(_random, 0.5, 0.5);
        var mutant = _gaussian.Mutate(new FakeIndividual());
        CollectionAssert.AreNotEqual(new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0 }, mutant.Chromosome.Genes.ToArray());
    }
}
