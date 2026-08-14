using System.Text.Json;

namespace BoxPusher;

// Saves and loads levels the user has built themselves
public class CustomLevelStore
{
    private const string FileName = "customlevels.json";

    private static string LocalPath
    {
        get
        {
            return Path.Combine(FileSystem.AppDataDirectory, FileName);
        }
    }

    public static List<Level> Load()
    {
        try
        {
            if (File.Exists(LocalPath))
            {
                string json = File.ReadAllText(LocalPath);
                List<Level> loaded = JsonSerializer.Deserialize<List<Level>>(json);

                if (loaded != null)
                {
                    return loaded;
                }
            }
        }
        catch (Exception)
        {
            // Corrupt file, start with none
        }

        return new List<Level>();
    }

    public static void Save(List<Level> levels)
    {
        try
        {
            string json = JsonSerializer.Serialize(levels);
            File.WriteAllText(LocalPath, json);
        }
        catch (Exception)
        {
            // Nothing useful to do if saving fails
        }
    }

    // Add one level to the saved list
    public static void Add(Level level)
    {
        List<Level> all = Load();
        all.Add(level);
        Save(all);
    }

    // Remove a level by name
    public static void Delete(string name)
    {
        List<Level> all = Load();
        all.RemoveAll(l => l.Name == name);
        Save(all);
    }
}