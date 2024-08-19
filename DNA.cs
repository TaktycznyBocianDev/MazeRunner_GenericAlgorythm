using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeRunnerGenericAlg
{
    public class DNA
    {
        public List<char> genes = new List<char>();
        public int fitness = 0;

        /// <summary>
        /// Constructor for creating new random DNA with definied lenght
        /// </summary>
        /// <param name="genePool"></param>
        /// <param name="initialDnaLenght"></param>
        public DNA(string genePool, int initialDnaLenght)
        {
            for (int i = 0; i < initialDnaLenght; i++)
            {
                genes.Add(GetRandomCharacter(genePool, new Random()));
            }
        }

        /// <summary>
        /// Constructor for creating DNA from pre existing list of chars
        /// </summary>
        /// <param name="chars"></param>
        public DNA(List<char> chars)
        {
            this.genes = chars;
        }

        /// <summary>
        /// Fitness calculated by distance between player and target. Additionally, approaching to target is taken into consideration".
        /// </summary>
        /// <param name="player"></param>
        /// <param name="target"></param>
        /// <param name="maxDnaLenght"></param>
        public void CalculateFitness(Player player, Target target, int maxDnaLenght)
        {
            float distance = CalculateDistance(player.GetCurrentPosition().X, player.GetCurrentPosition().Y, target.GetCurrentPosition().X, target.GetCurrentPosition().Y);
            fitness = (int)distance;

            if (player.Victory) fitness += 100; //Penalty for winning, and then escaping (if win occured, there is no more fitness anyway)
            fitness += player.DistanceGain;

            if(fitness <= 0) fitness = int.MaxValue;
        }

        /// <summary>
        /// Changes gene in dna, according to mutation rate.
        /// </summary>
        /// <param name="mutationRate"></param>
        /// <param name="genePool"></param>
        /// <param name="rnd"></param>
        public void Mutate(double mutationRate, string genePool, Random rnd)
        {
            for (int i = 0; i < genes.Count; i++)
            {
                if (rnd.NextDouble() < mutationRate)
                {
                    genes[i] = GetRandomCharacter(genePool, new Random());
                }

            }
        }

        /// <summary>
        /// Add new gene to dna sequence 
        /// </summary>
        /// <param name="genePool"></param>
        /// <param name="maxDnaLenght"></param>
        public void ProlongDNA(string genePool, int maxDnaLenght)
        {
            genes.Add(GetRandomCharacter(genePool, new Random()));
        }

        public override string ToString()
        {
            return new string(genes.ToArray());
        }

        private char GetRandomCharacter(string text, Random rng)
        {
            int index = rng.Next(text.Length);
            return text[index];
        }
        private float CalculateDistance(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return MathF.Sqrt(dx * dx + dy * dy);
        }
    }
}
