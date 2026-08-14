namespace BoxPusher;

// App settings, saved using MAUI's built in Preferences store
public static class AppSettings
{
    public static bool DarkMode
    {
        get { return Preferences.Get("DarkMode", false); }
        set { Preferences.Set("DarkMode", value); }
    }

    public static bool SoundOn
    {
        get { return Preferences.Get("SoundOn", true); }
        set { Preferences.Set("SoundOn", value); }
    }

    public static int CellSize
    {
        get { return Preferences.Get("CellSize", 40); }
        set { Preferences.Set("CellSize", value); }
    }

    // Switch the whole app between light and dark
    public static void ApplyTheme()
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = DarkMode ? AppTheme.Dark : AppTheme.Light;
        }
    }
}