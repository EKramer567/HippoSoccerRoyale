using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    // trigger to score a point when a marble enters.
    // further checking isn't required since a score zone collider should be set to only collide with
    // objects on the layer that marbles (and only marbles) exist
    private void OnTriggerEnter(Collider other)
    {
        ScoreZonesManager.Instance.AddPoint(gameObject.tag);
    }
}
