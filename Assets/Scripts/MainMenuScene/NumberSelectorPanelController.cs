using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumberSelectorPanelController : MonoBehaviour
{
    public int Value { get; private set; }

    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private TMP_Text _valueText;

    private int _minimum = 0;
    private int _maximum = 0;

    public void Setup(int startingNumber, int minimum, int maximum)
    {
        Value = startingNumber;
        _minimum = minimum;
        _maximum = maximum;

        _valueText.text = Value.ToString();
    }

    private void Start()
    {
        _leftButton.onClick.AddListener(OnLeftButtonClicked);
        _rightButton.onClick.AddListener(OnRightButtonClicked);
    }

    private void OnLeftButtonClicked()
    {
        if (_minimum < Value)
        {
            Value--;
            _valueText.text = Value.ToString();
        }
    }

    private void OnRightButtonClicked()
    {
        if (Value < _maximum)
        {
            Value++;
            _valueText.text = Value.ToString();
        }
    }

    private void OnDestroy()
    {
        _leftButton.onClick.RemoveListener(OnLeftButtonClicked);
        _rightButton.onClick.RemoveListener(OnRightButtonClicked);
    }
}