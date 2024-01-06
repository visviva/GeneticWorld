using System.Runtime.InteropServices;

namespace GeneticWorld.Model;

public interface IIndividual
{
    double Fitness { get; }
    Chromosome Chromosome { get; set; }

    IIndividual create(Chromosome chromosome);
}

