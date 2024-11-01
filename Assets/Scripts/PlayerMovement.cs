using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

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
        moveVal = moveAction.ReadValue<Vector2>();
        //Debug.Log("moveVal x: " + moveVal.x + ", moveVal y: " + moveVal.y);
    }
}
