using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallsController : MonoBehaviour
{
    public Action BallsStoppedEvent;

    [SerializeField] private BallEntityController _ballPrefab;

    private List<BallEntityController> _balls = new List<BallEntityController>();
    /// <summary>
    /// The position where the balls will gather upon pressing the "Retrieve balls" button.
    /// The balls stay here at the beginning of the round.
    /// </summary>
    private Vector2 _ballsGatheringPosition;
    private bool _gatheringPositionCanChange;

    private IEnumerator _shootBallsCoroutine = null;

    public void Initialize(int initialBallCount, Vector2 initialBallPosition)
    {
        _balls.Clear();

        for (int i = 0; i < initialBallCount; i++)
        {
            BallEntityController ball = Instantiate(_ballPrefab, transform);
            ball.transform.localPosition = initialBallPosition;
            ball.SetStartingPosition(initialBallPosition);
            ball.FloorTouchedEvent += OnBallTouchedFloor;
            ball.StoppedEvent += OnBallStopped;
            _balls.Add(ball);
        }

        // The Rigidbody2D of the ball needs 1 frame to set the ball's position,
        // so we need to wait 1 frame.
        StartCoroutine(InitializeBallGatheringPositionCoroutine());

        _gatheringPositionCanChange = false;
    }

    public Vector2 GetBallsGatheringPosition()
    {
        return _ballsGatheringPosition;
    }

    public void ShootBalls(Vector2 direction)
    {
        _shootBallsCoroutine = ShootBallCoroutine(direction);
        StartCoroutine(_shootBallsCoroutine);

        _gatheringPositionCanChange = true;
    }

    public void DestroyAllBalls()
    {
        for (int i = _balls.Count - 1; i >= 0; i--)
        {
            if (_balls[i] != null)
            {
                _balls[i].FloorTouchedEvent -= OnBallTouchedFloor;
                _balls[i].StoppedEvent -= OnBallStopped;
                Destroy(_balls[i].gameObject);
            }
        }
    }

    public void RetrieveBalls()
    {
        if (_balls.Any(ball => ball.IsMoving))
        {
            if (_shootBallsCoroutine != null)
            {
                StopCoroutine(_shootBallsCoroutine);
                _shootBallsCoroutine = null;
            }

            foreach (BallEntityController ball in _balls)
            {
                ball.SetDestinationPosition(_ballsGatheringPosition);
            }

            _gatheringPositionCanChange = false;
        }
    }

    private IEnumerator InitializeBallGatheringPositionCoroutine()
    {
        yield return 0;

        if (_balls.Count > 0)
        {
            _ballsGatheringPosition = _balls[0].GetPosition();
        }
    }

    private IEnumerator ShootBallCoroutine(Vector2 direction)
    {
        for (int i = 0; i < _balls.Count; i++)
        {
            BallEntityController ballEntity = _balls[i];
            ballEntity.SetMovingDirection(direction);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnBallTouchedFloor(BallEntityController ball)
    {
        if (_gatheringPositionCanChange)
        {
            // First ball
            _ballsGatheringPosition = ball.GetPosition();
            ball.Stop();
            _gatheringPositionCanChange = false;
        }
        else
        {
            // Other balls
            ball.SetDestinationPosition(_ballsGatheringPosition);
        }
    }

    private void OnBallStopped()
    {
        if (_balls.Any(ball => ball.IsMoving) == false)
        {
            BallsStoppedEvent?.Invoke();
        }
    }

    private void OnDestroy()
    {
        DestroyAllBalls();
    }
}
