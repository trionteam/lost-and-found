#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float beltSpeed;

    public float beltPositionY;

    private Rigidbody2D _rigidBody;

    public Collider2D moveCollider;

    [SerializeField]
    [HideInInspector]
    private float _width = 1.0f;

    public Transform[] cogs;

    public Transform[] leftCogs;
    public Transform[] rightCogs;

    public SpriteRenderer leftTopBelt;
    public SpriteRenderer rightTopBelt;
    public SpriteRenderer leftBottomBelt;
    public SpriteRenderer rightBottomBelt;

    public BoxCollider2D beltCollider;
    public BoxCollider2D movementCollider;

    private float _beltPosition = 0.0f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Debug.Assert(_rigidBody != null);

        Debug.Assert(leftTopBelt != null);
        Debug.Assert(leftBottomBelt != null);
        Debug.Assert(rightTopBelt != null);
        Debug.Assert(rightBottomBelt != null);
    }

    private void Start()
    {
        _beltPosition = 0.0f;
        UpdateBelts(_beltPosition);
    }

    private void UpdateBelts(float split)
    {
        var halfWidth = _width / 2.0f;
        float topY = leftTopBelt.transform.localPosition.y;
        float bottomY = leftBottomBelt.transform.localPosition.y;
        SetBeltPosition(leftTopBelt, -halfWidth, split, topY);
        SetBeltPosition(rightTopBelt, split, halfWidth, topY);
        SetBeltPosition(leftBottomBelt, -halfWidth, split, bottomY);
        SetBeltPosition(rightBottomBelt, split, halfWidth, bottomY);
    }

    private void SetBeltPosition(SpriteRenderer belt, float left, float right, float y)
    {
        belt.size = new Vector2(right - left, belt.size.y);
        belt.transform.localPosition = new Vector3((left + right) / 2.0f, y);
    }

    private void FixedUpdate()
    {
        Collider2D[] colliders = new Collider2D[64];
        int numCollisions = moveCollider.GetContacts(colliders);
        for (int i = 0; i < numCollisions; ++i)
        {
            var collider = colliders[i];
            if (collider.isTrigger) continue;
            Rigidbody2D rigidBody = collider.attachedRigidbody;
            var newPosition = new Vector2(rigidBody.position.x + Time.fixedDeltaTime * beltSpeed,
                                          rigidBody.position.y);
            collider.attachedRigidbody.velocity = new Vector2(beltSpeed, 0.0f);
        }
    }

    private static readonly float DentSize = 16.0f / 500.0f;

    private void Update()
    {
        _beltPosition += Time.deltaTime * beltSpeed;
        while (_beltPosition < DentSize) _beltPosition += DentSize;
        while (_beltPosition > DentSize) _beltPosition -= DentSize;
        UpdateBelts(_beltPosition);

        if (cogs == null || cogs.Length == 0) return;
        var spriteSize = cogs[0].GetComponent<SpriteRenderer>().bounds.size.y;

        float angularDelta = 360.0f * (Time.deltaTime * beltSpeed) / (Mathf.PI * spriteSize);
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, -angularDelta);
        foreach (var cog in cogs)
        {
            cog.transform.localRotation = cog.transform.localRotation * rotation;
        }
    }

    public void ResetWidth()
    {
        UpdateBelts(_beltPosition);

        foreach (var leftCog in leftCogs)
        {
            leftCog.localPosition = new Vector3(-_width / 2.0f, leftCog.localPosition.y, leftCog.localPosition.z);
        }
        foreach (var rightCog in rightCogs)
        {
            rightCog.localPosition = new Vector3(_width / 2.0f, rightCog.localPosition.y, rightCog.localPosition.z);
        }

        beltCollider.size = new Vector2(_width, beltCollider.size.y);
        movementCollider.size = new Vector2(_width, movementCollider.size.y);
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