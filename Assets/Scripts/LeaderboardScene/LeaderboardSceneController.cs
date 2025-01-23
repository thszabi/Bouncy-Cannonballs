using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardSceneController : MonoBehaviour
{
    [SerializeField] RectTransform _leaderboardContent;
    [SerializeField] RectTransform _leaderboardScrollView;
    [SerializeField] TMP_Text _thankYouForPlayingText;
    [SerializeField] LeaderboardRowView _leaderboardRowPrefab;

    [SerializeField] LeaderboardRowView _pinnedRow;

    [SerializeField] Button _nextLevelButton;
    [SerializeField] Button _backToMainMenuButton;

    private LeaderboardRowView _playersRow = null;

    void Start()
    {
        LeaderboardData leaderboardData = LoadLeaderboardData();
        List<LeaderboardRowData> leaderboardRows = new(leaderboardData.Rows);

        int indexOfPlayer = AddPlayerToLeaderboard(leaderboardRows);

        if (indexOfPlayer == -1)
        {
            _pinnedRow.gameObject.SetActive(false);
        }

        // Keep only the top 100 players:
        leaderboardRows = leaderboardRows.GetRange(0, Mathf.Min(100, leaderboardRows.Count));

        SetupLeaderboardContent(leaderboardRows, indexOfPlayer);

        ScrollToPlayersScore(indexOfPlayer, leaderboardRows.Count);

        bool lastLevelCompleted = LevelResource.Load(GlobalController.Instance.CurrentLevel + 1) == null;
        if (GlobalController.Instance.IsGameOver || lastLevelCompleted)
        {
            SaveLeaderboardData(leaderboardData, leaderboardRows);

            _thankYouForPlayingText.gameObject.SetActive(!GlobalController.Instance.IsGameOver && lastLevelCompleted);
            _backToMainMenuButton.gameObject.SetActive(true);
            _nextLevelButton.gameObject.SetActive(false);
            _backToMainMenuButton.onClick.AddListener(OnBackToMainMenuButtonClicked);
        }
        else
        {
            _backToMainMenuButton.gameObject.SetActive(false);
            _nextLevelButton.gameObject.SetActive(true);
            _nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        }
    }

    private LeaderboardData LoadLeaderboardData()
    {
        string leaderboardDataPath = Path.Combine(Application.persistentDataPath, "leaderboard.json");

        if (File.Exists(leaderboardDataPath))
        {
            return JsonUtility.FromJson<LeaderboardData>(File.ReadAllText(leaderboardDataPath));
        }
        else
        {
            TextAsset defaultLeaderboardData = Resources.Load<TextAsset>("default_leaderboard");
            return JsonUtility.FromJson<LeaderboardData>(defaultLeaderboardData.text);
        }
    }

    private void SaveLeaderboardData(LeaderboardData leaderboardData, List<LeaderboardRowData> leaderboardRows)
    {
        string leaderboardDataPath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        leaderboardData.Rows = leaderboardRows.ToArray();
        string outputJson = JsonUtility.ToJson(leaderboardData, true);
        File.WriteAllText(leaderboardDataPath, outputJson);
    }

    private int AddPlayerToLeaderboard(List<LeaderboardRowData> leaderboardRows)
    {
        int indexOfPlayer = -1;

        // Adding the player to the leaderboard if they are in the top 100:
        if (leaderboardRows[^1].Score < GlobalController.Instance.GlobalPlayerScore)
        {
            for (int i = leaderboardRows.Count - 1; i >= 0; i--)
            {
                LeaderboardRowData leaderboardRowData = leaderboardRows[i];

                if (leaderboardRowData.Score >= GlobalController.Instance.GlobalPlayerScore)
                {
                    indexOfPlayer = i + 1;
                    leaderboardRows.Insert(indexOfPlayer,
                        new LeaderboardRowData(GlobalController.Instance.PlayerName, GlobalController.Instance.GlobalPlayerScore));
                    break;
                }
            }

            // In case the player is 1st on the leaderboard:
            if (indexOfPlayer == -1)
            {
                indexOfPlayer = 0;
                leaderboardRows.Insert(indexOfPlayer,
                    new LeaderboardRowData(GlobalController.Instance.PlayerName, GlobalController.Instance.GlobalPlayerScore));
            }
        }

        return indexOfPlayer;
    }

    private void SetupLeaderboardContent(List<LeaderboardRowData> leaderboardRows, int indexOfPlayer)
    {
        for (int i = 0; i < leaderboardRows.Count; i++)
        {
            LeaderboardRowData leaderboardRowData = leaderboardRows[i];

            LeaderboardRowView leaderboardRow = Instantiate(_leaderboardRowPrefab, _leaderboardContent);
            leaderboardRow.Setup(i + 1, leaderboardRowData.Name, leaderboardRowData.Score);

            if (i == indexOfPlayer)
            {
                _playersRow = leaderboardRow;
                _pinnedRow.Setup(i + 1, leaderboardRowData.Name, leaderboardRowData.Score);
                _pinnedRow.gameObject.SetActive(true);
            }
        }
    }

    private void ScrollToPlayersScore(int indexOfPlayer, int leaderboardRowsCount)
    {
        if (indexOfPlayer != -1)
        {
            float rowHeight = _leaderboardRowPrefab.GetComponent<RectTransform>().sizeDelta.y;
            int maximumRowsInContentWindow = (int)(_leaderboardScrollView.sizeDelta.y / rowHeight);
            float maximumScrollValue = leaderboardRowsCount - maximumRowsInContentWindow;
            float scrollValue = indexOfPlayer * rowHeight;
            scrollValue = Mathf.Clamp(scrollValue, 0.0f, maximumScrollValue * rowHeight);
            _leaderboardContent.anchoredPosition = new Vector2(0, scrollValue);
        }
    }

    private void Update()
    {
        UpdatePinnedRowPosition();
    }

    private void UpdatePinnedRowPosition()
    {
        if (_playersRow != null)
        {
            float rowHeight = _leaderboardRowPrefab.GetComponent<RectTransform>().sizeDelta.y;
            float lastVisiblePosition = _leaderboardScrollView.sizeDelta.y - rowHeight;

            float playersRowPosition = _playersRow.GetComponent<RectTransform>().anchoredPosition.y;
            float pinnedRowPosition = _leaderboardContent.anchoredPosition.y + playersRowPosition;

            pinnedRowPosition = Mathf.Clamp(pinnedRowPosition, -1f * lastVisiblePosition, 0);
            _pinnedRow.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, pinnedRowPosition);
        }
    }

    private void OnNextLevelButtonClicked()
    {
        GlobalController.Instance.CurrentLevel++;
        SceneManager.LoadScene("GameScene");
    }

    private void OnBackToMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void OnDestroy()
    {
        _backToMainMenuButton.onClick.RemoveAllListeners();
        _nextLevelButton.onClick.RemoveAllListeners();
    }
}