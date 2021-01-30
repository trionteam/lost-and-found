using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var lostItem = collision.gameObject.GetComponent<LostItem>();
        if (lostItem != null)
        {
            lostItem.Shred();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.3f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
