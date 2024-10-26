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

    int trScoreVal, tlScoreVal, brScoreVal, blScoreVal = 0;

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

    public static ScoreZonesManager Instance { get; private set; }

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

        playerZone = (PlayerZoneEnum)UnityEngine.Random.Range(0, 3);

        switch (playerZone)
        {
            case PlayerZoneEnum.TOP_RIGHT:
                trText.SetText(PLAYER_HEADER);
                trText.color = Color.red;
                trPanel.color = P1_Panel_Color;
                break;
            case PlayerZoneEnum.TOP_LEFT:
                tlText.SetText(PLAYER_HEADER);
                tlText.color = Color.red;
                tlPanel.color = P1_Panel_Color;
                break;
            case PlayerZoneEnum.BOTTOM_RIGHT:
                brText.SetText(PLAYER_HEADER);
                brText.color = Color.red;
                brPanel.color = P1_Panel_Color;
                break;
            case PlayerZoneEnum.BOTTOM_LEFT:
                blText.SetText(PLAYER_HEADER);
                blText.color = Color.red;
                blPanel.color = P1_Panel_Color;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPoint(string zoneTag)
    {
        if (zoneTag == topRightZone)
        {
            trScoreVal++;
            trNum.SetText(trScoreVal.ToString());
        }
        else if (zoneTag == topLeftZone)
        {
            tlScoreVal++;
            tlNum.SetText(tlScoreVal.ToString());
        }
        else if (zoneTag == bottomRightZone)
        {
            brScoreVal++;
            brNum.SetText(brScoreVal.ToString());
        }
        else if (zoneTag == bottomLeftZone)
        {
            blScoreVal++;
            blNum.SetText(blScoreVal.ToString());
        }
        else 
        {
            Debug.LogError("Invalid zone was hit when attempting score trigger. Tag = " + zoneTag);
        }
    }

    public void ResetScores()
    {
        trScoreVal = 0;
        tlScoreVal = 0;
        brScoreVal = 0;
        blScoreVal = 0;
        trNum.SetText("0");
        trNum.SetText("0");
        brNum.SetText("0");
        blNum.SetText("0");
    }

    public void ResetPlayerZone()
    {
        trText.SetText(COM_HEADER);
        trText.color = Color.gray;
        trPanel.color = COM_Panel_Color;

        tlText.SetText(COM_HEADER);
        tlText.color = Color.gray;
        tlPanel.color = COM_Panel_Color;

        brText.SetText(COM_HEADER);
        brText.color = Color.gray;
        brPanel.color = COM_Panel_Color;

        blText.SetText(COM_HEADER);
        blText.color = Color.gray;
        blPanel.color = COM_Panel_Color;
    }

    /// <summary>
    /// Sends the highest score on the field to be in the highscores 
    /// (this game isn't multiplayer so only do this if the player has the highest score)
    /// </summary>
    public void GetWinningScore()
    {
        int[] tempScoreArr = new int[4];
        tempScoreArr[0] = trScoreVal;
        tempScoreArr[1] = tlScoreVal;
        tempScoreArr[2] = brScoreVal;
        tempScoreArr[3] = blScoreVal;

        int maxScore = Mathf.Max(tempScoreArr);

        switch (playerZone)
        {
            case PlayerZoneEnum.TOP_RIGHT:
                if (trScoreVal == maxScore)
                {
                    PlayerPrefsManager.Instance.SortNewScoreToHighscores(maxScore);
                }
                break;
            case PlayerZoneEnum.TOP_LEFT:
                if (tlScoreVal == maxScore)
                {
                    PlayerPrefsManager.Instance.SortNewScoreToHighscores(maxScore);
                }
                break;
            case PlayerZoneEnum.BOTTOM_RIGHT:
                if (brScoreVal == maxScore)
                {
                    PlayerPrefsManager.Instance.SortNewScoreToHighscores(maxScore);
                }
                break;
            case PlayerZoneEnum.BOTTOM_LEFT:
                if (blScoreVal == maxScore)
                {
                    PlayerPrefsManager.Instance.SortNewScoreToHighscores(maxScore);
                }
                break;
            default:
                break;
        }
    }
}
