using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour
{
    [SerializeField]
    private GlobalItemQueue _globalItemQueue;

    private void Awake()
    {
        Debug.Assert(_globalItemQueue != null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var lostItem = collision.gameObject.GetComponent<LostItem>();
        if (lostItem != null)
        {
            _globalItemQueue.ShredLostItem(lostItem);
        }
    }

    private void OnDrawGizmos()
    {
        var collider = GetComponent<BoxCollider2D>();

        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.3f);
        var position = transform.position + new Vector3(collider.offset.x, collider.offset.y);
        Gizmos.DrawCube(position, collider.size);
    }
}
