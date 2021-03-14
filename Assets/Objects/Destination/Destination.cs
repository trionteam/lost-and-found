using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    [SerializeField]
    private GlobalItemQueue _globalItemQueue = default;

    private LostItemType _acceptedItemType;

    [SerializeField]
    private SpriteRenderer _spriteRenderer = default;
    [SerializeField]
    private SpriteRenderer _paperRenderer = default;

    [SerializeField]
    private DustCloud _cloudPrefab = default;

    [SerializeField]
    private float _spriteSize = 0.7f;

    [SerializeField]
    private float _flashDurationSeconds = 0.2f;
    
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
        Debug.Assert(_globalItemQueue != null);
        Debug.Assert(_spriteRenderer != null);
        Debug.Assert(_paperRenderer != null);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        LostItem item = collision.gameObject.GetComponent<LostItem>();
        if (item != null && item.ItemType == _acceptedItemType)
        {
            _globalItemQueue.CollectLostItem(item);
            PickNextItem();
            StartCoroutine(FlashOnCollect());
        }
    }

    private IEnumerator FlashOnShred()
    {
        var cloud = Instantiate(_cloudPrefab, transform);
        cloud.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.2f);
        _paperRenderer.enabled = false;

        yield return new WaitForSeconds(0.35f);
        _paperRenderer.enabled = true;
        _spriteRenderer.enabled = true;
    }

    private IEnumerator FlashOnCollect()
    {
        _paperRenderer.enabled = false;
        _spriteRenderer.enabled = false;
        yield return new WaitForSeconds(_flashDurationSeconds);
        _paperRenderer.enabled = true;
        _spriteRenderer.enabled = true;
    }

    private void Start()
    {
        PickNextItem();
    }

    void PickNextItem()
    {
        AcceptedItemType = _globalItemQueue.NextSearchedItem();
    }

    public void ItemShredded()
    {
        StartCoroutine(FlashOnShred());
        PickNextItem();
    }

    void UpdateSprite()
    {
        var sprite = _acceptedItemType.sprite;
        var size = sprite.bounds.size;
        var maxSize = Mathf.Max(size.x, size.y);
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.transform.localScale = new Vector3(_spriteSize / maxSize, _spriteSize / maxSize);
    }
}
