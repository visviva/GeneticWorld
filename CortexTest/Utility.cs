using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkTest
{
    internal static class Utility
    {
        public static bool Equals(double x, double y, double tolerance)
        {
            double diff = Math.Abs(x - y);
            return diff <= tolerance ||
                   diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
        }
    }

    class MockRandom : IRandomNumber
    {
        public double GetRand_minus1_1()
        {
            return 1.0;
        }
    }
}
