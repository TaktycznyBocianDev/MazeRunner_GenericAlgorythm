using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MazeRunnerGenericAlg
{
    public class PlayersPopulation
    {

        public Player[] population;
        public List<Player> matingPool;

        Target target;
        private string genePool;
        private double mutationRate;
        private double evoRate;
        private int maxPopulation;
        private int maxGenesCount;
        private int startGenesCount;
        private int gridRows;
        private int gridCols;
        private int cellSize;
        private List<Block> blocks;
        private DNA dna;
        private Vector2 startingPos;

        public PlayersPopulation(Target target, string genePool, double mutationRate, double evoRate, int maxPopulation, int maxGenesCount, int startGenesCount, int gridRows, int gridCols, int cellSize, List<Block> blocks, DNA dna, Vector2 startingPos)
        {
            this.evoRate = evoRate;
            this.target = target;
            this.genePool = genePool;
            this.mutationRate = mutationRate;
            this.maxPopulation = maxPopulation;
            this.maxGenesCount = maxGenesCount;
            this.startGenesCount = startGenesCount;
            this.gridRows = gridRows;
            this.gridCols = gridCols;
            this.cellSize = cellSize;
            this.blocks = blocks;
            this.dna = dna;
            this.startingPos = startingPos;

            this.population = new Player[this.maxPopulation];
            this.matingPool = new List<Player>();
        }

        public void InitializePopulation()
        {
            for (int i = 0; i < maxPopulation; i++)
            {
                population[i] = new Player(gridRows, gridCols, cellSize, blocks, new DNA(genePool, startGenesCount), startingPos);
            }
        }

        public void SetFitnessForEachPlayer()
        {
            foreach (Player player in population)
            {
                player.playerDNA.CalculateFitness(player, target, maxGenesCount);
            }
        }

        public void SelectTheFittest()
        {
            matingPool.Clear();

            float bestFitness = float.MaxValue;

            foreach (Player player in population)
            {
                
                if (player.playerDNA.fitness < bestFitness)
                {
                    bestFitness = player.playerDNA.fitness;
                }
            }

            float maxRange = bestFitness * 2;

            foreach (Player player in population)
            {
                if (player.playerDNA.fitness <= maxRange)
                {
                    matingPool.Add(player);
                }
            }
        }

        public DNA CrossMutationDNA()
        {
            Random rnd = new Random();
            Player parentUno = matingPool[rnd.Next(matingPool.Count())];
            Player parentDos = matingPool[rnd.Next(matingPool.Count())];

            int countUno = parentUno.playerDNA.genes.Count/2;
            int countDos = parentDos.playerDNA.genes.Count/2;

            List<char> firsSequence = new List<char>();
            List<char> secondSequence = new List<char>();
            
            for (int i = 0;i < countUno; i++)
            {
                firsSequence.Add(parentUno.playerDNA.genes[i]);
            }
            for (int i = 0; i < countDos; i++)
            {
                secondSequence.Add(parentDos.playerDNA.genes[i]);
            }

            List<char> finalSequence = [.. firsSequence, .. firsSequence];

            DNA newDNA = new DNA(finalSequence);
            newDNA.Mutate(mutationRate, genePool, new Random());

            if (rnd.NextDouble() < evoRate)
            {
                newDNA.ProlongDNA(genePool, maxGenesCount);
            }

            return newDNA;

        }

        public void CreateNewGeneration()
        {

            population = new Player[this.maxPopulation];

            for (int i = 0; i < maxPopulation; i++)
            {
                population[i] = new Player(gridRows, gridCols, cellSize, blocks, CrossMutationDNA(), startingPos);
            }

        }




    }
}
