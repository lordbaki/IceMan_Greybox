using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb == null)
            return;

        if (!rb.CompareTag("Player"))
            return;

        IceCondition ice = rb.GetComponent<IceCondition>();

        if (ice != null)
        {
            ice.FailRun();
        }
    }
}