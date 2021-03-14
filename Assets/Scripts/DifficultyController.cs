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

    public float screenProbability = 0.5f;
    public float trashProbability = 0.3f;
    public bool uniqueItems = true;

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
        _difficultyEditor.text = string.Format("{0}", screenProbability);
        _trashProbabilityEditor.text = string.Format("{0}", trashProbability);
        _uniqueItemsCheckbox.isOn = uniqueItems;
    }

    public void UpdateValuesFromEditor()
    {
        uniqueItems = _uniqueItemsCheckbox.isOn;

        float difficulty;
        if (float.TryParse(_difficultyEditor.text, out difficulty)
            && difficulty >= 0.0f
            && difficulty <= 1.0f)
        {
            screenProbability = difficulty;
        }

        float parsedTrashProbability;
        if (float.TryParse(_trashProbabilityEditor.text, out parsedTrashProbability)
            && parsedTrashProbability >= 0.0f
            && parsedTrashProbability <= 1.0f)
        {
            trashProbability = parsedTrashProbability;
        }

        float parsedSpeed;
        if (float.TryParse(_speedEditor.text, out parsedSpeed)
            && parsedSpeed > 0.1f)
        {
            _speedScaling = parsedSpeed;
        }

        UpdateEditorsFromValues();
    }
}
