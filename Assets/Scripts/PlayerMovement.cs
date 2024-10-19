using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField]
    private InputActionAsset playerControlActionAsset;

    [SerializeField]
    private string actionMapName = "PlayerController";

    [SerializeField]
    private string moveString = "Movement";
    [SerializeField]
    private string kickString = "Kick";

    [SerializeField]
    Transform visualModel;
    [SerializeField, Range(0, 1)]
    float slerpIntensity = 0.3f;
    [SerializeField, Range(15, 75)]
    float maxShoveAngle = 15.0f;

    Vector2 moveVal;
    Vector3 currentMovement;
    float distance;
    Rigidbody kinematicRigidBody;
    CapsuleCollider capCollider;

    InputAction moveAction;
    InputAction kickAction;

    [SerializeField]
    float speed = 5.0f;

    [SerializeField]
    LayerMask wallMask;

    public Vector2 MoveInput { get; private set; }
    public bool KickInput { get; private set; }

    public static PlayerMovement Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        moveAction = playerControlActionAsset.FindActionMap(actionMapName).FindAction(moveString);
        kickAction = playerControlActionAsset.FindActionMap(actionMapName).FindAction(kickString);
        RegisterInputActions();

        if (kinematicRigidBody == null)
        {
            kinematicRigidBody = GetComponent<Rigidbody>();
        }
        if (capCollider == null)
        {
            capCollider = GetComponent<CapsuleCollider>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void RegisterInputActions()
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

    // Update is called once per frame
    void FixedUpdate()
    {
        moveVal = moveAction.ReadValue<Vector2>();
        Vector3 worldDirection = transform.TransformDirection(moveVal.x, 0, moveVal.y);
        worldDirection.Normalize();

        Vector3 centerPoint = transform.rotation * capCollider.center + transform.position;
        Vector3 bottom = centerPoint + transform.rotation * Vector3.down * (capCollider.height / 2 - capCollider.radius);
        Vector3 top = centerPoint + transform.rotation * Vector3.up * (capCollider.height / 2 - capCollider.radius);

        // capture what could be hit
        RaycastHit hit;

        currentMovement.x = worldDirection.x * speed;
        currentMovement.z = worldDirection.z * speed;

        // cast a capsule forward from the character and see if it would collide with a wall if it moved any further
        if (!Physics.CapsuleCast(top, bottom, capCollider.radius, worldDirection, out hit, (capCollider.radius / 2) + (speed * Time.deltaTime), wallMask))
        {
            kinematicRigidBody.MovePosition(transform.position + (currentMovement * Time.deltaTime));
        }
        else
        {
            // attempt at smoothing movement against walls and preventing stickage
            int bounces = 0;
            float distToMove = currentMovement.magnitude;
            while (bounces < 4 && distToMove > Mathf.Epsilon)
            {
                // move against normal of walls slightly to help prevent getting caught on them
                float fract = hit.distance / distToMove;
                Vector3 posToMove = transform.position + (hit.normal * fract);

                distToMove *= (1 - fract);

                float angleBetween = Vector3.Angle(hit.normal, worldDirection);
                angleBetween = Mathf.Min(maxShoveAngle, Mathf.Abs(angleBetween));
                float normalizedAngle = angleBetween / maxShoveAngle;

                distToMove *= Mathf.Pow(1 - normalizedAngle, 0.5f) * 0.9f + 0.1f;

                Vector3 proj = Vector3.ProjectOnPlane(posToMove, hit.normal).normalized * distToMove;

                kinematicRigidBody.MovePosition(transform.position + (100 * proj * Time.deltaTime));

                bounces++;
            }
        }

        // rotate toward movement direction
        if (moveVal.x != 0 || moveVal.y != 0)
        {
            visualModel.rotation = Quaternion.Slerp(visualModel.rotation, Quaternion.LookRotation(worldDirection), slerpIntensity);
        }

        if (kickAction.IsPressed())
        {
            // TODO: do a kick
        }
    }
}
