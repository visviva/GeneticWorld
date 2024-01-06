using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("GeneticModelTest")]

namespace GeneticWorld.Model;

internal class UniformCrossover(IRandomGenerator random) : ICrossover
{
    public Chromosome Crossover(Chromosome parentA, Chromosome parentB)
    {
        ArgumentNullException.ThrowIfNull(random);
        ArgumentNullException.ThrowIfNull(parentA);
        ArgumentNullException.ThrowIfNull(parentB);

        if (parentA.Genes.Count != parentB.Genes.Count)
        {
            throw new MismatchedInputSizeException("Genome of parent must have the same size.");
        }

        return new Chromosome(parentA.Genes.Zip(parentB.Genes, (a, b) => coinToss() ? a : b).ToList());
    }
    private bool coinToss() => random.GetRandomNumberInRange(0, 1) >= 0.5;
}
