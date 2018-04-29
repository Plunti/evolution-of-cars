using System;

namespace NeuralNet
{
    public class NeuralMath
    {
        private static Random Random = new Random();

        public static double RandomDouble()
        {
            return Random.NextDouble();
        }

        public static double ReLU(double x)
        {
            if (x >= 0)
                return x;
            else
                return x / 20;
        }
    }
}
