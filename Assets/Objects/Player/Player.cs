using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float _movementSpeed = 1.0f;
    public float _acceleration = 0.95f;

    public LostItem _heldObject = null;
    private Vector2 _heldObjectRelativePosition;

    public Collider2D _pickupCollider;

    private Rigidbody2D _rigidBody;

    private Vector2 _previousDelta;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Debug.Assert(_rigidBody != null);
    }

    private void FixedUpdate()
    {
        float xVelocity = _movementSpeed * Input.GetAxis("Horizontal");
        float yVelocity = _movementSpeed * Input.GetAxis("Vertical");
        var delta = new Vector2(Time.fixedDeltaTime * xVelocity,
                                Time.fixedDeltaTime * yVelocity);
        delta = Vector2.Lerp(delta, _previousDelta, _acceleration);
        var newPosition = _rigidBody.position + delta;
        _rigidBody.MovePosition(newPosition);
        _previousDelta = delta;

        if (_heldObject != null)
        {
            var rigidBody = _heldObject.GetComponent<Rigidbody2D>();
            rigidBody.MovePosition(_rigidBody.position + _heldObjectRelativePosition);
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

                lostItem.Pickup();
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
    }

    void DropItem()
    {
        if (_heldObject == null) return;
        _heldObject.Drop();
        _heldObject = null;
    }
}
