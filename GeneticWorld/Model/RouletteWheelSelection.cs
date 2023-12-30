namespace GeneticWorld.Model
{
    public class RouletteWheelSelection : ISelectionMethod
    {
        public T Select<T>(IRandomGenerator randGen, List<T> population) where T : IIndividual
        {
            if (!population.Any())
            {
                throw new InvalidOperationException("Selection for an empty population is not possible");
            }

            var preparedPopulation = new Dictionary<IIndividual, double>();

            foreach (var individual in population)
            {
                preparedPopulation[individual] = individual.Fitness;
            }

            var weightedSelector = new WeightedRandomSelector<IIndividual>(randGen, preparedPopulation);
            return (T)weightedSelector.SelectRandomItem();
        }
    }
}

