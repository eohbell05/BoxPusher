using Microsoft.Maui.Controls.Shapes;

namespace BoxPusher;

// Plays a level the user just built. Saves it only if they solve it.
public partial class TestPage : ContentPage
{
    private Level level;
    private GameState state;

    private const int CellSize = 40;

    public TestPage(Level level)
    {
        InitializeComponent();

        this.level = level;
        state = GameState.StartFrom(level);

        DrawBoard();
    }

    private void DrawBoard()
    {
        BoardGrid.Children.Clear();
        BoardGrid.RowDefinitions.Clear();
        BoardGrid.ColumnDefinitions.Clear();

        for (int row = 0; row < state.Height; row++)
        {
            BoardGrid.RowDefinitions.Add(new RowDefinition { Height = CellSize });
        }
        for (int col = 0; col < state.Width; col++)
        {
            BoardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = CellSize });
        }

        for (int row = 0; row < state.Height; row++)
        {
            for (int col = 0; col < state.Width; col++)
            {
                char c = state.Grid[row, col];

                BoxView floor = new BoxView();
                floor.Color = GetFloorColour(c);
                BoardGrid.Add(floor, col, row);

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

        MovesLabel.Text = "Moves: " + state.Moves + "  -  solve it to save it";
    }

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

    private async void DoMove(int rowChange, int colChange)
    {
        bool moved = state.Move(rowChange, colChange);

        if (moved)
        {
            DrawBoard();

            if (state.IsSolved())
            {
                // The level works, so it is safe to save
                CustomLevelStore.Add(level);

                await DisplayAlert("Saved",
                    "Your level works and has been saved to your custom levels.",
                    "OK");

                await Navigation.PopToRootAsync();
            }
        }
    }

    private void OnUpClicked(object sender, EventArgs e) { DoMove(-1, 0); }
    private void OnDownClicked(object sender, EventArgs e) { DoMove(1, 0); }
    private void OnLeftClicked(object sender, EventArgs e) { DoMove(0, -1); }
    private void OnRightClicked(object sender, EventArgs e) { DoMove(0, 1); }

    private void OnUndoClicked(object sender, EventArgs e)
    {
        if (state.Undo())
        {
            DrawBoard();
        }
    }

    private void OnResetClicked(object sender, EventArgs e)
    {
        state = GameState.StartFrom(level);
        DrawBoard(); ;
    }

    private async void OnGiveUpClicked(object sender, EventArgs e)
    {
        bool sure = await DisplayAlert("Give up?",
            "Your level will not be saved. Go back to the editor?",
            "Yes", "No");

        if (sure)
        {
            await Navigation.PopAsync();
        }
    }
}