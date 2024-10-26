using UnityEngine;
using TMPro;
using static GameStateController;
using UnityEngine.Events;

public class HighscoresManager : MonoBehaviour
{
    [SerializeField]
    GameObject highScoresPanel;

    [SerializeField]
    TMP_Text[] namesFields;

    [SerializeField]
    TMP_Text[] scoresFields;

    UnityEvent OnEnableHighscoresPanelEvent;

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

    private void Start()
    {
        if (OnEnableHighscoresPanelEvent == null)
        {
            OnEnableHighscoresPanelEvent = new UnityEvent();
        }
        DisableHighscoresPanel();

        OnEnableHighscoresPanelEvent.AddListener(PlayerPrefsManager.Instance.DisplayHighscores);
    }

    /// <summary>
    /// Update lines on the score panel to show correct info
    /// </summary>
    /// <param name="index">Index (function to be used in a for loop)</param>
    public void UpdateHighscoreField(int index, string name, int score, bool isNewHighscore)
    {
        namesFields[index].SetText(name);
        scoresFields[index].SetText(score.ToString());
        if (isNewHighscore) 
        {
            namesFields[index].color = Color.cyan;
            scoresFields[index].color = Color.cyan;
        }
        else
        {
            namesFields[index].color = Color.yellow;
            scoresFields[index].color = Color.yellow;
        }
    }

    public void EnableHighscoresPanel()
    {
        highScoresPanel.SetActive(true);
        OnEnableHighscoresPanelEvent.Invoke();
    }

    public void DisableHighscoresPanel()
    {
        highScoresPanel.SetActive(false);
        Debug.Log("Disabling Highscores Panel");
    }
}
