using UnityEngine;

public class GlobalController : MonoBehaviour
{
    public static GlobalController Instance;

    public int GlobalPlayerScore { get; set; } = 0;
    public int LevelPlayerScore { get; set; } = 0;
    public int CurrentLevel { get; set; } = 1;
    public string PlayerName { get; set; } = "Player";
    public bool IsGameOver { get; set; } = true;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGameSession(string playerName, int startingLevel)
    {
        GlobalPlayerScore = 0;
        LevelPlayerScore = 0;
        CurrentLevel = startingLevel;
        PlayerName = playerName;
        IsGameOver = false;
    }
}