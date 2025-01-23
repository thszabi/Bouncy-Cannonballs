using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplierSceneController : MonoBehaviour
{
    [Tooltip("In seconds")]
    public float CountAnimationLength = 2.0f;

    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _levelScoreValueText;
    [SerializeField] private TMP_Text _globalScoreValueText;
    [SerializeField] private RectTransform _multiplierPanel;
    [SerializeField] private RectTransform _levelScorePanel;
    [SerializeField] private RectTransform _globalScorePanel;

    [SerializeField] private Button _1xButton;
    [SerializeField] private Button _3xButton;
    [SerializeField] private Button _5xButton;
    [SerializeField] private Button _leaderboardButton;

    private float _animationLength = 0.0f;
    private float _elapsedAnimationTime = 0.0f;

    void Start()
    {
        if (GlobalController.Instance.IsGameOver)
        {
            _titleText.text = "You lost!";
            _globalScoreValueText.text = GlobalController.Instance.GlobalPlayerScore.ToString();

            _levelScorePanel.gameObject.SetActive(false);
            _multiplierPanel.gameObject.SetActive(false);
        }
        else
        {
            _titleText.text = "You won!";
            _levelScoreValueText.text = GlobalController.Instance.LevelPlayerScore.ToString();

            _globalScorePanel.gameObject.SetActive(false);
            _leaderboardButton.gameObject.SetActive(false);

            _1xButton.onClick.AddListener(() => OnMultiplierButtonClicked(1));
            _3xButton.onClick.AddListener(() => OnMultiplierButtonClicked(3));
            _5xButton.onClick.AddListener(() => OnMultiplierButtonClicked(5));
        }

        _leaderboardButton.onClick.AddListener(OnLeaderboardButtonClicked);
    }

    private void OnMultiplierButtonClicked(int multiplier)
    {
        _1xButton.interactable = false;
        _3xButton.interactable = false;
        _5xButton.interactable = false;

        GlobalController.Instance.LevelPlayerScore *= multiplier;

        _levelScoreValueText.text = GlobalController.Instance.LevelPlayerScore.ToString();
        _levelScoreValueText.GetComponent<Animator>().Play("ScoreBumpAnimation", -1, 0.0f);

        GlobalController.Instance.GlobalPlayerScore += GlobalController.Instance.LevelPlayerScore;

        StartCoroutine(StartNumberAnimationCoroutine());
    }

    private IEnumerator StartNumberAnimationCoroutine()
    {
        yield return new WaitForSeconds(1.0f);

        int originalScore = GlobalController.Instance.GlobalPlayerScore - GlobalController.Instance.LevelPlayerScore;

        _globalScoreValueText.text = originalScore.ToString();
        _globalScorePanel.gameObject.SetActive(true);
        _leaderboardButton.gameObject.SetActive(true);

        _elapsedAnimationTime = 0.0f;
        _animationLength = CountAnimationLength;
    }

    private void OnLeaderboardButtonClicked()
    {
        SceneManager.LoadScene("LeaderboardScene");
    }

    private void Update()
    {
        if (_elapsedAnimationTime < _animationLength)
        {
            int newScore = GlobalController.Instance.GlobalPlayerScore;
            int originalScore = newScore - GlobalController.Instance.LevelPlayerScore;

            _elapsedAnimationTime += Time.deltaTime;
            int currentValue = (int)Mathf.Lerp(originalScore, newScore, _elapsedAnimationTime / _animationLength);
            _globalScoreValueText.text = currentValue.ToString();
        }
    }

    private void OnDestroy()
    {
        _1xButton.onClick.RemoveAllListeners();
        _3xButton.onClick.RemoveAllListeners();
        _5xButton.onClick.RemoveAllListeners();
        _leaderboardButton.onClick.RemoveAllListeners();
    }
}