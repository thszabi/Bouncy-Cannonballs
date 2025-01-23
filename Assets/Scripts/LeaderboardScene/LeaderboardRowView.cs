using TMPro;
using UnityEngine;

public class LeaderboardRowView : MonoBehaviour
{
    [SerializeField] private TMP_Text _rankText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _scoreValueText;

    public void Setup(int rank, string name, int scoreValue)
    {
        _rankText.text = rank.ToString() + ".";
        _nameText.text = name;
        _scoreValueText.text = scoreValue.ToString();
    }
}
