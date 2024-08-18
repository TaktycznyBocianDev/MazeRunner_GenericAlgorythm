using Raylib_cs;

class Player
{
    private int x; // X position in pixels
    private int y; // Y position in pixels
    private int size; // Size of the player
    private Color color; // Color of the player
    private int cellSize; // Size of each grid cell

    // Constructor
    public Player(int gridRows, int gridCols, int cellSize, List<Block> blocks)
    {
        // Initialize player's size and color
        size = (int)(cellSize * 0.75); // 3/4 of the cell size
        color = Color.Red;
        this.cellSize = cellSize;

        // Start player in the center of the grid, ensuring it's not on a block
        bool positionFound = false;
        Random random = new Random();

        while (!positionFound)
        {
            // Randomly choose a position in the grid
            int startRow = random.Next(0, gridRows);
            int startCol = random.Next(0, gridCols);

            // Center the player in the chosen cell
            x = startCol * cellSize + (cellSize - size) / 2;
            y = startRow * cellSize + (cellSize - size) / 2;

            // Check if the chosen position collides with any block
            positionFound = true;
            foreach (var block in blocks)
            {
                if (block.IsCollidingWithPlayer(x, y, size))
                {
                    positionFound = false;
                    break;
                }
            }
        }
    }

    // Update the player's position based on input and check for collisions
    public void Update(List<Block> blocks, Target target, int screenWidth, int screenHeight)
    {
        int oldX = x;
        int oldY = y;

        // Move player based on WASD keys, one cell at a time
        if (Raylib.IsKeyPressed(KeyboardKey.W)) y -= cellSize; // Up
        if (Raylib.IsKeyPressed(KeyboardKey.S)) y += cellSize; // Down
        if (Raylib.IsKeyPressed(KeyboardKey.A)) x -= cellSize; // Left
        if (Raylib.IsKeyPressed(KeyboardKey.D)) x += cellSize; // Right

        // Ensure the player stays within the grid boundaries
        x = Math.Clamp(x, 0, screenWidth - size);
        y = Math.Clamp(y, 0, screenHeight - size);

        // Align player to the center of the cell
        x = (x / cellSize) * cellSize + (cellSize - size) / 2;
        y = (y / cellSize) * cellSize + (cellSize - size) / 2;

        // Check collision with blocks
        foreach (var block in blocks)
        {
            if (block.IsCollidingWithPlayer(x, y, size))
            {
                // If collision, revert to old position
                x = oldX;
                y = oldY;
                break;
            }
        }

        // Check collision with target
        if (target.IsCollidingWithPlayer(x + size / 2, y + size / 2, size))
        {
            Console.WriteLine("You won!"); // Placeholder action
            // You can trigger a win state here
        }
    }

    // Draw the player
    public void Draw()
    {
        Raylib.DrawRectangle(x, y, size, size, color);
        // Later, you can replace this with a DrawTexture call if you want to use a PNG texture for the player
    }
}