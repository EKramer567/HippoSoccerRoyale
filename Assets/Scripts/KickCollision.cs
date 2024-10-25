using UnityEngine;

public class KickCollision : MonoBehaviour
{
    [SerializeField]
    float kickForceVal = 2.0f;
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "RollyBalls")
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            Vector3 dir = transform.forward;/*(other.transform.position - transform.position).normalized*/;
            rb.AddForce(dir * kickForceVal);
        }
    }
}
