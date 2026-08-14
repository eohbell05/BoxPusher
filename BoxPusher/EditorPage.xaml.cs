using Microsoft.Maui.Controls.Shapes;

namespace BoxPusher;

public partial class EditorPage : ContentPage
{
    // The level being built, as a 2D grid of characters
    private char[,] grid;
    private int height = 8;
    private int width = 8;

    // What the user is currently placing
    private char currentTool = '#';

    private const int CellSize = 34;

    public EditorPage()
    {
        InitializeComponent();

        SetUpPickers();
        NewGrid(width, height);
        UpdateStatus();
    }

    private void SetUpPickers()
    {
        for (int i = 5; i <= 12; i++)
        {
            WidthPicker.Items.Add(i.ToString());
            HeightPicker.Items.Add(i.ToString());
        }

        // Default to 8x8
        WidthPicker.SelectedIndex = 3;
        HeightPicker.SelectedIndex = 3;
    }

    // Start a blank grid, walled around the outside
    private void NewGrid(int newWidth, int newHeight)
    {
        width = newWidth;
        height = newHeight;
        grid = new char[height, width];

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                bool edge = (row == 0 || row == height - 1 || col == 0 || col == width - 1);
                grid[row, col] = edge ? '#' : ' ';
            }
        }

        DrawGrid();
    }

    private void DrawGrid()
    {
        EditGrid.Children.Clear();
        EditGrid.RowDefinitions.Clear();
        EditGrid.ColumnDefinitions.Clear();

        for (int row = 0; row < height; row++)
        {
            EditGrid.RowDefinitions.Add(new RowDefinition { Height = CellSize });
        }
        for (int col = 0; col < width; col++)
        {
            EditGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = CellSize });
        }

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                char c = grid[row, col];

                BoxView floor = new BoxView();
                floor.Color = GetFloorColour(c);
                EditGrid.Add(floor, col, row);

                if (c == '@' || c == '+')
                {
                    Ellipse player = new Ellipse();
                    player.Fill = Colors.CornflowerBlue;
                    player.Margin = 5;
                    EditGrid.Add(player, col, row);
                }
                else if (c == '$' || c == '*')
                {
                    BoxView box = new BoxView();
                    box.Color = (c == '*') ? Colors.LimeGreen : Colors.SaddleBrown;
                    box.Margin = 4;
                    box.CornerRadius = 3;
                    EditGrid.Add(box, col, row);
                }

                // An invisible button on top so the cell can be tapped
                int tapRow = row;
                int tapCol = col;

                Button tap = new Button();
                tap.BackgroundColor = Colors.Transparent;
                tap.Clicked += (sender, e) => CellTapped(tapRow, tapCol);
                EditGrid.Add(tap, col, row);
            }
        }
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

    private void CellTapped(int row, int col)
    {
        // Only one player is allowed, so clear any existing one first
        if (currentTool == '@')
        {
            for (int r = 0; r < height; r++)
            {
                for (int c = 0; c < width; c++)
                {
                    if (grid[r, c] == '@')
                    {
                        grid[r, c] = ' ';
                    }
                    else if (grid[r, c] == '+')
                    {
                        grid[r, c] = '.';
                    }
                }
            }
        }

        grid[row, col] = currentTool;

        DrawGrid();
        UpdateStatus();
    }

    private void OnToolClicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;

        if (button == WallButton) currentTool = '#';
        else if (button == FloorButton) currentTool = ' ';
        else if (button == BoxButton) currentTool = '$';
        else if (button == TargetButton) currentTool = '.';
        else if (button == PlayerButton) currentTool = '@';

        UpdateStatus();
    }

    private void OnNewGridClicked(object sender, EventArgs e)
    {
        int w = WidthPicker.SelectedIndex + 5;
        int h = HeightPicker.SelectedIndex + 5;

        NewGrid(w, h);
        UpdateStatus();
    }

    // Count what is on the grid so the user can see if it is valid
    private void UpdateStatus()
    {
        int boxes = 0;
        int targets = 0;
        int players = 0;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                char c = grid[row, col];

                if (c == '$' || c == '*') boxes++;
                if (c == '.' || c == '*' || c == '+') targets++;
                if (c == '@' || c == '+') players++;
            }
        }

        string tool = "Wall";
        if (currentTool == ' ') tool = "Floor";
        else if (currentTool == '$') tool = "Box";
        else if (currentTool == '.') tool = "Target";
        else if (currentTool == '@') tool = "Player";

        StatusLabel.Text = "Placing: " + tool
            + "   |   Boxes: " + boxes
            + "   Targets: " + targets
            + "   Players: " + players;
    }

    private async void OnTestClicked(object sender, EventArgs e)
    {
        string name = NameEntry.Text;

        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Name needed", "Give your level a name first.", "OK");
            return;
        }

        // Count everything again so we can check the level makes sense
        int boxes = 0;
        int targets = 0;
        int players = 0;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                char c = grid[row, col];

                if (c == '$' || c == '*') boxes++;
                if (c == '.' || c == '*' || c == '+') targets++;
                if (c == '@' || c == '+') players++;
            }
        }

        if (players != 1)
        {
            await DisplayAlert("Not valid", "Your level needs exactly one player.", "OK");
            return;
        }

        if (boxes == 0)
        {
            await DisplayAlert("Not valid", "Your level needs at least one box.", "OK");
            return;
        }

        if (boxes != targets)
        {
            await DisplayAlert("Not valid",
                "You have " + boxes + " boxes and " + targets + " targets. They must match.",
                "OK");
            return;
        }

        // Turn the grid into a Level
        Level level = new Level();
        level.Name = name;

        for (int row = 0; row < height; row++)
        {
            string line = "";

            for (int col = 0; col < width; col++)
            {
                line += grid[row, col];
            }

            level.Rows.Add(line);
        }

        // Send the user off to play it. It only saves if they finish it.
        TestPage page = new TestPage(level);
        await Navigation.PushAsync(page);
    }
}