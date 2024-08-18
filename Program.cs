using MazeRunnerGenericAlg;
using Raylib_cs;
using System;
using System.Collections.Generic;

class Program
{

    /// <summary>
    /// This program generate level - grid with obstacles, player and target. Grid size is modified by seting screen 
    /// size and size of cells.
    /// </summary>
    static void Main()
    {
        // Initialize the Raylib window
        int screenWidth = 800;
        int screenHeight = 800;
        Raylib.InitWindow(screenWidth, screenHeight, "Kirunia's treat");

        // Define grid dimensions based on screen size and cell size
        int cellSize = 50; // Size of each cell
        int gridRows = screenHeight / cellSize;
        int gridCols = screenWidth / cellSize;     

        // Initialize blocks, target, and player for the first time
        List<Block> blocks;
        Target target;
        Player[] players;
        InitializeGame(out blocks, out target, out players);

        // Function to initialize blocks, target, and player
        void InitializeGame(out List<Block> blocks, out Target target, out Player[] players)
        {
            // Create a list to store blocks
            blocks = new List<Block>();

            // Number of blocks to create
            int blockCount = 10;

            // Initialize blocks
            for (int i = 0; i < blockCount; i++)
            {
                Block block = new Block(gridRows, gridCols, cellSize, blocks);
                blocks.Add(block);
            }

            // Create a target ensuring it doesn't overlap with any block
            target = new Target(gridRows, gridCols, cellSize, blocks);

            DNA dna = new DNA(['a']);

            // Initialize the player ensuring it doesn't overlap with any block
            players =
            [
                new Player(gridRows, gridCols, cellSize, blocks, dna, new Player.StartingPos(1,1)),
                new Player(gridRows, gridCols, cellSize, blocks, dna, new Player.StartingPos(1,1)),
            ];
        }

        // Main game loop
        while (!Raylib.WindowShouldClose()) // Detect window close button or ESC key
        {
            // Check for 'R' key press to restart the game
            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                InitializeGame(out blocks, out target, out players);
            }

            // Update player movement
            foreach(Player player in players)
            {
                player.Update(blocks, target, screenWidth, screenHeight);
            }
            

            // Start drawing
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RayWhite);

            // Draw the grid
            for (int row = 0; row < gridRows; row++)
            {
                for (int col = 0; col < gridCols; col++)
                {
                    // Calculate the position of each cell
                    int x = col * cellSize;
                    int y = row * cellSize;

                    // Draw the cell as a white square with a black border
                    Raylib.DrawRectangle(x, y, cellSize, cellSize, Color.White);
                    Raylib.DrawRectangleLines(x, y, cellSize, cellSize, Color.Black);
                }
            }

            // Draw the blocks
            foreach (var block in blocks)
            {
                block.Draw();
            }

            // Draw the target
            target.Draw();

            // Draw the player
            foreach (Player player in players)
            {
                player.Draw();
            }
            
            // End drawing
            Raylib.EndDrawing();
        }

        // De-initialize Raylib window
        Raylib.CloseWindow();
    }
}