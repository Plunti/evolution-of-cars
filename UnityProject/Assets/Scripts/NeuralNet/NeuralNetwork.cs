using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeuralNet
{
    [Serializable]
    public class NeuralNetwork
    {
        public ReadOnlyCollection<UInt32> Structure;
        public NeuralSection[] Sections;

        public NeuralNetwork() { }

        public NeuralNetwork(params UInt32[] structure)
        {
            this.Structure = new List<UInt32>(structure).AsReadOnly();
            Sections = new NeuralSection[structure.Length - 1];
            for (int i = 0; i < Sections.Length; i++)
                Sections[i] = new NeuralSection(structure[i], structure[i + 1]);
        }

        public NeuralNetwork(NeuralNetwork basis)
        {
            Structure = basis.Structure;
            Sections = new NeuralSection[Structure.Count - 1];
            for (int i = 0; i < Sections.Length; i++)
                Sections[i] = new NeuralSection(basis.Sections[i]);
        }

        public double[] FeedForward(double[] input)
        {
            double[] output = input;
            for (int i = 0; i < Sections.Length; i++)
                output = Sections[i].FeedForward(output);
            return output;
        }

        public NeuralNetwork Mutate(double probability, double amount)
        {
            for (int i = 0; i < Sections.Length; i++)
                Sections[i].Mutate(probability, amount);
            return this;
        }

        public static NeuralNetwork Crossover(NeuralNetwork parent1, NeuralNetwork parent2, double uniformRate)
        {
            NeuralNetwork child = new NeuralNetwork(parent1);
            for (int i = 0; i < child.Sections.Length; i++)
            {
                double[][] weights = child.Sections[i].Weights;
                for (int k = 0; k < weights.Length; k++)
                    for (int m = 0; m < weights[k].Length; m++)
                        if (NeuralMath.RandomDouble() > uniformRate)
                            weights[k][m] = parent2.Sections[i].Weights[k][m];
            }
            return child;
        }
    }
}
