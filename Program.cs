using MazeRunnerGenericAlg;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

class Program
{

    /// <summary>
    /// This program generate level - grid with obstacles, player and target. Grid size is modified by seting screen 
    /// size and size of cells.
    /// </summary>
    static void Main()
    {
        bool gameStarted = false;

        const string GENEPOOL = "WASD ";
        const int SCREENWIDTH = 860;
        const int SCREENHEIGHT = 860;
        const int CELLSIZE = 20;
        const int BLOCKCOUNT = 30;
        const double MUTATIONRATE = 0.01;
        const double EVOLUTIONRATE = 0.95;
        const int MAXPOPULATION = 40;
        const int MAXGENECOUNT = 2000;
        const int STARTGENECOUNT = 5;
        const float MATERANGE = 1.05f;
        const float WAITTIMEINSEC = 0.03f;
        Vector2 STARTPOSITION = new Vector2(0, 0);
        Vector2 TARGETPOSITION = new Vector2((SCREENWIDTH - 50)/CELLSIZE, (SCREENHEIGHT - 50)/CELLSIZE);
        const bool USETARGETPOSITION = true;
        const int DNAPROLOGNATIONRATE = 5;

        int generationsCounter = 1;
        DNA? victoryDNA;
        Raylib.InitWindow(SCREENWIDTH, SCREENHEIGHT, "Kirunia's treat");

        int gridRows = SCREENHEIGHT / CELLSIZE;
        int gridCols = SCREENWIDTH / CELLSIZE;

        // Declare and initialize blocks, target, and players population for the first time
        List<Block> blocks;
        Target target;
        PlayersPopulation playersPopulation;


        InitializeGame(out blocks, out target, out playersPopulation);
        void InitializeGame(out List<Block> blocks, out Target target, out PlayersPopulation playersPopulation)
        {
            // Create a list to store blocks
            blocks = new List<Block>();

            // Number of blocks to create
            int blockCount = BLOCKCOUNT;

            // Initialize blocks
            for (int i = 0; i < blockCount; i++)
            {
                Block block = new Block(gridRows, gridCols, CELLSIZE, blocks);
                blocks.Add(block);
            }

            // Create a target ensuring it doesn't overlap with any block
            if(USETARGETPOSITION)
            {
                target = new Target(gridRows, gridCols, CELLSIZE, blocks, TARGETPOSITION);
            }
            else
            {
                target = new Target(gridRows, gridCols, CELLSIZE, blocks);
            }
            

            // Initialize the population object
            playersPopulation = new PlayersPopulation
                (target, GENEPOOL, MUTATIONRATE, EVOLUTIONRATE, MAXPOPULATION,
                MAXGENECOUNT, STARTGENECOUNT, gridRows, gridCols, CELLSIZE, blocks, STARTPOSITION, MATERANGE, DNAPROLOGNATIONRATE);


        }

        // Function to initialize blocks, target, and player
        Player winner = new Player(gridRows, gridCols, CELLSIZE, blocks, new DNA(GENEPOOL, STARTGENECOUNT), STARTPOSITION, target);
        bool isWinnerSet = false;
        bool stopDravingPlayers = false;

        playersPopulation.InitializePopulation();

        // Main game loop
        while (!Raylib.WindowShouldClose()) // Detect window close button or ESC key
        {

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);

            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {

                gameStarted = true;

            }

            if (gameStarted)
            {
                // Check for 'R' key press to restart the game
                if (Raylib.IsKeyPressed(KeyboardKey.R))
                {
                    InitializeGame(out blocks, out target, out playersPopulation);
                    playersPopulation.InitializePopulation();
                    isWinnerSet = false;
                    stopDravingPlayers = false;
                    generationsCounter = 1;
                }

                foreach (Player player in playersPopulation.population)
                {
                    player.Update(blocks, target, SCREENWIDTH, SCREENHEIGHT);
                }

                // Draw the grid
                for (int row = 0; row < gridRows; row++)
                {
                    for (int col = 0; col < gridCols; col++)
                    {
                        // Calculate the position of each cell
                        int x = col * CELLSIZE;
                        int y = row * CELLSIZE;

                        // Draw the cell as a white square with a black border
                        Raylib.DrawRectangle(x, y, CELLSIZE, CELLSIZE, Color.White);
                        Raylib.DrawRectangleLines(x, y, CELLSIZE, CELLSIZE, Color.Black);
                    }
                }

                // Draw the blocks
                foreach (var block in blocks)
                {
                    block.Draw();
                }

                // Draw the target
                target.Draw();

                if (!stopDravingPlayers)
                {
                    // Draw the player
                    for (int i = 0; i < playersPopulation.population.Length; i++)
                    {
                        Player player = playersPopulation.population[i];
                        player.Draw(i);
                    }
                }


                if (playersPopulation.IsMovementFinished())
                {
                    playersPopulation.SetFitnessForEachPlayer();
                    playersPopulation.SelectTheFittest();

                    if (!playersPopulation.CheckVictory(out victoryDNA))
                    {

                        DescribePop(playersPopulation, generationsCounter);
                        playersPopulation.CreateNewGeneration();
                        generationsCounter++;
                    }
                    else
                    {
                        if (!isWinnerSet)
                        {
                            winner = new Player(gridRows, gridCols, CELLSIZE, blocks, victoryDNA, STARTPOSITION, target);
                            isWinnerSet = true;
                            stopDravingPlayers = true;
                            winner.Victory = false;
                        }
                        else
                        {
                            winner.Update(blocks, target, SCREENWIDTH, SCREENHEIGHT);
                            winner.Draw(0);
                            if (winner.Victory) isWinnerSet = false;
                            Raylib.WaitTime(WAITTIMEINSEC + 0.1);
                        }
                        DrawInfo(winner.PlayerDNA, generationsCounter);

                    }

                }
                Raylib.WaitTime(WAITTIMEINSEC);

                
            }
            else
            {
                int screenPadding = 10;
                int fontSize = 20;
                Raylib.DrawText("Left click mouse button to start.", screenPadding, screenPadding, fontSize, Color.Blue);
            }

            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }

    static void DescribePop(PlayersPopulation playersPopulation, int generationsCounter)
    {
        int bestFitness = 0;
        int averageFitness = 0;
        int averageDnaLenght = 0;
        for (int i = 0; i < playersPopulation.population.Length; i++)
        {
            Player player = playersPopulation.population[i];
            Console.WriteLine(i.ToString() + "" + player.ToString());
            averageFitness += player.PlayerDNA.fitness;
            averageDnaLenght += player.PlayerDNA.genes.Count;

            if (bestFitness == 0)
            {
                bestFitness = player.PlayerDNA.fitness;
            }
            if (bestFitness != 0 && player.PlayerDNA.fitness < bestFitness)
            {
                bestFitness = player.PlayerDNA.fitness;
            }
        }
        averageFitness /= playersPopulation.population.Length;
        averageDnaLenght /= playersPopulation.population.Length;
        Console.WriteLine("Best fitness: " + bestFitness.ToString());
        Console.WriteLine("Average fitness: " + averageFitness.ToString());
        Console.WriteLine("Average DNA lenght: " + averageDnaLenght.ToString());
        Console.WriteLine("Current generation: " + generationsCounter.ToString());
    }

    static public void DrawInfo(DNA bestDNA, int generationsCounter)
    {
        int screenPadding = 10;
        int fontSize = 20;
        int lineHeight = 25;
        Raylib.DrawText("Current generation: " + (generationsCounter - 1).ToString(), screenPadding, screenPadding, fontSize, Color.Blue);
        Raylib.DrawText("Winner DNA: " + new string(bestDNA.genes.ToArray()), screenPadding, screenPadding + lineHeight, fontSize, Color.Green);
    }

}