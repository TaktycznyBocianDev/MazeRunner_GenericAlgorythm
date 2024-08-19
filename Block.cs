using Raylib_cs;

public class Block
{
    public int X { get; private set; } // X position in pixels
    public int Y { get; private set; } // Y position in pixels

    private int size;
    private Color color;

    /// <summary>
    /// Creates a new block (obstacle) and place it at a random, unique position on the grid.
    /// </summary>
    /// <param name="gridRows">Number of rows in the grid.</param>
    /// <param name="gridCols">Number of columns in the grid.</param>
    /// <param name="cellSize">Size of each cell in pixels.</param>
    /// <param name="existingBlocks">List of existing blocks to ensure unique positions.</param>
    public Block(int gridRows, int gridCols, int cellSize, List<Block> existingBlocks)
    {
        this.size = cellSize;
        this.color = Color.Black;
        PlaceInUniquePosition(gridRows, gridCols, existingBlocks);
    }

    /// <summary>
    /// Places the block in a unique position within the grid, avoiding overlap with existing blocks.
    /// </summary>
    /// <param name="gridRows">Number of rows in the grid.</param>
    /// <param name="gridCols">Number of columns in the grid.</param>
    /// <param name="existingBlocks">List of existing blocks to check for overlaps.</param>
    private void PlaceInUniquePosition(int gridRows, int gridCols, List<Block> existingBlocks)
    {
        Random random = new Random();
        bool positionFound = false;

        while (!positionFound)
        {
            int blockRow = random.Next(0, gridRows);
            int blockCol = random.Next(0, gridCols);

            X = blockCol * size;
            Y = blockRow * size;

            positionFound = !existingBlocks.Exists(block => block.IsOccupying(X, Y));
        }
    }

    /// <summary>
    /// Determines if the block is occupying a specific grid position.
    /// </summary>
    /// <param name="targetX">The X position to check.</param>
    /// <param name="targetY">The Y position to check.</param>
    /// <returns>True if the block occupies the position, otherwise false.</returns>
    public bool IsOccupying(int targetX, int targetY)
    {
        return X == targetX && Y == targetY;
    }

    /// <summary>
    /// Checks if the block is colliding with a player based on their position and size.
    /// </summary>
    /// <param name="playerX">The player's X position.</param>
    /// <param name="playerY">The player's Y position.</param>
    /// <param name="playerSize">The player's size.</param>
    /// <returns>True if the player collides with the block, otherwise false.</returns>
    public bool IsCollidingWithPlayer(int playerX, int playerY, int playerSize)
    {
        return playerX < X + size && playerX + playerSize > X && playerY < Y + size && playerY + playerSize > Y;
    }

    /// <summary>
    /// Draws the block on the screen.
    /// </summary>
    public void Draw()
    {
        Raylib.DrawRectangle(X, Y, size, size, color);
    }
}