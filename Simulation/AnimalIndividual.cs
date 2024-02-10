using Evolution;

namespace Simulation;

internal class AnimalIndividual : IIndividual
{
    public double Fitness { get; set; } = 0.0;

    public Chromosome Chromosome { get; set; }

    public AnimalIndividual(double fitness, Chromosome chromosome)
    {
        Chromosome = chromosome;
        Fitness = fitness;
    }

    public IIndividual create(Chromosome chromosome) => new AnimalIndividual(0.0, chromosome);

    public static AnimalIndividual FromAnimal(Animal animal) => new AnimalIndividual(animal.Satiation, animal.Chromosome);

    public Animal IntoAnimal(IRandomGenerator rng)
    {
        return Animal.FromChromosome(rng, Chromosome);
    }
}
