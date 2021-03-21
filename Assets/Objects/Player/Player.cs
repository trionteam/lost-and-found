using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private interface PlayerInput
    {
        Vector2 Direction { get; }
        bool PickUpButtonPressed { get; }
        bool PickUpButtonReleased { get; }

        void Update();
    }

    private class ActualInput : PlayerInput
    {
        public Vector2 Direction { get => new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); }
        public bool PickUpButtonPressed { get => Input.GetButtonDown("Fire1"); }
        public bool PickUpButtonReleased { get => Input.GetButtonUp("Fire1"); }

        public void Update() { }
    }

    private class AiInput : PlayerInput
    {
        private static float PickUpDistance = 0.07f;

        private static float DefaultPositionDistance = 0.5f;

        public Vector2 Direction
        {
            get
            {
                var targetDirection = _target != null ? TargetPosition - PlayerPosition : new Vector2();
                var distanceToTarget = targetDirection.magnitude;
                if (distanceToTarget < RequiredDistanceToTarget)
                {
                    return new Vector2();
                }
                return targetDirection.normalized;
            }
        }
        public bool PickUpButtonPressed { get; private set; }
        public bool PickUpButtonReleased { get; private set; }

        private GlobalItemQueue _globalQueue;

        private Player _player;

        private Transform _target;

        private Vector2 PlayerPosition
        {
            get => _player.GetComponent<Rigidbody2D>().position;
        }

        private Vector2 TargetPosition
        {
            get => new Vector2(_target.position.x, _target.position.y);
        }

        private float RequiredDistanceToTarget
        {
            get => _target == _player._aiDefaultPosition ? DefaultPositionDistance : PickUpDistance;
        }

        public AiInput(GlobalItemQueue globalQueue, Player player)
        {
            Debug.Assert(globalQueue != null);
            _globalQueue = globalQueue;
            Debug.Assert(player != null);
            _player = player;
        }

        public void Update()
        {
            PickUpButtonPressed = false;
            PickUpButtonReleased = false;
            if (_player._heldObject != null)
            {
                var destination = _globalQueue.ItemDestination(_player._heldObject);
                if (destination == null)
                {
                    // The held item is no longer searched, drop it.
                    PickUpButtonReleased = true;
                    return;
                }
                else if (_target == null || _target == _player._heldObject.transform)
                {
                    // The player is holding an object that needs to be returned, but it does
                    // not have the right destination set. Set it now.
                    _target = destination.transform;
                    return;
                }
            }
            else if (_target != null && _target != _player._aiDefaultPosition)
            {
                if (_target.GetComponent<LostItem>() == null)
                {
                    // The destination is not an object to pick up. Reset it.
                    _target = _player._aiDefaultPosition;
                    return;
                }
                else
                {
                    float distanceToTarget = (TargetPosition - PlayerPosition).magnitude;
                    if (distanceToTarget < PickUpDistance)
                    {
                        PickUpButtonPressed = true;
                    }
                }
            }
            else  // _target == null && _heldObject == null
            {
                var searchedItemsOnScreen = FindObjectsOfType<LostItem>()
                    .Where(item => _globalQueue.ItemDestination(item) != null)
                    .ToArray();
                if (searchedItemsOnScreen.Length > 0)
                {
                    _target = searchedItemsOnScreen[Random.Range(0, searchedItemsOnScreen.Length)].transform;
                }
            }
        }
    }

    [SerializeField]
    private float _maxSpeedUp = 4.0f;
    [SerializeField]
    private float _maxSpeedDown = 6.0f;
    [SerializeField]
    private float _maxSpeedHorizontal = 5.0f;

    private float MaxSpeedUp { get => _maxSpeedUp * _difficulty.PlayerSpeedScaling; }
    private float MaxSpeedDown { get => _maxSpeedDown * _difficulty.PlayerSpeedScaling; }
    private float MaxSpeedHorizontal { get => _maxSpeedHorizontal * _difficulty.PlayerSpeedScaling; }

    [SerializeField]
    private float _acceleration = 0.95f;

    [SerializeField]
    private Collider2D _pickupCollider = default;

    private Rigidbody2D _rigidBody;

    [SerializeField]
    private SpriteRenderer _sprite = default;
    [SerializeField]
    private Transform _picker = default;
    [SerializeField]
    private float _spriteMaxRotationDeg = 15.0f;

    private LostItem _heldObject = null;
    private Animator _animator;

    private DifficultyController _difficulty;

    private PlayerInput _input = new ActualInput();

    [SerializeField]
    private GlobalItemQueue _globalQueue = default;

    private Transform _aiDefaultPosition;

    public bool ControlledByAi
    {
        get => _input is AiInput;
        set => _input = value ? (PlayerInput)new AiInput(_globalQueue, this) : (PlayerInput)new ActualInput();
    }

    [SerializeField]
    private float _minAnimationSpeed = 0.5f;

    [SerializeField]
    private float _maxAnimationSpeed = 2.0f;

    private void Awake()
    {
        _difficulty = DifficultyController.Instance;
        Debug.Assert(_difficulty != null);

        _rigidBody = GetComponent<Rigidbody2D>();
        Debug.Assert(_rigidBody != null);

        _animator = GetComponent<Animator>();
        Debug.Assert(_animator != null);

        GameController.Instance.OnGameStart += ResetOnGameStart;
    }

    private void Start()
    {
        _aiDefaultPosition = new GameObject().transform;
        _aiDefaultPosition.position = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 direction = _input.Direction;
        float xVelocity = MaxSpeedHorizontal * direction.x;
        float yAxis = direction.y;
        float yVelocity = yAxis > 0.0f ? MaxSpeedUp * yAxis : MaxSpeedDown * yAxis;
        var newVelocity = new Vector2(xVelocity, yVelocity);
        _rigidBody.velocity = Vector2.Lerp(newVelocity, _rigidBody.velocity, _acceleration);

        if (_heldObject != null)
        {
            var rigidBody = _heldObject.GetComponent<Rigidbody2D>();
            rigidBody.MovePosition(_picker.position);
            rigidBody.velocity = _rigidBody.velocity;
        }
    }
    private void Update()
    {
        _input.Update();
        if (_input.PickUpButtonPressed)
        {
            var lostItem = PickedUpItem();
            if (lostItem != null)
            {
                lostItem.Pickup(this);
                _heldObject = lostItem;
            }
        }
        if (_input.PickUpButtonReleased && _heldObject != null)
        {
            DropItem();
        }

        float xVelocity = _rigidBody.velocity.x;
        float rotation = -_spriteMaxRotationDeg * Mathf.Abs(xVelocity) / MaxSpeedHorizontal;
        _sprite.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);

        if (xVelocity < 0.0f)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else if (xVelocity > 0.0f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        float yVelocityMagniture = (_rigidBody.velocity.y / (2.0f * MaxSpeedDown)) + 0.5f;
        _animator.speed = _minAnimationSpeed + (_maxAnimationSpeed - _minAnimationSpeed) * yVelocityMagniture;
    }

    private void DropItem()
    {
        if (_heldObject == null) return;
        _heldObject.Drop();
        _heldObject = null;
    }

    /// <summary>
    /// Returns the item that would be picked up by the player at the current frame. Returns
    /// <c>null</c> when no item matches.
    /// </summary>
    /// <returns>The item that would be picked up by the player or <c>null</c> if no item would
    /// be picked up.</returns>
    private LostItem PickedUpItem()
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
            return lostItem;
        }
        return null;
    }

    public void ResetOnGameStart()
    {
        // TODO(ondrasej): Revert control - make the start of the game an event on GameController.
        _heldObject = null;
        _rigidBody.MovePosition(new Vector2());
        _rigidBody.velocity = new Vector2();
    }
}
