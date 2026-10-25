using UnityEngine;

public class HammerDamage : MonoBehaviour
{
    public float minimumHitSpeed = 3f;
    public float damageMultiplier = 3f;
    public float damageCooldown = 0.5f;

    private float lastDamageTime = -10f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody playerRb = collision.rigidbody;

        if (playerRb == null || !playerRb.CompareTag("Player"))
            return;

        if (Time.time - lastDamageTime < damageCooldown)
            return;

        float hitStrength = collision.relativeVelocity.magnitude;

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