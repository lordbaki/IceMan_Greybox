using UnityEngine;

public class RunStats : MonoBehaviour
{
    public float meltDamage;
    public float hammerDamage;
    public float landingDamage;

    public int hammerHits;
    public float hardestLanding;

    public float runTime;

    private float startTime;
    private bool finished;

    void Start()
    {
        startTime = Time.time;
    }

    public void AddMeltDamage(float amount)
    {
        meltDamage += amount;
    }

    public void AddHammerDamage(float amount)
    {
        hammerDamage += amount;
        hammerHits++;
    }

    public void AddLandingDamage(float amount, float impactSpeed)
    {
        landingDamage += amount;

        if (impactSpeed > hardestLanding)
            hardestLanding = impactSpeed;
    }

    public void FinishRun()
    {
        if (finished)
            return;

        finished = true;
        runTime = Time.time - startTime;
    }
}