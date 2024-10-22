using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    [SerializeField]
    PlayerStateController stateController;

    PlayerStateController.CharacterStates currentAnimState;
    PlayerStateController.CharacterStates prevAnimState;

    [SerializeField]
    Animator animator;

    [SerializeField]
    int layerIndex = 0;

    [SerializeField]
    string idleAnimString, walkAnimString, runAnimString, kickAnimString;

    // Start is called before the first frame update
    void Start()
    {
        if (stateController != null)
        {
            GetComponent<PlayerStateController>();
        }
        currentAnimState = PlayerStateController.CharacterStates.IDLE;
        prevAnimState = PlayerStateController.CharacterStates.DISABLED;
        stateController.CurrentState = PlayerStateController.CharacterStates.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        currentAnimState = stateController.CurrentState;
        if (currentAnimState != prevAnimState)
        {
            switch (currentAnimState)
            {
                case PlayerStateController.CharacterStates.IDLE:
                    if (animator.HasState(layerIndex, Animator.StringToHash(idleAnimString)))
                    {
                        animator.Play(idleAnimString);
                    }
                    break;
                case PlayerStateController.CharacterStates.WALKING:
                    if (animator.HasState(layerIndex, Animator.StringToHash(walkAnimString)))
                    {
                        animator.Play(walkAnimString);
                    }
                    break;
                case PlayerStateController.CharacterStates.RUNNING:
                    if (animator.HasState(layerIndex, Animator.StringToHash(runAnimString)))
                    {
                        animator.Play(runAnimString);
                    }
                    break;
                case PlayerStateController.CharacterStates.KICKING:
                    if (animator.HasState(layerIndex, Animator.StringToHash(kickAnimString)))
                    {
                        animator.Play(kickAnimString);
                    }
                    break;
                default:
                    break;
            }
            prevAnimState = currentAnimState;
        }
        else if (currentAnimState == PlayerStateController.CharacterStates.KICKING)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(kickAnimString))
            {
                stateController.CurrentState = PlayerStateController.CharacterStates.RUNNING;
                currentAnimState = PlayerStateController.CharacterStates.RUNNING;
            }
        }
    }
}
