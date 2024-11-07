using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class that a Player (not a COM) uses to initialize its action maps and update its movement value
/// May merge this with MovementInputController but for now the name PlayerMovement makes the function of this more obvious
/// </summary>
public class PlayerMovement : MovementInputController
{
    private void Awake()
    {
        ActionMapName = "PlayerController";
        moveAction = actionAsset.FindActionMap(ActionMapName).FindAction(MoveString);
        kickAction = actionAsset.FindActionMap(ActionMapName).FindAction(KickString);
        RegisterInputActions();
    }
    private void FixedUpdate()
    {
        MovementValue = moveAction.ReadValue<Vector2>();
        //Debug.Log("moveVal x: " + moveVal.x + ", moveVal y: " + moveVal.y);
    }
}
