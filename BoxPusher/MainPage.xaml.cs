namespace BoxPusher;

public partial class MainPage : ContentPage
{
    private GameState state;

    // How big each cell is drawn, in device units
    private const int CellSize = 40;

    public MainPage()
    {
        InitializeComponent();
        LoadTestLevel();
    }

    // A hard-coded level just so we can test the game logic
    private void LoadTestLevel()
    {
        Level level = new Level();
        level.Name = "Test Level";
        level.Rows.Add("#######");
        level.Rows.Add("#     #");
        level.Rows.Add("# $ . #");
        level.Rows.Add("#  @  #");
        level.Rows.Add("#######");

        state = GameState.StartFrom(level);
        DrawBoard();
    }

    // Rebuild the whole board from the current state
    private void DrawBoard()
    {
        BoardGrid.Children.Clear();
        BoardGrid.RowDefinitions.Clear();
        BoardGrid.ColumnDefinitions.Clear();

        // Set up the right number of rows and columns
        for (int row = 0; row < state.Height; row++)
        {
            BoardGrid.RowDefinitions.Add(new RowDefinition { Height = CellSize });
        }
        for (int col = 0; col < state.Width; col++)
        {
            BoardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = CellSize });
        }

        // Add one label per cell
        for (int row = 0; row < state.Height; row++)
        {
            for (int col = 0; col < state.Width; col++)
            {
                Label cell = new Label();
                cell.Text = state.Grid[row, col].ToString();
                cell.FontSize = 24;
                cell.HorizontalTextAlignment = TextAlignment.Center;
                cell.VerticalTextAlignment = TextAlignment.Center;

                BoardGrid.Add(cell, col, row);
            }
        }

        MovesLabel.Text = "Moves: " + state.Moves;
    }

    // Shared by all four buttons
    private void DoMove(int rowChange, int colChange)
    {
        bool moved = state.Move(rowChange, colChange);

        if (moved)
        {
            DrawBoard();

            if (state.IsSolved())
            {
                DisplayAlert("Well done", "Level complete in " + state.Moves + " moves", "OK");
            }
        }
    }

    private void OnUpClicked(object sender, EventArgs e)
    {
        DoMove(-1, 0);
    }

    private void OnDownClicked(object sender, EventArgs e)
    {
        DoMove(1, 0);
    }

    private void OnLeftClicked(object sender, EventArgs e)
    {
        DoMove(0, -1);
    }

    private void OnRightClicked(object sender, EventArgs e)
    {
        DoMove(0, 1);
    }
}