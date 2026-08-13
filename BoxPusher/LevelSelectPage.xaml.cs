namespace BoxPusher;

public partial class LevelSelectPage : ContentPage
{
    private List<Level> levels;

    // The page hands the chosen index back to whoever opened it
    private Action<int> onLevelChosen;

    public LevelSelectPage(List<Level> levels, Action<int> onLevelChosen)
    {
        InitializeComponent();

        this.levels = levels;
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

            Button button = new Button();
            button.Text = levels[i].Name;
            button.FontSize = 18;

            button.Clicked += async (sender, e) =>
            {
                onLevelChosen(index);
                await Navigation.PopAsync();
            };

            LevelList.Children.Add(button);
        }
    }
}