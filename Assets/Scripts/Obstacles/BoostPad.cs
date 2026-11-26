
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    public float boostStrength = 20f;

    [Header("Audio")]
    public AudioSource boostAudio;

    private void Awake()
    {
        if (boostAudio == null)
            boostAudio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;

        if (rb == null)
            return;

        if (rb.GetComponent<PlayerCarController>() == null)
            return;

        if (boostAudio != null && !boostAudio.isPlaying)
            boostAudio.Play();
    }

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
