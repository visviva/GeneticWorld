using System.Xml.XPath;

namespace Evolution;

public class GeneticAlgorithm
{
    readonly ISelectionMethod _selector;
    readonly ICrossover _crossover;
    readonly IRandomHelper _randomGenerator;
    readonly IMutationMethod _mutationMethod;

    public GeneticAlgorithm(IRandomHelper rng, ISelectionMethod selector, ICrossover crossover, IMutationMethod mutationMethod)
    {
        _selector = selector;
        _crossover = crossover;
        _randomGenerator = rng;
        _mutationMethod = mutationMethod;
    }

    public List<IIndividual> Evolve(List<IIndividual> population)
    {
        if (population.Count == 0)
        {
            throw new EmptyPopulationException("The population to evolve is empty");
        }

        var newPopulation = new List<IIndividual>();

        for (int i = 0; i < population.Count; i++)
        {
            (IIndividual, IIndividual) parents = Selection(population);
            IIndividual descendant = Crossover(parents);
            IIndividual mutatedDescendant = Mutation(descendant);
            newPopulation.Add(mutatedDescendant);
        }

        return newPopulation;
    }

    private (IIndividual, IIndividual) Selection(List<IIndividual> population)
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
