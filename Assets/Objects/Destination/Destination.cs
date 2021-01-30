using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    public LostItemType[] acceptedItemTypes;

    private LostItemType _acceptedItemType;

    public SpriteRenderer _spriteRenderer;
    
    public LostItemType AcceptedItemType
    {
        get => _acceptedItemType;
        set
        {
            _acceptedItemType = value;
            UpdateSprite();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        LostItem item = collision.gameObject.GetComponent<LostItem>();
        if (item != null && item.itemType == _acceptedItemType)
        {
            item.Collect();
            PickNextItem();
        }
    }

    private void Start()
    {
        PickNextItem();
    }

    void PickNextItem()
    {
        int nextItemIndex = Random.Range(0, acceptedItemTypes.Length);
        AcceptedItemType = acceptedItemTypes[nextItemIndex];
    }

    void UpdateSprite()
    {
        var sprite = _acceptedItemType.sprite;
        var size = sprite.bounds.size;
        var maxSize = Mathf.Max(size.x, size.y);
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.transform.localScale = new Vector3(1.0f / maxSize, 1.0f / maxSize);
    }
}
