using Microsoft.Maui.Controls.Shapes;
namespace BoxPusher;

public partial class MainPage : ContentPage
{
    private GameState state;

    // Read from settings each time so changes take effect
    private int CellSize
    {
        get { return AppSettings.CellSize; }
    }

    // All the built-in levels, once they have loaded
    private List<Level> levels = new List<Level>();

    // Which one we are playing
    private int currentIndex = 0;

    // Completion and best scores, loaded from the device
    private Progress progress = Progress.Load();

    public MainPage()
    {
        InitializeComponent();
    }

    // Runs every time the page appears on screen
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Only load once
        if (levels.Count == 0)
        {
            await LoadLevelsAsync();
        }
    }

    private async Task LoadLevelsAsync()
    {
        MovesLabel.Text = "Loading levels...";

        levels = await LevelStore.GetBuiltInLevelsAsync();

        if (levels.Count == 0)
        {
            MovesLabel.Text = "Could not load levels";
            await DisplayAlert("Problem",
                "The levels could not be downloaded. Check your internet connection and restart the app.",
                "OK");
            return;
        }

        StartLevel(0);
    }

    // Start playing the level at this position in the list
    private void StartLevel(int index)
    {
        currentIndex = index;
        state = GameState.StartFrom(levels[index]);
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
    private async void DoMove(int rowChange, int colChange)
    {
        if (state == null)
        {
            return;
        }

        bool moved = state.Move(rowChange, colChange);

        if (moved)
        {
            DrawBoard();

            if (state.IsSolved())
            {
                // Save that this level was finished, and the score if it beats the old one
                progress.RecordCompletion(levels[currentIndex].Name, state.Moves);


                await DisplayAlert("Well done",
                    levels[currentIndex].Name + " complete in " + state.Moves + " moves",
                    "OK");

                // Move on to the next level if there is one
                if (currentIndex + 1 < levels.Count)
                {
                    StartLevel(currentIndex + 1);
                }
                else
                {
                    await DisplayAlert("Finished", "You have completed all the levels!", "OK");
                }
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

    private void OnUndoClicked(object sender, EventArgs e)
    {
        if (state == null)
        {
            return;
        }

        bool undone = state.Undo();

        if (undone)
        {
            DrawBoard();
        }
    }

    private void OnResetClicked(object sender, EventArgs e)
    {
        if (levels.Count == 0)
        {
            return;
        }

        // Just build a fresh state from the original level
        StartLevel(currentIndex);
    }

    private async void OnLevelsClicked(object sender, EventArgs e)
    {
        if (levels.Count == 0)
        {
            return;
        }

        LevelSelectPage page = new LevelSelectPage(levels, progress, StartLevel);
        await Navigation.PushAsync(page);
    }

    private async void OnEditorClicked(object sender, EventArgs e)
    {
        EditorPage page = new EditorPage();
        await Navigation.PushAsync(page);
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        SettingsPage page = new SettingsPage();
        await Navigation.PushAsync(page);
    }
}