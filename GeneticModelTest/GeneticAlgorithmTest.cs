﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticModelTest;

[TestClass]
public partial class GeneticAlgorithmTest
{
    class PseudoRandom : IRandomGenerator
    {
        readonly Queue<double> _doubles = new([0.359283, 0.436927, 0.170511, 0.423261, 0.904628, 0.295714, 0.806882, 0.744967, 0.356180, 0.906374, 0.592549, 0.252870, 0.560270, 0.484916, 0.764076, 0.460495, 0.550033, 0.620975, 0.518279, 0.786998, 0.363043, 0.934887, 0.131732, 0.199310, 0.979206, 0.288144, 0.420393, 0.070144, 0.469246, 0.125812, 0.103108, 0.335546, 0.452567, 0.051491, 0.960261, 0.651038, 0.062468, 0.372618, 0.495513, 0.623620, 0.935863, 0.823832, 0.051556, 0.290971, 0.083134, 0.335496, 0.729304, 0.906396, 0.226720, 0.083393, 0.028738, 0.539874, 0.740496, 0.521185, 0.748467, 0.202155, 0.995666, 0.249508, 0.810023, 0.687535, 0.871745, 0.410318, 0.339711, 0.758632, 0.231410, 0.511076, 0.258619, 0.246372, 0.097442, 0.868320, 0.565857, 0.811132, 0.210694, 0.301255, 0.498183, 0.274612, 0.526979, 0.610111, 0.132227, 0.402829, 0.494977, 0.108132, 0.735821, 0.116290, 0.705818, 0.789227, 0.141457, 0.454742, 0.785255, 0.318988, 0.130020, 0.543827, 0.635268, 0.380689, 0.244317, 0.676603, 0.425104, 0.997230, 0.615391, 0.298987, 0.003819, 0.481711, 0.684671, 0.185247, 0.836605, 0.976573, 0.908439, 0.489300, 0.491072, 0.515124, 0.464062, 0.840908, 0.307005, 0.164874, 0.861119, 0.958656, 0.711862, 0.308532, 0.723249, 0.651876, 0.808420, 0.863266, 0.374952, 0.890202, 0.941639, 0.123766, 0.673238, 0.100683, 0.790935, 0.381093, 0.176387, 0.029015, 0.418456, 0.658046, 0.272233, 0.134585, 0.510026, 0.281478, 0.692050, 0.106615, 0.363963, 0.993138, 0.081517, 0.441149, 0.588390, 0.661115, 0.565344, 0.131780, 0.522840, 0.324640, 0.278087, 0.342847, 0.411634, 0.013278, 0.955163, 0.814770, 0.447022, 0.386684, 0.709830, 0.962175, 0.017836, 0.546887, 0.553379, 0.979527, 0.751453, 0.050165, 0.814287, 0.987565, 0.015176, 0.412014, 0.516133, 0.828397, 0.732192, 0.541193, 0.954928, 0.354258, 0.845948, 0.032955, 0.731328, 0.843830, 0.300061, 0.558891, 0.928098, 0.594530, 0.726067, 0.147684, 0.274433, 0.320714, 0.639141, 0.212484, 0.945839, 0.415665, 0.604981, 0.437545, 0.054471, 0.107143, 0.304922, 0.548516, 0.962429, 0.693645, 0.960343, 0.525702, 0.704000, 0.169683, 0.776966, 0.842130, 0.222674, 0.434220, 0.498334, 0.289079, 0.478377, 0.893581, 0.305962, 0.563522, 0.448045, 0.514506, 0.308518, 0.241879, 0.255091, 0.010159, 0.621677, 0.564447, 0.733044, 0.570325, 0.567479, 0.832944, 0.106352, 0.239733, 0.403684, 0.660562, 0.539854, 0.891368, 0.610465, 0.189686, 0.599803, 0.064816, 0.975304, 0.962716, 0.910134, 0.499746, 0.154801, 0.739241, 0.123779, 0.066270, 0.809514, 0.876589, 0.057162, 0.069966, 0.023653, 0.568226, 0.838509, 0.203703, 0.170289, 0.788068, 0.341715, 0.870926, 0.702011, 0.551006, 0.803793, 0.898200, 0.994616, 0.623362, 0.386062, 0.663138, 0.180917, 0.873369, 0.968509, 0.421128, 0.841522, 0.284380, 0.386088, 0.571741, 0.639434, 0.731009, 0.095094, 0.002681, 0.328780, 0.790413, 0.014579, 0.313590, 0.137315, 0.788717, 0.159764, 0.817072, 0.980220, 0.193989, 0.413178, 0.296030, 0.841953, 0.513003, 0.366821, 0.599130, 0.109520, 0.909780, 0.545142, 0.279775, 0.513570, 0.690982, 0.454236, 0.474671, 0.397475, 0.710739, 0.162887, 0.506782, 0.277034, 0.120558, 0.555060, 0.657821, 0.665599, 0.170573, 0.884845, 0.572119, 0.482369, 0.412116, 0.471663, 0.653521, 0.620281, 0.375925, 0.283570, 0.249753, 0.305454, 0.855057, 0.116808, 0.748296, 0.110281, 0.779953, 0.695004, 0.794737, 0.084760, 0.371385, 0.542621, 0.598840, 0.487894, 0.980710, 0.387681, 0.357778, 0.537606, 0.976915, 0.058884, 0.639271, 0.864104, 0.756382, 0.028277, 0.024860, 0.721478, 0.749125, 0.363632, 0.340857, 0.267457, 0.883878, 0.479215, 0.553641, 0.277057, 0.980058, 0.131091, 0.092878, 0.222604, 0.199791, 0.315832, 0.531335, 0.579782, 0.095672, 0.460616, 0.757586, 0.717877, 0.985277, 0.800382, 0.610759, 0.131945, 0.161316, 0.899693, 0.398755, 0.138582, 0.930770, 0.101017, 0.842157, 0.371049, 0.098787, 0.846273, 0.401312, 0.319812, 0.355416, 0.567542, 0.593697, 0.539179, 0.880703, 0.535042, 0.616514, 0.510817, 0.314786, 0.337036, 0.946039, 0.475925, 0.016376, 0.211167, 0.116020, 0.594441, 0.053170, 0.350900, 0.987362, 0.559797, 0.969539, 0.276345, 0.372458, 0.898943, 0.136338, 0.599266, 0.154177, 0.224711, 0.862147, 0.547252, 0.362503, 0.272223, 0.797605, 0.171288, 0.264238, 0.412484, 0.931534, 0.295407, 0.563782, 0.935694, 0.414377, 0.107853, 0.262240]);
        public double GetRandomNumberInRange(double minimum, double maximum) => _doubles.Dequeue() * (maximum - minimum) + minimum;
    }

    class FakeIndividual : IIndividual
    {
        public double Fitness => Chromosome.Genes.Sum();

        public Chromosome Chromosome { get; set; } = new([]);

        public IIndividual create(Chromosome chromosome)
        {
            var newIndividual = new FakeIndividual
            {
                Chromosome = chromosome
            };
            return newIndividual;
        }
    }

    readonly List<IIndividual> _individuals =
    [
        new FakeIndividual { Chromosome = new Chromosome([0.0, 0.0, 0.0]) },
        new FakeIndividual { Chromosome = new Chromosome([1.0, 1.0, 1.0]) },
        new FakeIndividual { Chromosome = new Chromosome([1.0, 2.0, 1.0]) },
        new FakeIndividual { Chromosome = new Chromosome([1.0, 2.0, 4.0]) }
    ];

    GeneticAlgorithm _geneticAlgorithm;
    IRandomGenerator _randomGenerator;
    ISelectionMethod _selectionMethod;
    ICrossover _crossover;
    IMutationMethod _mutationMethod;

    [TestInitialize]
    public void Init()
    {
        _randomGenerator = new PseudoRandom();
        _selectionMethod = new RouletteWheelSelection();
        _crossover = new UniformCrossover(_randomGenerator);
        _mutationMethod = new GaussianMutation(_randomGenerator, 0.5, 0.5);

        _geneticAlgorithm = new GeneticAlgorithm(_randomGenerator, _selectionMethod, _crossover, _mutationMethod);
    }

    [TestMethod]
    public void TestEvolve()
    {
        var newPopulation = _geneticAlgorithm.Evolve(_individuals);

        Assert.AreEqual(4, newPopulation.Count);

        CollectionAssert.AreEqual(new List<double> { 1.0, 2.453187, 1.280135 }, newPopulation[0].Chromosome.Genes.ToList());
        CollectionAssert.AreEqual(new List<double> { 1.0, 2.0, 0.510397 }, newPopulation[1].Chromosome.Genes.ToList());
        CollectionAssert.AreEqual(new List<double> { 0.7737165, 2.0, 1.186309 }, newPopulation[2].Chromosome.Genes.ToList());
        CollectionAssert.AreEqual(new List<double> { 0.832252, 2.0, 3.985631 }, newPopulation[3].Chromosome.Genes.ToList());
    }
}
