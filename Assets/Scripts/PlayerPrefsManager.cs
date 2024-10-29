using System.Linq;
using TMPro;
using UnityEngine;

struct PlayerScore
{
    public int score;
    public string playerName;
    public bool isNewHighscore;
}

public class PlayerPrefsManager : MonoBehaviour
{
    string inputName;
    int newHighscore;

    const int NUM_SCORES_TO_KEEP = 10;

    const string HIGHSCORE_PREFIX = "HS_";
    const string PLAYER_NAME_PREFIX = "HS_PLAYER_";
    const string BLANK_PLAYER_SLOT = "---";
    const string INDEX_SUFFIX = "_SCORE_INDEX";

    PlayerScore[] highScoresPairs = new PlayerScore[NUM_SCORES_TO_KEEP];

    public static int SCORES_TO_KEEP { get { return NUM_SCORES_TO_KEEP; } }

    public static PlayerPrefsManager Instance { get; private set; }

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PopulateHighscorePairs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sort a new highscore entry into the existing scores
    /// </summary>
    /// <param name="newScore"></param>
    public void SortNewScoreToHighscores(int newScore) // *** TODO: add out string newName for player input
    {
        // sort the new score into the array if we have at least one score to replace
        if (highScoresPairs.Last().score < newScore)
        {
            int j;
            for (j = highScoresPairs.Length - 1; j >= 0 && highScoresPairs[j].score < newScore; j--)
            {
                if (j < highScoresPairs.Length - 1)
                {
                    highScoresPairs[j + 1].playerName = highScoresPairs[j].playerName;
                    highScoresPairs[j + 1].score = highScoresPairs[j].score;
                    highScoresPairs[j + 1].isNewHighscore = false;
                }
            }
            highScoresPairs[j + 1].playerName = inputName; // *** TODO: Add player input for their name
            highScoresPairs[j + 1].score = newHighscore;
            highScoresPairs[j + 1].isNewHighscore = true;
            SaveHighScorePlayerPrefs();
        }
    }

    /// <summary>
    /// Display the highscores panel with updated placements
    /// </summary>
    public void DisplayHighscores()
    {
        //PopulateHighscorePairs() to be called beforehand if you just want the pull from playerprefs instead of current
        for (int i = 0;  i < highScoresPairs.Length; i++)
        {
            HighscoresManager.Instance.UpdateHighscoreField(i, highScoresPairs[i].playerName, 
                highScoresPairs[i].score, highScoresPairs[i].isNewHighscore);
            if (highScoresPairs[i].isNewHighscore)
            {
                highScoresPairs[i].isNewHighscore = false;
            }
        }
    }

    /// <summary>
    /// Save off the highscores to playerprefs after they have been populated
    /// </summary>
    void SaveHighScorePlayerPrefs()
    {
        for (int i = 0; i < NUM_SCORES_TO_KEEP; i++)
        {
            PlayerPrefs.SetString(PLAYER_NAME_PREFIX + i.ToString() + INDEX_SUFFIX, highScoresPairs[i].playerName);
            PlayerPrefs.SetInt(HIGHSCORE_PREFIX + i.ToString() + INDEX_SUFFIX, highScoresPairs[i].score);
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Populate highscore pairs to be used in sorting later
    /// </summary>
    void PopulateHighscorePairs()
    {
        for (int i = 0; i < NUM_SCORES_TO_KEEP - 1; i++)
        {
            string playerKey = PLAYER_NAME_PREFIX + i.ToString() + INDEX_SUFFIX;
            string scoreKey = HIGHSCORE_PREFIX + i.ToString() + INDEX_SUFFIX;
            if (PlayerPrefs.HasKey(playerKey) && PlayerPrefs.HasKey(scoreKey))
            {
                // Get data from playerprefs to display
                highScoresPairs[i].playerName = PlayerPrefs.GetString(playerKey);
                highScoresPairs[i].score = PlayerPrefs.GetInt(scoreKey);
                highScoresPairs[i].isNewHighscore = false;
            }
            else
            {
                // Populate with blanks if there arent enough highscores set on the board to fill the slots
                highScoresPairs[i].playerName = BLANK_PLAYER_SLOT;
                highScoresPairs[i].score = 0;
                highScoresPairs[i].isNewHighscore = false;
            }
        }
    }

    public void InputNewPlayerHighscore(string newPlayerName, int newScore)
    {
        inputName = newPlayerName;
        newHighscore = newScore;
        SortNewScoreToHighscores(newHighscore);
    }
}
