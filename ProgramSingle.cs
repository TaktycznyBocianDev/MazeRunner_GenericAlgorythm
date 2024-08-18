using Raylib_cs;
using System;
using System.Collections.Generic;

class ProgramSingle
{

    /// <summary>
    /// This is version for one player - as a safe copy
    /// </summary>
    static void MainSingle()
    {
        // Initialize the Raylib window
        int screenWidth = 800;
        int screenHeight = 800;
        Raylib.InitWindow(screenWidth, screenHeight, "2D Grid with Blocks, Target, and Player");

        // Define grid dimensions based on screen size and cell size
        int cellSize = 50; // Size of each cell
        int gridRows = screenHeight / cellSize;
        int gridCols = screenWidth / cellSize;

        // Function to initialize blocks, target, and player
        void InitializeGame(out List<Block> blocks, out Target target, out Player player)
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

            // Initialize the player ensuring it doesn't overlap with any block
            player = new Player(gridRows, gridCols, cellSize, blocks);
        }

        // Initialize blocks, target, and player for the first time
        List<Block> blocks;
        Target target;
        Player player;
        InitializeGame(out blocks, out target, out player);

        // Main game loop
        while (!Raylib.WindowShouldClose()) // Detect window close button or ESC key
        {
            // Check for 'R' key press to restart the game
            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                InitializeGame(out blocks, out target, out player);
            }

            // Update player movement
            player.Update(blocks, target, screenWidth, screenHeight);

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
            player.Draw();

            // End drawing
            Raylib.EndDrawing();
        }

        // De-initialize Raylib window
        Raylib.CloseWindow();
    }
}