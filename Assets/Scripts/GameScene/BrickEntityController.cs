using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[ExecuteInEditMode]
public class BrickEntityController : MonoBehaviour
{
    public Action<BrickEntityController> DestroyedEvent;
    public Action ReachedBottomEvent;

    [SerializeField] private int _health = 50;
    [SerializeField] private Color _brickColor = Color.black;
    [SerializeField] private Color _textColor = Color.white;

    [Space(10)]
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private SpriteRenderer _sprite;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();

        _text.text = _health.ToString();
        _sprite.color = _brickColor;
        _text.color = _textColor;
    }

    private void OnValidate()
    {
        _text.text = _health.ToString();
        _sprite.color = _brickColor;
        _text.color = _textColor;
    }

    public void MoveDown()
    {
        transform.localPosition += Vector3.down;
        if (transform.localPosition.y <= -4.0f)
        {
            ReachedBottomEvent?.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            _health--;
            _text.text = _health.ToString();

            _animator.Play("BrickHitAnimation", -1, 0.0f);
        }

        if (_health <= 0)
        {
            Destroy(gameObject);
            DestroyedEvent?.Invoke(this);
        }
    }
}
