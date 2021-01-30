using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostItemSource : MonoBehaviour
{
    public GameObject[] lostItemPrefabs;

    public float minPeriod;
    public float maxPeriod;

    public float nextItemDropTime;
    public Vector2 initialVelocity;

    private void Awake()
    {
        Debug.Assert(lostItemPrefabs != null);
        foreach(var lostItem in lostItemPrefabs)
        {
            Debug.Assert(lostItem != null);
        }
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
        int itemIndex = Random.Range(0, lostItemPrefabs.Length);
        var itemPrefab = lostItemPrefabs[itemIndex];

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
