using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ZonesEnums;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    List<HippoMovementController> hippoMovementControllers;

    [SerializeField]
    List<MovementInputController> inputControllers;

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

    public void SetPlayerZoneAssignment(int index, PlayerZoneEnum zone)
    {
        hippoMovementControllers[index].AssignPlayerZone(zone);
        hippoMovementControllers[index].transform.position
                = ArenaLocations.Instance.StartLocationsList[(int)(zone)].position;
    }

    /// <summary>
    /// Put all players in the positions according to their zone assignment
    /// </summary>
    public void SetPlayerPosition(int index, PlayerZoneEnum zone)
    {
        for (int i = 0; i < hippoMovementControllers.Count; i++)
        {
            hippoMovementControllers[i].transform.position 
                = ArenaLocations.Instance.StartLocationsList[(int)(hippoMovementControllers[i].ThisPlayerZone)].position;
        }
    }
}
