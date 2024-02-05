using GeneticWorld.Model;

namespace GeneticWorld.Core;

public class AnimalIndividual : IIndividual
{
    public double Fitness { get; set; } = 0.0;

    public Chromosome Chromosome { get; set; }

    public AnimalIndividual(double fitness, Chromosome chromosome)
    {
        Chromosome = chromosome;
        Fitness = fitness;
    }

    public IIndividual create(Chromosome chromosome) => new AnimalIndividual(0.0, chromosome);

    public static AnimalIndividual FromAnimal(Animal animal)
    {
        throw new NotImplementedException();
    }

    public Animal IntoAnimal(IRandomGenerator rng)
    {
        throw new NotImplementedException();
    }
}
