
using System.Xml.XPath;

namespace GeneticWorld.Model;

public class GeneticAlgorithm
{
    readonly ISelectionMethod _selector;
    private readonly ICrossover _crossover;
    readonly IRandomGenerator _randomGenerator;

    public GeneticAlgorithm(IRandomGenerator rng, ISelectionMethod selector, ICrossover crossover)
    {
        _selector = selector;
        _crossover = crossover;
        _randomGenerator = rng;
    }

    public List<T> Evolve<T>(List<T> population) where T : IIndividual
    {
        if (population.Count == 0)
        {
            throw new EmptyPopulationException("The population to evolve is empty");
        }

        var newPopulation = new List<T>();

        for (int i = 0; i < population.Count; i++)
        {
            var parents = Selection(population);
            var descendant = Crossover(parents);
            var mutatedDescendant = Mutation(descendant);
            newPopulation.Add((T)mutatedDescendant);
        }

        return newPopulation;
    }

    private (IIndividual, IIndividual) Selection<T>(List<T> population) where T : IIndividual
    {
        return (_selector.Select(_randomGenerator, population), _selector.Select(_randomGenerator, population));
    }

    private IIndividual Mutation(object descendant)
    {
        throw new NotImplementedException();
    }

    private IIndividual Crossover((IIndividual, IIndividual) parents)
    {
        var childChromosome = _crossover.Crossover(parents.Item1.Chromosome, parents.Item2.Chromosome);
        throw new NotImplementedException();
    }

}
