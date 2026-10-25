using UnityEngine;

public class BoostPad : MonoBehaviour
{
    public float boostStrength = 20f;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb == null)
            return;

        if (rb.GetComponent<PlayerCarController>() == null)
            return;

        rb.AddForce(
            rb.transform.forward * boostStrength,
            ForceMode.Acceleration
        );
    }
}