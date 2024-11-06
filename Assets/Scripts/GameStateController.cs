using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameStateController : MonoBehaviour
{
    [SerializeField]
    TMP_Text timerText;

    [SerializeField]
    float newGameStartingTime = 60.0f;

    public enum GameStates
    {
        PLAYING,
        PAUSED,
        GAME_OVER
    }

    GameStates currentState = GameStates.PLAYING;

    float timer = 60.0f;
    int seconds = 60;

    UnityEvent<int> preGameFinishedEvent;

    UnityEvent<bool> gameFinishedEvent;

    UnityEvent gameRestartEvent;

    bool gameIsEnded = false;

    bool gameEndPanel = false;

    public static GameStateController Instance { get; private set; }

    public GameStates CurrentGameState { get { return currentState; } }

    public UnityEvent<bool> GameFinishedEvent { get { return gameFinishedEvent; } }

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
        if (gameFinishedEvent == null)
        {
            gameFinishedEvent = new UnityEvent<bool>();
        }
        if (preGameFinishedEvent == null)
        {
            preGameFinishedEvent = new UnityEvent<int>();
        }
        if (gameRestartEvent == null)
        {
            gameRestartEvent = new UnityEvent();
        }
        timer = newGameStartingTime;

        preGameFinishedEvent.AddListener(GetEndingPanel);
        preGameFinishedEvent.AddListener(HighscoresManager.Instance.GetNewScore);

        gameFinishedEvent.AddListener(HighscoresManager.Instance.EnableHighscoresPanel);

        gameRestartEvent.AddListener(HighscoresManager.Instance.DisableHighscoresPanel);
        gameRestartEvent.AddListener(HighscoresManager.Instance.DisableInputPanel);
        gameRestartEvent.AddListener(ScoreZonesManager.Instance.ResetScores);
        gameRestartEvent.AddListener(ScoreZonesManager.Instance.ResetPlayerZones);
        gameRestartEvent.AddListener(MarbleManager.Instance.DisableAllMarbles);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0 && !gameIsEnded)
        {
            timer -= Time.deltaTime;
            seconds = Convert.ToInt32(timer % 60);
            timerText.SetText(seconds.ToString());
        }
        else if (timer <= 0 && !gameIsEnded)
        {
            gameIsEnded = true;
            seconds = 0;
            timerText.SetText(seconds.ToString());

            currentState = GameStates.GAME_OVER;
            preGameFinishedEvent.Invoke(ScoreZonesManager.Instance.GetWinningScore());
            gameFinishedEvent.Invoke(gameEndPanel);
        }
    }

    void ResetTimer()
    {
        timer = newGameStartingTime;
        seconds = 60;
        timerText.SetText(seconds.ToString());
    }

    public void ResetGameState()
    {
        ResetTimer();
        gameIsEnded = false;
        currentState = GameStates.PLAYING;
        gameRestartEvent.Invoke();
    }

    public void GetEndingPanel(int winningScore)
    {
        // true if new highscore was made (greater than -1), false if not. Changes if the input panel or highscore panel displays on begin of game end
        gameEndPanel = (ScoreZonesManager.Instance.GetWinningScore() > 0);
    }
}
