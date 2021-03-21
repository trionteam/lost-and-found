using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private enum GameState
    {
        StartScreen,
        Game,
        Paused,
        EndScreen
    }

    private GameState _state;

    [SerializeField]
    private Player _player = null;

    [SerializeField]
    private GameObject _startScreen = null;
    [SerializeField]
    private GameObject _endScreen = null;
    [SerializeField]
    private GameObject _pauseScreen = null;

    [SerializeField]
    private GameObject[] _hearts = null;
    [SerializeField]
    private TextMeshPro _scoreDisplay = null;

    [SerializeField]
    private GlobalItemQueue _globalQueue = null;

    [SerializeField]
    private int _initialHealth = 3;

    /// <summary>
    /// The current score of the player. Setting it will also update the text
    /// in the HUD.
    /// </summary>
    public int Score
    { 
        get => _score; 
        private set 
        {
            _score = value;
            _scoreDisplay.text = string.Format("{0}", _score);
        }
    }
    /// <summary>
    /// The current score of the player. This should never be accessed
    /// directly, only through <see cref="Score"/>.
    /// </summary>
    private int _score = 0;

    /// <summary>
    /// The current health of the player. Setting it will also update the heart
    /// display in the HUD.
    /// </summary>
    public int Health
    {
        get => _health;
        private set
        {
            _health = Mathf.Max(0, value);
            UpdateHearts();
        }
    }
    /// <summary>
    /// The current health of the player. This should never be accessed
    /// directly, only through <see cref="Health"/>.
    /// </summary>
    private int _health = 0;

    public static GameController Instance
    {
        get => FindObjectOfType<GameController>();
    }

    public event Action OnGameStart;

    private void Awake()
    {
        Debug.Assert(FindObjectsOfType<GameController>().Length == 1);

        Debug.Assert(_globalQueue != null);
        _globalQueue.OnSearchedItemShredded += OnSearchedItemShredded;
        _globalQueue.OnItemFound += OnItemFound;

        Debug.Assert(_scoreDisplay != null);
    }

    void Start()
    {
        ShowIntroScreen();
    }

    void Update()
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
                if (Input.GetButtonDown("Pause"))
                {
                    PauseGame();
                }
                break;
            case GameState.EndScreen:
                if (Input.anyKeyDown)
                {
                    RestartGame();
                }
                break;
            case GameState.Paused:
                if (Input.GetButtonDown("Pause"))
                {
                    UnpauseGame();
                }
                break;
        }
    }

    private void ShowIntroScreen()
    {
        Health = _initialHealth;
        Score = 0;
        _player.ControlledByAi = true;
        _state = GameState.StartScreen;
        _startScreen.SetActive(true);
    }

    private void StartGame()
    {
        _state = GameState.Game;
        Health = _initialHealth;
        Score = 0;
        _startScreen.SetActive(false);
        _player.ControlledByAi = false;
        OnGameStart?.Invoke();
    }

    private void PauseGame()
    {
        _state = GameState.Paused;
        Time.timeScale = 0.0f;
        _pauseScreen.SetActive(true);
    }

    public void UnpauseGame()
    {
        _state = GameState.Game;
        Time.timeScale = 1.0f;
        _pauseScreen.SetActive(false);
    }

    private void EndGame()
    {
        if (_state == GameState.Paused)
        {
            UnpauseGame();
        }
        _state = GameState.EndScreen;
        _endScreen.SetActive(true);
        _player.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        // Reload the current scene to restart the game.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void OnItemFound(LostItemType itemType)
    {
        Score += itemType.scoreIncrease;
    }

    private void OnSearchedItemShredded(LostItemType itemType)
    {
        // Update the current health.
        Health -= itemType.healthDecrease;

        // End the game if needed.
        if (Health == 0 && _state != GameState.StartScreen)
        {
            EndGame();
        }
    }

    /// <summary>
    /// Updates the life indicator in the HUD.
    /// </summary>
    private void UpdateHearts()
    {
        // TODO(ondrasej): Move this code to the HUD.
        var visibleHearts = Mathf.Min(_hearts.Length, Health);
        for (int i = 0; i < visibleHearts; i++)
        {
            _hearts[i].SetActive(true);
        }
        for (int i = visibleHearts; i < _hearts.Length; i++)
        {
            _hearts[i].SetActive(false);
        }
    }
}
