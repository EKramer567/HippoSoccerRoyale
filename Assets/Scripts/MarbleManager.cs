using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public void PlaySplash(Collider collide, Vector3 objPosition)
    {
        StartCoroutine(WaitForSplash(collide, objPosition));
    }

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

    private IEnumerator WaitForSplash(Collider coll, Vector3 pos)
    {
        yield return new WaitForSeconds(splashWaitTime);
        SplashEffect(coll.ClosestPoint(pos));
    }
}
