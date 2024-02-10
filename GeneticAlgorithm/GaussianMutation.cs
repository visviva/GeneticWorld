namespace Evolution;

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
public class GaussianMutation(IRandomHelper random, double chance, double coefficient) : IMutationMethod
{
    public IIndividual Mutate(IIndividual individual)
    {
        var mutant = individual.Chromosome.Genes.ToList();

        for (int i = 0; i < mutant.Count; i++)
        {
            var sign = random.GetRandomSign();
            var doMutation = random.GetRandom_0_1() < chance;

            if (doMutation)
            {
                mutant[i] += sign * coefficient * random.GetRandom_0_1();
            }
        }

        return individual.create(new Chromosome(mutant));
    }
}
