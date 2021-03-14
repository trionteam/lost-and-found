using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalItemQueue : MonoBehaviour
{
    public delegate void ItemEventDelegate(LostItemType itemType);

    public LostItemCollection lostItems;
    public LostItemCollection trashItems;

    private List<LostItemType> _allItemTypes;

    private List<LostItemType> _lostItemQueue;
    private List<bool> _lostItemInQueueSearched;

    private List<LostItemType> _itemsOnScreen = new List<LostItemType>();

    public int lostItemQueueSize = 16;

    public Collider2D dangerZone;

    private Destination[] _destinations;

    private DifficultyController _difficulty;

    public event ItemEventDelegate OnItemFound;
    public event ItemEventDelegate OnLostItemShredded;

    private void Awake()
    {
        Debug.Assert(lostItems != null);
        Debug.Assert(trashItems != null);

        _allItemTypes = new List<LostItemType>();
        _allItemTypes.AddRange(lostItems.lostItems);
        _allItemTypes.AddRange(trashItems.lostItems);

        _lostItemQueue = new List<LostItemType>(lostItemQueueSize);
        _lostItemInQueueSearched = new List<bool>(lostItemQueueSize);

        _destinations = FindObjectsOfType<Destination>();

        _difficulty = DifficultyController.Instance;
        Debug.Assert(_difficulty != null);
    }

    private void Start()
    {
        _itemsOnScreen.Clear();
    }

    public LostItemType NextLostItem()
    {
        RefillQueue();
        var itemType = _lostItemQueue[0];
        _lostItemQueue.RemoveAt(0);
        _lostItemInQueueSearched.RemoveAt(0);

        _itemsOnScreen.Add(itemType);

        return itemType;
    }

    public LostItemType NextSearchedItem()
    {
        RefillQueue();
        var inDangerZone = ItemsInDangerZone();

        var isOnScreen = Random.Range(0.0f, 1.0f);
        if (isOnScreen < _difficulty.screenProbability)
        {
            var itemsToPickFromQuery = _itemsOnScreen.Where(
                item => lostItems.lostItems.Contains(item) &&
                        !inDangerZone.Contains(item));
            if (_difficulty.uniqueItems)
            {
                itemsToPickFromQuery = itemsToPickFromQuery.Where(
                    item => ItemDestination(item) == null);
            }
            var itemsToPickFrom = new List<LostItemType>(itemsToPickFromQuery);


            foreach (var destination in _destinations)
            {
                itemsToPickFrom.Remove(destination.AcceptedItemType);
            }
            if (itemsToPickFrom.Count > 0)
            {
                var index = Random.Range(0, itemsToPickFrom.Count);
                return itemsToPickFrom[index];
            }
        }
        for (int iteration = 0; iteration < lostItemQueueSize; iteration++)
        {
            int itemIndex = Random.Range(0, _lostItemQueue.Count);
            if (_lostItemInQueueSearched[itemIndex]) continue;
            var item = _lostItemQueue[itemIndex];
            if (trashItems.lostItems.IndexOf(item) >= 0) continue;
            if (inDangerZone.Contains(item)) continue;
            if (_difficulty.uniqueItems && ItemDestination(item) != null) continue;
            _lostItemInQueueSearched[itemIndex] = true;
            return item;
        }
        for (int iteration = 0; iteration < lostItems.lostItems.Count; ++iteration)
        {
            var itemIndex = Random.Range(0, lostItems.lostItems.Count);
            var item = lostItems.lostItems[itemIndex];
            if (inDangerZone.Contains(item)) continue;
            if (_difficulty.uniqueItems && ItemDestination(item) != null) continue;
            return item;
        }
        return lostItems.lostItems[Random.Range(0, lostItems.lostItems.Count)];
    }

    private void RefillQueue()
    {
        IEnumerable<LostItemType> itemsToPickFromQuery = lostItems.lostItems;
        if (_difficulty.uniqueItems)
        {
            itemsToPickFromQuery = itemsToPickFromQuery.Where(
                item => trashItems.lostItems.Contains(item) ||
                        !(_itemsOnScreen.Contains(item) ||
                          _lostItemQueue.Contains(item))).ToList();
        }
        var itemsToPickFrom = itemsToPickFromQuery.ToList();
        for (int i = _lostItemQueue.Count; i < lostItemQueueSize; ++i)
        {
            var isTrash = Random.Range(0.0f, 1.0f) < _difficulty.trashProbability;
            LostItemType itemType = null;
            if (!isTrash && itemsToPickFrom.Count > 0)
            {
                int nextItemIndex = Random.Range(0, itemsToPickFrom.Count);
                itemType = itemsToPickFrom[nextItemIndex];
                itemsToPickFrom.RemoveAt(nextItemIndex);
            }
            if (itemType == null)
            {
                var itemIndex = Random.Range(0, trashItems.lostItems.Count);
                itemType = trashItems.lostItems[itemIndex];
            }
            _lostItemQueue.Add(itemType);
            _lostItemInQueueSearched.Add(false);
        }
    }

    public Destination ItemDestination(LostItem item)
    {
        return ItemDestination(item.itemType);
    }
    public Destination ItemDestination(LostItemType itemType)
    {
        foreach (var destination in _destinations)
        {
            if (destination.AcceptedItemType == itemType) return destination;
        }
        return null;
    }

    public void ShredLostItem(LostItem item)
    {
        var itemDestination = ItemDestination(item);
        if (itemDestination != null)
        {
            itemDestination.ItemShredded();

            OnLostItemShredded?.Invoke(item.itemType);
        }
        item.Shred();
        _itemsOnScreen.Remove(item.itemType);
    }

    private List<LostItemType> ItemsInDangerZone()
    {
        var list = new List<LostItemType>();

        var filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask(LayerMask.GetMask("BeltItems"));
        var contacts = new Collider2D[64];
        int numColliders = dangerZone.GetContacts(filter, contacts);
        for (int i = 0; i < numColliders; ++i)
        {
            var lostItem = contacts[i].GetComponent<LostItem>();
            if (lostItem == null) continue;
            list.Add(lostItem.itemType);
        }
        return list;
    }

    public void CollectLostItem(LostItem item)
    {
        item.Collect();
        _itemsOnScreen.Remove(item.itemType);

        OnItemFound?.Invoke(item.itemType);
    }
}
