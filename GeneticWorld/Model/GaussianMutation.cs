namespace GeneticWorld.Model;

/// <summary>
/// 
/// </summary>
/// <param name="random">
/// <param name="chance"></param>
/// Probability of changing a gene:
/// - 0.0 = no genes will be touched
/// - 1.0 = all genes will be touched
/// </param>
/// <param name="coefficient">
/// Magnitude of that change:
/// - 0.0 = touched genes will not be modified
/// - 3.0 = touched genes will be += or -= by at most 3.0
/// </param>
public class GaussianMutation(IRandomGenerator random, double chance, double coefficient) : IMutationMethod
{
    public IIndividual Mutate(IIndividual individual)
    {
        var mutant = individual.Chromosome.Genes.ToList();

        for (int i = 0; i < mutant.Count; i++)
        {
            var sign = random.GetRandomNumberInRange(0, 1) >= 0.5 ? 1.0 : -1.0;
            var doMutation = random.GetRandomNumberInRange(0, 1) < chance;

            if (doMutation)
            {
                mutant[i] += sign * coefficient * random.GetRandomNumberInRange(0, 1);
            }
        }

        return individual.create(new Chromosome(mutant));
    }
}
