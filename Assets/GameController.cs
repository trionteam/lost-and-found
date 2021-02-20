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

    // Start is called before the first frame update
    void Start()
    {
        ShowIntroScreen();
    }

    // Update is called once per frame
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
                    // Reload the current scene to restart the game.
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    public void EndGame()
    {
        if (_state == GameState.Paused)
        {
            UnpauseGame();
        }
        _state = GameState.EndScreen;
        _endScreen.SetActive(true);
        _player.gameObject.SetActive(false);
    }
}
