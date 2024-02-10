namespace GeneticAlgorithmTest;

[TestClass]
public partial class ChromosomeTest
{
    readonly Chromosome _chromosome = new([3.0, 1.0, 2.0]);

    [TestMethod]
    public void TestChromosomeLength()
    {
        Assert.AreEqual(3, _chromosome.Length);
    }

    [TestMethod]
    public void TestChromosomeIteratorReadOnly()
    {
        var it = _chromosome.Genes.ToArray();
        Assert.AreEqual(3, it.Length);

        Assert.AreEqual(3.0, it[0]);
        Assert.AreEqual(1.0, it[1]);
        Assert.AreEqual(2.0, it[2]);
    }
}
