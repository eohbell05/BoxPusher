using System.Text.Json;

namespace BoxPusher;

// Handles getting the built-in levels onto the device and reading them back
public class LevelStore
{
    // Where the levels come from the first time the app runs
    private const string LevelsUrl =
        "https://raw.githubusercontent.com/eohbell05/BoxPusher/refs/heads/master/BoxPusher/levels.json";

    // The file name we save it under on the device
    private const string LevelsFileName = "levels.json";

    // Full path to the saved copy on this device
    private static string LocalPath
    {
        get
        {
            return Path.Combine(FileSystem.AppDataDirectory, LevelsFileName);
        }
    }

    // Get the built-in levels.
    // Downloads them the first time, then uses the saved copy after that.
    public static async Task<List<Level>> GetBuiltInLevelsAsync()
    {
        // If we have not saved them yet, try to download
        if (!File.Exists(LocalPath))
        {
            await TryDownloadLevelsAsync();
        }

        // If the file still is not there, the download failed
        if (!File.Exists(LocalPath))
        {
            return new List<Level>();
        }

        try
        {
            string json = File.ReadAllText(LocalPath);
            List<Level> levels = JsonSerializer.Deserialize<List<Level>>(json);

            if (levels == null)
            {
                return new List<Level>();
            }

            return levels;
        }
        catch (Exception)
        {
            // The saved file was corrupt or unreadable
            return new List<Level>();
        }
    }

    // Download the levels file and save it to the device
    private static async Task TryDownloadLevelsAsync()
    {
        try
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);

            string json = await client.GetStringAsync(LevelsUrl);

            // Only save it if it actually parses as levels
            List<Level> check = JsonSerializer.Deserialize<List<Level>>(json);

            if (check != null && check.Count > 0)
            {
                File.WriteAllText(LocalPath, json);
            }
        }
        catch (Exception)
        {
            // No internet, bad URL, server down - just carry on with no levels
        }
    }
}