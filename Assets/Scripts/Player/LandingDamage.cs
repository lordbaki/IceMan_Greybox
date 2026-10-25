using UnityEngine;

public class LandingDamage : MonoBehaviour
{
    public float safeImpactSpeed = 4.6f;
    public float maximumImpactSpeed = 6.5f;
    public float maximumDamage = 20f;
    public float damageCooldown = 0.75f;

    private IceCondition iceCondition;
    private RunStats runStats;

    private float lastDamageTime = -10f;

    void Awake()
    {
        iceCondition = GetComponent<IceCondition>();
        runStats = GetComponent<RunStats>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (iceCondition == null)
            return;

        if (Time.time - lastDamageTime < damageCooldown)
            return;

        if (collision.contactCount == 0)
            return;

        Vector3 surfaceNormal =
            collision.contacts[0].normal;

        if (surfaceNormal.y < 0.5f)
            return;

        float impactSpeed = Mathf.Abs(
            Vector3.Dot(
                collision.relativeVelocity,
                surfaceNormal
            )
        );

        if (impactSpeed <= safeImpactSpeed)
            return;

        float normalizedImpact =
            Mathf.InverseLerp(
                safeImpactSpeed,
                maximumImpactSpeed,
                impactSpeed
            );

        float damage =
            normalizedImpact *
            normalizedImpact *
            maximumDamage;

        float iceBefore =
            iceCondition.currentIce;

        iceCondition.TakeDamage(damage);

        float actualDamage =
            iceBefore - iceCondition.currentIce;

        if (runStats != null && actualDamage > 0f)
        {
            runStats.AddLandingDamage(
                actualDamage,
                impactSpeed
            );
        }

        lastDamageTime = Time.time;
    }
}