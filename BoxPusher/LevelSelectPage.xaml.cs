namespace BoxPusher;

public partial class LevelSelectPage : ContentPage
{
    private List<Level> levels;
    private Progress progress;

    // The page hands the chosen index back to whoever opened it
    private Action<int> onLevelChosen;

    public LevelSelectPage(List<Level> levels, Progress progress, Action<int> onLevelChosen)
    {
        InitializeComponent();

        this.levels = levels;
        this.progress = progress;
        this.onLevelChosen = onLevelChosen;

        BuildList();
    }

    private void BuildList()
    {
        LevelList.Children.Clear();

        for (int i = 0; i < levels.Count; i++)
        {
            // Capture the index for this button
            int index = i;
            string name = levels[i].Name;

            Button button = new Button();
            button.FontSize = 18;

            // Show a tick and the best score for levels already finished
            if (progress.IsCompleted(name))
            {
                button.Text = name + " - Completed - Best: " + progress.GetBestMoves(name) + " moves";
                button.BackgroundColor = Colors.SeaGreen;
            }
            else
            {
                button.Text = name;
            }

            button.Clicked += async (sender, e) =>
            {
                onLevelChosen(index);
                await Navigation.PopAsync();
            };

            LevelList.Children.Add(button);
        }
    }
}