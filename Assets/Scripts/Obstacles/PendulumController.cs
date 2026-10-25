using UnityEngine;

public class PendulumController : MonoBehaviour
{
    public float swingAngle = 55f;
    public float swingSpeed = 1.5f;

    private Rigidbody rb;
    private Quaternion startRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startRotation = rb.rotation;
    }

    void FixedUpdate()
    {
        float angle =
            Mathf.Sin(Time.time * swingSpeed) *
            swingAngle;

        Quaternion swingRotation =
            Quaternion.Euler(0f, 0f, angle);

        rb.MoveRotation(
            startRotation * swingRotation
        );
    }
}