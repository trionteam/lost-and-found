using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    public GlobalItemQueue globalItemQueue;

    private LostItemType _acceptedItemType;

    public SpriteRenderer _spriteRenderer;

    public GameObject flashObject;
    public float flashDurationSeconds = 0.2f;
    
    public LostItemType AcceptedItemType
    {
        get => _acceptedItemType;
        set
        {
            _acceptedItemType = value;
            UpdateSprite();
        }
    }

    private void Awake()
    {
        Debug.Assert(globalItemQueue != null);
        Debug.Assert(flashObject != null);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        LostItem item = collision.gameObject.GetComponent<LostItem>();
        if (item != null && item.itemType == _acceptedItemType)
        {
            item.Collect();
            PickNextItem();
            StartCoroutine(FlashOnCollect());
        }
    }

    private IEnumerator FlashOnCollect()
    {
        flashObject.SetActive(true);
        yield return new WaitForSeconds(flashDurationSeconds);
        flashObject.SetActive(false);
    }

    private void Start()
    {
        PickNextItem();
    }

    void PickNextItem()
    {
        AcceptedItemType = globalItemQueue.NextSearchedItem();
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
