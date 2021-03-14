using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _maxSpeedUp = 4.0f;
    [SerializeField]
    private float _maxSpeedDown = 6.0f;
    [SerializeField]
    private float _maxSpeedHorizontal = 5.0f;

    [SerializeField]
    private float _acceleration = 0.95f;

    [SerializeField]
    private Collider2D _pickupCollider;

    private Rigidbody2D _rigidBody;

    private Vector2 _previousDelta;

    [SerializeField]
    private SpriteRenderer _sprite;
    [SerializeField]
    private Transform _picker;
    [SerializeField]
    private float _spriteMaxRotationDeg = 15.0f;

    private LostItem _heldObject = null;
    private Vector2 _heldObjectRelativePosition;
    private Animator _animator;

    [SerializeField]
    private float _minAnimationSpeed = 0.5f;

    [SerializeField]
    private float _maxAnimationSpeed = 2.0f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Debug.Assert(_rigidBody != null);

        _animator = GetComponent<Animator>();
        Debug.Assert(_animator != null);
    }

    private void FixedUpdate()
    {
        float xVelocity = _maxSpeedHorizontal * Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        float yVelocity = yAxis > 0.0f ? _maxSpeedUp * yAxis : _maxSpeedDown * yAxis;
        var newVelocity = new Vector2(xVelocity, yVelocity);
        _rigidBody.velocity = Vector2.Lerp(newVelocity, _rigidBody.velocity, _acceleration);

        if (_heldObject != null)
        {
            var rigidBody = _heldObject.GetComponent<Rigidbody2D>();
            // rigidBody.MovePosition(_rigidBody.position + _heldObjectRelativePosition);
            rigidBody.MovePosition(_picker.position);
            rigidBody.velocity = _rigidBody.velocity;
        }
    }
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Collider2D[] colliders = new Collider2D[64];
            LayerMask mask = LayerMask.GetMask("BeltItems");
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(mask);
            filter.useTriggers = true;
            int numColliders = _pickupCollider.GetContacts(filter, colliders);

            for (int i = 0; i < numColliders; ++i)
            {
                var lostItem = colliders[i].GetComponent<LostItem>();
                if (lostItem == null) continue;

                lostItem.Pickup(this);
                _heldObject = lostItem;
                _heldObjectRelativePosition = colliders[i].attachedRigidbody.position - _rigidBody.position;
                break;
            }
        }
        if (Input.GetButtonUp("Fire1") && _heldObject != null)
        {
            DropItem();
        }

        float xVelocity = _rigidBody.velocity.x;
        float rotation = -_spriteMaxRotationDeg * Mathf.Abs(xVelocity) / _maxSpeedHorizontal;
        _sprite.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);

        if (xVelocity < 0.0f)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else if (xVelocity > 0.0f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        float yVelocityMagniture = (_rigidBody.velocity.y / (2.0f * _maxSpeedDown)) + 0.5f;
        _animator.speed = _minAnimationSpeed + (_maxAnimationSpeed - _minAnimationSpeed) * yVelocityMagniture;
    }

    void DropItem()
    {
        if (_heldObject == null) return;
        _heldObject.Drop();
        _heldObject = null;
    }
}
