using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A class to handle marble object spawning, as well as particle effects that happen when they fall off the arena
/// </summary>
public class MarbleManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject> marbles;

    Queue<int> indeciesOfInactiveMarbles;

    [SerializeField]
    float spawnDelayTime = 1.5f;

    [SerializeField]
    GameObject spawnPoint;

    [SerializeField]
    int maxNumberActiveMarbles = 5;

    [SerializeField]
    List<ParticleSystem> splashParticleObjectsList;

    Queue<ParticleSystem> splashParticleObjects;

    [SerializeField]
    float splashWaitTime = 0.2f;

    float spawnDelayTimer = 0;
    bool spawnTimerActive = false;
    int activeCount;

    public List<GameObject> SceneMarbles { get { return marbles; } }

    public static MarbleManager Instance { get; private set; }

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
        indeciesOfInactiveMarbles = new Queue<int>();
        splashParticleObjects = new Queue<ParticleSystem>();
        Physics.gravity = new Vector3(0, -30, 0);
    }

    private void Start()
    {
        foreach (ParticleSystem p in splashParticleObjectsList)
        {
            splashParticleObjects.Enqueue(p);
        }
    }


    // Update is called once per frame
    void Update()
    {
        activeCount = marbles.Count(obj => obj.activeInHierarchy);
        if (activeCount < maxNumberActiveMarbles)
        {
            for (int i = 0; i < marbles.Count; i++)
            {
                // collect which marbles are currently inactive
                if (!marbles[i].activeInHierarchy && !indeciesOfInactiveMarbles.Contains(i))
                {
                    indeciesOfInactiveMarbles.Enqueue(i);
                    spawnTimerActive = true;
                }
            }
        }
        else
        {
            spawnTimerActive = false;
        }
        // re-enable the first available disabled marble after the spawn delay timer
        UpdateMarbleSpawning();
    }

    /// <summary>
    /// Spawn marbles in a random range of locations around the arena after a set time and only while they are not all on field
    /// </summary>
    void UpdateMarbleSpawning()
    {
        if (spawnTimerActive && activeCount < maxNumberActiveMarbles)
        {
            spawnDelayTimer += Time.deltaTime;
            if (spawnDelayTimer >= spawnDelayTime)
            {
                // random vector to prevent the marbles stacking up on eachother when spawning undisturbed
                Vector3 rand = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
                marbles[indeciesOfInactiveMarbles.Peek()].transform.position = spawnPoint.transform.position + rand;
                marbles[indeciesOfInactiveMarbles.Peek()].SetActive(true);
                indeciesOfInactiveMarbles.Dequeue();
                spawnDelayTimer = 0;
            }
        }
    }

    /// <summary>
    /// Disable all marble objects
    /// Used in resetting
    /// </summary>
    public void DisableAllMarbles()
    {
        spawnDelayTimer = 0;
        foreach (GameObject marb in marbles)
        {
            marb.SetActive(false);
        }
        //Debug.Log("Disabling all marbles on field");
    }

    /// <summary>
    /// Play the splash particle effect when colliding with water
    /// </summary>
    /// <param name="collide">The collider of the water</param>
    /// <param name="objPosition">Position of the object at the point of collision</param>
    public void PlaySplash(Collider collide, Vector3 objPosition)
    {
        StartCoroutine(WaitForSplash(collide, objPosition));
    }

    /// <summary>
    /// Cycle through particle objects and play one that isn't being used
    /// </summary>
    /// <param name="location">Location at which to play this particle effect</param>
    private void SplashEffect(Vector3 location)
    {
        ParticleSystem frontEffect = splashParticleObjects.Peek();
        if (!frontEffect.isPlaying)
        {
            splashParticleObjects.Peek().gameObject.SetActive(true);
            splashParticleObjects.Peek().transform.position = location;
            splashParticleObjects.Peek().Play();
            splashParticleObjects.Enqueue(splashParticleObjects.Dequeue());
        }
    }

    /// <summary>
    /// Wait for a time before playing a splash particle effect
    /// </summary>
    /// <param name="coll">Collider of the water</param>
    /// <param name="pos">Position of marble</param>
    /// <returns>WaitForSeconds</returns>
    private IEnumerator WaitForSplash(Collider coll, Vector3 pos)
    {
        yield return new WaitForSeconds(splashWaitTime);
        SplashEffect(coll.ClosestPoint(pos));
    }
}
