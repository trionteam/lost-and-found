﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostItemSource : MonoBehaviour
{
    public GlobalItemQueue globalItemQueue;

    public float minPeriod;
    public float maxPeriod;

    public float nextItemDropTime;
    public Vector2 initialVelocity;

    private void Awake()
    {
        Debug.Assert(globalItemQueue != null);
    }

    private void Start()
    {
        SetNextItemDropTime();
    }

    private void FixedUpdate()
    {
        if (nextItemDropTime <= Time.fixedTime)
        {
            DropItem();
        }
    }

    private void DropItem()
    {
        var itemPrefab = globalItemQueue.NextLostItem().prefab;

        var item = Instantiate<GameObject>(itemPrefab, transform);
        var rigidBody = item.GetComponent<Rigidbody2D>();
        rigidBody.velocity = initialVelocity;

        SetNextItemDropTime();
    }

    private void SetNextItemDropTime()
    {
        nextItemDropTime = Time.fixedTime + Random.Range(minPeriod, maxPeriod);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(initialVelocity.x, initialVelocity.y, 0.0f));
    }
}
