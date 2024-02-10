namespace Evolution;

public class RouletteWheelSelection : ISelectionMethod
{
    public IIndividual Select(IRandomHelper randGen, List<IIndividual> population)
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
        return weightedSelector.SelectRandomItem();
    }
}
