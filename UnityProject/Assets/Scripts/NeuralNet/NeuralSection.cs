using System;

namespace NeuralNet
{
    [Serializable]
    public class NeuralSection
    {
        public double[][] Weights { get; set; }

        public NeuralSection() { }

        public NeuralSection(UInt32 inputCount, UInt32 outputCount)
        {
            Weights = new double[inputCount + 1][];
            for (int i = 0; i < Weights.Length; i++)
                Weights[i] = new double[outputCount];
            for (int i = 0; i < Weights.Length; i++)
                for (int k = 0; k < Weights[i].Length; k++)
                    Weights[i][k] = NeuralMath.RandomDouble() - 0.5f;
        }

        public NeuralSection(NeuralSection main)
        {
            Weights = new double[main.Weights.Length][];
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = new double[main.Weights[0].Length];
                for (int k = 0; k < Weights[i].Length; k++)
                    Weights[i][k] = main.Weights[i][k];
            }
        }

        public double[] FeedForward(double[] input)
        {
            double[] output = new double[Weights[0].Length];
            for (int i = 0; i < Weights.Length; i++)
                for (int k = 0; k < Weights[i].Length; k++)
                    if (i == Weights.Length - 1)
                        output[k] += Weights[i][k];
                    else
                        output[k] += Weights[i][k] * input[i];

            for (int i = 0; i < output.Length; i++)
                output[i] = NeuralMath.ReLU(output[i]);

            return output;
        }

        public void Mutate(double probability, double amount)
        {
            for (int i = 0; i < Weights.Length; i++)
                for (int k = 0; k < Weights[i].Length; k++)
                    if (NeuralMath.RandomDouble() < probability)
                        Weights[i][k] = NeuralMath.RandomDouble() * (amount * 2) - amount;
        }
    }
}