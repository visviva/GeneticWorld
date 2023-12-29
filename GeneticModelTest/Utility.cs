using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticModelTest
{
    internal static class Utility
    {
        public static bool Equals(double x, double y, double tolerance)
        {
            var diff = Math.Abs(x - y);
            return diff <= tolerance ||
                   diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
        }
    }

    class MockRandom : IRandomGenerator
    {
        public double GetRandomNumberInRange(double minNumber, double maxNumber)
        {
            return 1.0;
        }
    }
}
