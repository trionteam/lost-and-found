using UnityEngine;
using UnityEngine.UI;

public class DifficultyController : MonoBehaviour
{
    [SerializeField]
    private InputField _difficultyEditor = default;

    [SerializeField]
    private InputField _trashProbabilityEditor = default;

    [SerializeField]
    private InputField _speedEditor = default;

    [SerializeField]
    private InputField _playerSpeedEditor = default;

    [SerializeField]
    private Toggle _uniqueItemsCheckbox = default;

    [SerializeField]
    private Toggle _uniqueSearchedItemsCheckbox = default;

    [SerializeField]
    [Tooltip("The probability that the searched item is picked from items on the screen.")]
    private float _screenProbability = 0.5f;

    [SerializeField]
    private float _trashProbability = 0.3f;

    [SerializeField]
    private bool _uniqueItems = true;

    [SerializeField]
    private bool _uniqueSearchedItems = true;

    [SerializeField]
    private float _minDropPeriod = 3.0f;
    [SerializeField]
    private float _maxDropPeriod = 5.0f;

    [SerializeField]
    private float _speedScaling = 1.0f;

    [SerializeField]
    private float _playerSpeedScaling = 1.0f;

    [SerializeField]
    private float _beltSpeed = 1.0f;

    public float MinDropPeriod { get => _minDropPeriod / _speedScaling; }
    public float MaxDropPeriod { get => _maxDropPeriod / _speedScaling; }

    public float BeltSpeed { get => _speedScaling * _beltSpeed; }

    public float SpeedScaling { get => _speedScaling; }

    public float PlayerSpeedScaling { get => _playerSpeedScaling; }

    public float ScreenProbability { get => _screenProbability; }

    public float TrashProbability { get => _trashProbability; }

    public bool UniqueItems { get => _uniqueItems; }

    public bool UniqueSearchedItems { get => _uniqueSearchedItems; }

    public static DifficultyController Instance
    {
        get => FindObjectOfType<DifficultyController>();
    }

    private void Awake()
    {
        var allDifficultyControllers = FindObjectsOfType<DifficultyController>();
        Debug.Assert(allDifficultyControllers.Length == 1);
    }

    private void Start()
    {
        UpdateEditorsFromValues();
    }

    private void UpdateEditorsFromValues()
    {
        _difficultyEditor.text = string.Format("{0}", ScreenProbability);
        _trashProbabilityEditor.text = string.Format("{0}", TrashProbability);
        _uniqueItemsCheckbox.isOn = UniqueItems;
        _uniqueSearchedItemsCheckbox.isOn = UniqueSearchedItems;
        _speedEditor.text = string.Format("{0}", SpeedScaling);
        _playerSpeedEditor.text = string.Format("{0}", PlayerSpeedScaling);
    }

    public void UpdateValuesFromEditor()
    {
        _uniqueItems = _uniqueItemsCheckbox.isOn;
        _uniqueSearchedItems = _uniqueSearchedItemsCheckbox.isOn;

        float difficulty;
        if (float.TryParse(_difficultyEditor.text, out difficulty)
            && difficulty >= 0.0f
            && difficulty <= 1.0f)
        {
            _screenProbability = difficulty;
        }

        float parsedTrashProbability;
        if (float.TryParse(_trashProbabilityEditor.text, out parsedTrashProbability)
            && parsedTrashProbability >= 0.0f
            && parsedTrashProbability <= 1.0f)
        {
            _trashProbability = parsedTrashProbability;
        }

        float parsedSpeed;
        if (float.TryParse(_speedEditor.text, out parsedSpeed)
            && parsedSpeed > 0.1f)
        {
            _speedScaling = parsedSpeed;
        }

        float parsedPlayerSpeed;
        if (float.TryParse(_playerSpeedEditor.text, out parsedPlayerSpeed)
            && parsedPlayerSpeed > 0.1f)
        {
            _playerSpeedScaling = parsedPlayerSpeed;
        }

        UpdateEditorsFromValues();
    }
}
