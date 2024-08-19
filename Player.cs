using MazeRunnerGenericAlg;
using Raylib_cs;
using System.Numerics;

public class Player
{
    public DNA playerDNA;
    public Vector2 startingPos;
    public bool endOfMovement;
    public bool victory;

    private int dnaIterator = 0;
    private int x; // X position in pixels
    private int y; // Y position in pixels
    private int size; // Size of the player
    private Color color; // Color of the player
    private int cellSize; // Size of each grid cell

    public event Action<DNA> OnWin;

    /// <summary>
    /// Creates new player with DNA and in concrete position
    /// </summary>
    /// <param name="gridRows"></param>
    /// <param name="gridCols"></param>
    /// <param name="cellSize"></param>
    /// <param name="blocks"></param>
    /// <param name="dna"></param>
    /// <param name="startingPos"></param>
    public Player(int gridRows, int gridCols, int cellSize, List<Block> blocks, DNA dna, Vector2 startingPos)
    {
        playerDNA = dna;
        dnaIterator = 0;
        endOfMovement = false;
        victory= false;

        // Initialize player's size and random color
        size = (int)(cellSize * 0.75);
        color = GetRandomColor();
        this.cellSize = cellSize;

        // Start player in the center of the grid, ensuring it's not on a block      
        bool positionFound = false;

        while (!positionFound)
        {
            // Center the player in the starting cell
            x = (int)startingPos.X * cellSize + (cellSize - size) / 2;
            y = (int)startingPos.Y * cellSize + (cellSize - size) / 2;

            // Check if the chosen position collides with any block, if collides - move player starting position
            positionFound = true;
            foreach (var block in blocks)
            {
                if (block.IsCollidingWithPlayer(x, y, size))
                {
                    positionFound = false;
                    startingPos.X++;
                    startingPos.Y++;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Updates player position, using DNA as template for movements. Checks collisions.
    /// </summary>
    /// <param name="blocks"></param>
    /// <param name="target"></param>
    /// <param name="screenWidth"></param>
    /// <param name="screenHeight"></param>
    public void Update(List<Block> blocks, Target target, int screenWidth, int screenHeight)
    {
        int oldX = x;
        int oldY = y;

        if (dnaIterator >= playerDNA.genes.Count)
        {
            endOfMovement = true;
        }
        else
        {
            // Get the next move character
            char move = playerDNA.genes[dnaIterator];

            // Perform movement based on the DNA character
            if (move == 'W') y -= cellSize; // Up
            if (move == 'S') y += cellSize; // Down
            if (move == 'A') x -= cellSize; // Left
            if (move == 'D') x += cellSize; // Right
                                            // Space means no movement

            // Increment the iterator for the next movement
            dnaIterator++;
        }

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
        if (target.IsCollidingWithPlayer(x + size / 2, y + size / 2, size) && endOfMovement)
        {
            victory = true;
        }      
    }

    public string ToString(int nr)
    {
        return $"{nr}. Player with DNA: {playerDNA.ToString()} | Fitness: {playerDNA.fitness.ToString()} | Position - x:{x} y:{y}";
    }

    public void Draw(int number)
    {
        // Draw the rectangle
        Raylib.DrawRectangle(x, y, size, size, color);

        // Convert the number to a string
        string numberText = number.ToString();

        // Calculate the position to center the text inside the rectangle
        int textWidth = Raylib.MeasureText(numberText, 20); // 20 is the font size
        int textX = x + (size - textWidth) / 2; // Center horizontally
        int textY = y + (size - 20) / 2; // Center vertically, 20 is the font size

        // Draw the number centered on the rectangle
        Raylib.DrawText(numberText, textX, textY, 20, Color.Black);
    }

    public Vector2 GetCurrentPosition() { return new Vector2(x, y); }
    private static Color GetRandomColor()
    {
        // Create a random number generator
        Random random = new Random();

        // Generate random values for each color channel and alpha
        byte r = (byte)random.Next(256);
        byte g = (byte)random.Next(256);
        byte b = (byte)random.Next(256);
        byte a = 255; // Fully opaque

        // Return a Color object with the random values
        return new Color(r, g, b, a);
    }
}