using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostItem : MonoBehaviour
{
    public float weight;

    public LostItemType itemType;

    public GameObject _explosion;

    private Rigidbody2D _rigidBody;

    private float _originalZ;

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
        transform.position = new Vector3(transform.position.x, transform.position.y, _originalZ);
    }

    public void Pickup(Player player)
    {
        gameObject.layer = Layers.HeldItems;
        _rigidBody.freezeRotation = true;
        _originalZ = transform.position.z;
        transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z - 0.01f);
    }

    public void Shred()
    {
        Destroy(gameObject);
        Instantiate(_explosion, transform.position - 0.2f * Vector3.back, transform.rotation);
    }
}
