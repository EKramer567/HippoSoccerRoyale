using System.Linq;
using UnityEngine;

struct PlayerScore
{
    public int score;
    public string playerName;
}

public class PlayerPrefsManager : MonoBehaviour
{
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
                }
            }
            highScoresPairs[j + 1].playerName = BLANK_PLAYER_SLOT; // *** TODO: Add player input for their name
            highScoresPairs[j + 1].score = newScore;
        }
        SaveHighScorePlayerPrefs(); // *** TODO move this inside the if statement so we dont have to Save in order to display highscores
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

            // *** TODO write a function in this class that just returns whatever's in playerprefs
            // in case NONE of the highscores are updated
            HighscoresManager.Instance.UpdateHighscoreField(i, highScoresPairs[i].playerName, highScoresPairs[i].score);
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
            }
            else
            {
                // Populate with blanks if there arent enough highscores set on the board to fill the slots
                highScoresPairs[i].playerName = BLANK_PLAYER_SLOT;
                highScoresPairs[i].score = 0;
            }
        }
    }
}
