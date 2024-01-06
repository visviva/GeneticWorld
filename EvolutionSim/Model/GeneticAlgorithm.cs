
using System.Xml.XPath;

namespace GeneticWorld.Model;

public class GeneticAlgorithm
{
    readonly ISelectionMethod _selector;
    readonly ICrossover _crossover;
    readonly IRandomGenerator _randomGenerator;
    readonly IMutationMethod _mutationMethod;

    public GeneticAlgorithm(IRandomGenerator rng, ISelectionMethod selector, ICrossover crossover, IMutationMethod mutationMethod)
    {
        _selector = selector;
        _crossover = crossover;
        _randomGenerator = rng;
        _mutationMethod = mutationMethod;
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
            (IIndividual, IIndividual) parents = Selection(population);
            IIndividual descendant = Crossover(parents);
            IIndividual mutatedDescendant = Mutation(descendant);
            newPopulation.Add((T)mutatedDescendant);
        }

        return newPopulation;
    }

    private (IIndividual, IIndividual) Selection<T>(List<T> population) where T : IIndividual
    {
        return (_selector.Select(_randomGenerator, population), _selector.Select(_randomGenerator, population));
    }

    private IIndividual Mutation(IIndividual descendant)
    {
        return _mutationMethod.Mutate(descendant);
    }

    private IIndividual Crossover((IIndividual, IIndividual) parents)
    {
        var childChromosome = _crossover.Crossover(parents.Item1.Chromosome, parents.Item2.Chromosome);
        return parents.Item1.create(childChromosome);
    }

}
