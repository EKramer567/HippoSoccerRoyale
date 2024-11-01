using UnityEngine;
using UnityEngine.InputSystem;

public class MovementInputController : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField]
    protected private InputActionAsset actionAsset;

    protected Vector2 moveVal;
    protected InputAction moveAction;
    protected InputAction kickAction;

    private string actionMapName;
    private string moveString = "Movement";
    private string kickString = "Kick";

    protected virtual string ActionMapName
    {
        get { return actionMapName; }
        set { actionMapName = value; }
    }

    protected virtual string MoveString
    {
        get { return moveString; }
        set { moveString = value; }
    }

    protected virtual string KickString
    {
        get { return kickString; }
        set { kickString = value; }
    }


    public Vector2 MoveInput { get; private set; }
    public bool KickInput { get; private set; }

    public Vector2 MovementValue { get { return moveVal; } }

    /*private void Awake()
    {
        moveAction = actionAsset.FindActionMap(actionMapName).FindAction(moveString);
        kickAction = actionAsset.FindActionMap(actionMapName).FindAction(kickString);
        RegisterInputActions();
    }*/

    public void EnableActions()
    {
        if (!moveAction.enabled)
        {
            moveAction.Enable();
            kickAction.Enable();
        }
    }

    public void DisableActions()
    {
        if (moveAction.enabled)
        {
            moveAction.Disable();
            kickAction.Disable();
        }
    }

    public bool GetKickActionPressed()
    {
        return kickAction.IsPressed();
    }

    protected void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        kickAction.performed += context => KickInput = true;
        kickAction.performed += context => KickInput = false;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        kickAction.Enable();
    }
    private void OnDisable()
    {
        moveAction.Disable();
        kickAction.Disable();
    }
}
