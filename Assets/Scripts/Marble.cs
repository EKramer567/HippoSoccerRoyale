using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        rb.velocity = Vector3.zero;
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

    void OnCollisionEnter(Collision collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) == "PointZones")
        {
            gameObject.SetActive(false);
        }
    }
}
