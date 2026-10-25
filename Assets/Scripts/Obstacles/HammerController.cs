using UnityEngine;

public class HammerController : MonoBehaviour
{
    public float lowAngle = 0f;
    public float highAngle = 80f;
    public float swingSpeed = 2f;

    private Rigidbody rb;
    private Quaternion startLocalRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startLocalRotation = transform.localRotation;
    }

    void FixedUpdate()
    {
        float t = (Mathf.Sin(Time.time * swingSpeed) + 1f) / 2f;

        float angle = Mathf.Lerp(lowAngle, highAngle, t);

        Quaternion localSwing =
            startLocalRotation * Quaternion.Euler(0f, 0f, angle);

        Quaternion targetRotation;

        if (transform.parent != null)
            targetRotation = transform.parent.rotation * localSwing;
        else
            targetRotation = localSwing;

        rb.MoveRotation(targetRotation);
    }
}