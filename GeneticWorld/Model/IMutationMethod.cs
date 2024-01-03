namespace GeneticWorld.Model;

public interface IMutationMethod
{
    IIndividual Mutate(IIndividual individual);
}
