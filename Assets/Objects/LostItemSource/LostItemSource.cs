using UnityEngine;

public class LostItemSource : MonoBehaviour
{
    [SerializeField]
    private GlobalItemQueue _globalItemQueue = default;

    private float _nextItemDropTime;

    [SerializeField]
    private Vector2 _initialVelocity = new Vector2(0.0f, -1.0f);

    private DifficultyController _difficulty;

    private void Awake()
    {
        Debug.Assert(_globalItemQueue != null);

        _difficulty = DifficultyController.Instance;
        Debug.Assert(_difficulty != null);
    }

    private void Start()
    {
        SetNextItemDropTime();
    }

    private void FixedUpdate()
    {
        if (_nextItemDropTime <= Time.fixedTime)
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
        var itemPrefab = _globalItemQueue.NextLostItem().prefab;
        var item = Instantiate<GameObject>(itemPrefab, transform);
        var rigidBody = item.GetComponent<Rigidbody2D>();
        rigidBody.velocity = _initialVelocity;
    }

    private void SetNextItemDropTime()
    {
        _nextItemDropTime =
            Time.fixedTime + Random.Range(_difficulty.MinDropPeriod, _difficulty.MaxDropPeriod);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(_initialVelocity.x, _initialVelocity.y, 0.0f));
    }
}
