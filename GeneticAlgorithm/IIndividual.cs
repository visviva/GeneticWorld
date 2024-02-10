using System.Runtime.InteropServices;

namespace Evolution;

public interface IIndividual
{
    double Fitness { get; }
    Chromosome Chromosome { get; set; }

    IIndividual create(Chromosome chromosome);
}

