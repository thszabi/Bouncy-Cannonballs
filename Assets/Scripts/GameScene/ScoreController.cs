using System;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public Action<int> ScoreChangedEvent;

    [SerializeField] private int _scoreForDestroyingBrick = 10;

    private int _levelScore;

    void Start()
    {
        _levelScore = 0;
    }

    public void OnBrickDestroyed()
    {
        _levelScore += _scoreForDestroyingBrick;
        ScoreChangedEvent?.Invoke(_levelScore);
    }

    public int GetScore()
    {
        return _levelScore;
    }

    public void MultiplyScore(int multiplier)
    {
        _levelScore *= multiplier;
        ScoreChangedEvent?.Invoke(_levelScore);
    }
}