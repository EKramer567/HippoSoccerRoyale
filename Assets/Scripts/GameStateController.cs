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

    UnityEvent gameFinished;

    bool gameIsEnded = false;

    public static GameStateController Instance { get; private set; }

    public GameStates CurrentGameState { get { return currentState; } }

    public UnityEvent GameFinished { get { return gameFinished; } }

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
        if (gameFinished == null)
        {
            gameFinished = new UnityEvent();
        }
        timer = newGameStartingTime;

        gameFinished.AddListener(HighscoresManager.Instance.EnableHighscoresPanel);
        gameFinished.AddListener(ScoreZonesManager.Instance.GetWinningScore);
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
            gameFinished.Invoke();
        }
    }

    void ResetTimer()
    {
        timer = newGameStartingTime;
        seconds = 60;
        timerText.SetText(seconds.ToString());
    }
}
