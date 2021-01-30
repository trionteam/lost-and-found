using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostItem : MonoBehaviour
{
    public float weight;

    public LostItemType itemType;

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        var renderer = GetComponentInChildren<SpriteRenderer>();
        renderer.sprite = itemType.sprite;
    }

    public void Collect()
    {
        Destroy(gameObject);
        // TODO: Increase score.
    }

    public void Drop()
    {
        gameObject.layer = Layers.BeltItems;
        _rigidBody.freezeRotation = false;
    }

    public void Pickup()
    {
        gameObject.layer = Layers.HeldItems;
        _rigidBody.freezeRotation = true;
    }

    public void Shred()
    {
        Destroy(gameObject);
    }
}
