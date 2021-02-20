using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private Player _player;

    [SerializeField]
    private GameObject _startScreen;
    [SerializeField]
    private GameObject _endScreen;
    [SerializeField]
    private GameObject _pauseScreen;

    [SerializeField]
    private GameObject[] _hearts;
    [SerializeField]
    private TextMeshPro _scoreDisplay;

    [SerializeField]
    private GlobalItemQueue _globalQueue;

    [SerializeField]
    private int _initialHealth = 3;

    private int _score = 0;
    private int _health = 0;

    private void Awake()
    {
        Debug.Assert(_globalQueue != null);
        _globalQueue.OnLostItemShredded += OnLostItemShredded;
        _globalQueue.OnItemFound += OnItemFound;

        Debug.Assert(_scoreDisplay != null);
    }

    void Start()
    {
        _health = _initialHealth;
        _score = 0;
        _scoreDisplay.text = "0";
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
        _player.gameObject.SetActive(false);
        _state = GameState.StartScreen;
        Time.timeScale = 0.0f;
        _startScreen.SetActive(true);
    }

    private void StartGame()
    {
        _state = GameState.Game;
        Time.timeScale = 1.0f;
        _startScreen.SetActive(false);
        _player.gameObject.SetActive(true);
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
        _score += itemType.scoreIncrease;
        _scoreDisplay.text = string.Format("{0}", _score);
    }

    private void OnLostItemShredded(LostItemType itemType)
    {
        // Update the current health.
        _health = Mathf.Max(0, _health - itemType.healthDecrease);

        // Update the life indicator in the HUD.
        // TODO(ondrasej): Move this code to the HUD.
        var visibleHearts = Mathf.Min(_hearts.Length, _health);
        for (int i = 0; i < visibleHearts; i++)
        {
            _hearts[i].SetActive(true);
        }
        for (int i = visibleHearts; i < _hearts.Length; i++)
        {
            _hearts[i].SetActive(false);
        }

        // End the game if needed.
        if (_health == 0)
        {
            EndGame();
        }
    }
}
