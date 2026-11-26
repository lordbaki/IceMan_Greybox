using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpikeTrap : MonoBehaviour
{
    public Transform spikesVisual;

    [Header("Timing")]
    public float downTime = 2.5f;
    public float upTime = 1.2f;
    public float moveTime = 0.3f;
    public float phaseOffset = 0f;

    [Header("Settings")]
    public float retractDistance = 1.2f;
    public float iceDamage = 8f;

    [Header("Impact")]
    [Range(0f, 0.9f)]
    public float speedLoss = 0.35f;

    private Vector3 raisedPosition;
    private bool damageActive;
    private bool hitThisCycle;
    private float timer;

    void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;

        if (spikesVisual == null)
        {
            Debug.LogWarning("Spike visual is not assigned.");
            enabled = false;
            return;
        }

        raisedPosition = spikesVisual.localPosition;
        timer = phaseOffset;
    }

    void Update()
    {
        float cycleLength =
            downTime + upTime + moveTime * 2f;

        timer = Mathf.Repeat(
            timer + Time.deltaTime, cycleLength
        );

        float extension;

        if (timer < downTime)
        {
            extension = 0f;
        }
        else if (timer < downTime + moveTime)
        {
            extension = (timer - downTime) / moveTime;
        }
        else if (timer < downTime + moveTime + upTime)
        {
            extension = 1f;
        }
        else
        {
            extension = 1f -
                (timer - downTime - moveTime - upTime)
                / moveTime;
        }

        extension = Mathf.Clamp01(extension);

        spikesVisual.localPosition =
            raisedPosition - Vector3.up *
            retractDistance * (1f - extension);

        damageActive = extension >= 0.8f;

        if (!damageActive)
            hitThisCycle = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (!damageActive || hitThisCycle)
            return;

        Rigidbody rb = other.attachedRigidbody;

        if (rb == null || !rb.CompareTag("Player"))
            return;

        IceCondition ice = rb.GetComponent<IceCondition>();

        if (ice == null)
            return;

        // Apply and record spike damage exactly once per active cycle.
        if (!ice.TakeSpikeDamage(iceDamage))
            return;

        hitThisCycle = true;

        // Preserve the existing slowdown on non-fatal hits.
        // FailRun freezes the Rigidbody, so do not change it afterward.
        if (ice.HasFailed() || rb.isKinematic)
            return;

        Vector3 velocity = rb.linearVelocity;
        velocity.x *= 1f - speedLoss;
        velocity.z *= 1f - speedLoss;
        rb.linearVelocity = velocity;
    }
}