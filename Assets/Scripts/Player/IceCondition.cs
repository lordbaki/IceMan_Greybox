using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IceCondition : MonoBehaviour
{
    public float maxIce = 100f;
    public float meltRate = 2f;

    public Slider iceBar;
    public GameObject failurePanel;

    public float currentIce;

    [Header("HUD Feedback (Optional)")]
    public Graphic iceFill;
    public TMP_Text icePercentText;
    public TMP_Text damageText;
    public TMP_Text iceStatusText;

    [Header("HUD Colors")]
    public Color normalIceColor = new Color32(89, 207, 237, 255);
    public Color impactColor = new Color32(255, 91, 91, 255);
    public Color lowIceColor = new Color32(255, 189, 85, 255);

    [Header("HUD Animation")]
    [Range(0f, 100f)] public float lowIceThreshold = 20f;
    [Min(0.01f)] public float flashDuration = 0.30f;
    [Min(0.01f)] public float damageDisplayDuration = 0.90f;
    public float damageFloatDistance = 26f;

    [Header("Hurt Audio (Optional)")]
    public AudioSource hurtAudioSource;
    public AudioClip hurtSound;
    [Range(0f, 1f)] public float hurtVolume = 1f;

    [Header("Game Over Audio (Optional)")]
    public AudioSource backgroundMusic;
    public AudioSource gameOverAudioSource;
    public AudioClip gameOverSound;
    [Range(0f, 1f)] public float gameOverVolume = 1f;

    private float flashRemaining;
    private float damageRemaining;
    private float displayedDamage;
    private Vector2 damageStartPosition;
    private int lastDisplayedPercent = -1;

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

        // Use the existing slider fill automatically if no override is assigned.
        if (iceFill == null && iceBar != null && iceBar.fillRect != null)
            iceFill = iceBar.fillRect.GetComponent<Graphic>();

        if (damageText != null)
        {
            damageStartPosition = damageText.rectTransform.anchoredPosition;
            damageText.text = string.Empty;
            damageText.raycastTarget = false;
        }

        RefreshHUD();
    }

    void Update()
    {
        flashRemaining = Mathf.Max(0f, flashRemaining - Time.deltaTime);
        damageRemaining = Mathf.Max(0f, damageRemaining - Time.deltaTime);

        if (!melting || failed)
        {
            RefreshHUD();
            return;
        }

        float iceBefore = currentIce;

        currentIce -= meltRate * Time.deltaTime;
        currentIce = Mathf.Clamp(currentIce, 0f, maxIce);

        float meltedAmount = iceBefore - currentIce;

        if (runStats != null)
            runStats.AddMeltDamage(meltedAmount);

        if (iceBar != null)
            iceBar.value = currentIce;

        RefreshHUD();

        if (currentIce <= 0f)
            FailRun();
    }

    public void TakeDamage(float amount)
    {
        TakeDamage(amount, false);
    }
    public void TakeDamage(float amount, bool playHurtSound)
    {
        if (!melting || failed)
            return;

        float iceBefore = currentIce;
        currentIce -= amount;
        currentIce = Mathf.Clamp(currentIce, 0f, maxIce);

        if (iceBar != null)
            iceBar.value = currentIce;

        float actualDamage = iceBefore - currentIce;
        if (actualDamage > 0f)
        {
            ShowImpact(actualDamage);

            if (playHurtSound && hurtAudioSource != null && hurtSound != null)
                hurtAudioSource.PlayOneShot(hurtSound, hurtVolume);
        }

        RefreshHUD();

        if (currentIce <= 0f)
            FailRun();
    }

    private void ShowImpact(float amount)
    {
        // Combine fast consecutive impacts into one readable damage number.
        displayedDamage = damageRemaining > 0f
            ? displayedDamage + amount
            : amount;

        flashRemaining = flashDuration;
        damageRemaining = damageDisplayDuration;

        if (damageText != null)
        {
            float percentLost = maxIce > 0f
                ? displayedDamage / maxIce * 100f : 0f;
            damageText.text = "-" + percentLost.ToString("0.#") + "%";
            damageText.rectTransform.anchoredPosition = damageStartPosition;
        }
    }

    private void RefreshHUD()
    {
        float percent = maxIce > 0f
            ? Mathf.Clamp01(currentIce / maxIce) * 100f : 0f;
        bool lowIce = percent <= lowIceThreshold;

        int roundedPercent = Mathf.CeilToInt(percent);
        if (icePercentText != null && roundedPercent != lastDisplayedPercent)
        {
            icePercentText.text = roundedPercent + "%";
            lastDisplayedPercent = roundedPercent;
        }
        Color baseColor = normalIceColor;
        if (lowIce)
        {
            // Slow, subtle warning pulse. No rapid blinking.
            float pulse = melting && !failed
                ? 0.5f + 0.5f * Mathf.Sin(Time.time * Mathf.PI * 2f * 1.1f)
                : 0f;
            baseColor = Color.Lerp(lowIceColor, impactColor, pulse * 0.45f);
        }

        float flash = flashRemaining / Mathf.Max(flashDuration, 0.01f);
        Color fillColor = Color.Lerp(baseColor, impactColor, Mathf.Clamp01(flash));
        if (iceFill != null)
            iceFill.color = fillColor;

        if (iceStatusText != null)
        {
            string status = lowIce ? "LOW ICE" : "ICE CONDITION";
            if (iceStatusText.text != status)
                iceStatusText.text = status;
            iceStatusText.color = lowIce ? lowIceColor : Color.white;
        }

        if (icePercentText != null)
            icePercentText.color = lowIce ? lowIceColor : Color.white;

        if (damageText == null)
            return;

        if (damageRemaining > 0f)
        {
            float progress = 1f - Mathf.Clamp01(damageRemaining /
                Mathf.Max(damageDisplayDuration, 0.01f));
            float rise = 1f - (1f - progress) * (1f - progress);
            damageText.rectTransform.anchoredPosition = damageStartPosition +
                Vector2.up * (damageFloatDistance * rise);

            Color numberColor = impactColor;
            numberColor.a *= 1f - Mathf.SmoothStep(0f, 1f,
                Mathf.InverseLerp(0.45f, 1f, progress));
            damageText.color = numberColor;
        }
        else
        {
            if (damageText.text.Length > 0)
                damageText.text = string.Empty;
            damageText.rectTransform.anchoredPosition = damageStartPosition;
        }
    }

    public bool TakeSpikeDamage(float amount)
    {
        if (!melting || failed || amount <= 0f || currentIce <= 0f)
            return false;

        // Record only the ice actually lost, before a possible FailRun.
        float actualDamage = currentIce -
            Mathf.Clamp(currentIce - amount, 0f, maxIce);

        if (actualDamage <= 0f)
            return false;

        if (runStats != null)
            runStats.AddSpikeDamage(actualDamage);

        TakeDamage(amount, true);
        return true;
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
        if (failed)
            return;

        failed = true;
        melting = false;

        if (backgroundMusic != null)
            backgroundMusic.Stop();

        if (gameOverAudioSource != null && gameOverSound != null)
            gameOverAudioSource.PlayOneShot(gameOverSound, gameOverVolume);

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