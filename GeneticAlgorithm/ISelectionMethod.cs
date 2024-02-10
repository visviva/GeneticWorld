namespace Evolution;

public interface ISelectionMethod
{
    public IIndividual Select(IRandomHelper randGen, List<IIndividual> population);
}


