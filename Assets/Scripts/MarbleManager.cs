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

    float spawnDelayTimer = 0;
    bool spawnTimerActive = false;
    int activeCount;

    // Start is called before the first frame update
    void Start()
    {
        indeciesOfInactiveMarbles = new Queue<int>();
        Physics.gravity = new Vector3(0, -30, 0);
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
                Vector3 rand = new Vector3(Random.Range(0f, 0.5f), 0, Random.Range(0f, 0.5f));
                marbles[indeciesOfInactiveMarbles.Peek()].transform.position = spawnPoint.transform.position + rand;
                marbles[indeciesOfInactiveMarbles.Peek()].SetActive(true);
                indeciesOfInactiveMarbles.Dequeue();
                spawnDelayTimer = 0;
            }
        }
    }
}
