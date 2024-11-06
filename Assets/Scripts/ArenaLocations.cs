using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArenaLocations : MonoBehaviour
{
    [Header("Starting Locations (tr, tl, br, bl in order), max of 4 entries")]
    [SerializeField]
    List<Transform> startLocations;

    [Header("Zone Locations (tr, tl, br, bl in order), max of 4 entries")]
    [SerializeField]
    List<Transform> zoneLocations;

    Vector3 centerLocation = new Vector3(0, 1.91f, 0);

    public List<Transform> StartLocationsList { get { return startLocations; } }

    public List<Transform> ZoneLocationsList { get { return zoneLocations; } }

    public Vector3 CenterLocation { get { return centerLocation; } }

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
