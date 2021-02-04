using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public float acceleration = 0.95f;

    public LostItem _heldObject = null;
    private Vector2 _heldObjectRelativePosition;

    public Collider2D _pickupCollider;

    private Rigidbody2D _rigidBody;

    private Vector2 _previousDelta;

    public SpriteRenderer sprite;
    public Transform picker;
    public float spriteMaxRotationDeg = 15.0f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Debug.Assert(_rigidBody != null);
    }

    private void FixedUpdate()
    {
        float xVelocity = movementSpeed * Input.GetAxis("Horizontal");
        float yVelocity = movementSpeed * Input.GetAxis("Vertical");
        var newVelocity = new Vector2(xVelocity, yVelocity);
        _rigidBody.velocity = Vector2.Lerp(newVelocity, _rigidBody.velocity, acceleration);

        if (_heldObject != null)
        {
            var rigidBody = _heldObject.GetComponent<Rigidbody2D>();
            // rigidBody.MovePosition(_rigidBody.position + _heldObjectRelativePosition);
            rigidBody.MovePosition(picker.position);
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
            _heldObject.Drop();
            _heldObject = null;
        }

        float xVelocity = _rigidBody.velocity.x;
        float rotation = -spriteMaxRotationDeg * Mathf.Abs(xVelocity) / movementSpeed;
        sprite.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);

        if (xVelocity < 0.0f)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else if (xVelocity > 0.0f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    void DropItem()
    {
        if (_heldObject == null) return;
        _heldObject.Drop();
        _heldObject = null;
    }
}
