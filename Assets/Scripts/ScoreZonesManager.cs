using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ZonesEnums;

namespace ZonesEnums
{
    public enum PlayerZoneEnum
    {
        TOP_RIGHT,
        TOP_LEFT,
        BOTTOM_RIGHT,
        BOTTOM_LEFT
    }

    public enum PlayerIdentity
    {
        PLAYER,
        COM_1,
        COM_2,
        COM_3,
        NONE
    }
}

/// <summary>
/// Class to handle the function of score zones and score zone assignments
/// </summary>
public class ScoreZonesManager : MonoBehaviour
{
    [Header("Score Identifier Text Fields")]
    [SerializeField]
    List<TMP_Text> playerIdentifierTextFields;

    const string PLAYER_HEADER = "P1";
    const string COM_HEADER = "COM";

    [Header("Score Number Fields (tr, tl, br, bl in order), max of 4 entries")]
    [SerializeField]
    List<TMP_Text> scoreNumFields;

    [Header("Score UI Panels (tr, tl, br, bl in order), max of 4 entries")]
    [SerializeField]
    List<Image> scoreNumPanels;

    [Header("Zone Tag Strings (tr, tl, br, bl in order), max of 4 entries")]
    [SerializeField]
    List<string> zoneTagStrings;

    [Header("Panel Colors")]
    [SerializeField]
    Color P1_Panel_Color;
    [SerializeField]
    Color COM_1_Panel_Color, COM_2_Panel_Color, COM_3_Panel_Color;

    CharacterZoneData[] characterZoneRelations = new CharacterZoneData[4];
    Score_UI_Element[] UI_ElementSections = new Score_UI_Element[4];

    Dictionary<string, int> tagToZoneIndex;

    public static ScoreZonesManager Instance { get; private set; }


    /// <summary>
    /// A struct to hold the data of which player's zone is where and what score value it has
    /// </summary>
    struct CharacterZoneData
    {
        PlayerZoneEnum zone;
        PlayerIdentity id;
        int zoneScoreValue;
        Score_UI_Element UI_Elements;

        public PlayerZoneEnum Zone { get { return zone; } }
        public PlayerIdentity Identifier { get { return id; } }
        public int ScoreValue { get { return zoneScoreValue; } }

        public void SetValues(PlayerZoneEnum z, PlayerIdentity identity, Score_UI_Element uiELementSection)
        {
            zone = z;
            id = identity;
            UI_Elements = uiELementSection;
        }

        public void IncrementScore()
        {
            zoneScoreValue++;
            UI_Elements.SetScoreText(zoneScoreValue.ToString());
        }

        public void ResetScore()
        {
            zoneScoreValue = 0;
            UI_Elements.SetScoreText(zoneScoreValue.ToString());
        }

        public void Set_UI_Colors(Color identifierColor, Color scoreNumColor, Color panelColor)
        {
            UI_Elements.Set_UI_Section_Colors(identifierColor, scoreNumColor, panelColor);
        }
    }

    /// <summary>
    /// A struct to try to organize all the UI elements to be coordinated and in one place for each section of the UI
    /// </summary>
    struct Score_UI_Element
    {
        TMP_Text identifierText;
        TMP_Text scoreText;
        Image panel;

        public void Set_UI_Section(TMP_Text idText, TMP_Text scoreNumText, Image panelImg)
        {
            identifierText = idText;
            scoreText = scoreNumText;
            panel = panelImg;
        }

        public void Set_UI_Section_Colors(Color idTextColor, Color scoreNumColor, Color panelColor)
        {
            identifierText.color = idTextColor;
            scoreText.color = scoreNumColor;
            panel.color = panelColor;
        }

        public void SetScoreText(string text)
        {
            scoreText.SetText(text);
        }

        public void SetIdentifierText(string text)
        {
            identifierText.SetText(text);
        }

        public override string ToString()
        {
            return panel.name + "|" + identifierText.name + "|" + scoreText.name;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initialize score values and randomize player zones
    /// </summary>
    private void Start()
    {
        tagToZoneIndex = new Dictionary<string, int>();
        // The amount of EVERYTHING here should always be 4
        try
        {
            for (int i = 0; i < zoneTagStrings.Count; i++)
            {
                tagToZoneIndex.Add(zoneTagStrings[i], i);
                UI_ElementSections[i].Set_UI_Section(playerIdentifierTextFields[i], scoreNumFields[i], scoreNumPanels[i]);
            }
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.LogError("ERROR! : The list of zone tags, identifier text fields, score num fields, or score panel fields is not size 4");
            Console.WriteLine(e.Message);
            throw new ArgumentException("Index parameter is out of range.", e);
        }

        ResetPlayerZones();
        ResetScores();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Increment the score by 1. To be called in OnTriggerEnter of a score zone
    /// </summary>
    /// <param name="zoneTag">The tag of the zone collision</param>
    public void AddPoint(string zoneTag)
    {
        characterZoneRelations[tagToZoneIndex[zoneTag]].IncrementScore();
    }

    /// <summary>
    /// Zero out all score values
    /// </summary>
    public void ResetScores()
    {
        for (int  i = 0; i < characterZoneRelations.Length; i++)
        {
            characterZoneRelations[i].ResetScore();
        }
        //Debug.Log("Scores have been reset");
    }

    /// <summary>
    /// Resets and re-randomizes the zones that each player is assigned to
    /// </summary>
    public void ResetPlayerZones()
    {
        // set all zone assignments randomly
        List<PlayerIdentity> randomIDs = new List<PlayerIdentity> { PlayerIdentity.PLAYER, PlayerIdentity.COM_1, PlayerIdentity.COM_2, PlayerIdentity.COM_3 };
        randomIDs = randomIDs.OrderBy(i => Guid.NewGuid()).ToList();

        // populate player and COM players to their random zones and change colors accordingly
        for (int i = 0; i < characterZoneRelations.Length; i++)
        {
            if (randomIDs[i] == PlayerIdentity.PLAYER)
            {
                UI_ElementSections[i].SetIdentifierText(PLAYER_HEADER);
            }
            else
            {
                UI_ElementSections[i].SetIdentifierText(COM_HEADER);
            }
            
            characterZoneRelations[i].SetValues((PlayerZoneEnum)i, randomIDs[i], UI_ElementSections[i]);
            Set_UI_Colors(randomIDs[i], UI_ElementSections[i]);
            PlayerManager.Instance.SetPlayerZoneAssignment(characterZoneRelations[i].Identifier, characterZoneRelations[i].Zone);
        }
        //Debug.Log("Disabling Highscores Panel");
    }

    /// <summary>
    /// Set UI Colors for panels and score text
    /// </summary>
    /// <param name="identity">The identity of the player whose panel section we're changing</param>
    /// <param name="uiElement">Reference to collection of UI elements for this section</param>
    void Set_UI_Colors(PlayerIdentity identity, Score_UI_Element uiElement)
    {
        switch (identity)
        {
            case PlayerIdentity.PLAYER:
                uiElement.Set_UI_Section_Colors(P1_Panel_Color.WithAlpha(1.0f), P1_Panel_Color.WithAlpha(255.0f), P1_Panel_Color);
                Debug.Log("Setting color of panel ID " + identity.ToString() + " to " + P1_Panel_Color.ToString());
                break;
            case PlayerIdentity.COM_1:
                uiElement.Set_UI_Section_Colors(Color.gray, Color.gray, COM_1_Panel_Color);
                Debug.Log("Setting color of panel ID " + identity.ToString() + " to " + COM_1_Panel_Color.ToString());
                break;
            case PlayerIdentity.COM_2:
                uiElement.Set_UI_Section_Colors(Color.gray, Color.gray, COM_2_Panel_Color);
                Debug.Log("Setting color of panel ID " + identity.ToString() + " to " + COM_2_Panel_Color.ToString());
                break;
            case PlayerIdentity.COM_3:
                uiElement.Set_UI_Section_Colors(Color.gray, Color.gray, COM_3_Panel_Color);
                Debug.Log("Setting color of panel ID " + identity.ToString() + " to " + COM_3_Panel_Color.ToString());
                break;
            default:
                uiElement.Set_UI_Section_Colors(Color.gray, Color.gray, Color.gray.WithAlpha(210.0f));
                Debug.LogError("Error: Attempted to set colors for player identity that was out of normal range: " + identity.ToString()
                    + " at UI Element " + uiElement.ToString());
                break;
        }
    }

    /// <summary>
    /// Sends the highest score on the field to be in the highscores 
    /// (this game isn't multiplayer so only do this if the player has the highest score)
    /// returns true if a new score was set, false if not
    /// </summary>
    public int GetWinningScore()
    {
        int[] tempScoreArr = new int[4];

        for (int i = 0; i < tempScoreArr.Length; i++)
        {
            tempScoreArr[i] = characterZoneRelations[i].ScoreValue;
        }

        int maxScore = Mathf.Max(tempScoreArr);

        for (int i = 0; i < characterZoneRelations.Length; i++)
        {
            if (characterZoneRelations[i].ScoreValue >= maxScore && characterZoneRelations[i].Identifier == PlayerIdentity.PLAYER)
            {
                return characterZoneRelations[i].ScoreValue;
            }
        }
        return -1;
    }
}
