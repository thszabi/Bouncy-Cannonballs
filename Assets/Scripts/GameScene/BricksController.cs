using System;
using System.Collections.Generic;
using UnityEngine;

public class BricksController : MonoBehaviour
{
    public Action BrickDestroyedEvent;

    public bool IsEveryBrickDestroyed => _bricks.Count == 0;
    public bool IsBricksReachedBottom { get; private set; }

    private List<BrickEntityController> _bricks;

    void Start()
    {
        IsBricksReachedBottom = false;

        _bricks = new List<BrickEntityController>();
        BrickEntityController[] bricks = FindObjectsByType<BrickEntityController>(FindObjectsSortMode.None);
        _bricks.AddRange(bricks);

        foreach (BrickEntityController brick in bricks)
        {
            brick.DestroyedEvent += OnBrickDestroyed;
            brick.ReachedBottomEvent += OnBrickReachedBottom;
        }
    }

    public void MoveBricksDown()
    {
        for (int i = _bricks.Count - 1; i >= 0; i--)
        {
            BrickEntityController brick = _bricks[i];
            if (brick != null)
            {
                brick.MoveDown();
            }
            else
            {
                brick.DestroyedEvent -= OnBrickDestroyed;
                brick.ReachedBottomEvent -= OnBrickReachedBottom;
                _bricks.RemoveAt(i);
            }
        }
    }

    private void OnBrickDestroyed(BrickEntityController brick)
    {
        brick.DestroyedEvent -= OnBrickDestroyed;
        brick.ReachedBottomEvent -= OnBrickReachedBottom;
        _bricks.Remove(brick);
        BrickDestroyedEvent?.Invoke();
    }

    private void OnBrickReachedBottom()
    {
        IsBricksReachedBottom = true;
    }
}