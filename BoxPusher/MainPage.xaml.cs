using Microsoft.Maui.Controls.Shapes;
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

        // Draw each cell
        for (int row = 0; row < state.Height; row++)
        {
            for (int col = 0; col < state.Width; col++)
            {
                char c = state.Grid[row, col];

                // The floor underneath everything
                BoxView floor = new BoxView();
                floor.Color = GetFloorColour(c);
                BoardGrid.Add(floor, col, row);

                // The player or box on top, if there is one
                if (c == '@' || c == '+')
                {
                    Ellipse player = new Ellipse();
                    player.Fill = Colors.CornflowerBlue;
                    player.Margin = 6;
                    BoardGrid.Add(player, col, row);
                }
                else if (c == '$' || c == '*')
                {
                    BoxView box = new BoxView();
                    box.Color = (c == '*') ? Colors.LimeGreen : Colors.SaddleBrown;
                    box.Margin = 5;
                    box.CornerRadius = 4;
                    BoardGrid.Add(box, col, row);
                }
            }
        }

        MovesLabel.Text = "Moves: " + state.Moves;
    }

    // Walls are dark, targets are highlighted, everything else is plain floor
    private Color GetFloorColour(char c)
    {
        if (c == '#')
        {
            return Colors.DimGray;
        }

        if (c == '.' || c == '*' || c == '+')
        {
            return Colors.LightGoldenrodYellow;
        }

        return Colors.WhiteSmoke;
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