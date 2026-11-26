using UnityEngine;

public class PendulumController : MonoBehaviour
{
    public float swingAngle = 55f;
    public float swingSpeed = 1.5f;

    [Header("Impact Audio")]
    public AudioSource impactAudio;
    [Min(0f)] public float soundCooldown = 0.15f;
    [Min(0.01f)] public float fullVolumeSpeed = 8f;

    private float lastSoundTime = -10f;

    private Rigidbody rb;
    private Quaternion startRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startRotation = rb.rotation;

        if (impactAudio == null)
            impactAudio = GetComponent<AudioSource>();
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
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody playerRb = collision.rigidbody;

        if (playerRb == null || !playerRb.CompareTag("Player"))
            return;

        if (impactAudio == null || impactAudio.clip == null)
            return;

        if (Time.time - lastSoundTime < soundCooldown)
            return;

        float intensity = Mathf.InverseLerp(
            0f, fullVolumeSpeed,
            collision.relativeVelocity.magnitude
        );

        float volume = Mathf.Lerp(0.4f, 1f, intensity);
        impactAudio.PlayOneShot(impactAudio.clip, volume);
        lastSoundTime = Time.time;
    }
}