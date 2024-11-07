using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ZonesEnums;
using static UnityEditor.Experimental.GraphView.GraphView;
using Unity.VisualScripting;

/// <summary>
/// Class to handle spawn locations and zone assignments
/// </summary>
public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    List<HippoMovementController> hippoMovementControllers;

    [SerializeField]
    List<InputControllerData> inputControllers;

    public static PlayerManager Instance { get; private set; }

    private void Awake()
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

    /// <summary>
    /// Set a player's zone assignment
    /// </summary>
    /// <param name="id">Identifier of this player</param>
    /// <param name="zone">Zone that this player should be assigned to</param>
    public void SetPlayerZoneAssignment(PlayerIdentity id, PlayerZoneEnum zone)
    {
        if (id < PlayerIdentity.NONE)
        {
            // Assign all players their zone
            hippoMovementControllers[(int)id].AssignPlayerZone(zone);

            // Put all players in the positions according to their zone assignment
            hippoMovementControllers[(int)id].transform.position = ArenaLocations.Instance.StartLocationsList[(int)(zone)].position;

            // Give the player controllers the location of their starting position (to use as default navigation)
            inputControllers[(int)id].StartLocation = ArenaLocations.Instance.StartLocationsList[(int)(zone)].position;

            // Give the player controllers the location of the zone they're trying to score into
            inputControllers[(int)id].ZoneLocation = ArenaLocations.Instance.ZoneLocationsList[(int)(zone)].position;

            // Point player toward center of the arena
            hippoMovementControllers[(int)id].PointPlayerToCenter();

            //Debug.Log("Setting player starting positions - " + hippoMovementControllers[(int)(zone)].name + " gets starting position "
            //+ ArenaLocations.Instance.StartLocationsList[(int)(zone)].position + " which indicates the " + zone.ToString() + " zone...");
        }
        else
        {
            Debug.LogError("Player Zone Assignment failed - Identity was out of range of usable values");
        }
    }
}
