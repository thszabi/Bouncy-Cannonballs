using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSceneController : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private NumberSelectorPanelController _levelSelector;

    private void Start()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _quitButton.onClick.AddListener(OnQuitButtonClicked);

        int numberOfLevels = Resources.LoadAll<LevelResource>("").Length;

        _levelSelector.Setup(1, 1, numberOfLevels);
    }

    private void OnStartButtonClicked()
    {
        string playerName = string.IsNullOrEmpty(_nameInputField.text) ? "Player" : _nameInputField.text;
        GlobalController.Instance.StartNewGameSession(playerName, _levelSelector.Value);
        SceneManager.LoadScene("GameScene");
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        _startButton.onClick.RemoveListener(OnStartButtonClicked);
        _quitButton.onClick.RemoveListener(OnQuitButtonClicked);
    }
}