using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Class to hold information about the arena that other classes need
/// </summary>
public class ArenaLocations : MonoBehaviour
{
    [Header("Starting Locations (tr, tl, br, bl in order), max of 4 entries")]
    [SerializeField]
    List<Transform> startLocations;

    [Header("Zone Locations (tr, tl, br, bl in order), max of 4 entries")]
    [SerializeField]
    List<Transform> zoneLocations;

    Vector3 centerLocation = new Vector3(0, 1.0f, 0);

    [SerializeField]
    float arenaRadius = 21.0f;

    /// <summary>
    /// List of starting locations, used to spawn the players closer to their assigned zone
    /// </summary>
    public List<Transform> StartLocationsList { get { return startLocations; } }

    /// <summary>
    /// List of score zones
    /// </summary>
    public List<Transform> ZoneLocationsList { get { return zoneLocations; } }

    /// <summary>
    /// Location of the center of the arena, used for facing the players inward on game start/reset
    /// </summary>
    public Vector3 CenterLocation { get { return centerLocation; } }

    /// <summary>
    /// Radius of the circular arena, used to help COM players stop chasing marbles that have fallen off
    /// </summary>
    public float ArenaRadius { get { return arenaRadius; } }

    public static ArenaLocations Instance { get; private set; }

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
}
