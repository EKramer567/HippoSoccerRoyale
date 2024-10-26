using UnityEngine;
using TMPro;
using static GameStateController;

public class HighscoresManager : MonoBehaviour
{
    [SerializeField]
    GameObject highScoresPanel;

    [SerializeField]
    TMP_Text[] namesFields;

    [SerializeField]
    TMP_Text[] scoresFields;

    public static HighscoresManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Update lines on the score panel to show correct info
    /// </summary>
    /// <param name="index">Index (function to be used in a for loop)</param>
    public void UpdateHighscoreField(int index, string name, int score)
    {
        namesFields[index].SetText(name);
        scoresFields[index].SetText(score.ToString());
    }

    public void EnableHighscoresPanel()
    {
        highScoresPanel.SetActive(true);
    }

    public void DisableHighscoresPanel()
    {
        highScoresPanel.SetActive(false);
    }
}
