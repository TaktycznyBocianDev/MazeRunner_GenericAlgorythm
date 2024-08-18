using Raylib_cs;

class Target
{
    private int x; // X position in pixels
    private int y; // Y position in pixels
    private int radius; // Radius of the target
    private Color color; // Color of the target

    // Constructor
    public Target(int gridRows, int gridCols, int cellSize, List<Block> blocks)
    {
        // Initialize target's size and color
        radius = cellSize / 2 - 1; // Slightly smaller than the cell
        color = Color.Brown;

        // Generate random position within the grid
        Random random = new Random();
        bool positionFound = false;

        while (!positionFound)
        {
            int targetRow = random.Next(0, gridRows);
            int targetCol = random.Next(0, gridCols);

            x = targetCol * cellSize + cellSize / 2;
            y = targetRow * cellSize + cellSize / 2;

            // Check if the position is free of blocks
            positionFound = true;
            foreach (var block in blocks)
            {
                if (block.IsOccupying(x - cellSize / 2, y - cellSize / 2))
                {
                    positionFound = false;
                    break;
                }
            }
        }
    }

    // Draw the target
    public void Draw()
    {
        Raylib.DrawCircle(x, y, radius, color);
    }

    // Check if the player collides with the target
    public bool IsCollidingWithPlayer(int playerX, int playerY, int playerSize)
    {
        int dx = x - playerX;
        int dy = y - playerY;
        int distanceSquared = dx * dx + dy * dy;
        int combinedRadius = radius + playerSize / 2;
        return distanceSquared <= combinedRadius * combinedRadius;
    }
}