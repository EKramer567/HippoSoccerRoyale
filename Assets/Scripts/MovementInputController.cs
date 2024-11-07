using UnityEngine;
using UnityEngine.InputSystem;

public class MovementInputController : InputControllerData
{
    [Header("Input Action Asset")]
    [SerializeField]
    protected private InputActionAsset actionAsset;

    protected InputAction moveAction;
    protected InputAction kickAction;

    private string actionMapName;
    private string moveString = "Movement";
    private string kickString = "Kick";

    /// <summary>
    /// This player's action map name
    /// </summary>
    protected virtual string ActionMapName
    {
        get { return actionMapName; }
        set { actionMapName = value; }
    }

    /// <summary>
    /// String to represent the action for movement
    /// </summary>
    protected virtual string MoveString
    {
        get { return moveString; }
        set { moveString = value; }
    }

    /// <summary>
    /// String to represent the action for kicking
    /// </summary>
    protected virtual string KickString
    {
        get { return kickString; }
        set { kickString = value; }
    }

    /// <summary>
    /// Property that captures the player's movement input (WASD, left control stick, etc)
    /// </summary>
    public Vector2 MoveInput { get; private set; }

    /// <summary>
    /// Property that captures the player's input for the kick action (space, south button, etc)
    /// </summary>
    public bool KickInput { get; private set; }

    /// <summary>
    /// Enable input actions
    /// </summary>
    public override void EnableActions()
    {
        if (!moveAction.enabled)
        {
            moveAction.Enable();
            kickAction.Enable();
        }
    }

    /// <summary>
    /// Disable input actions
    /// </summary>
    public override void DisableActions()
    {
        if (moveAction.enabled)
        {
            moveAction.Disable();
            kickAction.Disable();
        }
    }

    /// <summary>
    /// Get whether the player is kicking
    /// </summary>
    /// <returns>Whether the kick action should be performed</returns>
    public override bool GetKickActionPressed()
    {
        return kickAction.IsPressed();
    }

    /// <summary>
    /// Register the actions to read correctly
    /// </summary>
    protected void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        kickAction.performed += context => KickInput = true;
        kickAction.performed += context => KickInput = false;
    }

    /// <summary>
    /// Enable actions in the event of this script component being enabled
    /// </summary>
    private void OnEnable()
    {
        moveAction.Enable();
        kickAction.Enable();
    }

    /// <summary>
    /// Disable actions in the event of this script component being disabled
    /// </summary>
    private void OnDisable()
    {
        moveAction.Disable();
        kickAction.Disable();
    }
}
