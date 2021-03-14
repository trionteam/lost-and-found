using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalItemQueue : MonoBehaviour
{
    public delegate void ItemEventDelegate(LostItemType itemType);

    [SerializeField]
    public LostItemCollection _lostItems = default;

    [SerializeField]
    public LostItemCollection _trashItems = default;

    [SerializeField]
    private int _lostItemQueueSize = 4;

    [SerializeField]
    private Collider2D _dangerZone = default;

    private List<LostItemType> _allItemTypes;

    private List<LostItemType> _lostItemQueue;
    private List<bool> _lostItemInQueueSearched;

    private List<LostItemType> _itemsOnScreen = new List<LostItemType>();

    private Destination[] _destinations;

    private DifficultyController _difficulty;

    public event ItemEventDelegate OnItemFound;
    public event ItemEventDelegate OnLostItemShredded;

    private void Awake()
    {
        Debug.Assert(_lostItems != null);
        Debug.Assert(_trashItems != null);

        _allItemTypes = new List<LostItemType>();
        _allItemTypes.AddRange(_lostItems.lostItems);
        _allItemTypes.AddRange(_trashItems.lostItems);

        _lostItemQueue = new List<LostItemType>(_lostItemQueueSize);
        _lostItemInQueueSearched = new List<bool>(_lostItemQueueSize);

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
        if (isOnScreen < _difficulty.ScreenProbability)
        {
            var itemsToPickFromQuery = _itemsOnScreen.Where(
                item => _lostItems.lostItems.Contains(item) &&
                        !inDangerZone.Contains(item));
            if (_difficulty.UniqueItems)
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
        for (int iteration = 0; iteration < _lostItemQueueSize; iteration++)
        {
            int itemIndex = Random.Range(0, _lostItemQueue.Count);
            if (_lostItemInQueueSearched[itemIndex]) continue;
            var item = _lostItemQueue[itemIndex];
            if (_trashItems.lostItems.IndexOf(item) >= 0) continue;
            if (inDangerZone.Contains(item)) continue;
            if (_difficulty.UniqueSearchedItems && ItemDestination(item) != null) continue;
            _lostItemInQueueSearched[itemIndex] = true;
            return item;
        }
        for (int iteration = 0; iteration < _lostItems.lostItems.Count; ++iteration)
        {
            var itemIndex = Random.Range(0, _lostItems.lostItems.Count);
            var item = _lostItems.lostItems[itemIndex];
            if (inDangerZone.Contains(item)) continue;
            if (_difficulty.UniqueItems && ItemDestination(item) != null) continue;
            return item;
        }
        return _lostItems.lostItems[Random.Range(0, _lostItems.lostItems.Count)];
    }

    private void RefillQueue()
    {
        IEnumerable<LostItemType> itemsToPickFromQuery = _lostItems.lostItems;
        if (_difficulty.UniqueItems)
        {
            itemsToPickFromQuery = itemsToPickFromQuery.Where(
                item => _trashItems.lostItems.Contains(item) ||
                        !(_itemsOnScreen.Contains(item) ||
                          _lostItemQueue.Contains(item))).ToList();
        }
        var itemsToPickFrom = itemsToPickFromQuery.ToList();
        for (int i = _lostItemQueue.Count; i < _lostItemQueueSize; ++i)
        {
            var isTrash = Random.Range(0.0f, 1.0f) < _difficulty.TrashProbability;
            LostItemType itemType = null;
            if (!isTrash && itemsToPickFrom.Count > 0)
            {
                int nextItemIndex = Random.Range(0, itemsToPickFrom.Count);
                itemType = itemsToPickFrom[nextItemIndex];
                itemsToPickFrom.RemoveAt(nextItemIndex);
            }
            if (itemType == null)
            {
                var itemIndex = Random.Range(0, _trashItems.lostItems.Count);
                itemType = _trashItems.lostItems[itemIndex];
            }
            _lostItemQueue.Add(itemType);
            _lostItemInQueueSearched.Add(false);
        }
    }

    public Destination ItemDestination(LostItem item)
    {
        return ItemDestination(item.ItemType);
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

            OnLostItemShredded?.Invoke(item.ItemType);
        }
        item.Shred();
        _itemsOnScreen.Remove(item.ItemType);
    }

    private List<LostItemType> ItemsInDangerZone()
    {
        var list = new List<LostItemType>();

        var filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask(LayerMask.GetMask("BeltItems"));
        var contacts = new Collider2D[64];
        int numColliders = _dangerZone.GetContacts(filter, contacts);
        for (int i = 0; i < numColliders; ++i)
        {
            var lostItem = contacts[i].GetComponent<LostItem>();
            if (lostItem == null) continue;
            list.Add(lostItem.ItemType);
        }
        return list;
    }

    public void CollectLostItem(LostItem item)
    {
        item.Collect();
        _itemsOnScreen.Remove(item.ItemType);

        OnItemFound?.Invoke(item.ItemType);
    }
}
