using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreZonesManager : MonoBehaviour
{
    enum PlayerZoneEnum
    {
        TOP_RIGHT,
        TOP_LEFT,
        BOTTOM_RIGHT,
        BOTTOM_LEFT
    }

    PlayerZoneEnum playerZone;

    [Header("Score Text Fields")]
    [SerializeField]
    TMP_Text trText;
    [SerializeField]
    TMP_Text tlText;
    [SerializeField]
    TMP_Text brText;
    [SerializeField]
    TMP_Text blText;

    const string PLAYER_HEADER = "P1";
    const string COM_HEADER = "COM";

    [Header("Score Number Fields")]
    [SerializeField]
    TMP_Text trNum;
    [SerializeField]
    TMP_Text tlNum;
    [SerializeField]
    TMP_Text brNum;
    [SerializeField]
    TMP_Text blNum;

    [SerializeField]
    Image trPanel;
    [SerializeField]
    Image tlPanel;
    [SerializeField]
    Image brPanel;
    [SerializeField]
    Image blPanel;

    [Header("Zones")]
    [SerializeField]
    string topRightZone;
    [SerializeField]
    string topLeftZone;
    [SerializeField]
    string bottomRightZone;
    [SerializeField]
    string bottomLeftZone;

    [SerializeField]
    Color COM_Panel_Color, P1_Panel_Color;

    CharacterZonePair[] characterZonePairs = new CharacterZonePair[4];

    public static ScoreZonesManager Instance { get; private set; }

    struct CharacterZonePair
    {
        public PlayerZoneEnum zone;
        public int zoneScoreValue;
        public bool isPlayer;

        public void SetValues(PlayerZoneEnum z, int zoneScoreVal, bool isPlayerCharacter)
        {
            zone = z;
            zoneScoreValue = zoneScoreVal;
            isPlayer = isPlayerCharacter;
        }
    }

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
        ResetScores();
        ResetPlayerZone();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPoint(string zoneTag)
    {
        if (zoneTag == topRightZone)
        {
            characterZonePairs[0].zoneScoreValue++;
            trNum.SetText(characterZonePairs[0].zoneScoreValue.ToString());
        }
        else if (zoneTag == topLeftZone)
        {
            characterZonePairs[1].zoneScoreValue++;
            tlNum.SetText(characterZonePairs[1].zoneScoreValue.ToString());
        }
        else if (zoneTag == bottomRightZone)
        {
            characterZonePairs[2].zoneScoreValue++;
            brNum.SetText(characterZonePairs[2].zoneScoreValue.ToString());
        }
        else if (zoneTag == bottomLeftZone)
        {
            characterZonePairs[3].zoneScoreValue++;
            blNum.SetText(characterZonePairs[3].zoneScoreValue.ToString());
        }
        else 
        {
            Debug.LogError("Invalid zone was hit when attempting score trigger. Tag = " + zoneTag);
        }
    }

    public void ResetScores()
    {
        
        for (int  i = 0; i < characterZonePairs.Length; i++)
        {
            characterZonePairs[i].zoneScoreValue = 0;
        }
        trNum.SetText("0");
        tlNum.SetText("0");
        brNum.SetText("0");
        blNum.SetText("0");
        Debug.Log("Resetting scores");
    }

    public void ResetPlayerZone()
    {
        trText.SetText(COM_HEADER);
        trText.color = Color.gray;
        trPanel.color = COM_Panel_Color;
        characterZonePairs[0].SetValues(PlayerZoneEnum.TOP_RIGHT, 0, false);

        tlText.SetText(COM_HEADER);
        tlText.color = Color.gray;
        tlPanel.color = COM_Panel_Color;
        characterZonePairs[1].SetValues(PlayerZoneEnum.TOP_LEFT, 0, false);

        brText.SetText(COM_HEADER);
        brText.color = Color.gray;
        brPanel.color = COM_Panel_Color;
        characterZonePairs[2].SetValues(PlayerZoneEnum.BOTTOM_RIGHT, 0, false);

        blText.SetText(COM_HEADER);
        blText.color = Color.gray;
        blPanel.color = COM_Panel_Color;
        characterZonePairs[3].SetValues(PlayerZoneEnum.BOTTOM_LEFT, 0, false);

        playerZone = (PlayerZoneEnum)UnityEngine.Random.Range(0, 3);

        switch (playerZone)
        {
            case PlayerZoneEnum.TOP_RIGHT:
                trText.SetText(PLAYER_HEADER);
                trText.color = Color.red;
                trPanel.color = P1_Panel_Color;
                characterZonePairs[0].isPlayer = true;
                break;
            case PlayerZoneEnum.TOP_LEFT:
                tlText.SetText(PLAYER_HEADER);
                tlText.color = Color.red;
                tlPanel.color = P1_Panel_Color;
                characterZonePairs[1].isPlayer = true;
                break;
            case PlayerZoneEnum.BOTTOM_RIGHT:
                brText.SetText(PLAYER_HEADER);
                brText.color = Color.red;
                brPanel.color = P1_Panel_Color;
                characterZonePairs[2].isPlayer = true;
                break;
            case PlayerZoneEnum.BOTTOM_LEFT:
                blText.SetText(PLAYER_HEADER);
                blText.color = Color.red;
                blPanel.color = P1_Panel_Color;
                characterZonePairs[3].isPlayer = true;
                break;
            default:
                break;
        }

        Debug.Log("Disabling Highscores Panel");
    }

    /// <summary>
    /// Sends the highest score on the field to be in the highscores 
    /// (this game isn't multiplayer so only do this if the player has the highest score)
    /// </summary>
    public void GetWinningScore()
    {
        int[] tempScoreArr = new int[4];
        tempScoreArr[0] = characterZonePairs[0].zoneScoreValue;
        tempScoreArr[1] = characterZonePairs[1].zoneScoreValue;
        tempScoreArr[2] = characterZonePairs[2].zoneScoreValue;
        tempScoreArr[3] = characterZonePairs[3].zoneScoreValue;

        int maxScore = Mathf.Max(tempScoreArr);

        for (int i = 0; i < characterZonePairs.Length; i++)
        {
            if (characterZonePairs[i].zoneScoreValue >= maxScore && characterZonePairs[i].isPlayer)
            {
                PlayerPrefsManager.Instance.SortNewScoreToHighscores(characterZonePairs[i].zoneScoreValue);
                break;
            }
        }
    }
}
