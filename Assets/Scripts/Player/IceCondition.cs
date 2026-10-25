using UnityEngine;
using UnityEngine.UI;

public class IceCondition : MonoBehaviour
{
    public float maxIce = 100f;
    public float meltRate = 2f;

    public Slider iceBar;
    public GameObject failurePanel;

    public float currentIce;

    private bool melting = true;
    private bool failed = false;

    private Rigidbody rb;
    private RunStats runStats;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        runStats = GetComponent<RunStats>();

        currentIce = maxIce;

        if (iceBar != null)
        {
            iceBar.maxValue = maxIce;
            iceBar.value = currentIce;
        }
    }

    void Update()
    {
        if (!melting || failed)
            return;

        float iceBefore = currentIce;

        currentIce -= meltRate * Time.deltaTime;
        currentIce = Mathf.Clamp(currentIce, 0f, maxIce);

        float meltedAmount = iceBefore - currentIce;

        if (runStats != null)
            runStats.AddMeltDamage(meltedAmount);

        if (iceBar != null)
            iceBar.value = currentIce;

        if (currentIce <= 0f)
            FailRun();
    }

    public void TakeDamage(float amount)
    {
        if (!melting || failed)
            return;

        currentIce -= amount;
        currentIce = Mathf.Clamp(currentIce, 0f, maxIce);

        if (iceBar != null)
            iceBar.value = currentIce;

        if (currentIce <= 0f)
            FailRun();
    }

    public void StopMelting()
    {
        melting = false;
    }

    public float GetIcePercentage()
    {
        return (currentIce / maxIce) * 100f;
    }

    public bool HasFailed()
    {
        return failed;
    }

    public void FailRun()
    {
        failed = true;
        melting = false;

        RunStats stats = GetComponent<RunStats>();

        if (stats != null)
            stats.FinishRun();

        if (failurePanel != null)
            failurePanel.SetActive(true);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}