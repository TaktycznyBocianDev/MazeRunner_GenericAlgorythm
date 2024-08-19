using Raylib_cs;
using System.Numerics;

public class Target
{
    public int X { get; private set; } // X position in pixels
    public int Y { get; private set; } // Y position in pixels
    private int radius; 
    private Color color;

    /// <summary>
    /// Creates a new target in a random position within the grid, avoiding blocks.
    /// </summary>
    /// <param name="gridRows">Number of rows in the grid.</param>
    /// <param name="gridCols">Number of columns in the grid.</param>
    /// <param name="cellSize">Size of each cell in the grid.</param>
    /// <param name="blocks">List of existing blocks to avoid when placing the target.</param>
    public Target(int gridRows, int gridCols, int cellSize, List<Block> blocks)
    {
        radius = cellSize / 2 - 1; // Slightly smaller than the cell
        color = Color.Brown;

        PlaceInUniquePosition(gridRows, gridCols, cellSize, blocks);
    }

    /// <summary>
    /// Creates a new target at a specified position, ensuring it doesn't overlap with blocks.
    /// </summary>
    /// <param name="gridRows">Number of rows in the grid.</param>
    /// <param name="gridCols">Number of columns in the grid.</param>
    /// <param name="cellSize">Size of each cell in the grid.</param>
    /// <param name="blocks">List of existing blocks to avoid when placing the target.</param>
    /// <param name="targetPosition">Specified position for the target.</param>
    public Target(int gridRows, int gridCols, int cellSize, List<Block> blocks, Vector2 targetPosition)
    {
        radius = cellSize / 2 - 1; // Slightly smaller than the cell
        color = Color.Brown;

        PlaceAtSpecifiedPosition(targetPosition, cellSize, blocks);
    }

    /// <summary>
    /// Places the target in a unique position within the grid, avoiding blocks.
    /// </summary>
    /// <param name="gridRows">Number of rows in the grid.</param>
    /// <param name="gridCols">Number of columns in the grid.</param>
    /// <param name="cellSize">Size of each cell in the grid.</param>
    /// <param name="blocks">List of existing blocks to avoid when placing the target.</param>
    private void PlaceInUniquePosition(int gridRows, int gridCols, int cellSize, List<Block> blocks)
    {
        Random random = new Random();
        bool positionFound = false;

        while (!positionFound)
        {
            int targetRow = random.Next(0, gridRows);
            int targetCol = random.Next(0, gridCols);

            X = targetCol * cellSize + cellSize / 2;
            Y = targetRow * cellSize + cellSize / 2;

            // Check if the position is free of blocks
            positionFound = !blocks.Exists(block => block.IsOccupying(X - cellSize / 2, Y - cellSize / 2));
        }
    }

    /// <summary>
    /// Places the target at a specified position, ensuring it doesn't overlap with blocks.
    /// If the position overlaps with a block, the target is shifted to a nearby position.
    /// </summary>
    /// <param name="targetPosition">The specified position for the target.</param>
    /// <param name="cellSize">Size of each cell in the grid.</param>
    /// <param name="blocks">List of existing blocks to avoid when placing the target.</param>
    private void PlaceAtSpecifiedPosition(Vector2 targetPosition, int cellSize, List<Block> blocks)
    {
        bool positionFound = false;

        while (!positionFound)
        {
            X = (int)targetPosition.X * cellSize + cellSize / 2;
            Y = (int)targetPosition.Y * cellSize + cellSize / 2;

            // Check if the position is free of blocks
            positionFound = !blocks.Exists(block => block.IsOccupying(X - cellSize / 2, Y - cellSize / 2));

            if (!positionFound)
            {
                targetPosition.X += 1;
                targetPosition.Y += 1;
            }
        }
    }
   
    /// <summary>
    /// Draws the target on the screen.
    /// </summary>
    public void Draw()
    {
        Raylib.DrawCircle(X, Y, radius, color);
    }
   
    /// <summary>
    /// Checks if the target is colliding with the player.
    /// </summary>
    /// <param name="playerX">The player's X position.</param>
    /// <param name="playerY">The player's Y position.</param>
    /// <param name="playerSize">The player's size.</param>
    /// <returns>True if the player collides with the target, otherwise false.</returns>
    public bool IsCollidingWithPlayer(int playerX, int playerY, int playerSize)
    {
        int dx = X - playerX;
        int dy = Y - playerY;
        int distanceSquared = dx * dx + dy * dy;
        int combinedRadius = radius + playerSize / 2;
        return distanceSquared <= combinedRadius * combinedRadius;
    }

    /// <summary>
    /// Gets the current position of the target as a Vector2.
    /// </summary>
    /// <returns>The current position of the target.</returns>
    public Vector2 GetCurrentPosition() { return new Vector2(X, Y); }
}