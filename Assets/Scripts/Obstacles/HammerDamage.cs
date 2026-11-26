using UnityEngine;

public class HammerDamage : MonoBehaviour
{
    public float minimumHitSpeed = 3f;
    public float damageMultiplier = 3f;
    public float damageCooldown = 0.5f;

    [Header("Audio")]
    public AudioSource impactAudio;
    [Min(0f)] public float soundCooldown = 0.15f;

    private float lastSoundTime = -10f;

    private void Awake()
    {
        if (impactAudio == null)
            impactAudio = GetComponent<AudioSource>();
    }

    private float lastDamageTime = -10f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody playerRb = collision.rigidbody;

        if (playerRb == null || !playerRb.CompareTag("Player"))
            return;

        float hitStrength = collision.relativeVelocity.magnitude;

        // Play collision audio independently of ice damage.
        if (impactAudio != null && impactAudio.clip != null &&
            Time.time - lastSoundTime >= soundCooldown)
        {
            float intensity = Mathf.InverseLerp(0f, 10f, hitStrength);
            float volumeScale = Mathf.Lerp(0.45f, 1f, intensity);
            impactAudio.PlayOneShot(impactAudio.clip, volumeScale);
            lastSoundTime = Time.time;
        }

        if (Time.time - lastDamageTime < damageCooldown)
            return;

        if (hitStrength < minimumHitSpeed)
            return;

        IceCondition ice =
            playerRb.GetComponent<IceCondition>();

        RunStats stats =
            playerRb.GetComponent<RunStats>();

        if (ice == null)
            return;

        float damage =
            (hitStrength - minimumHitSpeed) *
            damageMultiplier;

        float iceBefore = ice.currentIce;

        ice.TakeDamage(damage);

        float actualDamage =
            iceBefore - ice.currentIce;

        if (stats != null && actualDamage > 0f)
            stats.AddHammerDamage(actualDamage);

        lastDamageTime = Time.time;
    }
}