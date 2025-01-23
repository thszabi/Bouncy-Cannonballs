using System;
using UnityEngine;

public class BallEntityController : MonoBehaviour
{
    public Action<BallEntityController> FloorTouchedEvent;
    public Action StoppedEvent;

    public bool IsMoving => _movingDirection.HasValue || _destinationPosition.HasValue;

    [SerializeField] private float _speed = 0.5f;
    [SerializeField] private Rigidbody2D _rigidBody2D;
    [SerializeField] private Collider2D _collider2D;

    private Vector2? _movingDirection = null;
    private Vector2? _destinationPosition = null;
    private Vector2 _combinedNormalVector = Vector2.zero;

    public void SetStartingPosition(Vector2 startingPosition)
    {
        _rigidBody2D.position = startingPosition;
    }

    public void SetMovingDirection(Vector2? movingDirection)
    {
        _movingDirection = movingDirection;
    }

    public Vector2 GetPosition()
    {
        return _rigidBody2D.position;
    }

    public void Stop()
    {
        _movingDirection = null;
        _destinationPosition = null;
        _rigidBody2D.velocity = Vector2.zero;
        StoppedEvent?.Invoke();
    }

    public void SetDestinationPosition(Vector2? destinationPosition)
    {
        _movingDirection = null;
        _destinationPosition = destinationPosition;
        _collider2D.enabled = false;
    }

    private void FixedUpdate()
    {
        if (_destinationPosition.HasValue)
        {
            Vector2 movingDirection = _destinationPosition.Value - _rigidBody2D.position;

            if (movingDirection.sqrMagnitude <= 0.01f)
            {
                _rigidBody2D.position = _destinationPosition.Value;
                _rigidBody2D.velocity = Vector2.zero;

                _collider2D.enabled = true;
                _destinationPosition = null;
                StoppedEvent?.Invoke();
            }
            else
            {
                _rigidBody2D.velocity = movingDirection.normalized * _speed;
            }
        }
        else if (_movingDirection.HasValue)
        {
            if (_combinedNormalVector != Vector2.zero)
            {
                _movingDirection = Utils.MirrorVector(_movingDirection.Value, _combinedNormalVector.normalized);
                _combinedNormalVector = Vector2.zero;
            }

            _rigidBody2D.velocity = _movingDirection.Value.normalized * _speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_movingDirection.HasValue)
        {
            if (collision.gameObject.CompareTag("Floor"))
            {
                FloorTouchedEvent?.Invoke(this);
            }
            else
            {
                ContactPoint2D point = collision.GetContact(0);
                _combinedNormalVector += point.normal;
            }
        }
    }
}
