using Raylib_cs;

public class Block
{
    public int x; // X position in pixels
    public int y; // Y position in pixels
    private int size; // Size of the block
    private Color color; // Color of the block

    // Constructor
    public Block(int gridRows, int gridCols, int cellSize, List<Block> existingBlocks)
    {
        // Initialize block's size and color
        size = cellSize;
        color = Color.Black;

        // Ensure that each block is placed in a unique position
        Random random = new Random();
        bool positionFound = false;

        while (!positionFound)
        {
            int blockRow = random.Next(0, gridRows);
            int blockCol = random.Next(0, gridCols);

            x = blockCol * cellSize;
            y = blockRow * cellSize;

            // Check if this position is already occupied by another block
            positionFound = true;
            foreach (var block in existingBlocks)
            {
                if (block.x == x && block.y == y)
                {
                    positionFound = false;
                    break;
                }
            }
        }
    }

    // Check if this block occupies the given grid cell
    public bool IsOccupying(int targetX, int targetY)
    {
        return x == targetX && y == targetY;
    }

    // Check if the player collides with this block
    public bool IsCollidingWithPlayer(int playerX, int playerY, int playerSize)
    {
        return playerX < x + size && playerX + playerSize > x && playerY < y + size && playerY + playerSize > y;
    }

    // Draw the block
    public void Draw()
    {
        Raylib.DrawRectangle(x, y, size, size, color);
    }
}