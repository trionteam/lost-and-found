using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float _beltSpeed;

    public float _beltPositionY;

    private Rigidbody2D _rigidBody;

    public Collider2D _moveCollider;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        //Debug.Assert(_beltPiecePrefab != null);
    }

    private void Start()
    {
        //CreateBeltPieces();
    }

    private void FixedUpdate()
    {
        Collider2D[] colliders = new Collider2D[64];
        int numCollisions = _moveCollider.GetContacts(colliders);
        for (int i = 0; i < numCollisions; ++i)
        {
            var collider = colliders[i];
            if (collider.isTrigger) continue;
            Rigidbody2D rigidBody = collider.attachedRigidbody;
            var newPosition = new Vector2(rigidBody.position.x + Time.fixedDeltaTime * _beltSpeed,
                                          rigidBody.position.y);
            // collider.attachedRigidbody.MovePosition(newPosition);
            collider.attachedRigidbody.velocity = new Vector2(_beltSpeed, 0.0f);
        }
    }

}
