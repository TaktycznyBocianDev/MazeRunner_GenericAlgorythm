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
        public bool victory = false;
        public DNA victoryDNA;

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
        private Vector2 startingPos;
        private float mateRange;

        /// <summary>
        /// Allows to create full initial and later generations. Will be good to remove some params, but it is what it is. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="genePool"></param>
        /// <param name="mutationRate"></param>
        /// <param name="evoRate"></param>
        /// <param name="maxPopulation"></param>
        /// <param name="maxGenesCount"></param>
        /// <param name="startGenesCount"></param>
        /// <param name="gridRows"></param>
        /// <param name="gridCols"></param>
        /// <param name="cellSize"></param>
        /// <param name="blocks"></param>
        /// <param name="dna"></param>
        /// <param name="startingPos"></param>
        public PlayersPopulation(Target target, string genePool, double mutationRate, double evoRate, int maxPopulation, int maxGenesCount, int startGenesCount, int gridRows, int gridCols, int cellSize, List<Block> blocks, Vector2 startingPos, float mateRange)
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
            this.startingPos = startingPos;
            this.population = new Player[this.maxPopulation];
            this.matingPool = new List<Player>();
           
            if (mateRange <= 1) mateRange = 1.1f;
            this.mateRange = mateRange;
        }

        /// <summary>
        /// Creates population of players in same starting point, but with difrent, random DNA.
        /// </summary>
        public void InitializePopulation()
        {
            for (int i = 0; i < maxPopulation; i++)
            {
                population[i] = new Player(gridRows, gridCols, cellSize, blocks, new DNA(genePool, startGenesCount), startingPos);               
            }
        }


        /// <summary>
        /// For each player calculates fitness for it's DNA with proper function from DNA class.
        /// </summary>
        public void SetFitnessForEachPlayer()
        {
            foreach (Player player in population)
            {
                player.playerDNA.CalculateFitness(player, target, maxGenesCount);
            }
        }

        /// <summary>
        /// Selects the fittest players, using best fitness as base, multiplay it by 2, and adding players to mating pool if their fittness is in this range.
        /// </summary>
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

        /// <summary>
        /// Performs operations on DNA in order to create new DNA for new generation.
        /// </summary>
        /// <returns>DNA</returns>
        public DNA CrossMutationDNA()
        {
            Random rnd = new Random();
            Player parentUno = matingPool[rnd.Next(matingPool.Count)];
            Player parentDos = matingPool[rnd.Next(matingPool.Count)];
            Player parentTres = matingPool[rnd.Next(matingPool.Count)];
            Player parentQuatro = matingPool[rnd.Next(matingPool.Count)];

            int countUno = parentUno.playerDNA.genes.Count / 4;
            int countDos = parentDos.playerDNA.genes.Count / 4;
            int countTres = parentTres.playerDNA.genes.Count / 4;
            int countQuatro = parentQuatro.playerDNA.genes.Count / 4;

            List<char> finalSequence = new List<char>();

            finalSequence.AddRange(parentUno.playerDNA.genes.Take(countUno));

            finalSequence.AddRange(parentDos.playerDNA.genes.Skip(countDos).Take(countDos));

            finalSequence.AddRange(parentTres.playerDNA.genes.Skip(2 * countTres).Take(countTres));

            finalSequence.AddRange(parentQuatro.playerDNA.genes.Skip(3 * countQuatro).Take(countQuatro));

            DNA newDNA = new DNA(finalSequence);
            newDNA.Mutate(mutationRate, genePool, new Random());

            if (rnd.NextDouble() <= evoRate)
            {
                for (int i = 0; i < 4; i++)
                {
                    newDNA.ProlongDNA(genePool, maxGenesCount);
                }             
            }

            return newDNA;

        }

        /// <summary>
        /// Creates new generation in place of old one.
        /// </summary>
        public void CreateNewGeneration()
        {
            population = new Player[this.maxPopulation];

            for (int i = 0; i < maxPopulation; i++)
            {
                population[i] = new Player(gridRows, gridCols, cellSize, blocks, CrossMutationDNA(), startingPos);
            }
        }

        /// <summary>
        /// Checks all players and return false if any of them still have movements
        /// </summary>
        /// <returns></returns>
        public bool IsMovementFinished()
        {
            int tmp = 0; //I assume there is still movement, so there are no finished players
            foreach (Player player in population)
            {
                if (player.endOfMovement) tmp++; //For each player that ends movement, add one more to counter
            }
            if (tmp != population.Length) return false; //If any player still moves, return false, as movement is not finished.
            return true; //if all ends
        }

        /// <summary>
        /// Checks if any of Players gets to target
        /// </summary>
        /// <param name="dna"></param>
        /// <returns>true if any player wins</returns>
        public bool CheckVictory(out DNA? dna)
        {
            foreach (Player player in population)
            {
                if (player.victory)
                {
                    dna = player.playerDNA;
                    return true;
                }
                    
            }
            dna = null;
            return false;
        }

        public float CalcAvrFitness()
        {
            float tmp = 0;
            foreach (Player player in population)
            {
                tmp += player.playerDNA.fitness;
            }
            return tmp / population.Length;

        }

        public float CalcAvrDNALenght()
        {
            float tmp = 0;
            foreach (Player player in population)
            {
                tmp += player.playerDNA.genes.Count;
            }
            return tmp / population.Length;
        }

        public int GetBestFitness()
        {
            int bestFitness = 0;
            foreach(Player player in population)
            {
                if (bestFitness == 0)
                {
                    bestFitness = player.playerDNA.fitness;
                }
                if (bestFitness != 0 && player.playerDNA.fitness < bestFitness)
                {
                    bestFitness = player.playerDNA.fitness;
                }
            }
            return bestFitness;
        }

    }
}
