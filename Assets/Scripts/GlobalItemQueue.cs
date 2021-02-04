using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalItemQueue : MonoBehaviour
{
    public LostItemCollection lostItems;
    public LostItemCollection trashItems;

    private List<LostItemType> _allItemTypes;

    private List<LostItemType> _lostItemQueue;
    private List<bool> _lostItemInQueueSearched;

    public Text healthDisplay;
    public TextMeshPro scoreDisplay;

    public int lostItemQueueSize = 16;
    public float initialHealth = 100.0f;

    private int _score = 0;
    public int Score 
    {
        get => _score;
        private set
        {
            _score = value;
            scoreDisplay.text = string.Format("{0}", value);
        }
    }

    private float _health = 100.0f;
    public float Health
    {
        get => _health;
        private set
        {
            _health = value;
            healthDisplay.text = string.Format("{0}", value);
        }
    }

    private void Awake()
    {
        Debug.Assert(lostItems != null);
        Debug.Assert(trashItems != null);
        Debug.Assert(scoreDisplay != null);

        _allItemTypes = new List<LostItemType>();
        _allItemTypes.AddRange(lostItems.lostItems);
        _allItemTypes.AddRange(trashItems.lostItems);

        _lostItemQueue = new List<LostItemType>(lostItemQueueSize);
        _lostItemInQueueSearched = new List<bool>(lostItemQueueSize);
    }

    private void Start()
    {
        Score = 0;
    }

    public LostItemType NextLostItem()
    {
        RefillQueue();
        var itemType = _lostItemQueue[0];
        _lostItemQueue.RemoveAt(0);
        _lostItemInQueueSearched.RemoveAt(0);
        return itemType;
    }

    public LostItemType NextSearchedItem()
    {
        RefillQueue();
        for(; ;)
        {
            int itemIndex = Random.Range(0, _lostItemQueue.Count);
            if (_lostItemInQueueSearched[itemIndex]) continue;
            var item = _lostItemQueue[itemIndex];
            if (System.Array.IndexOf(trashItems.lostItems, item) >= 0) continue;
            _lostItemInQueueSearched[itemIndex] = true;
            return item;
        }
    }

    private void RefillQueue()
    {
        for (int i = _lostItemQueue.Count; i < lostItemQueueSize; ++i)
        {
            int nextItemIndex = Random.Range(0, _allItemTypes.Count);
            _lostItemQueue.Add(_allItemTypes[nextItemIndex]);
            _lostItemInQueueSearched.Add(false);
        }
    }

    public void ShredLostItem(LostItem item)
    {
        Health = Mathf.Max(Health - item.itemType.healthDecrease, 0.0f);
        item.Shred();
    }

    public void CollectLostItem(LostItem item)
    {
        Score += item.itemType.scoreIncrease;
        item.Collect();
    }
}
