using System.Xml.XPath;

namespace Evolution;

public class GeneticAlgorithm
{
    readonly ISelectionMethod _selector;
    readonly ICrossover _crossover;
    readonly IRandomHelper _randomGenerator;
    readonly IMutationMethod _mutationMethod;
    readonly int _eliteCount;

    public GeneticAlgorithm(IRandomHelper rng, ISelectionMethod selector, ICrossover crossover, IMutationMethod mutationMethod, int eliteCount = 1)
    {
        if (eliteCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(eliteCount), "Elite count cannot be negative");
        }

        _selector = selector;
        _crossover = crossover;
        _randomGenerator = rng;
        _mutationMethod = mutationMethod;
        _eliteCount = eliteCount;
    }

    public List<IIndividual> Evolve(List<IIndividual> population)
    {
        if (population.Count == 0)
        {
            throw new EmptyPopulationException("The population to evolve is empty");
        }

        var elites = population
            .OrderByDescending(individual => individual.Fitness)
            .Take(Math.Min(_eliteCount, population.Count))
            .Select(Clone)
            .ToList();
        var newPopulation = new List<IIndividual>(population.Count);
        newPopulation.AddRange(elites);

        for (int i = newPopulation.Count; i < population.Count; i++)
        {
            (IIndividual, IIndividual) parents = Selection(population);
            IIndividual descendant = Crossover(parents);
            IIndividual mutatedDescendant = Mutation(descendant);
            newPopulation.Add(mutatedDescendant);
        }

        return newPopulation;
    }

    private static IIndividual Clone(IIndividual individual)
    {
        var chromosome = new Chromosome(individual.Chromosome.Genes.ToList());
        return individual.create(chromosome);
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
