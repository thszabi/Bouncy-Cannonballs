using UnityEngine;

[CreateAssetMenu(fileName = "LevelXXXX", menuName = "ScriptableObjects/LevelResource")]
public class LevelResource : ScriptableObject
{
    public LevelData LevelData;

    public static LevelResource Load(int levelNumber)
    {
        string levelNumberStr = levelNumber.ToString("D4");
        return Resources.Load<LevelResource>($"Levels/Level{levelNumberStr}");
    }
}
