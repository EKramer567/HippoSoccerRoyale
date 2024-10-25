using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class Marble : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= -10)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "Walls")
        {
            Vector3 dir = Vector3.zero + collision.GetContact(0).normal;

            rb.AddForce(dir * 10, ForceMode.Impulse);
            //rb.linearVelocity = dir * rb.linearVelocity.magnitude;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Water")
        {
            Vector3 pos = transform.position;
            MarbleManager.Instance.PlaySplash(other, pos);
        }
    }
}
