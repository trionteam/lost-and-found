using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    private enum Direction { Left, Right }

    private float BeltSpeed
    {
        get
        {
            var speed = _difficulty.BeltSpeed * _speedScaling;
            return _direction == Direction.Left ? -speed : speed;
        }
    }

    [SerializeField]
    private Direction _direction;

    [SerializeField]
    private float _speedScaling = 1.0f;

    [SerializeField]
    [HideInInspector]
    private float _width = 1.0f;

    [SerializeField]
    private Transform[] _cogs = default;

    [SerializeField]
    private Transform[] _leftCogs = default;
    [SerializeField]
    private Transform[] _rightCogs = default;

    [SerializeField]
    private SpriteRenderer _leftTopBelt = default;
    [SerializeField]
    private SpriteRenderer _rightTopBelt = default;
    [SerializeField]
    private SpriteRenderer _leftBottomBelt = default;
    [SerializeField]
    private SpriteRenderer _rightBottomBelt = default;

    [SerializeField]
    private BoxCollider2D _beltCollider = default;
    [SerializeField]
    private BoxCollider2D _movementCollider = default;

    #region Properties filled during awake
    private DifficultyController _difficulty;

    private Rigidbody2D _rigidBody;

    /// <summary>
    /// The current position of the belt. Determines the angle of the cogs and the
    /// position/scale of the belt items.
    /// </summary>
    private float _beltPosition = 0.0f;
    #endregion

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Debug.Assert(_rigidBody != null);

        Debug.Assert(_cogs != null && _cogs.All(cog => cog != null));
        Debug.Assert(_leftCogs != null && _leftCogs.All(cog => cog != null));
        Debug.Assert(_rightCogs != null && _rightCogs.All(cog => cog != null));

        Debug.Assert(_leftTopBelt != null);
        Debug.Assert(_leftBottomBelt != null);
        Debug.Assert(_rightTopBelt != null);
        Debug.Assert(_rightBottomBelt != null);

        Debug.Assert(_beltCollider != null);
        Debug.Assert(_movementCollider != null);

        _difficulty = DifficultyController.Instance;
        Debug.Assert(_difficulty != null);
    }

    private void Start()
    {
        _beltPosition = 0.0f;
        UpdateBelts(_beltPosition);
    }

    private void UpdateBelts(float split)
    {
        var halfWidth = _width / 2.0f;
        float topY = _leftTopBelt.transform.localPosition.y;
        float bottomY = _leftBottomBelt.transform.localPosition.y;
        SetBeltPosition(_leftTopBelt, -halfWidth, split, topY);
        SetBeltPosition(_rightTopBelt, split, halfWidth, topY);
        SetBeltPosition(_leftBottomBelt, -halfWidth, split, bottomY);
        SetBeltPosition(_rightBottomBelt, split, halfWidth, bottomY);
    }

    private void SetBeltPosition(SpriteRenderer belt, float left, float right, float y)
    {
        belt.size = new Vector2(right - left, belt.size.y);
        belt.transform.localPosition = new Vector3((left + right) / 2.0f, y);
    }

    private void FixedUpdate()
    {
        Collider2D[] colliders = new Collider2D[64];
        int numCollisions = _movementCollider.GetContacts(colliders);
        for (int i = 0; i < numCollisions; ++i)
        {
            var collider = colliders[i];
            if (collider.isTrigger) continue;
            Rigidbody2D rigidBody = collider.attachedRigidbody;
            var newPosition = new Vector2(rigidBody.position.x + Time.fixedDeltaTime * BeltSpeed,
                                          rigidBody.position.y);
            collider.attachedRigidbody.velocity = new Vector2(BeltSpeed, 0.0f);
        }
    }

    private static readonly float DentSize = 16.0f / 500.0f;

    private void Update()
    {
        _beltPosition += Time.deltaTime * BeltSpeed;
        while (_beltPosition < DentSize) _beltPosition += DentSize;
        while (_beltPosition > DentSize) _beltPosition -= DentSize;
        UpdateBelts(_beltPosition);

        if (_cogs == null || _cogs.Length == 0) return;
        var spriteSize = _cogs[0].GetComponent<SpriteRenderer>().bounds.size.y;

        float angularDelta = 360.0f * (Time.deltaTime * BeltSpeed) / (Mathf.PI * spriteSize);
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, -angularDelta);
        foreach (var cog in _cogs)
        {
            cog.transform.localRotation = cog.transform.localRotation * rotation;
        }
    }

    public void ResetWidth()
    {
        UpdateBelts(_beltPosition);

        foreach (var leftCog in _leftCogs)
        {
            leftCog.localPosition = new Vector3(-_width / 2.0f, leftCog.localPosition.y, leftCog.localPosition.z);
        }
        foreach (var rightCog in _rightCogs)
        {
            rightCog.localPosition = new Vector3(_width / 2.0f, rightCog.localPosition.y, rightCog.localPosition.z);
        }

        _beltCollider.size = new Vector2(_width, _beltCollider.size.y);
        _movementCollider.size = new Vector2(_width, _movementCollider.size.y);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ConveyorBelt))]
public class ConveyorBeltEditor : Editor
{
    SerializedProperty _width;

    private void OnEnable()
    {
        _width = serializedObject.FindProperty("_width");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI(); 
        serializedObject.Update();
        DrawDefaultInspector();
        EditorGUILayout.PropertyField(_width);
        serializedObject.ApplyModifiedProperties();

        var belt = (ConveyorBelt)serializedObject.targetObject;
        belt.ResetWidth();
    }
}
#endif