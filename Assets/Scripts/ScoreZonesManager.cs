using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    [Header("Zones")]
    [SerializeField]
    string topRightZone, topLeftZone, bottomRightZone, bottomLeftZone;

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

        playerZone = (PlayerZoneEnum)Random.Range(0, 3);

        switch (playerZone)
        {
            case PlayerZoneEnum.TOP_RIGHT:
                trText.SetText(PLAYER_HEADER);
                trText.color = Color.red;
                break;
            case PlayerZoneEnum.TOP_LEFT:
                tlText.SetText(PLAYER_HEADER);
                tlText.color = Color.red;
                break;
            case PlayerZoneEnum.BOTTOM_RIGHT:
                brText.SetText(PLAYER_HEADER);
                brText.color = Color.red;
                break;
            case PlayerZoneEnum.BOTTOM_LEFT:
                blText.SetText(PLAYER_HEADER);
                blText.color = Color.red;
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
            Debug.Log(zoneTag + " adds point! Current Score: " + trScoreVal);
        }
        else if (zoneTag == topLeftZone)
        {
            tlScoreVal++;
            tlNum.SetText(tlScoreVal.ToString());
            Debug.Log(zoneTag + " adds point! Current Score: " + tlScoreVal);
        }
        else if (zoneTag == bottomRightZone)
        {
            brScoreVal++;
            brNum.SetText(brScoreVal.ToString());
            Debug.Log(zoneTag + " adds point! Current Score: " + brScoreVal);
        }
        else if (zoneTag == bottomLeftZone)
        {
            blScoreVal++;
            blNum.SetText(blScoreVal.ToString());
            Debug.Log(zoneTag + " adds point! Current Score: " + blScoreVal);
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
        tlText.SetText(COM_HEADER);
        tlText.color = Color.gray;
        brText.SetText(COM_HEADER);
        brText.color = Color.gray;
        blText.SetText(COM_HEADER);
        blText.color = Color.gray;
    }
}
