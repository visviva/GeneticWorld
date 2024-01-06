using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GeneticModelTest")]

namespace GeneticWorld.Model;

public class Chromosome
{
    private readonly List<double> _genes;

    public Chromosome(List<double> genes) => _genes = genes;

    public int Length => _genes.Count;

    public IReadOnlyList<double> Genes => _genes;
}
