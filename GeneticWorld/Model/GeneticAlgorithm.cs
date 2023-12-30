
namespace GeneticWorld.Model
{
    public class GeneticAlgorithm
    {
        readonly ISelectionMethod Selector;
        readonly IRandomGenerator RandomGenerator;

        public GeneticAlgorithm(IRandomGenerator rng, ISelectionMethod selector)
        {
            Selector = selector;
            RandomGenerator = rng;
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
            return (Selector.Select(RandomGenerator, population), Selector.Select(RandomGenerator, population));
        }

        private IIndividual Mutation(object descendant)
        {
            throw new NotImplementedException();
        }

        private IIndividual Crossover((IIndividual, IIndividual) parents)
        {
            throw new NotImplementedException();
        }

    }
}
