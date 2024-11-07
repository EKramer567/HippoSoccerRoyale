using UnityEngine;


/// <summary>
/// Class for holding basic location and movement vectors common to both normal and COM players
/// To be used as a parent for controller classes
/// </summary>
public class InputControllerData : MonoBehaviour
{
    bool movementEnabled = true;
    protected Vector2 moveVal;
    protected bool kicking = false;
    private Vector3 zoneLocation;
    private Vector3 startLocation;

    protected bool MovementEnabled { get { return movementEnabled; } }

    /// <summary>
    /// Normalized input vector indicating direction of movement
    /// </summary>
    public Vector2 MovementValue
    {
        get { return moveVal; }
        set { moveVal = value; }
    }

    /// <summary>
    /// Whether or not this entity is kicking
    /// </summary>
    public bool Kicking
    {
        get { return kicking; }
        set { kicking = value; }
    }

    /// <summary>
    /// Location of this player's score zone
    /// </summary>
    public Vector3 ZoneLocation
    {
        get
        {
            return zoneLocation;
        }
        set
        {
            zoneLocation = value;
        }
    }

    /// <summary>
    /// Location of this player's spawn point
    /// </summary>
    public Vector3 StartLocation
    {
        get
        {
            return startLocation;
        }
        set
        {
            startLocation = value;
        }
    }

    /// <summary>
    /// Whether the kick action is to be executed
    /// To be overridden in child controller classes
    /// </summary>
    /// <returns>Nothing, this should be overridden</returns>
    public virtual bool GetKickActionPressed()
    {
        // Empty
        return false;
    }

    /// <summary>
    /// Enable whether the player can act
    /// Player controller overrides this to use keyboard/controller inputs
    /// </summary>
    public virtual void EnableActions()
    {
        if (!movementEnabled) { movementEnabled = true; }
    }

    /// <summary>
    /// Disable whether the player can act
    /// Player controller overrides this to use keyboard/controller inputs
    /// </summary>
    public virtual void DisableActions()
    {
        if (movementEnabled) { movementEnabled = false; }
    }
}
