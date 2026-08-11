namespace BoxPusher;

public class Level
{
    public string Name { get; set; } = "";
    public List<string> Rows { get; set; } = new List<string>();

    public int GetHeight()
    {
        return Rows.Count;
    }

    public int GetWidth()
    {
        int widest = 0;
        foreach (string row in Rows)
        {
            if (row.Length > widest)
            {
                widest = row.Length;
            }
        }
        return widest;
    }
}