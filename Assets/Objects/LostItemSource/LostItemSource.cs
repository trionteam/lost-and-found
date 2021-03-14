using UnityEngine;

public class LostItemSource : MonoBehaviour
{
    public GlobalItemQueue globalItemQueue;

    public float nextItemDropTime;
    public Vector2 initialVelocity;

    private DifficultyController _difficulty;

    private void Awake()
    {
        Debug.Assert(globalItemQueue != null);

        _difficulty = DifficultyController.Instance;
        Debug.Assert(_difficulty != null);
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
        var animator = GetComponent<Animator>();
        animator.Play("Spit");

        SetNextItemDropTime();
    }

    private void SpitItem()
    {
        var itemPrefab = globalItemQueue.NextLostItem().prefab;
        var item = Instantiate<GameObject>(itemPrefab, transform);
        var rigidBody = item.GetComponent<Rigidbody2D>();
        rigidBody.velocity = initialVelocity;
    }

    private void SetNextItemDropTime()
    {
        nextItemDropTime =
            Time.fixedTime + Random.Range(_difficulty.MinDropPeriod, _difficulty.MaxDropPeriod);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(initialVelocity.x, initialVelocity.y, 0.0f));
    }
}
