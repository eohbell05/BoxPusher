namespace BoxPusher;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();

        // Load the saved settings into the controls
        DarkModeSwitch.IsToggled = AppSettings.DarkMode;
        SoundSwitch.IsToggled = AppSettings.SoundOn;
        CellSizeSlider.Value = AppSettings.CellSize;

        UpdateCellSizeLabel();
    }

    private void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        AppSettings.DarkMode = e.Value;
        AppSettings.ApplyTheme();
    }

    private void OnSoundToggled(object sender, ToggledEventArgs e)
    {
        AppSettings.SoundOn = e.Value;
    }

    private void OnCellSizeChanged(object sender, ValueChangedEventArgs e)
    {
        AppSettings.CellSize = (int)e.NewValue;
        UpdateCellSizeLabel();
    }

    private void UpdateCellSizeLabel()
    {
        CellSizeLabel.Text = "Cells are " + AppSettings.CellSize + " units across";
    }

    private async void OnResetProgressClicked(object sender, EventArgs e)
    {
        bool sure = await DisplayAlert("Reset progress",
            "This will clear all your completed levels and best scores. Are you sure?",
            "Yes", "No");

        if (sure)
        {
            Progress fresh = new Progress();
            fresh.Save();

            await DisplayAlert("Done", "Progress has been cleared.", "OK");
        }
    }
}