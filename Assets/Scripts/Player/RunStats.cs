using UnityEngine;

public class RunStats : MonoBehaviour
{
    public float meltDamage;
    public float hammerDamage;
    public float landingDamage;
    public float spikeDamage;

    public int hammerHits;
    public int spikeHits;
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

    public void AddSpikeDamage(float amount)
    {
        if (finished || amount <= 0f)
            return;

        spikeDamage += amount;
        spikeHits++;
    }

    public void FinishRun()
    {
        if (finished)
            return;

        finished = true;
        runTime = Time.time - startTime;
    }
}