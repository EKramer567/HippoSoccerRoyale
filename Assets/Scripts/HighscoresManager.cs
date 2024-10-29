using UnityEngine;
using TMPro;
using static GameStateController;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class HighscoresManager : MonoBehaviour
{
    [SerializeField]
    GameObject highScoresPanel;

    [SerializeField]
    GameObject inputPanel;

    [SerializeField]
    TMP_InputField inputField;

    [SerializeField]
    TMP_Text inputPanelScoreText;

    [SerializeField]
    TMP_Text[] namesFields;

    [SerializeField]
    TMP_Text[] scoresFields;

    const string UI_ACTION_MAP_NAME = "UI";
    const string UI_SUBMIT_ACTION_NAME = "Submit";
    InputAction submitAction;
    [SerializeField]
    private InputActionAsset UI_submitActionAsset;

    const string INPUT_VALID_CHARACTERS = 
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
        "abcdefghijklmnopqrstuvwxyz" +
        "1234567890!@#$%^&*()_+<>:| ";
    const int INPUT_CHARACTER_LIMIT = 14;

    UnityEvent<string, int> OnReceiveInputEvent;

    UnityEvent OnEnableHighscoresPanelEvent;

    int winningHighScore;

    public bool SubmittedName { get; private set; }

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

        submitAction = UI_submitActionAsset.FindActionMap(UI_ACTION_MAP_NAME).FindAction(UI_SUBMIT_ACTION_NAME);
        RegisterInputActions();
    }

    private void Start()
    {
        if (OnEnableHighscoresPanelEvent == null)
        {
            OnEnableHighscoresPanelEvent = new UnityEvent();
        }

        if (OnReceiveInputEvent == null)
        {
            OnReceiveInputEvent = new UnityEvent<string, int>();
        }

        submitAction = UI_submitActionAsset.FindActionMap(UI_ACTION_MAP_NAME).FindAction(UI_SUBMIT_ACTION_NAME);
        submitAction.Disable();

        // set character limit for name input
        inputField.characterLimit = INPUT_CHARACTER_LIMIT;
        inputField.text = "";

        DisableHighscoresPanel();

        OnReceiveInputEvent.AddListener(PlayerPrefsManager.Instance.InputNewPlayerHighscore);
        OnEnableHighscoresPanelEvent.AddListener(PlayerPrefsManager.Instance.DisplayHighscores);

        inputField.onValidateInput = (string text, int charIndex, char addedChar) =>
        {
            return ValidateChar(INPUT_VALID_CHARACTERS, addedChar);
        };
    }

    private void Update()
    {
        // submitAction is disabled until the input panel is enabled, so this only works while the panel is active
        // just acts as a way to submit the text by hitting Enter or something similar instead of having to click the button
        if (submitAction.IsPressed())
        {
            ReceiveInput();
        }
    }

    void RegisterInputActions()
    {
        submitAction.performed += context => SubmittedName = true;
        submitAction.performed += context => SubmittedName = false;
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

    public void EnableInputPanel()
    {
        inputField.text = "";
        inputPanel.SetActive(true);
        submitAction.Enable();
    }

    public void ReceiveInput()
    {
        OnReceiveInputEvent.Invoke(inputField.text, winningHighScore);
        DisableInputPanel();
        EnableHighscoresPanel(false); //we're done setting a new high score and getting the name, so open the highscores panel
    }

    public void DisableInputPanel()
    {
        inputPanel.SetActive(false);
        submitAction.Disable();
    }

    public void EnableHighscoresPanel(bool newHighscoreSet)
    {
        if (newHighscoreSet)
        {
            EnableInputPanel();
        }
        else
        {
            highScoresPanel.SetActive(true);
            OnEnableHighscoresPanelEvent.Invoke();
        }
    }

    public void DisableHighscoresPanel()
    {
        highScoresPanel.SetActive(false);
        Debug.Log("Disabling Highscores Panel");
    }

    public void GetNewScore(int winningScore)
    {
        winningHighScore = winningScore;
        inputPanelScoreText.SetText(winningHighScore.ToString());
    }

    private char ValidateChar(string validChars, char addedChar)
    {
        if (validChars.IndexOf(addedChar) != -1)
        {
            // character is valid, return the char
            return addedChar;
        }
        else
        {
            // character is invalid, return empty char
            return '\0';
        }
    }
}
