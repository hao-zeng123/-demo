using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
		PalHealth health = collision.gameObject.GetComponent<PalHealth>();

        if (health)
        {
            Vector2 hitDir = collision.transform.position - transform.parent.position;
            hitDir = hitDir.normalized;

            health.BeHit(new float[] { 1, hitDir.x, hitDir.y });
        }
    }
}
