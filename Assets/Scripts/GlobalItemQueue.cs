using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalItemQueue : MonoBehaviour
{
    private enum GameState
    {
        StartScreen,
        Game,
        EndScreen
    }

    public LostItemCollection lostItems;
    public LostItemCollection trashItems;

    private List<LostItemType> _allItemTypes;

    private List<LostItemType> _lostItemQueue;
    private List<bool> _lostItemInQueueSearched;

    public TextMeshPro scoreDisplay;

    public int lostItemQueueSize = 16;
    public int initialHealth = 3;

    public Collider2D dangerZone;

    public Destination[] destinations;
    public GameObject[] hearts;

    public Image startScreen;
    public Image endScreen;

    private GameState _state;

    public Player player;

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

    private int _health = 3;
    public int Health
    {
        get => _health;
        private set
        {
            _health = value;
            var visibleHearts = Mathf.Min(hearts.Length, value);
            for (int i = 0; i < visibleHearts; i++)
            {
                hearts[i].SetActive(true);
            }
            for (int i = visibleHearts; i < hearts.Length; i++)
            {
                hearts[i].SetActive(false);
            }
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
        Health = initialHealth;
        ShowIntroScreen();
    }

    private void ShowIntroScreen()
    {
        player.gameObject.SetActive(false);
        _state = GameState.StartScreen;
        Time.timeScale = 0.0f;
        startScreen.gameObject.SetActive(true);
    }

    private void StartGame()
    {
        _state = GameState.Game;
        Time.timeScale = 1.0f;
        startScreen.gameObject.SetActive(false);
        player.gameObject.SetActive(true);
    }

    private void EndGame()
    {
        _state = GameState.EndScreen;
        endScreen.gameObject.SetActive(true);
        player.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (_state)
        {
            case GameState.StartScreen:
                if (Input.anyKeyDown)
                {
                    StartGame();
                }
                break;
            case GameState.Game:
                break;
            case GameState.EndScreen:
                if (Input.anyKeyDown)
                {
                    // Reload the current scene to restart the game.
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                break;
        }
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
        var inDangerZone = ItemsInDangerZone();
        for(; ;)
        {
            int itemIndex = Random.Range(0, _lostItemQueue.Count);
            if (_lostItemInQueueSearched[itemIndex]) continue;
            var item = _lostItemQueue[itemIndex];
            if (System.Array.IndexOf(trashItems.lostItems, item) >= 0) continue;
            if (inDangerZone.Contains(item)) continue;
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

    public Destination ItemDestination(LostItem item)
    {
        foreach (var destination in destinations)
        {
            if (destination.AcceptedItemType == item.itemType) return destination;
        }
        return null;
    }

    public void ShredLostItem(LostItem item)
    {
        var itemDestination = ItemDestination(item);
        if (itemDestination != null)
        {
            itemDestination.ItemShredded();
            Health = Mathf.Max(Health - item.itemType.healthDecrease, 0);
            if (Health == 0)
            {
                EndGame();
            }
        }
        item.Shred();
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
        Score += item.itemType.scoreIncrease;
        item.Collect();
    }
}
