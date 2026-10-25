using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveDistance = 2.5f;
    public float moveSpeed = 1.2f;

    // 0 = starts moving right
    // 180 = starts moving left
    public float phaseOffset = 0f;

    private Rigidbody rb;
    private Vector3 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = rb.position;
    }

    void FixedUpdate()
    {
        float phase =
            Time.time * moveSpeed +
            phaseOffset * Mathf.Deg2Rad;

        float offset =
            Mathf.Sin(phase) * moveDistance;

        Vector3 targetPosition =
            startPosition +
            transform.right * offset;

        rb.MovePosition(targetPosition);
    }
}