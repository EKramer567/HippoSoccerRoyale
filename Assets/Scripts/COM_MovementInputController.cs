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

    Vector3 targetMarbleLocation;

    // Whether to start a new scan this update (prevents scanning every update to prevent lag), and whether a close target has been found
    bool startNewScan = true;
    bool closestSet = false;

    [SerializeField]
    LayerMask marbleMask;

    // Time between scans and a buffer to determine whether the COM has moved too close to target (prevents stuttering)
    const float CHECK_INTERVAL = 0.1f;
    const float DISTANCE_FROM_TARGET_BUFFER = 2.0f;
    const float UNITS_BEHIND_MARBLE_OFFSET = 2.0f;
    const float DISTANCE_FROM_INTERCEPT_BUFFER = 2.0f;
    const float SPHERECAST_RADIUS = 2f;
    const float SPHERECAST_DISTANCE = 50.0f;

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
            // stop moving
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
        }

        // Debugging
        Debug.DrawLine(ZoneLocation, target, Color.magenta);
        Debug.DrawRay(target, -(ZoneLocation - target), Color.yellow);
        Vector3 intercept = FindInterceptTarget(target);
        Vector3 closestPerp = FindClosestPerpendicular(-(ZoneLocation - target), targetMarbleLocation, UNITS_BEHIND_MARBLE_OFFSET);
        Debug.DrawLine(targetMarbleLocation, closestPerp, Color.black);
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

        for (int i = 0; i < targetMarbles.Count; i++)
        {
            float dist = Vector3.Distance(targetMarbles[i].transform.position, transform.position);

            float goalDist = Vector3.Distance(targetMarbles[i].transform.position, ZoneLocation);

            // only go after marbles that are active and aren't currently in the air or falling off the arena
            // and prioritize marbles that are closer to this COM's score zone
            if (goalDist < minGoalDist && targetMarbles[i].activeInHierarchy 
                && Mathf.Abs(targetMarbles[i].transform.position.y - transform.position.y) < 4.0f
                && Vector3.Distance(targetMarbles[i].transform.position, ArenaLocations.Instance.CenterLocation) < ArenaLocations.Instance.ArenaRadius)
            {
                interceptTargetLocation = FindInterceptTarget(targetMarbles[i].transform.position);

                // In order to try to get the marble into its own goal, COM needs to get behind the marble (mostly)
                if (Vector3.Distance(interceptTargetLocation, transform.position) > DISTANCE_FROM_INTERCEPT_BUFFER)
                {
                    RaycastHit hit;
                    Vector3 dir = (interceptTargetLocation - transform.position).normalized;
                    
                    // Cast a sphere toward the target intercept point and see if we'd collide with the marble we're after
                    if (Physics.SphereCast(transform.position, SPHERECAST_RADIUS, dir, out hit, SPHERECAST_DISTANCE, marbleMask))
                    {
                        if (hit.collider.gameObject == targetMarbles[i])
                        {
                            // Moving toward the target marble's intercept point would collide with the marble, move around it first
                            closestPosition = FindClosestPerpendicular(dir, targetMarbles[i].transform.position, UNITS_BEHIND_MARBLE_OFFSET);
                            minDist = dist;
                            minGoalDist = goalDist;
                            closestSet = true;
                        }
                    }
                    else
                    {
                        // Otherwise, just go for the intercept point
                        closestPosition = interceptTargetLocation;
                        minDist = dist;
                        minGoalDist = goalDist;
                        closestSet = true;
                    }
                }
                else
                {
                    closestPosition = targetMarbles[i].transform.position;
                    minDist = dist;
                    minGoalDist = goalDist;
                    closestSet = true;
                }

                // Debugging
                targetMarbleLocation = targetMarbles[i].transform.position;
            }
        }

        // If we didn't find a suitable target, just head back to the default position
        if (!closestSet)
        {
            closestPosition = StartLocation;
        }

        return closestPosition;
    }

    /// <summary>
    /// Calculate a point behind a marble where, if pushed from that side, the marble would go toward
    /// the COM's score zone
    /// </summary>
    /// <param name="marbleLocation">Position of the marble we're looking at</param>
    /// <returns></returns>
    Vector3 FindInterceptTarget(Vector3 marbleLocation)
    {        
        // step 1: create a vector indicating the direction between the target marble and the player's score zone location
        Vector3 behindMarbleDir = -(ZoneLocation - marbleLocation).normalized;

        // 1.1: make a line behind the marble using that direction vector, indicating a range of potential interception points
        //      (offset this line slightly so no potential interception points end up inside the marble)
        Vector3 behindMarblePos = marbleLocation + (behindMarbleDir * UNITS_BEHIND_MARBLE_OFFSET);
        Vector3 behindMarbleEndPos = marbleLocation + (behindMarbleDir * (UNITS_BEHIND_MARBLE_OFFSET + 2));

        // step 2: going backward from the step 2 vector, get the closest interception point where the player should go to be able to aim the marble at their zone
        return GetClosestPointOnFiniteLine(transform.position, behindMarblePos, behindMarbleEndPos);

        // step 3: use that point to calculate the difference between the player-marble vector and the marble-zone vector and turn toward the marble
        //         if the vectors are similar enough (if player distance from the closest point along that line is small enough)
    }

    /// <summary>
    /// Find a point perpendicular to a line at a particular point, at a particular distance away
    /// This is used when moving in a straight line toward an intercept point would make the COM collide with the target marble
    /// </summary>
    /// <param name="dirLine">A direction vector to act as the line to get a perpendicular point from</param>
    /// <param name="point">A point on that direction vector line to project the perpendicular line from</param>
    /// <param name="distance">How far the resulting location should be from the perpendicular line</param>
    /// <returns>A point along the perpendicular line, at the designated distance away from the origin</returns>
    Vector3 FindClosestPerpendicular(Vector3 dirLine, Vector3 point, float distance)
    {
        Vector3 perpTar = Vector3.zero;

        Vector3 left = Vector3.Cross(dirLine.normalized, Vector3.up).normalized;
        Vector3 right = -left;

        Vector3 projPointLeft = point + (left * UNITS_BEHIND_MARBLE_OFFSET * 2);
        Vector3 projPointRight = point + (right * UNITS_BEHIND_MARBLE_OFFSET * 2);

        if (Vector3.Distance(transform.position, projPointLeft) < Vector3.Distance(transform.position, projPointRight))
        {
            perpTar = projPointLeft;
        }
        else
        {
            perpTar = projPointRight;
        }

        return perpTar;
    }

    /// <summary>
    /// Coroutine just for waiting a certain time between target scans
    /// Just so the COM doesn't scan every update
    /// </summary>
    /// <returns>WaitForSeconds</returns>
    IEnumerator WaitForSearch()
    {
        yield return new WaitForSeconds(CHECK_INTERVAL);
        startNewScan = true;
    }

    /// <summary>
    /// Kick action to be called when appropriate for the COM to kick a marble
    /// (I decided not to implement this since the COM players are already difficult enough)
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

    /// <summary>
    /// Get closest point to the specified location, on a line that has beginning and ending point
    /// </summary>
    /// <param name="loc">The location of the object we want to measure from</param>
    /// <param name="startPoint">The beginning of the line segment to follow</param>
    /// <param name="endPoint">The end of the line segment</param>
    /// <returns>Closest point on that line relative to the designated location</returns>
    Vector3 GetClosestPointOnFiniteLine(Vector3 loc, Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 line_direction = endPoint - startPoint;
        float line_length = line_direction.magnitude;
        line_direction.Normalize();
        float project_length = Mathf.Clamp(Vector3.Dot(loc - startPoint, line_direction), 0f, line_length);
        return startPoint + line_direction * project_length;
    }
}
