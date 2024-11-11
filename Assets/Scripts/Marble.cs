using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// A single marble and what happens when it respawns or collides with something
/// </summary>
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

    private void OnTriggerEnter(Collider other)
    {
        // Play splashing particles when entering water
        if (LayerMask.LayerToName(other.gameObject.layer) == "Water")
        {
            Vector3 pos = transform.position;
            MarbleManager.Instance.PlaySplash(other, pos);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Disable the marble when leaving the box collider for the water
        if (LayerMask.LayerToName(other.gameObject.layer) == "Water")
        {
            this.gameObject.SetActive(false);
        }
    }
}
