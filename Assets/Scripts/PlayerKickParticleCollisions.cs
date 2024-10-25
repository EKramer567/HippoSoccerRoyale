using UnityEngine;

public class PlayerKickParticleCollisions : MonoBehaviour
{
    [SerializeField]
    float kickForceVal = 2.0f;

    private void OnParticleCollision(GameObject other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "RollyBalls")
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            Vector3 dir = (other.transform.position - transform.position).normalized;
            rb.AddForce(dir * kickForceVal);
        }
    }
}
