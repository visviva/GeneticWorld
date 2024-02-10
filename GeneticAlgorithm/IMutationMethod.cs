namespace Evolution;

public interface IMutationMethod
{
    IIndividual Mutate(IIndividual individual);
}
