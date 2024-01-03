namespace GeneticWorld.Model
{
    public interface ISelectionMethod
    {
        public T Select<T>(IRandomGenerator randGen, List<T> population) where T : IIndividual;
    }
}

