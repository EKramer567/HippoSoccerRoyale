using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    public enum CharacterStates
    {
        IDLE,
        WALKING,
        RUNNING,
        KICKING
    }

    private CharacterStates currentState;

    public static PlayerStateController Instance { get; private set; }

    public CharacterStates CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }
}
