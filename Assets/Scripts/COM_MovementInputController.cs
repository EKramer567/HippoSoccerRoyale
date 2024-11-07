using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

/// <summary>
/// Controller class for COM actions
/// </summary>
public class COM_MovementInputController : InputControllerData
{
    // List of marbles for the COMs to keep track of
    List<GameObject> targetMarbles;
    List<Marble> marbleComponents;

    // Location and direction information for the COM to calculate movement vector
    Vector2 direction;
    Vector3 target;
    Vector3 interceptTargetLocation;

    // Whether to start a new scan this update (prevents scanning every update to prevent lag), and whether a close target has been found
    bool startNewScan = true;
    bool closestSet = false;

    // Time between scans and a buffer to determine whether the COM has moved too close to target (prevents stuttering)
    const float CHECK_INTERVAL = 0.1f;
    const float DISTANCE_FROM_TARGET_BUFFER = 2.0f;

    void Start()
    {        
        targetMarbles = MarbleManager.Instance.SceneMarbles;
        marbleComponents = new List<Marble>();

        for (int i = 0; i < targetMarbles.Count; i++)
        {
            marbleComponents.Add(targetMarbles[i].GetComponent<Marble>());
        }
    }

    private void FixedUpdate()
    {
        Debug.DrawLine(ArenaLocations.Instance.CenterLocation, new Vector3(ArenaLocations.Instance.ArenaRadius, 1, 0), Color.black);
        if (MovementEnabled)
        {
            ScanForTarget();
            SetMoveValue();
        }
        else
        {
            MovementValue = Vector2.zero;
        }
    }

    /// <summary>
    /// Take in the target and create a normalized movement vector to use as a fake input
    /// </summary>
    void SetMoveValue()
    {
        float targetDistance = Vector3.Distance(target, transform.position);

        Vector2 fakeInputValue;

        if (targetDistance > DISTANCE_FROM_TARGET_BUFFER)
        {
            // go after closest marbles
            Vector3 v3Direction = target - transform.position;
            fakeInputValue = new Vector2(v3Direction.x, v3Direction.z).normalized;
        }
        else
        {
            fakeInputValue = Vector2.zero;
        }

        MovementValue = fakeInputValue;
    }

    /// <summary>
    /// Find a target to start moving toward
    /// </summary>
    void ScanForTarget()
    {
        if (startNewScan)
        {
            startNewScan = false;
            target = FindTarget();
            StartCoroutine(WaitForSearch());
            Debug.DrawLine(transform.position, target, Color.black);
            //Debug.Log("New Scan Started...");
        }
        Debug.DrawLine(transform.position, target, Color.cyan);
    }

    /// <summary>
    /// Find the closest target marble, prioritizing marbles that are closer to this COM's score zone
    /// </summary>
    /// <returns>Location of the target marble</returns>
    Vector3 FindTarget()
    {
        float minDist = 10000;
        float minGoalDist = 10000;

        // begin by setting closestPosition to the target position that was correct the previous update
        Vector3 closestPosition = target;

        if (targetMarbles.Count == 0)
        {
            //dont need to move since there's nothing to chase after
            return transform.position;
        }

        closestSet = false;

        for (int i = 0; i < targetMarbles.Count - 1; i++)
        {
            float dist = Vector3.Distance(targetMarbles[i].transform.position, transform.position);

            float goalDist = Vector3.Distance(targetMarbles[i].transform.position, ZoneLocation);

            // only go after marbles that are active and aren't currently in the air or falling off the arena
            // and prioritize marbles that are closer to this COM's score zone
            if (dist < minDist && goalDist < minGoalDist && targetMarbles[i].activeInHierarchy 
                && Mathf.Abs(targetMarbles[i].transform.position.y - transform.position.y) < DISTANCE_FROM_TARGET_BUFFER
                && Mathf.Abs(Vector3.Distance(targetMarbles[i].transform.position, ArenaLocations.Instance.CenterLocation)) < ArenaLocations.Instance.ArenaRadius)
            {
                marbleComponents[i].IsTargeted = true;
                closestPosition = targetMarbles[i].transform.position;
                minDist = dist;
                minGoalDist = goalDist;
                closestSet = true;
                //Debug.Log("Target Set - targetMarbles[" + i + "] ");
            }
        }
        if (!closestSet)
        {
            closestPosition = StartLocation;
            //Debug.Log("00000 Setting target to default");
        }

        return closestPosition;
    }

    IEnumerator WaitForSearch()
    {
        yield return new WaitForSeconds(CHECK_INTERVAL);
        startNewScan = true;
    }

    /// <summary>
    /// Kick action to be called when appropriate for the COM to kick a marble
    /// </summary>
    void Kick()
    {
        if (!Kicking)
        {
            Kicking = true;
        }
    }

    /// <summary>
    /// Override to allow the COM's kick action to be read later on
    /// </summary>
    /// <returns>Whether the kick action should be executed</returns>
    public override bool GetKickActionPressed()
    {
        if (Kicking)
        {
            Kicking = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
