using MazeRunnerGenericAlg;
using Raylib_cs;
using System.Numerics;

public class Player
{
    public DNA PlayerDNA { get; private set; }
    public Vector2 StartingPos { get; private set; }
    public bool EndOfMovement { get; private set; }
    public int DistanceGain { get; private set; }
    public bool Victory { get; set; }

    private int dnaIterator = 0;
    private int x;
    private int y;
    private int size;
    private Color color;
    private int cellSize;

    private Target target;
    private int previousDistance;
    private int currentDistance;



    /// <summary>
    /// Initializes a new player with a given DNA and starting position.
    /// </summary>
    public Player(int gridRows, int gridCols, int cellSize, List<Block> blocks, DNA dna, Vector2 startingPos, Target target)
    {
        PlayerDNA = dna;
        EndOfMovement = false;
        Victory = false;

        this.cellSize = cellSize;
        size = (int)(cellSize * 0.75);
        color = GetRandomColor();

        StartingPos = FindValidStartingPosition(startingPos, blocks);
        x = (int)StartingPos.X;
        y = (int)StartingPos.Y;
        this.target = target;
    }

    /// <summary>
    /// Updates player position using DNA movements and checks collisions.
    /// </summary>
    public void Update(List<Block> blocks, Target target, int screenWidth, int screenHeight)
    {
        if (dnaIterator >= PlayerDNA.genes.Count)
        {
            EndOfMovement = true;
            return;
        }

        // Store old position for collision checks
        int oldX = x;
        int oldY = y;

        //Calculate dystance between player and target before movement
        previousDistance = (int)CalculateDistance(oldX, oldY, target.X, target.Y);

        // Update position based on current gene
        MovePlayer(PlayerDNA.genes[dnaIterator]);

        //Calculate dystance between player and target after movement
        currentDistance = (int)CalculateDistance(x, y, target.X, target.Y);

        //Set distance gain
        SetDistanceGain(previousDistance, currentDistance);

        // Increment DNA iterator
        dnaIterator++;

        // Ensure the player stays within boundaries
        ClampPosition(screenWidth, screenHeight);

        // Check for collisions and revert if necessary
        if (IsCollidingWithBlocks(blocks))
        {
            x = oldX;
            y = oldY;
        }

        // Check for victory condition
        if (target.IsCollidingWithPlayer(x + size / 2, y + size / 2, size))
        {
            Console.WriteLine("Target hit");
            Victory = true;
        }
    }

    /// <summary>
    /// Draw player instance with it's number on top
    /// </summary>
    /// <param name="number"></param>
    public void Draw(int number)
    {
        Raylib.DrawRectangle(x, y, size, size, color);

        string numberText = number.ToString();
        int textWidth = Raylib.MeasureText(numberText, 20);
        int textX = x + (size - textWidth) / 2;
        int textY = y + (size - 20) / 2;

        Raylib.DrawText(numberText, textX, textY, 20, Color.Black);
    }

    /// <summary>
    /// Finds place for player - uses starting position and check, if there is any colision with blocks. If there is any, it tries to place it next to set starting point.
    /// </summary>
    /// <param name="startingPos">Position where player will be placed.</param>
    /// <param name="blocks">Existing obstacles</param>
    /// <returns></returns>
    private Vector2 FindValidStartingPosition(Vector2 startingPos, List<Block> blocks)
    {
        while (true)
        {
            int startX = (int)startingPos.X * cellSize + (cellSize - size) / 2;
            int startY = (int)startingPos.Y * cellSize + (cellSize - size) / 2;

            if (!blocks.Exists(block => block.IsCollidingWithPlayer(startX, startY, size)))
            {
                return new Vector2(startX, startY);
            }

            startingPos.X++;
            startingPos.Y++;
        }
    }

    /// <summary>
    /// Moves player using provided gene from DNA (char). Space or any other character than WASD means no movement
    /// </summary>
    /// <param name="move">Char - gene from DNA</param>
    private void MovePlayer(char move)
    {
        switch (move)
        {
            case 'W':
                y -= cellSize;
                break;
            case 'S':
                y += cellSize;
                break;
            case 'A':
                x -= cellSize;
                break;
            case 'D':
                x += cellSize;
                break;
                // Space or any other character means no movement
        }
    }

    /// <summary>
    /// Stops player from going away from grid.
    /// </summary>
    /// <param name="screenWidth">Game window width</param>
    /// <param name="screenHeight">Game window height</param>
    private void ClampPosition(int screenWidth, int screenHeight)
    {
        x = Math.Clamp(x, 0, screenWidth - size);
        y = Math.Clamp(y, 0, screenHeight - size);

        x = (x / cellSize) * cellSize + (cellSize - size) / 2;
        y = (y / cellSize) * cellSize + (cellSize - size) / 2;
    }

    private bool IsCollidingWithBlocks(List<Block> blocks)
    {
        return blocks.Exists(block => block.IsCollidingWithPlayer(x, y, size));
    }

    /// <summary>
    /// Calculates Fistance Gain. If we are closer to target, we get small prize. If we are futher away - big penalty.
    /// </summary>
    /// <param name="previousDistance">Calculated before movement</param>
    /// <param name="currentDistance">Calculated after movement</param>
    private void SetDistanceGain(int previousDistance, int currentDistance)
    {
        if (previousDistance == currentDistance) DistanceGain += 0;
        if (previousDistance > currentDistance)
        {
            DistanceGain += -(previousDistance - currentDistance) * 2;
        }
        if (previousDistance < currentDistance)
        {
            DistanceGain += -(previousDistance - currentDistance) * 100;
        }
    }

    private static Color GetRandomColor()
    {
        Random random = new Random();
        byte r = (byte)random.Next(256);
        byte g = (byte)random.Next(256);
        byte b = (byte)random.Next(256);
        byte a = 255; 

        return new Color(r, g, b, a);
    }

    public Vector2 GetCurrentPosition() => new Vector2(x, y);

    public override string ToString()
    {
        return $"Player with DNA: {PlayerDNA} | Fitness: {PlayerDNA.fitness} | Distance Gain: {DistanceGain}";
    }

    private float CalculateDistance(float x1, float y1, float x2, float y2)
    {
        float dx = x2 - x1;
        float dy = y2 - y1;
        return MathF.Sqrt(dx * dx + dy * dy);
    }
}