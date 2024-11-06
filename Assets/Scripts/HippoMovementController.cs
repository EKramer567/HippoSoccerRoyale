using UnityEngine;
using ZonesEnums;

public class HippoMovementController : MonoBehaviour
{
    [SerializeField]
    Transform visualModel;

    [SerializeField, Range(0, 1)]
    float slerpIntensity = 0.3f;

    [SerializeField, Range(15, 75)]
    float maxShoveAngle = 15.0f;

    [SerializeField]
    CapsuleCollider capCollider;

    Vector3 currentMovement;
    float distance;
    Rigidbody kinematicRigidBody;

    [SerializeField]
    float speed = 5.0f;

    [SerializeField]
    LayerMask wallMask;

    [SerializeField]
    ParticleSystem kickParticles;

    [SerializeField]
    SphereCollider kickCollider;

    PlayerStateController stateController;

    MovementInputController movementInputCtrl;

    PlayerZoneEnum thisPlayerZone;

    public PlayerZoneEnum ThisPlayerZone {  get { return thisPlayerZone; } }

    // Start is called before the first frame update
    void Start()
    {
        if (kinematicRigidBody == null)
        {
            kinematicRigidBody = GetComponent<Rigidbody>();
        }

        if (stateController == null)
        {
            stateController = GetComponent<PlayerStateController>();
        }

        if (movementInputCtrl == null)
        {
            movementInputCtrl = GetComponent<MovementInputController>();
        }

        stateController.CurrentState = PlayerStateController.CharacterStates.IDLE;

        // look at the center of the arena
        visualModel.rotation = Quaternion.LookRotation(ArenaLocations.Instance.CenterLocation - visualModel.position, visualModel.up);
    }

    private void FixedUpdate()
    {
        HippoMovement();
    }

    /// <summary>
    /// Movement function to be called in FixedUpdate to accomodate physics
    /// </summary>
    protected void HippoMovement()
    {
        Vector2 moveVal = movementInputCtrl.MovementValue;
        if (GameStateController.Instance.CurrentGameState != GameStateController.GameStates.PLAYING)
        {
            movementInputCtrl.DisableActions();
        }
        else
        {
            movementInputCtrl.EnableActions();
        }

        Vector3 worldDirection = transform.TransformDirection(moveVal.x, 0, moveVal.y);
        worldDirection.Normalize();

        Vector3 centerPoint = transform.rotation * capCollider.center + transform.position;
        Vector3 bottom = centerPoint + transform.rotation * Vector3.down * (capCollider.height / 2 - capCollider.radius);
        Vector3 top = centerPoint + transform.rotation * Vector3.up * (capCollider.height / 2 - capCollider.radius);

        // capture what could be hit
        RaycastHit hit;

        currentMovement.x = worldDirection.x * speed;
        currentMovement.z = worldDirection.z * speed;

        // rotate toward movement direction
        if (moveVal.x != 0 || moveVal.y != 0)
        {
            visualModel.rotation = Quaternion.Slerp(visualModel.rotation, Quaternion.LookRotation(worldDirection), slerpIntensity);
        }

        // cast a sphere forward from the character and see if it would collide with a wall if it moved any further
        if (!Physics.SphereCast(transform.position, capCollider.height, worldDirection, out hit, (capCollider.height / 2) + (speed * Time.deltaTime), wallMask))
        {
            kinematicRigidBody.MovePosition(transform.position + (currentMovement * Time.deltaTime));

            if (stateController.CurrentState != PlayerStateController.CharacterStates.RUNNING
                && currentMovement.magnitude > 0
                && stateController.CurrentState != PlayerStateController.CharacterStates.KICKING)
            {
                stateController.CurrentState = PlayerStateController.CharacterStates.RUNNING;
            }
            else if (currentMovement.magnitude <= 0
                && stateController.CurrentState != PlayerStateController.CharacterStates.KICKING)
            {
                stateController.CurrentState = PlayerStateController.CharacterStates.IDLE;
            }
        }
        else
        {
            // attempt at smoothing movement against walls and preventing stickage
            int bounces = 0;
            float distToMove = currentMovement.magnitude;
            while (bounces < 4 && distToMove > Mathf.Epsilon)
            {
                Vector3 xzHitNormal = new Vector3(hit.normal.x, 0, hit.normal.z).normalized;
                // move against normal of walls slightly to help prevent getting caught on them
                float fract = hit.distance / distToMove;
                Vector3 posToMove = transform.position + (xzHitNormal * fract);

                distToMove *= (1 - fract);

                float angleBetween = Vector3.Angle(xzHitNormal, worldDirection);
                angleBetween = Mathf.Min(maxShoveAngle, Mathf.Abs(angleBetween));
                float normalizedAngle = angleBetween / maxShoveAngle;

                distToMove *= Mathf.Pow(1 - normalizedAngle, 0.5f) * 0.9f + 0.1f;

                Vector3 proj = Vector3.ProjectOnPlane(posToMove, xzHitNormal).normalized * distToMove;

                kinematicRigidBody.MovePosition(transform.position + (100 * proj * Time.deltaTime));

                bounces++;
            }
        }

        kickCollider.enabled = kickParticles.isEmitting ? true : false;

        if (movementInputCtrl.GetKickActionPressed() && stateController.CurrentState != PlayerStateController.CharacterStates.KICKING)
        {
            stateController.CurrentState = PlayerStateController.CharacterStates.KICKING;
            kickParticles.Play();
        }
    }

    public void AssignPlayerZone(PlayerZoneEnum zone)
    {
        thisPlayerZone = zone;
    }
}
