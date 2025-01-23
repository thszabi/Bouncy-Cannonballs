[System.Serializable]
public class LeaderboardData
{
    public LeaderboardRowData[] Rows;
}

[System.Serializable]
public class LeaderboardRowData
{
    public string Name;
    public int Score;

    public LeaderboardRowData(string name, int score)
    {
        Name = name;
        Score = score;
    }
}
