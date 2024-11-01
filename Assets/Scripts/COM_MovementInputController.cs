using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

public class COM_MovementInputController : MovementInputController
{
    List<GameObject> targetMarbles;

    Vector2 direction;

    Vector3 target;

    bool startNewScan = true;

    bool closestSet = false;

    const float CHECK_INTERVAL = 0.1f;

    const float DISTANCE_FROM_TARGET_BUFFER = 2.0f;

    Gamepad COM_gamePad;

    private void Awake()
    {
        ActionMapName = "COM_Controller";
        moveAction = actionAsset.FindActionMap(ActionMapName).FindAction(MoveString);
        kickAction = actionAsset.FindActionMap(ActionMapName).FindAction(KickString);
        RegisterInputActions();
    }

    void Start()
    {
        COM_gamePad = InputSystem.AddDevice<Gamepad>();
        
        targetMarbles = MarbleManager.Instance.SceneMarbles;
    }

    private void FixedUpdate()
    {
        if (moveAction.enabled)
        {
            ScanForTarget();
            SetMoveValue();
        }
        else
        {
            moveVal = Vector2.zero;
        }
    }

    void SetMoveValue()
    {
        float targetDistance = Vector3.Distance(target, transform.position);

        Vector2 fakeInputValue;

        if (targetDistance > DISTANCE_FROM_TARGET_BUFFER)
        {
            // go after closest marbles
            Vector3 v3Direction = target - transform.position;
            //moveVal
            fakeInputValue = new Vector2(v3Direction.x, v3Direction.z).normalized;
            //Debug.Log("COM_ moveVal x: " + moveVal.x + ", COM_ moveVal y: " + moveVal.y);
        }
        else
        {
            //moveVal
            fakeInputValue= Vector2.zero;
        }

        moveVal = fakeInputValue;
    }

    void ScanForTarget()
    {
        if (startNewScan)
        {
            startNewScan = false;
            target = FindTarget();
            StartCoroutine(WaitForSearch());
            Debug.DrawLine(transform.position, target, Color.black);
            Debug.Log("New Scan Started...");
        }
        Debug.DrawLine(transform.position, target, Color.cyan);
    }

    Vector3 FindTarget()
    {
        float minDist = 10000;

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
            //float dist = (targetMarbles[i].transform.position - transform.position).sqrMagnitude;

            // only go after marbles that are active and aren't currently in the air
            // TODO : FIX THIS and also PREVENT GOING AFTER INACTIVE MARBLES
            if (dist < minDist && targetMarbles[i].activeInHierarchy 
                && Mathf.Abs(targetMarbles[i].transform.position.y - transform.position.y) < DISTANCE_FROM_TARGET_BUFFER)
            {
                closestPosition = targetMarbles[i].transform.position;
                minDist = dist;
                closestSet = true;
                Debug.Log("Target Set - targetMarbles[" + i + "] ");
            }
        }
        if (!closestSet)
        {
            closestPosition = Vector3.zero;
            Debug.Log("00000 Setting target to mid-arena");
        }

        return closestPosition;
    }

    IEnumerator WaitForSearch()
    {
        yield return new WaitForSeconds(CHECK_INTERVAL);
        startNewScan = true;
    }
}
