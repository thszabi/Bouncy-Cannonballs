using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUiController : MonoBehaviour
{
    public Action RetrieveBallsButtonClickedEvent;

    [SerializeField] private Button _retrieveBallsButton;
    [SerializeField] private TMP_Text _scoreValueText;
    [SerializeField] private TMP_Text _numberOfBallsValueText;

    private Animator _scoreValueAnimator;

    private void Start()
    {
        _retrieveBallsButton.onClick.AddListener(OnRetrieveBallsButtonPressed);
        _scoreValueAnimator = _scoreValueText.GetComponent<Animator>();
    }

    public void Initialize(int score, int numberOfBalls)
    {
        _scoreValueText.text = score.ToString();
        _numberOfBallsValueText.text = numberOfBalls.ToString();
    }

    public void OnScoreChanged(int score)
    {
        string scoreAsString = score.ToString();
        if (!_scoreValueText.text.Equals(scoreAsString))
        {
            _scoreValueText.text = score.ToString();
            _scoreValueAnimator.Play("ScoreBumpAnimation", -1, 0.0f);
        }
    }

    public void SetRetrieveBallsButtonInteractable(bool interactable)
    {
        _retrieveBallsButton.interactable = interactable;
    }

    private void OnRetrieveBallsButtonPressed()
    {
        RetrieveBallsButtonClickedEvent?.Invoke();
    }

    private void OnDestroy()
    {
        _retrieveBallsButton.onClick.RemoveListener(OnRetrieveBallsButtonPressed);
    }
}