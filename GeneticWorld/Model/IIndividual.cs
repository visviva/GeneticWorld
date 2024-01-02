namespace GeneticWorld.Model;

public interface IIndividual
{
    double Fitness { get; }

    Chromosome Chromosome { get; set; }
}

