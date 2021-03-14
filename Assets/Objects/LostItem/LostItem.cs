using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostItem : MonoBehaviour
{
    [SerializeField]
    private LostItemType _itemType = default;

    public LostItemType ItemType { get => _itemType; }

    [SerializeField]
    private GameObject _explosion = default;

    [SerializeField]
    private GameObject _dangerZoneIndicator = default;

    private Rigidbody2D _rigidBody;
    private GlobalItemQueue _globalItemQueue;

    private float _originalZ;

    private void Awake()
    {
        Debug.Assert(_itemType != null);
        Debug.Assert(_explosion != null);

        _rigidBody = GetComponent<Rigidbody2D>();
        Debug.Assert(_rigidBody != null);

        var renderer = GetComponentInChildren<SpriteRenderer>();
        renderer.sprite = ItemType.sprite;

        _globalItemQueue = GlobalItemQueue.Instance;
        Debug.Assert(_globalItemQueue != null);
    }

    private void Update()
    {
        if (_dangerZoneIndicator != null)
        {
            _dangerZoneIndicator.SetActive(
                _globalItemQueue.ItemDestination(ItemType) != null &&
                _globalItemQueue.IsInDangerZone(this));
        }
    }

    public void Collect()
    {
        Destroy(gameObject);
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
