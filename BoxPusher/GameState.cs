namespace BoxPusher;

public class GameState
{
    // The grid we are currently playing on. One char per cell.
    public char[,] Grid { get; set; }

    // Where the player is right now
    public int PlayerRow { get; set; }
    public int PlayerCol { get; set; }

    // How many moves the player has made
    public int Moves { get; set; }

    public int Height { get; set; }
    public int Width { get; set; }

    // Every previous grid, so we can step backwards
    private List<char[,]> history = new List<char[,]>();
    private List<int> playerRowHistory = new List<int>();
    private List<int> playerColHistory = new List<int>();

    // Is this cell a target square (with or without something on it)?
    public bool IsTarget(int row, int col)
    {
        char c = Grid[row, col];
        return c == '.' || c == '*' || c == '+';
    }

    // Is there a box on this cell?
    public bool IsBox(int row, int col)
    {
        char c = Grid[row, col];
        return c == '$' || c == '*';
    }

    // Is this cell a wall?
    public bool IsWall(int row, int col)
    {
        return Grid[row, col] == '#';
    }

    // Build a fresh playable state from a level
    public static GameState StartFrom(Level level)
    {
        GameState state = new GameState();
        state.Height = level.GetHeight();
        state.Width = level.GetWidth();
        state.Grid = new char[state.Height, state.Width];
        state.Moves = 0;

        for (int row = 0; row < state.Height; row++)
        {
            string line = level.Rows[row];

            for (int col = 0; col < state.Width; col++)
            {
                // Rows can be short, so pad with floor
                char c = ' ';
                if (col < line.Length)
                {
                    c = line[col];
                }

                state.Grid[row, col] = c;

                // Remember where the player starts
                if (c == '@' || c == '+')
                {
                    state.PlayerRow = row;
                    state.PlayerCol = col;
                }
            }
        }

        return state;
    }

    // Try to move the player. Returns true if the move happened.
    public bool Move(int rowChange, int colChange)
    {
        int newRow = PlayerRow + rowChange;
        int newCol = PlayerCol + colChange;

        // Can't walk off the edge of the grid
        if (newRow < 0 || newRow >= Height || newCol < 0 || newCol >= Width)
        {
            return false;
        }

        // Can't walk into a wall
        if (IsWall(newRow, newCol))
        {
            return false;
        }

        // If there is a box there, we need to push it
        if (IsBox(newRow, newCol))
        {
            int boxRow = newRow + rowChange;
            int boxCol = newCol + colChange;

            // Box can't be pushed off the grid
            if (boxRow < 0 || boxRow >= Height || boxCol < 0 || boxCol >= Width)
            {
                return false;
            }

            // Box can't be pushed into a wall or another box
            if (IsWall(boxRow, boxCol) || IsBox(boxRow, boxCol))
            {
                return false;
            }

            // Move the box
            SetCell(boxRow, boxCol, true, false);
            SetCell(newRow, newCol, false, false);
        }

        // Everything checks out, so remember this position before we change it
        SaveHistory();

        // Move the player off the old square
        SetCell(PlayerRow, PlayerCol, false, false);

        // Move the player onto the new square
        SetCell(newRow, newCol, false, true);

        PlayerRow = newRow;
        PlayerCol = newCol;
        Moves++;

        return true;
    }

    // Take a snapshot of where we are before making a move
    private void SaveHistory()
    {
        char[,] copy = new char[Height, Width];

        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                copy[row, col] = Grid[row, col];
            }
        }

        history.Add(copy);
        playerRowHistory.Add(PlayerRow);
        playerColHistory.Add(PlayerCol);
    }

    // Step back to the previous position. Returns true if there was one.
    public bool Undo()
    {
        if (history.Count == 0)
        {
            return false;
        }

        int last = history.Count - 1;

        Grid = history[last];
        PlayerRow = playerRowHistory[last];
        PlayerCol = playerColHistory[last];

        history.RemoveAt(last);
        playerRowHistory.RemoveAt(last);
        playerColHistory.RemoveAt(last);

        Moves--;

        return true;
    }

    // Put a box, the player, or nothing on a cell,
    // keeping the target underneath intact
    private void SetCell(int row, int col, bool hasBox, bool hasPlayer)
    {
        bool target = IsTarget(row, col);

        if (hasBox)
        {
            Grid[row, col] = target ? '*' : '$';
        }
        else if (hasPlayer)
        {
            Grid[row, col] = target ? '+' : '@';
        }
        else
        {
            Grid[row, col] = target ? '.' : ' ';
        }
    }

    // Solved when there are no boxes sitting off a target
    public bool IsSolved()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                if (Grid[row, col] == '$')
                {
                    return false;
                }
            }
        }
        return true;
    }
}