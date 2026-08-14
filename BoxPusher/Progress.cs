using System.Text.Json;

namespace BoxPusher;

// Tracks which levels have been completed and the best move count for each
public class Progress
{
    // Level name -> best number of moves
    public Dictionary<string, int> BestMoves { get; set; } = new Dictionary<string, int>();

    private const string FileName = "progress.json";

    private static string LocalPath
    {
        get
        {
            return Path.Combine(FileSystem.AppDataDirectory, FileName);
        }
    }

    // Read the saved progress, or start fresh if there is none
    public static Progress Load()
    {
        try
        {
            if (File.Exists(LocalPath))
            {
                string json = File.ReadAllText(LocalPath);
                Progress loaded = JsonSerializer.Deserialize<Progress>(json);

                if (loaded != null)
                {
                    return loaded;
                }
            }
        }
        catch (Exception)
        {
            // Corrupt file, just start fresh
        }

        return new Progress();
    }

    // Write the current progress to the device
    public void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(this);
            File.WriteAllText(LocalPath, json);
        }
        catch (Exception)
        {
            // Nothing useful to do if saving fails
        }
    }

    // Record a completion. Only overwrites if this run was better.
    public void RecordCompletion(string levelName, int moves)
    {
        if (BestMoves.ContainsKey(levelName))
        {
            if (moves < BestMoves[levelName])
            {
                BestMoves[levelName] = moves;
            }
        }
        else
        {
            BestMoves[levelName] = moves;
        }

        Save();
    }

    public bool IsCompleted(string levelName)
    {
        return BestMoves.ContainsKey(levelName);
    }

    public int GetBestMoves(string levelName)
    {
        if (BestMoves.ContainsKey(levelName))
        {
            return BestMoves[levelName];
        }

        return 0;
    }
}